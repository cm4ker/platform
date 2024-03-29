﻿using System.Collections.Immutable;
using Microsoft.CodeAnalysis;

namespace Aquila.CodeAnalysis.Symbols.Synthesized
{
    /// <summary>
    /// Synthesized method representing implementation of used trait method inside a containing class.
    /// </summary>
    sealed class SynthesizedTraitMethodSymbol : SynthesizedMethodSymbol
    {
        public SynthesizedTraitMethodSymbol(TypeSymbol containingType, string name, MethodSymbol traitmethod,
            Accessibility accessibility, bool isfinal = true)
            : base(containingType, name, traitmethod.IsStatic, !traitmethod.IsStatic, null, accessibility, isfinal)
        {
            _parameters = default; // as uninitialized

            this.ForwardedCall = traitmethod;
        }

        public override ImmutableArray<ParameterSymbol> Parameters
        {
            get
            {
                if (!_parameters.IsDefault && _parameters.Length != ForwardedCall.Parameters.Length)
                {
                    // parameters has changed during analysis,
                    // reset this as well
                    // IMPORTANT: we must not change it when emit started already
                    _parameters = default;
                }

                if (_parameters.IsDefault)
                {
                    ImmutableInterlocked.InterlockedInitialize(ref _parameters,
                        SynthesizedParameterSymbol.Create(this, ForwardedCall.Parameters));
                }

                //
                return _parameters;
            }
        }
    }
}