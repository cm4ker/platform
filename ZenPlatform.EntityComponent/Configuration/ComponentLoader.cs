using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using Microsoft.CodeAnalysis.Completion;
using SharpFileSystem;
using ZenPlatform.Configuration;
using ZenPlatform.Configuration.Contracts;
using ZenPlatform.Configuration.Contracts.Data;
using ZenPlatform.Configuration.Contracts.Store;
using ZenPlatform.Configuration.Contracts.TypeSystem;
using ZenPlatform.Configuration.Structure;
using ZenPlatform.Configuration.Structure.Data;
using ZenPlatform.Language.Ast.Definitions.Expressions;
using ZenPlatform.Language.Ast.Definitions.Statements;

namespace ZenPlatform.EntityComponent.Configuration
{
    public static class FileSystemExtensions
    {
        public static T Deserialize<T>(this IFileSystem fs, string path)
        {
            try
            {
                using (var stream = fs.OpenFile(FileSystemPath.Parse(path), FileAccess.Read))
                {
                    return XCHelper.DeserializeFromStream<T>(stream);
                }
            }
            catch
            {
                return default;
            }
        }

        public static byte[] GetBytes(this IFileSystem fs, string path)
        {
            using (var stream = fs.OpenFile(FileSystemPath.Parse(path), FileAccess.Read))
            {
                var memoryStream = new MemoryStream();
                stream.CopyTo(memoryStream);
                return memoryStream.ToArray();
            }
        }

        public static void Serialize(this IFileSystem fs, string path, object obj)
        {
            using (var stream = fs.OpenFile(FileSystemPath.Parse(path), FileAccess.Read))
                obj.SerializeToStream().CopyTo(stream);
        }

        public static void SaveBytes(IFileSystem fs, string path, byte[] data)
        {
            using (var stream = fs.OpenFile(FileSystemPath.Parse(path), FileAccess.Read))
                stream.Write(data, 0, data.Length);
        }
    }

    public class ComponentBuilder
    {
        
        
        public FileSystemPath Entry { get; set; }
        
        public void 
    }

    public class ObjectBuilder
    {
        private void BuildObject(IComponent component, IUniqueCounter counter, ITypeManager tm, MDEntity md)
        {
            var oType = tm.Type();
            oType.IsObject = true;

            oType.Id = md.ObjectId;
            oType.Name = md.Name;
            oType.IsCodeAvaliable = true;
            oType.IsQueryAvaliable = true;

            oType.ComponentId = component.Info.ComponentId;
            oType.Metadata = md;

            oType.SystemId = counter.GetId(oType.Id);

            tm.Register(oType);

            foreach (var prop in md.Properties)
            {
                var p = tm.Property();

                p.Name = prop.Name;
                p.Metadata = prop;
            }

            foreach (var table in md.Tables)
            {
                var t = tm.Table();
                t.Name = table.Name;
                t.Id = table.Guid;

                t.SystemId = counter.GetId(t.Id);
            }
        }

        private void BuildDto(IComponent component, IUniqueCounter counter, ITypeManager tm, MDEntity md)
        {
            var oType = tm.Type();
            oType.IsDto = true;

            oType.Id = md.ObjectId;
            oType.Name = "_" + md.Name;

            oType.ComponentId = component.Info.ComponentId;
            oType.Metadata = md;

            oType.SystemId = counter.GetId(oType.Id);

            tm.Register(oType);

            foreach (var prop in md.Properties)
            {
                var p = tm.Property();

                p.Name = prop.Name;
                p.Metadata = prop;
            }

            foreach (var table in md.Tables)
            {
                var t = tm.Table();
                t.Name = table.Name;
                t.Id = table.Guid;

                t.SystemId = counter.GetId(t.Id);
            }
        }

        private void BuildLink(IComponent component, IUniqueCounter counter, ITypeManager tm, MDEntity md)
        {
            var oType = tm.Type();
            oType.IsLink = true;
            oType.IsQueryAvaliable = true;

            oType.Id = md.LinkId;
            oType.Name = md.Name + "Link";

            oType.ComponentId = component.Info.ComponentId;
            oType.Metadata = md;

            oType.SystemId = counter.GetId(oType.Id);

            tm.Register(oType);

            foreach (var prop in md.Properties)
            {
                var p = tm.Property();

                p.Name = prop.Name;
                p.Metadata = prop;
            }

            foreach (var table in md.Tables)
            {
                var t = tm.Table();
                t.Name = table.Name;
                t.Id = table.Guid;

                t.SystemId = counter.GetId(t.Id);
            }
        }
    }

    public class ComponentLoader : IComponentManager
    {
        private Assembly _asm;

        private Dictionary<string, object> _loadedObjects;

        public ComponentLoader()
        {
            _asm = typeof(ComponentLoader).Assembly;
        }

        /// <summary>
        /// Загрузить объект компонента
        /// </summary>
        /// <param name="component"></param>
        /// <param name="loader"></param>
        /// <param name="reference"></param>
        /// <returns></returns>
        /// <exception cref="NullReferenceException"></exception>
        public virtual void LoadObject(IComponent component, IInfrastructure infrastructure, MDEntity typeMd)
        {
        }


        public IXCComponentInformation GetComponentInfo()
        {
            return new Info();
        }

        public IDataComponent GetComponentImpl(IComponent c)
        {
            var impl = new EntityComponent(c);
            impl.OnInitializing();
            return impl;
        }

        public void Load(IInfrastructure inf, IComponentRef comRef)
        {
            var com = inf.FileSystem.Deserialize<MDComponent>(comRef.Entry);

            var c = inf.TypeManager.Component();

            c.ComponentImpl = GetComponentImpl(c);
            c.Info = GetComponentInfo();
            c.Metadata = com;

            inf.TypeManager.Register(c);

            foreach (var mdFile in inf.FileSystem.GetEntities(FileSystemPath.Parse("/Entity/")))
            {
                var type = inf.FileSystem.Deserialize<MDEntity>(mdFile.Path);

                LoadObject(c, inf, type);
            }
        }

        public void Save(IInfrastructure saver, IComponentRef comRef)
        {
            var info = GetComponentInfo();
            var com = saver.TypeManager.FindComponent(info.ComponentId);
            var md = (MDComponent) com.Metadata;

            foreach (var type in com.GetTypes().Where(x => x.IsObject))
            {
                var typeMd = type.Metadata as MDEntity ??
                             throw new Exception("This type not support by this component");

                saver.FileSystem.Serialize("some path", typeMd);
            }

            saver.FileSystem.Serialize(comRef.Entry, md);
        }
    }
}