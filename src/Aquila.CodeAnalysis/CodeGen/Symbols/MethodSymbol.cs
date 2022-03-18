using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics;
using System.Reflection.Metadata;
using System.Text;
using Aquila.Compiler.Utilities;
using Microsoft.CodeAnalysis;
using Aquila.CodeAnalysis.CodeGen;
using Aquila.CodeAnalysis.Emit;
using Aquila.CodeAnalysis.Semantics;

namespace Aquila.CodeAnalysis.Symbols
{
    internal abstract partial class MethodSymbol
    {
        internal bool HasParamPlatformContext
        {
            get
            {
                var ps = Parameters;

                if (ps.Length != 0 && SpecialParameterSymbol.IsContextParameter(ps[0]))
                {
                    return true; // <ctx>
                }
                else
                {
                    return false;
                }
            }
        }
    }
}