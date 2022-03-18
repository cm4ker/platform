using Microsoft.CodeAnalysis.Emit;
using Aquila.CodeAnalysis.Emit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis.Symbols;
using Cci = Microsoft.Cci;

namespace Aquila.CodeAnalysis.Symbols
{
    internal partial class CSharpCustomModifier : Cci.ICustomModifier
    {
        bool Cci.ICustomModifier.IsOptional
        {
            get { return this.IsOptional; }
        }

        Cci.ITypeReference Cci.ICustomModifier.GetModifier(EmitContext context)
        {
            return ((PEModuleBuilder)context.Module).Translate((ITypeSymbolInternal)this.Modifier, context.SyntaxNode, context.Diagnostics);
        }
    }
}