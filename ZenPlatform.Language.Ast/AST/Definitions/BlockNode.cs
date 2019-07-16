using System;
using ZenPlatform.Compiler.Contracts.Symbols;
using ZenPlatform.Language.Ast.AST.Definitions.Functions;
using ZenPlatform.Language.Ast.AST.Definitions.Statements;
using ZenPlatform.Language.Ast.AST.Infrastructure;

namespace ZenPlatform.Language.Ast.AST.Definitions
{
    /// <summary>
    /// Блок инструкций
    /// </summary>
    public class BlockNode : AstNode, IScoped
    {
        public StatementCollection Statements;

        public SymbolTable SymbolTable { get; set; }

        /// <summary>
        /// Создать блок из коллекции инструкций
        /// </summary>
        public BlockNode(StatementCollection statements) : base(null)
        {
            if (statements == null)
                return;

            Statements = new StatementCollection();

            foreach (Statement statement in statements)
            {
                Statements.Add(statement);
            }
        }

        public override T Accept<T>(AstVisitorBase<T> visitor)
        {
            throw new NotImplementedException();
        }
    }

    /// <summary>
    /// Описывает тело типа (методы, поля, события, конструкторы и т.д.)
    /// </summary>
    public class TypeBody : AstNode, IScoped
    {
        public FunctionCollection Functions;
        public FieldCollection Fields;
        public PropertyCollection Properties;

        public SymbolTable SymbolTable { get; set; }


        public TypeBody(MemberCollection members) : base(null)
        {
            Functions = new FunctionCollection();
            Fields = new FieldCollection();
            Properties = new PropertyCollection();

            if (members == null)
                return;

            foreach (Member member in members)
            {
                if (member is Function func)
                    Functions.Add(func);

                if (member is Field field)
                    Fields.Add(field);

                if (member is Property prop)
                    Properties.Add(prop);
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