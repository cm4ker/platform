using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text.RegularExpressions;
using Microsoft.Extensions.Logging;
using Mono.Cecil;

namespace Aquila.Compiler.Cecil.CopyFeature
{
    internal interface IRepackContext
    {
        IList<AssemblyDefinition> MergedAssemblies { get; }
        ModuleDefinition TargetAssemblyMainModule { get; }
        PlatformFixer PlatformFixer { get; }
        ReflectionHelper ReflectionHelper { get; }
        MappingHandler MappingHandler { get; }
        AssemblyDefinition TargetAssemblyDefinition { get; }
        IList<AssemblyDefinition> OtherAssemblies { get; }
        AssemblyDefinition PrimaryAssemblyDefinition { get; }
        ModuleDefinition PrimaryAssemblyMainModule { get; }

        TypeDefinition GetMergedTypeFromTypeRef(TypeReference type);
        TypeReference GetExportedTypeFromTypeRef(TypeReference type);
        IMetadataScope MergeScope(IMetadataScope name);

        string FixTypeName(string assemblyName, string typeName);
        string FixAssemblyName(string assemblyName);
        string FixStr(string content);

        /// <summary>
        /// Fix assembly reference in attribute
        /// </summary>
        /// <param name="content">string to search in</param>
        /// <returns>new string with references fixed</returns>
        string FixReferenceInIkvmAttribute(string content);
    }

    public class ILRepack : IRepackContext
    {
        internal RepackOptions Options;
        internal ILogger Logger;

        internal IList<string> MergedAssemblyFiles { get; set; }

        internal string PrimaryAssemblyFile { get; set; }

        // contains all 'other' assemblies, but not the primary assembly
        public IList<AssemblyDefinition> OtherAssemblies { get; private set; }

        // contains all assemblies, primary (first one) and 'other'
        public IList<AssemblyDefinition> MergedAssemblies { get; private set; }
        public AssemblyDefinition TargetAssemblyDefinition { get; private set; }
        public AssemblyDefinition PrimaryAssemblyDefinition { get; private set; }
        public CustomAssemblyResolver GlobalAssemblyResolver { get; }

        public ModuleDefinition TargetAssemblyMainModule => TargetAssemblyDefinition.MainModule;
        public ModuleDefinition PrimaryAssemblyMainModule => PrimaryAssemblyDefinition.MainModule;


        private ReflectionHelper _reflectionHelper;
        private PlatformFixer _platformFixer;
        private MappingHandler _mappingHandler;

        private static readonly Regex TypeRegex = new Regex("^(.*?), ([^>,]+), .*$");

        ReflectionHelper IRepackContext.ReflectionHelper => _reflectionHelper;
        PlatformFixer IRepackContext.PlatformFixer => _platformFixer;
        MappingHandler IRepackContext.MappingHandler => _mappingHandler;
        private readonly Dictionary<AssemblyDefinition, int> _aspOffsets = new Dictionary<AssemblyDefinition, int>();

        internal RepackImporter _repackImporter;

        public ILRepack(RepackOptions options, ILogger logger, CecilTypeSystem ts)
        {
            Options = options;
            Logger = logger;

            _repackImporter = new RepackImporter(Logger, Options, this, _aspOffsets);

            GlobalAssemblyResolver = new CustomAssemblyResolver(ts);
        }

        private void ReadInputAssemblies()
        {
            MergedAssemblyFiles = Options.ResolveFiles();
            OtherAssemblies = new List<AssemblyDefinition>();
            // TODO: this could be parallelized to gain speed
            var primary = MergedAssemblyFiles.FirstOrDefault();
            var debugSymbolsRead = false;
            foreach (string assembly in MergedAssemblyFiles)
            {
                var result = ReadInputAssembly(assembly, primary == assembly);
                if (result.IsPrimary)
                {
                    PrimaryAssemblyDefinition = result.Definition;
                    PrimaryAssemblyFile = result.Assembly;
                }
                else
                    OtherAssemblies.Add(result.Definition);

                debugSymbolsRead |= result.SymbolsRead;
            }

            // prevent writing PDB if we haven't read any
            Options.DebugInfo = debugSymbolsRead;

            MergedAssemblies = new List<AssemblyDefinition>(OtherAssemblies);
            MergedAssemblies.Insert(0, PrimaryAssemblyDefinition);
        }

