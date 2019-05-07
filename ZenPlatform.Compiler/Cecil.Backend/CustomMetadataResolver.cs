using System;
using Mono.Cecil;

namespace ZenPlatform.Compiler.Cecil.Backend
{
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