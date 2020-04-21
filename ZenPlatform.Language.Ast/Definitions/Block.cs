using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading;
using ZenPlatform.Language.Ast.AST;
using ZenPlatform.Language.Ast.Definitions.Functions;
using ZenPlatform.Language.Ast.Definitions.Statements;
using ZenPlatform.Language.Ast.Infrastructure;

namespace ZenPlatform.Language.Ast.Definitions
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