using System;
using Aquila.Compiler.Contracts.Symbols;
using Aquila.Language.Ast.Definitions;
using Aquila.Shared.Tree;

namespace Aquila.Language.Ast
{
    public abstract class SyntaxNode : Node, IAstNode
    {
        public SyntaxNode(ILineInfo lineInfo)
        {
            if (lineInfo != null)
            {
                Line = lineInfo.Line;
                Position = lineInfo.Position;
            }
        }

        public int Line { get; set; }

        public int Position { get; set; }

        public abstract T Accept<T>(AstVisitorBase<T> visitor);
        
        public abstract void Accept(AstVisitorBase visitor);
    }
}