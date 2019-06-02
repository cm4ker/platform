using System;
using System.Collections.Generic;
using System.IO;
using Mono.Cecil;

namespace ZenPlatform.Compiler.Cecil.Backend
{
    // ReSharper disable once ClassWithVirtualMembersNeverInherited.Global
    public class CustomAssemblyResolver : IAssemblyResolver
    {
        private readonly CecilTypeSystem _cts;
        private static readonly string BaseDirectory = AppContext.BaseDirectory;
        private static readonly string RuntimeDirectory = Path.GetDirectoryName(typeof(object).Assembly.Location);

        private static readonly string NetstandardDirectory =
            @"C:\Program Files\dotnet\sdk\NuGetFallbackFolder\netstandard.library\2.0.3\build\netstandard2.0\ref";

        private static readonly string MacNetstandardDirectory =
            "/usr/local/share/dotnet/sdk/NuGetFallbackFolder/netstandard.library/2.0.3/build/netstandard2.0/ref";

        Dictionary<string, AssemblyDefinition> libraries;

        public CustomAssemblyResolver(CecilTypeSystem cts)
        {
            _cts = cts;
            libraries = new Dictionary<string, AssemblyDefinition>();
        }

        public virtual AssemblyDefinition Resolve(string fullName)
        {
            return Resolve(fullName, new ReaderParameters());
        }

        public virtual AssemblyDefinition Resolve(string fullName, ReaderParameters parameters)
        {
            return Resolve(AssemblyNameReference.Parse(fullName), parameters);
        }

        public AssemblyDefinition Resolve(AssemblyNameReference name)
        {
            return Resolve(name, new ReaderParameters());
        }

        public AssemblyDefinition Resolve(AssemblyNameReference name, ReaderParameters parameters)
        {
            if (name == null)
                throw new ArgumentNullException("name");

            AssemblyDefinition def;

            List<string> paths = new List<string>();
            paths.Add(BaseDirectory);
            paths.Add(NetstandardDirectory);
            paths.Add(MacNetstandardDirectory);
            paths.Add(RuntimeDirectory);
            paths.Add("");

            var libname = name.Name;

            if (libname == "mscorlib")
            {
                libname = "netstandard";
            }

            if (!libraries.TryGetValue(libname, out def))
            {
                foreach (var path in paths)
                {
                    var dllPath = Path.Combine(path, $"{libname}.dll");
                    if (File.Exists(dllPath))
                    {
                        def = AssemblyDefinition.ReadAssembly(dllPath, new ReaderParameters()
                        {
                            MetadataResolver = new Cecil.CustomMetadataResolver(_cts),
                            AssemblyResolver = this
                        });
                        libraries.Add(libname, def);

                        return def;
                    }
                }

                throw new AssemblyResolutionException(name);
            }

            return def;
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!disposing)
                return;

            foreach (var def in libraries.Values)
            {
                def.Dispose();
            }
        }

        public void Dispose()
        {
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }
    }
}