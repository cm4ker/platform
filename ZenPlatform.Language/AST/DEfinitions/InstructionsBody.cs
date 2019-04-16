using ZenPlatfrom.Language.AST.Definitions.Functions;
using ZenPlatfrom.Language.AST.Definitions.Statements;
using ZenPlatfrom.Language.AST.Definitions.Symbols;
using ZenPlatfrom.Language.AST.Infrastructure;

namespace ZenPlatfrom.Language.AST.Definitions
{
    /// <summary>
    /// Блок инструкций
    /// </summary>
    public class InstructionsBody
    {
        public StatementCollection Statements = null;

        public SymbolTable SymbolTable = null;

        /// <summary>
        /// Создать блок из коллекции инструкций
        /// </summary>
        public InstructionsBody(StatementCollection statements)
        {
            if (statements == null)
                return;

            Statements = new StatementCollection();

            foreach (Statement statement in statements)
            {
                Statements.Add(statement);
            }
        }
    }

    /// <summary>
    /// Описывает тело типа (методы, поля, события, конструкторы и т.д.)
    /// </summary>
    public class TypeBody
    {
        public FunctionCollection Functions = null;

        public SymbolTable SymbolTable = null;

        public TypeBody(MemberCollection members)
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
    }
}