using System.Collections.Generic;
using dnlib.DotNet;

namespace Aquila.Compiler.Roslyn.RoslynBackend
{
    internal class RoslynTypeCache
    {
        public RoslynTypeSystem TypeSystem { get; }

        Dictionary<ITypeDefOrRef, DefinitionEntry> _cache = new Dictionary<ITypeDefOrRef, DefinitionEntry>();

        public RoslynTypeCache(RoslynTypeSystem typeSystem)
        {
            TypeSystem = typeSystem;
        }

        class DefinitionEntry
        {
            public RoslynType Direct { get; set; }
        }

        public void RegisterType(RoslynType type)
        {
            _cache[type.TypeDef] = new DefinitionEntry {Direct = type};
        }

        public RoslynType Get(ITypeDefOrRef defOrRef)
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
                return new RoslynType(TypeSystem, ts.ResolveTypeDef(), ts, asm);
            }

            if (!_cache.TryGetValue(definition, out var definitionEntry))
                _cache[definition] = definitionEntry = new DefinitionEntry();
            else
                return definitionEntry.Direct;

            if (defOrRef is TypeDef def)
                return definitionEntry.Direct ??
                       (definitionEntry.Direct = new RoslynType(TypeSystem, def, reference, asm));


            return definitionEntry.Direct ??
                   (definitionEntry.Direct = new RoslynType(TypeSystem, definition, reference, asm));
        }
    }
}