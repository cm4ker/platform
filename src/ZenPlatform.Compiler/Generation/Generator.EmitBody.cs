using System;
using System.Linq;
using ZenPlatform.Compiler.Contracts;
using ZenPlatform.Compiler.Contracts.Symbols;
using ZenPlatform.Compiler.Helpers;
using ZenPlatform.Compiler.Roslyn;
using ZenPlatform.Language.Ast.Definitions;
using ZenPlatform.Language.Ast.Definitions.Functions;
using Statement = ZenPlatform.Language.Ast.Definitions.Statements.Statement;

namespace ZenPlatform.Compiler.Generation
{
    public partial class Generator
    {
        private void EmitBody(RBlockBuilder e, Block body, ILabel returnLabel, bool inTry = false)
        {
            foreach (Statement statement in body.Statements)
            {
                EmitStatement(e, statement, body, returnLabel, inTry);
            }
        }
    }
}