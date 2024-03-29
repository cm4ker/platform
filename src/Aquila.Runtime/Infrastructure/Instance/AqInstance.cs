﻿using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Xml.Serialization;
using Aquila.Core.Assemlies;
using Aquila.Core.Authentication;
using Aquila.Data;
using Aquila.Initializer;
using Aquila.Core.Contracts.Network;
using Aquila.Core.Migration;
using Aquila.Logging;
using Aquila.Metadata;
using Aquila.Migrations;
using Aquila.Runtime;
using Aquila.Runtime.Infrastructure.Helpers;
using Aquila.Runtime.Querying;

namespace Aquila.Core.Instance
{
    /// <summary>
    /// Platform instance
    /// </summary>
    public class AqInstance : IAqInstance
    {
        private AquilaAssemblyLoadContext _asmContext;
        private readonly ILogger _logger;
        private readonly AqUserManager _userManager;
        private readonly AqAuthenticationManager _authenticationManager;

        public AqInstance(ILogger<AqInstance> logger,
            DataContextManager contextManager, 
            AqUserManager userManager,
            AqMigrationManager migrationManager, 
            AqAuthenticationManager authenticationManager)
        {
            _logger = logger;
            _userManager = userManager;
            _authenticationManager = authenticationManager;

            MigrationManager = migrationManager;

            DataContextManager = contextManager;
        }

        public DatabaseRuntimeContext DatabaseRuntimeContext { get; private set; }
        public AqMigrationManager MigrationManager { get; }

        public string Name { get; private set; }

        public DataContextManager DataContextManager { get; }

        public Assembly BLAssembly { get; private set; }

        public bool PendingChanges => MigrationManager.CheckMigration();

        /// <summary>
        /// Initialize instance 
        /// </summary>
        /// <param name="config">Configuration</param>
        /// <exception cref="InvalidOperationException">Instance name not initialized</exception>
        public void Initialize(StartupConfig config)
        {
            Name = config.InstanceName ?? throw new InvalidOperationException("Can't create instance without name");
            
            MigrationRunner.Migrate(config.ConnectionString, config.DatabaseType);

            DataContextManager.Initialize(config.DatabaseType, config.ConnectionString);
            UpdateDBRContext();

            _logger.Info("Current configuration was loaded. It contains {0} elements",
                DatabaseRuntimeContext.Metadata.GetMetadata().EntityMetadata.Count());

            _authenticationManager.RegisterProvider(new BaseAuthenticationProvider(_userManager));
            _logger.Info("Auth provider was registered");

            Reload();
        }

        public void Migrate()
        {
            if (PendingChanges)
            {
                MigrationManager.Migrate();
                UpdateDBRContext();
                Reload();
            }
        }

        public void Deploy(Stream packageStream)
        {
            byte[] ReadFully(Stream input)
            {
                byte[] buffer = new byte[16 * 1024];
                using (MemoryStream ms = new MemoryStream())
                {
                    int read;
                    while ((read = input.Read(buffer, 0, buffer.Length)) > 0)
                    {
                        ms.Write(buffer, 0, read);
                    }

                    return ms.ToArray();
                }
            }

            ZipArchive arch = new ZipArchive(packageStream, ZipArchiveMode.Read);


            //maybe create transaction here?
            var dc = DataContextManager.GetContext();
            dc.BeginTransaction();

            DatabaseRuntimeContext.PendingMetadata.Clear(dc);
            DatabaseRuntimeContext.PendingFiles.Clear(dc);

            var entry = arch.GetEntry("manifest.xml");

            if (entry == null)
            {
                //ERROR
                throw new Exception("Package not contains manifest file");
            }

            AquilaPackageManifest manifest;

            using (var manifestStream = entry.Open())
            {
                var s = new XmlSerializer(typeof(AquilaPackageManifest));
                manifest = (AquilaPackageManifest)s.Deserialize(manifestStream) ??
                           throw new Exception("Manifest bad format");
            }

            foreach (var item in arch.Entries)
            {
                if (item.FullName.StartsWith("Metadata\\"))
                {
                    using var stream = item.Open();
                    using StreamReader sr = new StreamReader(stream);
                    var content = sr.ReadToEnd();
                    DatabaseRuntimeContext.PendingMetadata.GetMetadata().AddMetadata(EntityMetadata.FromYaml(content));
                }
                else
                {
                    FileType type = FileType.Unknown;

                    if (item.Name == manifest.MainAssembly)
                    {
                        type = FileType.MainAssembly;
                    }

                    var descr = new FileDescriptor
                    {
                        Name = item.FullName,
                        Type = type,
                        CreateDateTime = item.LastWriteTime.DateTime
                    };

                    using var stream = item.Open();


                    DatabaseRuntimeContext.PendingFiles.SaveFile(dc, descr,
                        ReadFully(stream));
                }
            }

            DatabaseRuntimeContext.PendingMetadata.SaveMetadata(dc);
            dc.CommitTransaction();
        }

