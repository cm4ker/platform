using System;
using Mono.Cecil;
using ZenPlatform.Compiler.AST.Definitions;

namespace ZenPlatform.Compiler.Cecil.Backend
{
    public class TypeResolver
    {
        private readonly CompilationUnit _cu;
        private readonly AssemblyDefinition _relativeAssembly;
        private readonly AssemblyDefinition _coreAssembly;

        public TypeResolver(CompilationUnit cu, AssemblyDefinition relativeAssembly)
        {
            _cu = cu;
            _relativeAssembly = relativeAssembly;

            var asmNr = (AssemblyNameReference) _relativeAssembly.MainModule.TypeSystem.CoreLibrary;
            var asmNd = new AssemblyNameDefinition(asmNr.Name, asmNr.Version);
            _coreAssembly = _relativeAssembly.MainModule.AssemblyResolver.Resolve(asmNd);
        }

        public TypeReference Resolve(ZType type)
        {
            var m = _relativeAssembly.MainModule;

            if (type.IsSystem)
            {
                switch (type)
                {
                    case ZInt t:
                        return m.TypeSystem.Int32;
                    case ZBool t:
                        return m.TypeSystem.Boolean;
                    case ZCharacter t:
                        return m.TypeSystem.Char;
                    case ZDouble t:
                        return m.TypeSystem.Double;
                    case ZVoid t:
                        return m.TypeSystem.Void;
                    case ZString t:
                        return m.TypeSystem.String;
                }
            }
            else if (!type.IsSystem && !type.IsArray && type is ZStructureType st)
            {
                if (type is null)
                {
                    TypeReference tr = null;
                    foreach (var ns in _cu.Namespaces)
                    {
                        if (tr != null) throw new Exception("Can't certainly identify the type");
                        tr = Resolve(ns, type.Name);
                    }

                    if (tr is null)
                        throw new Exception("Type not found: " + tr.Name);
                }

                return Resolve(type.Name);
            }
            else if (type.IsArray && type is ZArray a)
            {
                return new ArrayType(Resolve(a.TypeOfElements));
            }

            return null;
        }

        public TypeReference Resolve(string @namespace, string typeName)
        {
            foreach (var module in _relativeAssembly.Modules)
            {
                var result = module.GetType(@namespace, typeName);

                if (result != null) return result;
            }

            return null;
        }


        public ZType ResolvePlatformType(TypeReference typeReference)
        {
            var result = new ZStructureType(typeReference.Name, typeReference.Namespace);

            return result;
        }
    }
}