using System.Collections.Generic;
using ZenPlatform.Compiler.Contracts.Symbols;
using ZenPlatform.Language.Ast.AST;
using ZenPlatform.Language.Ast.Definitions.Statements;
using ZenPlatform.Language.Ast.Infrastructure;

namespace ZenPlatform.Language.Ast.Definitions
{
    /// <summary>
    /// Блок инструкций
    /// </summary>
    public partial class Block : IScoped
    {
        /// <summary>
        /// Создать блок из коллекции инструкций
        /// </summary>
        public Block(List<Statement> statements) : this(null, statements)
        {
            if (statements == null)
                return;

            Statements = statements;
        }
    }
}