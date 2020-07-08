using Aquila.Language.Ast.Misc;

namespace Aquila.Language.Ast.Definitions
{
    /// <summary>
    /// Блок инструкций
    /// </summary>
    public partial class Block 
    {
        public static Block Empty => new Block(null, new StatementList());

        /// <summary>
        /// Создать блок из коллекции инструкций
        /// </summary>
        public Block(StatementList statements) : this((ILineInfo) null, statements)
        {
        }
    }
}