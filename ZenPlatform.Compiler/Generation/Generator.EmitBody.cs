using System;
using System.Linq;
using ZenPlatform.Compiler.AST.Infrastructure;
using ZenPlatform.Compiler.Contracts;
using ZenPlatform.Compiler.Contracts.Symbols;
using ZenPlatform.Compiler.Helpers;
using ZenPlatform.Language.Ast.Definitions;
using ZenPlatform.Language.Ast.Definitions.Functions;
using ZenPlatform.Language.Ast.Definitions.Statements;

namespace ZenPlatform.Compiler.Generation
{
    public partial class Generator
    {
        private void EmitBody(IEmitter e, Block body, ILabel returnLabel,
            ref ILocal returnVariable, bool inTry = false)
        {
            foreach (Statement statement in body.Statements)
            {
                EmitStatement(e, statement, body, returnLabel, ref returnVariable, inTry);
            }
        }
    }
}