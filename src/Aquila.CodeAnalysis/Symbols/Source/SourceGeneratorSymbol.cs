using System.Diagnostics;
using Aquila.CodeAnalysis.Symbols.Synthesized;

namespace Aquila.CodeAnalysis.Symbols
{
    /// <summary>
    /// Synthesized method representing the generator state machine's next function.
    /// </summary>
    internal sealed partial class SourceGeneratorSymbol : SynthesizedMethodSymbol
    {
        public SourceGeneratorSymbol(SourceMethodSymbol originalMethod)
            : base(originalMethod.ContainingType,
                string.Format(WellKnownAquilaNames.GeneratorStateMachineNameFormatString, originalMethod.MethodName),
                isstatic: true, isvirtual: false,
                returnType: originalMethod.DeclaringCompilation.CoreTypes.Void, ps: null)
        {
            // Need to postpone settings the params because I can't access 'this' in base constructor call
            base.SetParameters(CreateParameters(originalMethod));
        }

        /// <summary>
        /// Parameters for <see cref="SourceGeneratorSymbol"/> method are defined by <c>GeneratorStateMachineDelegate</c>.
        /// </summary>
        ParameterSymbol[] CreateParameters(SourceMethodSymbol originalMethod)
        {
            // resolve type of $this
            var thisType = originalMethod.ContainingType ??
                           (TypeSymbol)originalMethod.DeclaringCompilation.ObjectType;

            // yield sm method signature
            var index = 0;
            return new[]
            {
                ThisParameter = new SpecialParameterSymbol(this, thisType, SpecialParameterSymbol.ThisName, index++),
            };
        }

        public ParameterSymbol ContextParameter { get; private set; }
        public ParameterSymbol ThisParameter { get; private set; }
        public ParameterSymbol LocalsParameter { get; private set; }
        public ParameterSymbol TmpLocalsParameter { get; private set; }
        public ParameterSymbol GeneratorParameter { get; private set; }
    }
}