using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using Mono.Cecil;

namespace ZenPlatform.Compiler.Cecil.Backend
{
    public class CustomAssemblyResolver : IAssemblyResolver
    {
        private static readonly string BaseDirectory = System.AppContext.BaseDirectory;
        private static readonly string RuntimeDirectory = Path.GetDirectoryName(typeof(object).Assembly.Location);

        private static readonly string NetstandardDirectory =
            @"C:\Program Files\dotnet\sdk\NuGetFallbackFolder\netstandard.library\2.0.3\build\netstandard2.0\ref";

        Dictionary<string, AssemblyDefinition> libraries;

        public CustomAssemblyResolver()
        {
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
                        def = AssemblyDefinition.ReadAssembly(dllPath);
                        this.libraries.Add(libname, def);
                        
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

            foreach (var def in this.libraries.Values)
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

    public class CustomMetadataResolver : IMetadataResolver
    {
        public TypeDefinition Resolve(TypeReference type)
        {
            throw new NotImplementedException();
        }

        public FieldDefinition Resolve(FieldReference field)
        {
            throw new NotImplementedException();
        }

        public MethodDefinition Resolve(MethodReference method)
        {
            throw new NotImplementedException();
        }
    }
}