        private AssemblyDefinitionContainer ReadInputAssembly(string assembly, bool isPrimary)
        {
            Logger.Info("Adding assembly for merge: " + assembly);
            try
            {
                ReaderParameters rp = new ReaderParameters(ReadingMode.Immediate)
                    {AssemblyResolver = GlobalAssemblyResolver};
                // read PDB/MDB?
                if (Options.DebugInfo &&
                    (File.Exists(Path.ChangeExtension(assembly, "pdb")) || File.Exists(assembly + ".mdb")))
                {
                    rp.ReadSymbols = true;
                }

                AssemblyDefinition mergeAsm;
                try
                {
                    mergeAsm = AssemblyDefinition.ReadAssembly(assembly, rp);
                }
                catch (BadImageFormatException e) when (!rp.ReadSymbols)
                {
                    throw new InvalidOperationException(
                        "ILRepack does not support merging non-.NET libraries (e.g.: native libraries)", e);
                }
                // cope with invalid symbol file
                catch (Exception) when (rp.ReadSymbols)
                {
                    rp.ReadSymbols = false;
                    try
                    {
                        mergeAsm = AssemblyDefinition.ReadAssembly(assembly, rp);
                    }
                    catch (BadImageFormatException e)
                    {
                        throw new InvalidOperationException(
                            "ILRepack does not support merging non-.NET libraries (e.g.: native libraries)", e);
                    }

                    Logger.Info("Failed to load debug information for " + assembly);
                }

                if (!Options.AllowZeroPeKind && (mergeAsm.MainModule.Attributes & ModuleAttributes.ILOnly) == 0)
                    throw new ArgumentException("Failed to load assembly with Zero PeKind: " + assembly);

                return new AssemblyDefinitionContainer
                {
                    Assembly = assembly,
                    Definition = mergeAsm,
                    IsPrimary = isPrimary,
                    SymbolsRead = rp.ReadSymbols
                };
            }
            catch
            {
                Logger.Error("Failed to load assembly " + assembly);
                throw;
            }
        }

        IMetadataScope IRepackContext.MergeScope(IMetadataScope scope)
        {
            return scope;
        }

        internal class AssemblyDefinitionContainer
        {
            public bool SymbolsRead { get; set; }
            public AssemblyDefinition Definition { get; set; }
            public string Assembly { get; set; }
            public bool IsPrimary { get; set; }
        }

        public enum Kind
        {
            Dll,
            Exe,
            WinExe,
            SameAsPrimaryAssembly
        }


        private TargetRuntime ParseTargetPlatform()
        {
            TargetRuntime runtime = PrimaryAssemblyMainModule.Runtime;
            if (Options.TargetPlatformVersion != null)
            {
                switch (Options.TargetPlatformVersion)
                {
                    case "v2":
                        runtime = TargetRuntime.Net_2_0;
                        break;
                    case "v4":
                        runtime = TargetRuntime.Net_4_0;
                        break;
                    default:
                        throw new ArgumentException(
                            $"Invalid TargetPlatformVersion: '{Options.TargetPlatformVersion}'");
                }

                _platformFixer.ParseTargetPlatformDirectory(runtime, Options.TargetPlatformDirectory);
            }

            return runtime;
        }

        private string ResolveTargetPlatformDirectory(string version)
        {
            if (version == null)
                return null;
            var platformBasePath = Path.GetDirectoryName(Path.GetDirectoryName(typeof(string).Assembly.Location));
            List<string> platformDirectories = new List<string>(Directory.GetDirectories(platformBasePath));
            var platformDir = version.Substring(1);
            if (platformDir.Length == 1) platformDir = platformDir + ".0";
            // mono platform dir is '2.0' while windows is 'v2.0.50727'
            var targetPlatformDirectory = platformDirectories
                .FirstOrDefault(x =>
                    Path.GetFileName(x).StartsWith(platformDir) || Path.GetFileName(x).StartsWith($"v{platformDir}"));
            if (targetPlatformDirectory == null)
                throw new ArgumentException(
                    $"Failed to find target platform '{Options.TargetPlatformVersion}' in '{platformBasePath}'");
            Logger.Info($"Target platform directory resolved to {targetPlatformDirectory}");
            return targetPlatformDirectory;
        }

