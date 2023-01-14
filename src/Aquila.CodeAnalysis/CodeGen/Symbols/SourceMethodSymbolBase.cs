using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Aquila.CodeAnalysis.CodeGen;
using Aquila.CodeAnalysis.Emit;
using Microsoft.CodeAnalysis;

namespace Aquila.CodeAnalysis.Symbols;

partial class SourceMethodSymbolBase
{
    /// <summary>
    /// Gets place referring to <c>Aquila.Core.AqContext</c> object.
    /// </summary>
    internal IPlace GetContextPlace(PEModuleBuilder module)
    {
        if (IsStatic)
        {
            if (HasParamPlatformContext)
            {
                return new ParamPlace(Parameters[0]); // <ctx>
            }
        }
        else
        {
            var thisPlace = GetThisPlace();
            if (thisPlace != null)
            {
                // <this>.<ctx> in instance methods
                var t = this.ContainingType;

                var ctx_field = t.GetMembers(SpecialParameterSymbol.ContextName).OfType<FieldSymbol>()
                    .FirstOrDefault();

                if (ctx_field != null) // might be null in interfaces
                {
                    return new FieldPlace(ctx_field, thisPlace, module);
                }
                else
                {
                    Debug.Assert(t.IsInterface);
                    return null;
                }
            }
        }

        return null;
    }

    /// <summary>
    /// Gets place of <c>this</c> parameter in CLR corresponding to <c>current class instance</c>.
    /// </summary>
    internal IPlace GetThisPlace()
    {
        return this.HasThis
            ? new ArgPlace(ContainingType, 0)
            : null;
    }

    /// <summary>
    /// Creates ghost stubs,
    /// i.e. methods with a different signature calling this method to comply with CLR standards.
    /// </summary>
    /// <returns>List of additional overloads.</returns>
    internal virtual IList<MethodSymbol> SynthesizeStubs(PEModuleBuilder module, DiagnosticBag diagnostic)
    {
        //
        EmitParametersDefaultValue(module, diagnostic);

        // TODO: resolve this already in SourceTypeSymbol.GetMembers(), now it does not get overloaded properly
        return SynthesizeOverloadsWithOptionalParameters(module, diagnostic);
    }

    /// <summary>
    /// Synthesizes method overloads in case there are optional parameters which explicit default value cannot be resolved as a <see cref="ConstantValue"/>.
    /// </summary>
    protected IList<MethodSymbol> SynthesizeOverloadsWithOptionalParameters(PEModuleBuilder module,
        DiagnosticBag diagnostic)
    {
        List<MethodSymbol> list = null;

        var implparams = ImplicitParameters;
        var srcparams = SourceParameters;
        var implicitVarArgs = VarargsParam;

        for (int i = 0; i <= srcparams.Length; i++) // how many to be copied from {srcparams}
        {
            var isfake = /*srcparams[i - 1].IsFake*/ implicitVarArgs != null && i > 0 &&
                                                     srcparams[i - 1].Ordinal >=
                                                     implicitVarArgs
                                                         .Ordinal; // parameter was replaced with [params]
            var
                hasdefault =
                    false; // i < srcparams.Length && srcparams[i].HasUnmappedDefaultValue();  // ConstantValue couldn't be resolved for optional parameter
        }

        return list ?? (IList<MethodSymbol>)Array.Empty<MethodSymbol>();
    }

    private void EmitParametersDefaultValue(PEModuleBuilder module, DiagnosticBag diagnostics)
    {
    }

    public virtual void Generate(CodeGenerator cg)
    {
        cg.GenerateScope(this.ControlFlowGraph.Start, int.MaxValue);
    }

    private void GenerateGeneratorMethod(CodeGenerator cg)
    {
    }
}