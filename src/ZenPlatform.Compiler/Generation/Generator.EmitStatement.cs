using ZenPlatform.Compiler.Contracts;
using ZenPlatform.Compiler.Roslyn;
using ZenPlatform.Language.Ast.Definitions;
using ZenPlatform.Language.Ast.Definitions.Statements;
using For = ZenPlatform.Language.Ast.Definitions.Statements.For;
using Return = ZenPlatform.Language.Ast.Definitions.Statements.Return;
using Statement = ZenPlatform.Language.Ast.Definitions.Statements.Statement;

namespace ZenPlatform.Compiler.Generation
{
    public partial class Generator
    {
        private void EmitStatement(RBlockBuilder e, Statement statement, Block context,
            ILabel returnLabel, bool inTry = false)
        {
            if (statement is ExpressionStatement es)
            {
                EmitExpression(e, es.Expression, context.SymbolTable);
            }
            else if (statement is Return ret)
            {
                if (ret.Expression != null)
                {
                    EmitExpression(e, ret.Expression, context.SymbolTable);
                    e.Ret();
                }
            }
            else if (statement is If ifStatement)
            {
                // Eval condition
                EmitExpression(e, ifStatement.Condition, context.SymbolTable);

                var @ifBlock = e.Block();

                EmitBody(ifBlock, ifStatement.IfBlock, returnLabel);

                ifBlock.EndBlock();

                if (ifStatement.ElseBlock == null)
                    e.Nothing();
                else
                {
                    var @elseBlock = e.Block();

                    EmitBody(elseBlock, ifStatement.ElseBlock, returnLabel);

                    ifBlock.EndBlock();
                }

                e.If();
            }

            else if (statement is While whileStatement)
            {
                EmitExpression(e, whileStatement.Condition, context.SymbolTable);

                var whileBlock = e.Block();

                EmitBody(whileBlock, whileStatement.Block, returnLabel);

                whileBlock.EndBlock();

                e.While();
            }
            else if (statement is For forStatement)
            {
                //
                // Generate for statement.
                //

                // Emit initializer
                EmitExpression(e, forStatement.Initializer, context.SymbolTable);
                // Emit condition
                EmitExpression(e, forStatement.Condition, context.SymbolTable);
                // Emit counter
                EmitExpression(e, forStatement.Counter, context.SymbolTable);

                // Emit body

                var forBlock = e.Block();

                EmitBody(forBlock, forStatement.Block, returnLabel);

                forBlock.EndBlock();

                e.For();
            }
            else if (statement is Try ts)
            {
                var tryBlock = e.Block();
                EmitBody(tryBlock, ts.TryBlock, returnLabel);
                tryBlock.EndBlock();

                var catchBlock = e.Block();
                EmitBody(catchBlock, ts.CatchBlock, returnLabel);
                catchBlock.EndBlock();

                e.Nothing()
                    .Try();
            }
            else if (statement is Match mt)
            {
                // foreach (var matchAtom in mt.Matches)
                // {
                //     var label = e.DefineLabel();
                //     //Load value
                //     EmitExpression(e, mt.Expression, context.SymbolTable);
                //     //Check is instance of the value
                //     e.IsInst(_map.GetClrType(matchAtom.Type));
                //     e.BrFalse(label);
                //
                //     EmitBody(e, matchAtom.Block, returnLabel, ref returnVariable, false);
                //
                //     e.MarkLabel(label);
                // }
            }
        }
    }
}