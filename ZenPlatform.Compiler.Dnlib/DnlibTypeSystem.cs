using System;
using System.Collections.Generic;
using System.Linq;
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

        private Dictionary<TypeReference, IType> _typeReferenceCache =
            new Dictionary<TypeReference, IType>();

        private DnlibTypeCache _typeCache;

        public DnlibTypeSystem(IEnumerable<string> paths, string targetPath = null)
        {
            _asms = new List<DnlibAssembly>();
            _resolver = new DnlibAssemblyResolver();

            if (targetPath != null)
                paths = paths.Concat(new[] {targetPath});


            foreach (var path in paths.Distinct())
            {
                var asm = AssemblyDef.Load(path, new ModuleCreationOptions());
                RegisterAssembly(asm);
            }
        }

        public DnlibAssemblyResolver Resolver => _resolver;

        public IReadOnlyList<IAssembly> Assemblies => _asms;

        internal IAssembly RegisterAssembly(AssemblyDef assemblyDef)
        {
            var result = new DnlibAssembly(this, assemblyDef);
            _asms.Add(result);
            return result;
        }

        public IAssembly FindAssembly(string assembly)
        {
            return RegisterAssembly(_resolver.Resolve(assembly, null));
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

        public IType Resolve(TypeDef type)
        {
            if (!_typeReferenceCache.TryGetValue(reference, out var rv))
            {
                TypeDefinition resolved = null;
                try
                {
                    resolved = reference.Resolve();
                }
                catch (AssemblyResolutionException)
                {
                }

                if (resolved != null)
                {
                    rv = _typeCache.Get(reference);
                }
                else
                {
                    var key = reference.FullName;
                    if (reference is GenericParameter gp)
                        key = ((TypeReference) gp.Owner).FullName + "|GenericParameter|" + key;
                    if (!_unresolvedTypeCache.TryGetValue(key, out rv))
                        _unresolvedTypeCache[key] =
                            rv = new UnresolvedCecilType(reference);
                }

                _typeReferenceCache[reference] = rv;
            }
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


        public DnlibType Get(TypeRef reference)
        {
            var definition = reference.Resolve();
            var asm = TypeSystem.FindAsm(definition.Module.Assembly);
            if (!_definitions.TryGetValue(definition, out var dentry))
                _definitions[definition] = dentry = new DefinitionEntry();
            if (reference is TypeDefinition def)
                return dentry.Direct ?? (dentry.Direct = new CecilType(TypeSystem, asm, def));

            var rtype = reference.GetType();
            if (!dentry.References.TryGetValue(rtype, out var rlist))
                dentry.References[rtype] = rlist = new List<CecilType>();
            var found = rlist.FirstOrDefault(t => CecilHelpers.Equals(t.Reference, reference));
            if (found != null)
                return found;
            var rv = new CecilType(TypeSystem, asm, definition, reference);
            rlist.Add(rv);
            return rv;
        }
    }
}