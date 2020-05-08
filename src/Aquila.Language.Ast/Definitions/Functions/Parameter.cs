using System;
using Aquila.Compiler.Contracts.Symbols;
using Aquila.Language.Ast.AST;
using Aquila.Language.Ast.Infrastructure;
using Aquila.Language.Ast.Symbols;

namespace Aquila.Language.Ast.Definitions.Functions
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

        public SymbolScopeBySecurity SymbolScope { get; set; }

        public TypeSyntax Type
        {
            get => (TypeSyntax) this.Children[0];
            set => this.Attach(0, (SyntaxNode) value.Clone());
        }
    }
}