        public static IEnumerable<AssemblyName> GetRepackAssemblyNames(Type typeInRepackedAssembly)
        {
            try
            {
            }
            catch (Exception)
            {
            }

            return Enumerable.Empty<AssemblyName>();
        }

        public static AssemblyName GetRepackAssemblyName(IEnumerable<AssemblyName> repackAssemblyNames,
            string repackedAssemblyName, Type fallbackType)
        {
            return repackAssemblyNames?.FirstOrDefault(name => name.Name == repackedAssemblyName) ??
                   fallbackType.Assembly.GetName();
        }

        void PrintRepackHeader()
        {
            var assemblies = GetRepackAssemblyNames(typeof(ILRepack));
            var ilRepack = GetRepackAssemblyName(assemblies, "ILRepack", typeof(ILRepack));
            Logger.Info($"IL Repack - Version {ilRepack.Version.ToString(3)}");
            Logger.Verbose($"Runtime: {typeof(ILRepack).Assembly.FullName}");
            Logger.Info(Options.ToCommandLine());
        }

        /// <summary>
        /// The actual repacking process, called by main after parsing arguments.
        /// When referencing this assembly, call this after setting the merge properties.
        /// </summary>
        public void Repack()
        {
        }

        private void ResolveSearchDirectories()
        {
        }


        string IRepackContext.FixStr(string content)
        {
            return FixStr(content, false);
        }

        string IRepackContext.FixReferenceInIkvmAttribute(string content)
        {
            return FixStr(content, true);
        }

        private string FixStr(string content, bool javaAttribute)
        {
            if (String.IsNullOrEmpty(content) || content.Length > 512 || content.IndexOf(", ") == -1 ||
                content.StartsWith("System."))
                return content;
            // TODO fix "TYPE, ASSEMBLYNAME, CULTURE" pattern
            // TODO fix "TYPE, ASSEMBLYNAME, VERSION, CULTURE, TOKEN" pattern
            var match = TypeRegex.Match(content);
            if (match.Success)
            {
                string type = match.Groups[1].Value;
                string targetAssemblyName = TargetAssemblyDefinition.FullName;
                if (javaAttribute)
                    targetAssemblyName = targetAssemblyName.Replace('.', '/') + ";";

                if (MergedAssemblies.Any(x => x.Name.Name == match.Groups[2].Value))
                {
                    return type + ", " + targetAssemblyName;
                }
            }

            return content;
        }

        string IRepackContext.FixTypeName(string assemblyName, string typeName)
        {
            // TODO handle renames
            return typeName;
        }

        string IRepackContext.FixAssemblyName(string assemblyName)
        {
            if (MergedAssemblies.Any(x => x.FullName == assemblyName))
            {
                // TODO no public key token !
                return TargetAssemblyDefinition.FullName;
            }

            return assemblyName;
        }

        private AssemblyNameDefinition Clone(AssemblyNameDefinition assemblyName)
        {
            AssemblyNameDefinition asmName = new AssemblyNameDefinition(assemblyName.Name, assemblyName.Version);
            asmName.Attributes = assemblyName.Attributes;
            asmName.Culture = assemblyName.Culture;
            asmName.Hash = assemblyName.Hash;
            asmName.HashAlgorithm = assemblyName.HashAlgorithm;
            asmName.PublicKey = assemblyName.PublicKey;
            asmName.PublicKeyToken = assemblyName.PublicKeyToken;
            return asmName;
        }

        TypeDefinition IRepackContext.GetMergedTypeFromTypeRef(TypeReference reference)
        {
            return _mappingHandler.GetRemappedType(reference);
        }

        TypeReference IRepackContext.GetExportedTypeFromTypeRef(TypeReference type)
        {
            return _mappingHandler.GetExportedRemappedType(type) ?? type;
        }
    }
}