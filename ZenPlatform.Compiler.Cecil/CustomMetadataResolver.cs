using Mono.Cecil;
using Mono.Collections.Generic;

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

        public override MethodDefinition Resolve(MethodReference method)
        {
            TypeDefinition type = this.Resolve(method.DeclaringType);
            if (type == null)
                return (MethodDefinition) null;
            method = method.GetElementMethod();
            if (!type.HasMethods)
                return (MethodDefinition) null;
            return this.GetMethod(type, method);
        }

        private MethodDefinition GetMethod(
            TypeDefinition type,
            MethodReference reference)
        {
            for (; type != null; type = this.Resolve(type.BaseType))
            {
                MethodDefinition method = GetMethod(type.Methods, reference);
                if (method != null)
                    return method;
                if (type.BaseType == null)
                    return (MethodDefinition) null;
            }

            return (MethodDefinition) null;
        }

        public static MethodDefinition GetMethod(
            Collection<MethodDefinition> methods,
            MethodReference reference)
        {
            for (int index = 0; index < methods.Count; ++index)
            {
                var a = MetadataResolver.GetMethod(methods, reference);
                if (a == null)
                {
                    if (methods[index].Name == reference.Name)
                        return methods[index];
                }
                else
                    return a;
            }

            return (MethodDefinition) null;
        }


        private static bool AreSame(TypeReference a, TypeReference b)
        {
            return true;
        }
    }
}