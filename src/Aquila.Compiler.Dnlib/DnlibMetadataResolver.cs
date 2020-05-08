using System;
using System.Collections.Generic;
using dnlib.DotNet;

namespace Aquila.Compiler.Dnlib
{
    public class DnlibMetadataResolver : IResolver
    {
        private Resolver _resolver;

        public DnlibMetadataResolver(IAssemblyResolver asmResolver)
        {
            _resolver = new Resolver(asmResolver);
        }

        public TypeDef Resolve(TypeRef typeRef, ModuleDef sourceModule)
        {
            return _resolver.Resolve(typeRef, sourceModule);
        }

        public IMemberForwarded Resolve(MemberRef memberRef)
        {
            return _resolver.Resolve(memberRef);
        }
    }
}