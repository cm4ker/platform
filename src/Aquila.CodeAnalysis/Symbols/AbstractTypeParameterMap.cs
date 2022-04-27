using System.Diagnostics;
using System.Text;
using Microsoft.CodeAnalysis;

namespace Aquila.CodeAnalysis.Symbols
{
    [DebuggerDisplay("{GetDebuggerDisplay(), nq}")]
    internal abstract class AbstractTypeParameterMap : AbstractTypeMap
    {
        protected readonly SmallDictionary<TypeParameterSymbol, TypeWithAnnotations> Mapping;

        protected AbstractTypeParameterMap(SmallDictionary<TypeParameterSymbol, TypeWithAnnotations> mapping)
        {
            this.Mapping = mapping;
        }

        protected sealed override TypeWithAnnotations SubstituteTypeParameter(TypeParameterSymbol typeParameter)
        {
            // It might need to be substituted directly.
            TypeWithAnnotations result;
            if (Mapping.TryGetValue(typeParameter, out result))
            {
                return result;
            }

            return TypeWithAnnotations.Create(typeParameter);
        }

        private string GetDebuggerDisplay()
        {
            var result = new StringBuilder("[");
            result.Append(this.GetType().Name);
            foreach (var kv in Mapping)
            {
                result.Append(" ").Append(kv.Key).Append(":").Append(kv.Value.Type);
            }

            return result.Append("]").ToString();
        }
    }
}