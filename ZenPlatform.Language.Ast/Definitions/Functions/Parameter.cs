using System;
using ZenPlatform.Compiler.Contracts.Symbols;
using ZenPlatform.Language.Ast.AST;
using ZenPlatform.Language.Ast.Infrastructure;

namespace ZenPlatform.Language.Ast.Definitions.Functions
{
    /// <summary>
    /// Описывает параметр
    /// </summary>
    public partial class Parameter : ITypedNode
    {
        public Parameter(ILineInfo li, string name, TypeSyntax type, PassMethod pm) : this(li, name, pm)
        {
            Type = type;
        }


        public SymbolType SymbolType => SymbolType.Variable;
        public SymbolScope SymbolScope { get; set; }
        public TypeSyntax Type { get; set; }
    }
}