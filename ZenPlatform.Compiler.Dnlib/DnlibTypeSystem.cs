using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Reflection.Metadata;
using dnlib.DotNet;
using ZenPlatform.Compiler.Contracts;
using IAssembly = ZenPlatform.Compiler.Contracts.IAssembly;
using IType = ZenPlatform.Compiler.Contracts.IType;

namespace ZenPlatform.Compiler.Dnlib
{
    public class DnlibTypeSystem : ITypeSystem
    {
        private List<DnlibAssembly> _asms;
        private DnlibAssemblyResolver _resolver;

        private Dictionary<ITypeDefOrRef, IType> _typeReferenceCache =
            new Dictionary<ITypeDefOrRef, IType>();

        private Dictionary<dnlib.DotNet.IAssembly, DnlibAssembly> _assemblyDic
            = new Dictionary<dnlib.DotNet.IAssembly, DnlibAssembly>();

        private Dictionary<string, IType> _qNameAssemblies = new Dictionary<string, IType>();

        private Dictionary<string, IType> _unresolvedTypeCache = new Dictionary<string, IType>();

        private DnlibTypeCache _typeCache;

        public DnlibTypeSystem(IEnumerable<string> paths, string targetPath = null)
        {
            _asms = new List<DnlibAssembly>();
            _resolver = new DnlibAssemblyResolver();

            _typeCache = new DnlibTypeCache(this);

            if (targetPath != null)
                paths = paths.Concat(new[] {targetPath});


            foreach (var path in paths.Distinct())
            {
                var asm = AssemblyDef.Load(path, new ModuleCreationOptions());
                RegisterAssembly(asm);
            }
        }

        public DnlibAssemblyResolver Resolver => _resolver;

        public IWellKnownTypes WellKnownTypes { get; }

        public IReadOnlyList<IAssembly> Assemblies => _asms;


        public Dictionary<string, AssemblyRef> AsmRefsCache = new Dictionary<string, AssemblyRef>();


        internal IAssembly RegisterAssembly(AssemblyDef assemblyDef)
        {
            var result = new DnlibAssembly(this, assemblyDef);
            return RegisterAssembly(result);
        }

        internal IAssembly RegisterAssembly(DnlibAssembly assembly)
        {
            _asms.Add(assembly);
            _assemblyDic[assembly.Assembly] = assembly;
            Resolver.RegisterAsm(assembly.Assembly);
            return assembly;
        }

        public IAssembly FindAssembly(string assembly)
        {
            return RegisterAssembly(_resolver.Resolve(assembly, null));
        }

        public IAssembly FindAssembly(dnlib.DotNet.IAssembly assembly)
        {
            if (assembly.IsCorLib())
            {
                return _assemblyDic.FirstOrDefault(x => x.Key.Name == "mscorlib").Value;
            }

            return _assemblyDic[assembly];
        }

        public IType FindType(string name)
        {
            foreach (var asm in _asms)
            {
                var found = asm.FindType(name);
                if (found != null)
                    return found;
            }

            return null;
        }

        public IType FindType(string name, string assembly) =>
            FindAssembly(assembly)?.FindType(name);

        public IType Resolve(ITypeDefOrRef reference)
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
                            _unresolvedTypeCache[key] =
                                rv = new UnresolvedDnlibType(tr);
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

        public DnlibType GetTypeFromReference(ITypeDefOrRef resolved)
        {
            return _typeCache.Get(resolved);
        }
    }

    internal class DnlibTypeCache
    {
        public DnlibTypeSystem TypeSystem { get; }

        Dictionary<TypeDef, DefinitionEntry> _definitions = new Dictionary<TypeDef, DefinitionEntry>();

        public DnlibTypeCache(DnlibTypeSystem typeSystem)
        {
            TypeSystem = typeSystem;
        }

        class DefinitionEntry
        {
            public DnlibType Direct { get; set; }
            public Dictionary<Type, List<DnlibType>> References { get; } = new Dictionary<Type, List<DnlibType>>();
        }

        public void RegisterType(DnlibType type)
        {
          
        }

        public DnlibType Get(ITypeDefOrRef defOrRef)
        {
            //var r = new DnlibContextResolver(TypeSystem, defOrRef.Module);

            var definition = defOrRef.ResolveTypeDef();

            IResolutionScope scope = defOrRef.Scope as IResolutionScope;

            if (scope is null) throw new Exception("Test");

            //if(defOrRef is TypeDef tda || defOrRef is TypeRef tra)
            var reference = new TypeRefUser(defOrRef.Module, defOrRef.Namespace, defOrRef.Name, scope);
            var na = reference.FullName;
            var asm = (DnlibAssembly) TypeSystem.FindAssembly(definition.Module.Assembly);

            if (defOrRef is TypeSpec ts)
            {
                return new DnlibType(TypeSystem, ts.ResolveTypeDef(), ts, asm);
            }

            if (!_definitions.TryGetValue(definition, out var dentry))
                _definitions[definition] = dentry = new DefinitionEntry();
            if (defOrRef is TypeDef def)
                return dentry.Direct ?? (dentry.Direct = new DnlibType(TypeSystem, def, reference, asm));

            var rtype = reference.GetType();
            if (!dentry.References.TryGetValue(rtype, out var rlist))
                dentry.References[rtype] = rlist = new List<DnlibType>();
            var found = rlist.FirstOrDefault(t => t.TypeDef.Equals(definition));
            if (found != null)
                return found;
            var rv = new DnlibType(TypeSystem, definition, reference, asm);
            rlist.Add(rv);
            return rv;
        }
    }
}