using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using dnlib.DotNet;

namespace Aquila.Compiler.Roslyn.RoslynBackend
{
    public class RoslynTypeSystem
    {
        private List<RoslynAssembly> _asms;

        private AssemblyResolver _resolver;

        private Dictionary<ITypeDefOrRef, RoslynType> _typeReferenceCache =
            new Dictionary<ITypeDefOrRef, RoslynType>();

        private Dictionary<dnlib.DotNet.IAssembly, RoslynAssembly> _assemblyDic
            = new Dictionary<dnlib.DotNet.IAssembly, RoslynAssembly>();

        private Dictionary<string, RoslynType> _qNameAssemblies = new Dictionary<string, RoslynType>();

        private Dictionary<string, RoslynType> _unresolvedTypeCache = new Dictionary<string, RoslynType>();


        private RoslynTypeCache _typeCache;

        public RoslynTypeSystem(RoslynPlatformFactory factory, IEnumerable<string> paths, string targetPath = null)
        {
            _asms = new List<RoslynAssembly>();

            _resolver = new AssemblyResolver();
            _resolver.PreSearchPaths.Add(
                "C:\\Program Files\\dotnet\\packs\\Microsoft.NETCore.App.Ref\\3.1.0\\ref\\netcoreapp3.1\\");
            _resolver.PreSearchPaths.Add(Directory.GetCurrentDirectory());
            _resolver.PreSearchPaths.Add(
                "C:\\Users\\qznc\\.nuget\\packages\\system.servicemodel.primitives\\4.7.0\\ref\\netcoreapp2.1\\");
            _resolver.PreSearchPaths.Add("C:\\Program Files\\dotnet\\shared\\Microsoft.AspNetCore.App\\3.1.1\\");
            _resolver.EnableTypeDefCache = true;
            _resolver.EnableFrameworkRedirect = false;
            _resolver.DefaultModuleContext = new ModuleContext(Resolver);
            _typeCache = new RoslynTypeCache(this);

            if (targetPath != null)
                paths = paths.Concat(new[] {targetPath});


            foreach (var path in paths.Distinct())
            {
                var asm = AssemblyDef.Load(path, new ModuleCreationOptions(new ModuleContext(Resolver)));
                RegisterAssembly(asm);
            }

            foreach (var dir in _resolver.PreSearchPaths)
            {
                var files = Directory.GetFiles(dir, "*.dll");

                foreach (var path in files)
                {
                    try
                    {
                        var asm = AssemblyDef.Load(path, new ModuleCreationOptions());
                        RegisterAssembly(asm);
                        Paths.Add(path);
                    }
                    catch (Exception ex)
                    {
                        //we are failed but we still can try compile
                    }
                }
            }


            Factory = factory;
        }

        public IAssemblyResolver Resolver => _resolver;

        public List<string> Paths { get; } = new List<string>();
        public RoslynPlatformFactory Factory { get; }

        public IReadOnlyList<RoslynAssembly> Assemblies => _asms;

        public Dictionary<string, AssemblyRef> AsmRefsCache = new Dictionary<string, AssemblyRef>();

        internal RoslynAssembly RegisterAssembly(AssemblyDef assemblyDef)
        {
            var result = new RoslynAssembly(this, assemblyDef);
            return RegisterAssembly(result);
        }

        internal RoslynAssembly RegisterAssembly(RoslynAssembly assembly)
        {
            if (_assemblyDic.TryGetValue(assembly.Assembly, out var result))
                return result;

            _asms.Add(assembly);
            _assemblyDic[assembly.Assembly] = assembly;
            //_resolver.RegisterAsm(assembly.Assembly);

            return assembly;
        }

        internal RoslynType RegisterType(RoslynType type)
        {
            _typeCache.RegisterType(type);
            return type;
        }

        public RoslynAssembly FindAssembly(string assembly)
        {
            if (assembly == "System.Private.CoreLib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=7cec85d7bea7798e")
                assembly = AssemblyRefUser.CreateMscorlibReferenceCLR40().FullName;

            return RegisterAssembly(_resolver.Resolve(assembly, null));
        }

        public RoslynAssembly FindAssembly(dnlib.DotNet.IAssembly assembly)
        {
            if (assembly.IsCorLib())
            {
                return _assemblyDic.FirstOrDefault(x => x.Key.Name == "mscorlib").Value;
            }

            if (_assemblyDic.ContainsKey(assembly))
                return _assemblyDic[assembly];
            else
                return FindAssembly(assembly.FullName);
        }

        public RoslynType FindType(string name)
        {
            foreach (var asm in _asms)
            {
                var found = asm.FindType(name);
                if (found != null)
                    return found;
            }

            return null;
        }

        public RoslynType Resolve(Type type)
        {
            return FindType(type.FullName, type.Assembly.FullName);
        }

        public RoslynType Resolve<T>()
        {
            return Resolve(typeof(T));
        }

        public RoslynType ResolveType(Type type)
        {
            return Resolve(type);
        }

        public RoslynType FindType(string name, string assembly) =>
            FindAssembly(assembly)?.FindType(name);

        public RoslynType Resolve(ITypeDefOrRef reference)
        {
            reference = reference ?? throw new ArgumentNullException(nameof(reference));

            if (!_qNameAssemblies.TryGetValue(reference.AssemblyQualifiedName, out var rv))
            {
                if (reference is TypeRef tr)
                {
                    TypeDef resolved = tr.Resolve();

                    if (resolved != null)
                    {
                        rv = _typeCache.Get(reference);
                    }
                    else
                    {
                        var key = reference.FullName;

                        //TODO: resolve generic parameters

                        if (!_unresolvedTypeCache.TryGetValue(key, out rv))
                            throw new Exception($"can't resolve: {key}");
                    }

                    _qNameAssemblies[reference.AssemblyQualifiedName] = rv;
                }
                else if (reference is TypeDef td)
                {
                    rv = _typeCache.Get(td);
                    _typeReferenceCache[reference] = rv;
                }
                else if (reference is TypeSpec ts)
                {
                    rv = _typeCache.Get(ts);
                    _typeReferenceCache[reference] = rv;
                }
            }

            return rv;
        }

        public RoslynType GetTypeFromReference(ITypeDefOrRef resolved)
        {
            return _typeCache.Get(resolved);
        }
    }
}