        public void UpdateAssembly(Assembly asm)
        {
            LoadAssembly(asm);
        }

        private void Reload()
        {
            var currentAssembly = GetCurrentAssembly();
            if (currentAssembly != null)
            {
                _asmContext ??= new AquilaAssemblyLoadContext();

                if (_asmContext.IsCollectible && !_asmContext.IsUnloading && _asmContext.Assemblies.Any())
                    _asmContext.Unload();

                if (_asmContext.IsUnloading)
                    _asmContext = new AquilaAssemblyLoadContext();

                _asmContext.Resolving += 
                    (context, name) => context.LoadFromByteArray(GetFile(name.Name + ".dll"));
                
                var asm = _asmContext.LoadFromByteArray(currentAssembly);
                LoadAssembly(asm);
                _logger.Info("Project '{0}' was loaded.", Name);
            }
            else
                _logger.Info("Assembly is empty");
        }

        private void LoadAssembly(Assembly asm)
        {
            _logger.Info("[Assembly] Starting init: {0}", asm.FullName);

            BLAssembly = asm;

            _logger.Info("[Assembly] Get tasks for runtime initialization");
            var r = BLAssembly.GetRuntimeInit();

            foreach (var item in r)
            {
                switch (item.attr.Kind)
                {
                    case RuntimeInitKind.TypeId:
                        var desc = DatabaseRuntimeContext.Descriptors.GetEntityDescriptor(
                            item.attr.Parameters[0] as string);

                        if (desc is null)
                            break;

                        ((FieldInfo)item.m).SetValue(null, desc.DatabaseId);
                        _logger.Info($"[Assembly] Set type id for {desc.DatabaseName} = {desc.DatabaseId}");
                        break;
                    case RuntimeInitKind.SelectQuery:
                    {
                        var mdFullName = item.attr.Parameters[0] as string;
                        var semantic = DatabaseRuntimeContext.Metadata.GetMetadata()
                            .GetSemantic(x => x.FullName == mdFullName);

                        if (semantic is null)
                            break;

                        var query = CRUDQueryGenerator.GetLoad(semantic, DatabaseRuntimeContext);
                        _logger.Info($"[Assembly] Generate select query for {mdFullName}:\n{query}");
                        (item.m as FieldInfo).SetValue(null, query);
                        break;
                    }
                    case RuntimeInitKind.UpdateQuery:
                    {
                        var mdFullName = item.attr.Parameters[0] as string;
                        var semantic = DatabaseRuntimeContext.Metadata.GetMetadata()
                            .GetSemantic(x => x.FullName == mdFullName);

                        if (semantic is null)
                            break;

                        var query = CRUDQueryGenerator.GetSaveUpdate(semantic, DatabaseRuntimeContext);
                        _logger.Info($"[Assembly] Generate update query for {mdFullName}:\n{query}");
                        (item.m as FieldInfo).SetValue(null, query);
                        break;
                    }
                    case RuntimeInitKind.InsertQuery:
                    {
                        var mdFullName = item.attr.Parameters[0] as string;
                        var semantic = DatabaseRuntimeContext.Metadata.GetMetadata()
                            .GetSemantic(x => x.FullName == mdFullName);
                        if (semantic is null)
                            break;

                        var query = CRUDQueryGenerator.GetSaveInsert(semantic, DatabaseRuntimeContext);
                        _logger.Info($"[Assembly] Generate insert query for {mdFullName}:\n{query}");
                        (item.m as FieldInfo).SetValue(null, query);
                        break;
                    }
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }
        }
        
        private void UpdateDBRContext()
        {
            DatabaseRuntimeContext = DatabaseRuntimeContext.CreateAndLoad(DataContextManager.GetContext());
        }

        private byte[] GetCurrentAssembly()
        {
            return DatabaseRuntimeContext.Files.GetMainAssembly(DataContextManager.GetContext());
        }

        private byte[] GetFile(string name)
        {
            return DatabaseRuntimeContext.Files.GetFile(DataContextManager.GetContext(), name);
        }

    }
}