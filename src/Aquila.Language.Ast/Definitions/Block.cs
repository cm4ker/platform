using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading;
using Aquila.Language.Ast.AST;
using Aquila.Language.Ast.Definitions.Functions;
using Aquila.Language.Ast.Definitions.Statements;
using Aquila.Language.Ast.Infrastructure;

namespace Aquila.Language.Ast.Definitions
{
    /// <summary>
    /// Блок инструкций
    /// </summary>
    public partial class Block : IScoped
    {
        public static Block Empty => new Block(null, new StatementList());

        /// <summary>
        /// Создать блок из коллекции инструкций
        /// </summary>
        public Block(StatementList statements) : this(null, statements)
        {
        }
    }
}