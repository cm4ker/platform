using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using ZenPlatform.Compiler.Contracts.Symbols;
using ZenPlatform.Language.Ast.AST.Definitions.Functions;
using ZenPlatform.Language.Ast.AST.Definitions.Statements;
using ZenPlatform.Language.Ast.AST.Infrastructure;

namespace ZenPlatform.Language.Ast.AST.Definitions
{
    /// <summary>
    /// Блок инструкций
    /// </summary>
    public class BlockNode : SyntaxNode, IScoped
    {
        public IReadOnlyList<Statement> Statements { get; }

        public SymbolTable SymbolTable { get; set; }

        /// <summary>
        /// Создать блок из коллекции инструкций
        /// </summary>
        public BlockNode(ImmutableList<Statement> statements) : base(null)
        {
            if (statements == null)
                return;

            Statements = statements;

            var slot = 0;
            foreach (var statement in Statements)
            {
                Children.SetSlot(statement, slot++);
            }
        }

        public override T Accept<T>(AstVisitorBase<T> visitor)
        {
            return visitor.VisitBlock(this);
        }
    }

    /// <summary>
    /// Описывает тело типа (методы, поля, события, конструкторы и т.д.)
    /// </summary>
    public class TypeBody : SyntaxNode, IScoped
    {
        private readonly List<Function> _functions;
        private readonly List<Field> _fields;
        private readonly List<Property> _properties;

        public IReadOnlyList<Function> Functions => _functions;

        public IReadOnlyList<Field> Fields => _fields;

        public IReadOnlyList<Property> Properties => _properties;

        public SymbolTable SymbolTable { get; set; }


        public TypeBody(ImmutableList<Member> members) : base(null)
        {
            _functions = new FunctionCollection();
            _fields = new FieldCollection();
            _properties = new PropertyCollection();

            if (members == null)
                return;

            var slot = 0;

            foreach (Member member in members)
            {
                if (member is Function func)
                    _functions.Add(func);

                if (member is Field field)
                    _fields.Add(field);

                if (member is Property prop)
                    _properties.Add(prop);

                Children.SetSlot(member, slot++);
            }
        }

        public override T Accept<T>(AstVisitorBase<T> visitor)
        {
            return visitor.VisitTypeBody(this);
        }
    }

    public interface IScoped : IAstNode
    {
        SymbolTable SymbolTable { get; set; }
    }
}