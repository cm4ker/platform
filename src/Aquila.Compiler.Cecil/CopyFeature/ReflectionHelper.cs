using System.Collections.Generic;
using System.Linq;
using Mono.Cecil;
using Mono.Collections.Generic;

namespace Aquila.Compiler.Cecil.CopyFeature
{
    internal class ReflectionHelper
    {
        private readonly IRepackContext _repack;

        internal ReflectionHelper(IRepackContext repack)
        {
            _repack = repack;
        }

        internal MethodDefinition FindMethodDefinitionInType(TypeDefinition type, MethodReference method)
        {
            return type.Methods.FirstOrDefault(
                x => x.Name == method.Name &&
                     AreSame(x.Parameters, method.Parameters) &&
                     AreSame(x.ReturnType, method.ReturnType) &&
                     x.GenericParameters.Count == method.GenericParameters.Count
            );
        }

        // nasty copy from MetadataResolver.cs for now
        internal bool AreSame(IList<ParameterDefinition> a, IList<ParameterDefinition> b)
        {
            var count = a.Count;

            if (count != b.Count)
                return false;

            if (count == 0)
                return true;

            for (int i = 0; i < count; i++)
                if (!AreSame(a[i].ParameterType, b[i].ParameterType))
                    return false;

            return true;
        }

        internal bool AreSame(TypeSpecification a, TypeSpecification b)
        {
            if (!AreSame(a.ElementType, b.ElementType))
                return false;

            if (a.IsGenericInstance)
                return AreSame((GenericInstanceType) a, (GenericInstanceType) b);

            if (a.IsRequiredModifier || a.IsOptionalModifier)
                return AreSame((IModifierType) a, (IModifierType) b);

            if (a.IsArray)
                return AreSame((ArrayType) a, (ArrayType) b);

            return true;
        }

        internal bool AreSame(ArrayType a, ArrayType b)
        {
            if (a.Rank != b.Rank)
                return false;

            // TODO: dimensions

            return true;
        }

        internal bool AreSame(IModifierType a, IModifierType b)
        {
            return AreSame(a.ModifierType, b.ModifierType);
        }

        internal bool AreSame(GenericInstanceType a, GenericInstanceType b)
        {
            if (!a.HasGenericArguments)
                return !b.HasGenericArguments;

            if (!b.HasGenericArguments)
                return false;

            if (a.GenericArguments.Count != b.GenericArguments.Count)
                return false;

            for (int i = 0; i < a.GenericArguments.Count; i++)
                if (!AreSame(a.GenericArguments[i], b.GenericArguments[i]))
                    return false;

            return true;
        }

        internal bool AreSame(GenericParameter a, GenericParameter b)
        {
            return a.Position == b.Position;
        }

        internal bool AreSame(TypeReference a, TypeReference b)
        {
            if (a == b)
                return true;
            if (a == null || b == null)
                return false;
            a = _repack.GetMergedTypeFromTypeRef(a) ?? a;
            b = _repack.GetMergedTypeFromTypeRef(b) ?? b;

            if (a.MetadataType != b.MetadataType)
                return false;

            if (a.IsGenericParameter)
                return AreSame((GenericParameter) a, (GenericParameter) b);

            if (a is TypeSpecification)
                return AreSame((TypeSpecification) a, (TypeSpecification) b);

            return a.FullName == b.FullName;
        }

        internal bool AreSame(Collection<CustomAttributeArgument> a, Collection<CustomAttributeArgument> b)
        {
            if (a.Count != b.Count)
                return false;
            for (int i = 0; i < a.Count; i++)
            {
                if (!AreSame(a[i], b[i]))
                    return false;
            }

            return true;
        }

        internal bool AreSame(CustomAttributeArgument a, CustomAttributeArgument b)
        {
            if (!AreSame(a.Type, b.Type))
                return false;
            if (a.Value == b.Value)
                return true;
            if (a.Value == null)
                return false;
            if (!a.Value.Equals(b.Value))
                return false;
            return true;
        }

        internal bool AreSame(Collection<CustomAttributeNamedArgument> a, Collection<CustomAttributeNamedArgument> b)
        {
            if (a.Count != b.Count)
                return false;
            foreach (var argA in a)
            {
                var argB = b.FirstOrDefault(x => x.Name == argA.Name);
                if (argB.Name == null)
                    return false;
                if (!AreSame(argA.Argument, argB.Argument))
                    return false;
            }

            return true;
        }
    }
}