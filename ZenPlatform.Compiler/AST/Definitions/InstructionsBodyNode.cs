using ZenPlatform.Compiler.AST.Definitions.Functions;
using ZenPlatform.Compiler.AST.Definitions.Statements;
using ZenPlatform.Compiler.AST.Definitions.Symbols;
using ZenPlatform.Compiler.AST.Infrastructure;
using ZenPlatform.Compiler.Visitor;

namespace ZenPlatform.Compiler.AST.Definitions
{
    /// <summary>
    /// Блок инструкций
    /// </summary>
    public class InstructionsBodyNode : AstNode
    {
        public StatementCollection Statements;

        public SymbolTable SymbolTable = null;

        /// <summary>
        /// Создать блок из коллекции инструкций
        /// </summary>
        public InstructionsBodyNode(StatementCollection statements) : base(null)
        {
            if (statements == null)
                return;

            Statements = new StatementCollection();

            foreach (Statement statement in statements)
            {
                Statements.Add(statement);
            }
        }

        public override void Accept(IVisitor visitor)
        {
            Statements.ForEach(visitor.Visit);
        }
    }

    /// <summary>
    /// Описывает тело типа (методы, поля, события, конструкторы и т.д.)
    /// </summary>
    public class TypeBody : AstNode
    {
        public FunctionCollection Functions;

        public SymbolTable SymbolTable = null;

        public TypeBody(MemberCollection members) : base(null)
        {
            if (members == null)
                return;

            Functions = new FunctionCollection();

            foreach (Member member in members)
            {
                if (member is Function func)
                    Functions.Add(func);
            }
        }

        public override void Accept(IVisitor visitor)
        {
            foreach (var function in Functions)
            {
                visitor.Visit(function);
            }
        }
    }
}