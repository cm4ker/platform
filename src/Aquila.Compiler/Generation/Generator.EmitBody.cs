using System;
using System.Linq;
using Aquila.Compiler.Contracts;
using Aquila.Compiler.Contracts.Symbols;
using Aquila.Compiler.Helpers;
using Aquila.Compiler.Roslyn;
using Aquila.Language.Ast.Definitions;
using Aquila.Language.Ast.Definitions.Functions;
using Statement = Aquila.Language.Ast.Definitions.Statements.Statement;

namespace Aquila.Compiler.Generation
{
    public partial class Generator
    {
        private void EmitBody(RoslynEmitter e, Block body, ILabel returnLabel, bool inTry = false)
        {
            foreach (Statement statement in body.Statements)
            {
                EmitStatement(e, statement, body, returnLabel, inTry);
            }
        }
    }
}