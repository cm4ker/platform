using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection.Metadata;
using dnlib.DotNet;

namespace ZenPlatform.Compiler.Dnlib
{
    public class DnlibMetadataResolver : IResolver
    {
        public TypeDef Resolve(TypeRef typeRef, ModuleDef sourceModule)
        {
            throw new NotImplementedException();
        }

        public IMemberForwarded Resolve(MemberRef memberRef)
        {
            throw new NotImplementedException();
        }
    }


    public class DnlibAssemblyResolver : IAssemblyResolver
    {
        private static readonly string BaseDirectory = AppContext.BaseDirectory;
        private static readonly string RuntimeDirectory = Path.GetDirectoryName(typeof(object).Assembly.Location);

        private static readonly string NetstandardDirectory =
            @"C:\Program Files\dotnet\sdk\NuGetFallbackFolder\netstandard.library\2.0.3\build\netstandard2.0\ref";

        private static readonly string MacNetstandardDirectory =
            "/usr/local/share/dotnet/sdk/NuGetFallbackFolder/netstandard.library/2.0.3/build/netstandard2.0/ref";

        Dictionary<string, AssemblyDef> libraries;


        public DnlibAssemblyResolver()
        {
            libraries = new Dictionary<string, AssemblyDef>();
        }

        public AssemblyDef Resolve(IAssembly assembly, ModuleDef sourceModule)
        {
            if (assembly == null)
                throw new ArgumentNullException("name");

            AssemblyDef def;

            List<string> paths = new List<string>();
            paths.Add(BaseDirectory);
            paths.Add(RuntimeDirectory);
            paths.Add(NetstandardDirectory);
            paths.Add(MacNetstandardDirectory);

            paths.Add("");

            var libname = assembly.Name;

            if (libname == "mscorlib" || libname == "System.Private.CoreLib")
            {
                libname = "mscorlib";
            }

            if (!libraries.TryGetValue(libname, out def))
            {
                foreach (var path in paths)
                {
                    var dllPath = Path.Combine(path, $"{libname}.dll");
                    if (File.Exists(dllPath))
                    {
                        def = AssemblyDef.Load(dllPath, new ModuleContext(this, new DnlibMetadataResolver()));
                        libraries.Add(libname, def);

                        return def;
                    }
                }

                throw new ResolveException(assembly.Name);
            }

            return def;
        }
    }
}