using Mono.Cecil;

namespace ZenPlatform.Compiler.Cecil
{
    public class CustomMetadataResolver : MetadataResolver
    {
        private readonly CecilTypeSystem _typeSystem;

        public CustomMetadataResolver(CecilTypeSystem typeSystem) : base(typeSystem)
        {
            _typeSystem = typeSystem;
        }

        public override TypeDefinition Resolve(TypeReference type)
        {
            type = type.GetElementType();
            
            
            
            var resolved = base.Resolve(type);
            return resolved;
        }
    }
}