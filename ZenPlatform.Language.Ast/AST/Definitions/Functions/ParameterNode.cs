using System;
using ZenPlatform.Compiler.AST.Definitions.Symbols;
using ZenPlatform.Compiler.Contracts.Symbols;
using ZenPlatform.Language.Ast.AST.Infrastructure;

namespace ZenPlatform.Language.Ast.AST.Definitions.Functions
{
    /// <summary>
    /// Описывает параметр
    /// </summary>
    public class ParameterNode : SyntaxNode, ITypedNode, IAstSymbol
    {
        private string _name;

        /// <summary>
        /// Parameter type.
        /// </summary>
        public TypeNode Type { get; set; }

        /// <summary>
        /// Parameter pass method.
        /// </summary>
        public PassMethod PassMethod { get; set; }

        /// <summary>
        /// Create parameter object.
        /// </summary>
        public ParameterNode(ILineInfo li, string name, TypeNode type, PassMethod passMethod) : base(li)
        {
            _name = name;
            Type = type;
            PassMethod = passMethod;
        }

        public string Name
        {
            get => _name;
        }

        public SymbolType SymbolType => SymbolType.Variable;

        public override T Accept<T>(AstVisitorBase<T> visitor)
        {
            return visitor.VisitParameter(this);
        }
    }
}