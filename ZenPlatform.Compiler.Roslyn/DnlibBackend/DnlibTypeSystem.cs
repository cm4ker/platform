using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using dnlib.DotNet;

namespace ZenPlatform.Compiler.Roslyn.DnlibBackend
{
    public class SreTypeSystem
    {
        private List<SreAssembly> _asms;

        private AssemblyResolver _resolver;
        //private SreAssemblyResolver _resolver;

        private Dictionary<ITypeDefOrRef, SreType> _typeReferenceCache =
            new Dictionary<ITypeDefOrRef, SreType>();

        private Dictionary<dnlib.DotNet.IAssembly, SreAssembly> _assemblyDic
            = new Dictionary<dnlib.DotNet.IAssembly, SreAssembly>();

        private Dictionary<string, SreType> _qNameAssemblies = new Dictionary<string, SreType>();

        private Dictionary<string, SreType> _unresolvedTypeCache = new Dictionary<string, SreType>();


        private SreTypeCache _typeCache;

        public SreTypeSystem(SrePlatformFactory factory, IEnumerable<string> paths, string targetPath = null)
        {
            _asms = new List<SreAssembly>();

            // _resolver = new AssemblyResolver();
            // _resolver.UseGAC = false;
            // _resolver.FindExactMatch = false;
            //
            // _resolver.PreSearchPaths.Add(
            //     //@"C:\Program Files\dotnet\shared\Microsoft.NETCore.App\3.1.1"
            //     "C:\\Program Files\\dotnet\\packs\\Microsoft.NETCore.App.Ref\\3.1.0\\ref\\netcoreapp3.1"
            // );
            // _resolver.PreSearchPaths.Add(AppContext.BaseDirectory);
            //
            // _resolver.DefaultModuleContext = new ModuleContext(_resolver, new Resolver(_resolver));

            _resolver = new AssemblyResolver();
            _resolver.PreSearchPaths.Add(
                "C:\\Program Files\\dotnet\\packs\\Microsoft.NETCore.App.Ref\\3.1.0\\ref\\netcoreapp3.1\\");
            _resolver.PreSearchPaths.Add(Directory.GetCurrentDirectory());
            _resolver.EnableTypeDefCache = true;
            _resolver.EnableFrameworkRedirect = false;
            _resolver.DefaultModuleContext = new ModuleContext(Resolver);
            _typeCache = new SreTypeCache(this);

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
                    var asm = AssemblyDef.Load(path, new ModuleCreationOptions());
                    RegisterAssembly(asm);
                    Paths.Add(path);
                }
            }


            Factory = factory;
        }

        public IAssemblyResolver Resolver => _resolver;

        public List<string> Paths { get; } = new List<string>();
        public SrePlatformFactory Factory { get; }

        public IReadOnlyList<SreAssembly> Assemblies => _asms;

        public Dictionary<string, AssemblyRef> AsmRefsCache = new Dictionary<string, AssemblyRef>();

        internal SreAssembly RegisterAssembly(AssemblyDef assemblyDef)
        {
            var result = new SreAssembly(this, assemblyDef);
            return RegisterAssembly(result);
        }

        internal SreAssembly RegisterAssembly(SreAssembly assembly)
        {
            if (_assemblyDic.TryGetValue(assembly.Assembly, out var result))
                return result;

            _asms.Add(assembly);
            _assemblyDic[assembly.Assembly] = assembly;
            //_resolver.RegisterAsm(assembly.Assembly);

            return assembly;
        }

        internal SreType RegisterType(SreType type)
        {
            _typeCache.RegisterType(type);
            return type;
        }

        public SreAssembly FindAssembly(string assembly)
        {
            if (assembly == "System.Private.CoreLib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=7cec85d7bea7798e")
                assembly = AssemblyRefUser.CreateMscorlibReferenceCLR40().FullName;

            return RegisterAssembly(_resolver.Resolve(assembly, null));
        }

        public SreAssembly FindAssembly(dnlib.DotNet.IAssembly assembly)
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

        public SreType FindType(string name)
        {
            foreach (var asm in _asms)
            {
                var found = asm.FindType(name);
                if (found != null)
                    return found;
            }

            return null;
        }

        public SreType Resolve(Type type)
        {
            return FindType(type.FullName, type.Assembly.FullName);
        }

        public SreType Resolve<T>()
        {
            return Resolve(typeof(T));
        }

        public SreType ResolveType(Type type)
        {
            return Resolve(type);
        }

        public SreType FindType(string name, string assembly) =>
            FindAssembly(assembly)?.FindType(name);

        public SreType Resolve(ITypeDefOrRef reference)
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

        public SreType GetTypeFromReference(ITypeDefOrRef resolved)
        {
            return _typeCache.Get(resolved);
        }
    }

    internal class SreTypeCache
    {
        public SreTypeSystem TypeSystem { get; }

        Dictionary<ITypeDefOrRef, DefinitionEntry> _cache = new Dictionary<ITypeDefOrRef, DefinitionEntry>();

        public SreTypeCache(SreTypeSystem typeSystem)
        {
            TypeSystem = typeSystem;
        }

        class DefinitionEntry
        {
            public SreType Direct { get; set; }
        }

        public void RegisterType(SreType type)
        {
            _cache[type.TypeDef] = new DefinitionEntry {Direct = type};
        }

        public SreType Get(ITypeDefOrRef defOrRef)
        {
            //var r = new SreContextResolver(TypeSystem, defOrRef.Module);

            if (defOrRef.ToTypeSig() is GenericVar)
                return null;

            var definition = defOrRef.ResolveTypeDef();

            IResolutionScope scope = defOrRef.Scope as IResolutionScope;

            //if(defOrRef is TypeDef tda || defOrRef is TypeRef tra)
            var reference = new TypeRefUser(defOrRef.Module, defOrRef.Namespace, defOrRef.Name, scope);
            var na = reference.FullName;
            var asm = TypeSystem.FindAssembly(definition.Module.Assembly);

            if (defOrRef is TypeSpec ts)
            {
                return new SreType(TypeSystem, ts.ResolveTypeDef(), ts, asm);
            }

            if (!_cache.TryGetValue(definition, out var definitionEntry))
                _cache[definition] = definitionEntry = new DefinitionEntry();
            else
                return definitionEntry.Direct;

            if (defOrRef is TypeDef def)
                return definitionEntry.Direct ??
                       (definitionEntry.Direct = new SreType(TypeSystem, def, reference, asm));


            return definitionEntry.Direct ??
                   (definitionEntry.Direct = new SreType(TypeSystem, definition, reference, asm));
        }
    }


    public static class TypeSystemExtensinos
    {
        public static SystemTypeBindings GetSystemBindings(this SreTypeSystem ts)
        {
            return new SystemTypeBindings(ts);
        }


        public static SreType FindType<T>(this SreTypeSystem ts)
        {
            return FindType(ts, typeof(T));
        }

        public static SreType FindType(this SreTypeSystem ts, Type type)
        {
            var name = type.Name;
            var @namespace = type.Namespace;
            var assembly = type.Assembly.GetName().FullName;

            if (assembly.Contains("System.Private.CoreLib"))
                assembly = ts.GetSystemBindings().MSCORLIB;

            return ts.FindType($"{@namespace}.{name}", assembly);
        }
    }
}