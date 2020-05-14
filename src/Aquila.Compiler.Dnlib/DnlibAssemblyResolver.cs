using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection.Metadata;
using dnlib.DotNet;

namespace Aquila.Compiler.Dnlib
{
    public class DnlibAssemblyResolver : IAssemblyResolver
    {
        private static readonly string BaseDirectory = AppContext.BaseDirectory;
        private static readonly string RuntimeDirectory = Path.GetDirectoryName(typeof(object).Assembly.Location);

        private static readonly string NetstandardDirectory =
            @"C:\Program Files\dotnet\sdk\NuGetFallbackFolder\netstandard.library\2.0.3\build\netstandard2.0\ref";

        private static readonly string MacNetstandardDirectory =
            "/usr/local/share/dotnet/sdk/NuGetFallbackFolder/netstandard.library/2.0.3/build/netstandard2.0/ref";

        Dictionary<string, AssemblyDef> libraries;
        private AssemblyResolver _defaultResolver;


        public DnlibAssemblyResolver()
        {
            libraries = new Dictionary<string, AssemblyDef>();

            _defaultResolver = new AssemblyResolver();
            _defaultResolver.FindExactMatch = false;
            _defaultResolver.EnableFrameworkRedirect = true;
            _defaultResolver.PreSearchPaths.Add(BaseDirectory);
            _defaultResolver.PreSearchPaths.Add(RuntimeDirectory);
            _defaultResolver.PreSearchPaths.Add(NetstandardDirectory);
            _defaultResolver.PreSearchPaths.Add(MacNetstandardDirectory);
        }

        public void RegisterAsm(AssemblyDef asm)
        {
            libraries.TryAdd(asm.Name, asm);
        }

        public AssemblyDef Resolve(IAssembly assembly, ModuleDef sourceModule)
        {
            if (assembly == null)
                throw new ArgumentNullException(nameof(assembly));

            AssemblyDef def;

            List<string> paths = new List<string>();
            paths.Add(BaseDirectory);
            paths.Add(RuntimeDirectory);
            paths.Add(NetstandardDirectory);
            paths.Add(MacNetstandardDirectory);

            paths.Add("");

            var libname = assembly.Name;

//            if (libname == "mscorlib" || libname == "System.Private.CoreLib")
//            {
//                libname = "mscorlib";
//            }

            if (!libraries.TryGetValue(libname, out def))
            {
                foreach (var path in paths)
                {
                    var dllPath = Path.Combine(path, $"{libname}.dll");
                    if (File.Exists(dllPath))
                    {
                        def = AssemblyDef.Load(dllPath,
                            new ModuleContext(this, new DnlibMetadataResolver(this)));

                        libraries.Add(libname, def);

                        return def;
                    }
                }

                var asm = _defaultResolver.Resolve(assembly, sourceModule);

                if (asm != null)
                {
                    asm.ManifestModule.Context = new ModuleContext(this, new DnlibMetadataResolver(this));
                    return asm;
                }


                throw new ResolveException(assembly.Name);
            }


            return def;
        }
    }
}