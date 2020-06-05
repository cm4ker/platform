using Aquila.Compiler.Aqua.TypeSystem;
using Aquila.Compiler.Contracts;
using Aquila.Language.Ast.Definitions;
using Aquila.Language.Ast.Definitions.Statements;
using For = Aquila.Language.Ast.Definitions.Statements.For;
using Return = Aquila.Language.Ast.Definitions.Statements.Return;
using Statement = Aquila.Language.Ast.Definitions.Statements.Statement;

namespace Aquila.Compiler.Generation
{
    public partial class Generator
    {
        private void EmitStatement(PCilBody e, Statement statement, Block context,
            PLabel returnLabel, ref PLocal returnVariable, bool inTry = false)
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

                var exit = e.DefineLabel();
                if (ifStatement.IfBlock != null && ifStatement.ElseBlock == null)
                {
                    e.BrFalse(exit);
                    EmitBody(e, ifStatement.IfBlock, returnLabel, ref returnVariable);
                }
                else if (ifStatement.IfBlock != null && ifStatement.ElseBlock != null)
                {
                    PLabel elseLabel = e.DefineLabel();

                    e.BrFalse(elseLabel);
                    EmitBody(e, ifStatement.IfBlock, returnLabel, ref returnVariable);
                    e.Br(exit);
                    e.MarkLabel(elseLabel);
                    EmitBody(e, ifStatement.ElseBlock, returnLabel, ref returnVariable);
                }

                e.MarkLabel(exit);
            }

            else if (statement is While whileStatement)
            {
                //
                // Generate while statement.
                //

                PLabel begin = e.DefineLabel();
                PLabel exit = e.DefineLabel();

                e.MarkLabel(begin);
                // Eval condition
                EmitExpression(e, whileStatement.Condition, context.SymbolTable);
                e.BrFalse(exit);
                EmitBody(e, whileStatement.Block, returnLabel, ref returnVariable);

                e.Br(begin)
                    .MarkLabel(exit);
            }
            else if (statement is For forStatement)
            {
                //
                // Generate for statement.
                //
                PLabel loop = e.DefineLabel();
                PLabel exit = e.DefineLabel();

                // Emit initializer
                EmitExpression(e, forStatement.Initializer, context.SymbolTable);
                e.MarkLabel(loop);
                // Emit condition
                EmitExpression(e, forStatement.Condition, context.SymbolTable);
                e.BrFalse(exit);
                // Emit body
                EmitBody(e, forStatement.Block, returnLabel, ref returnVariable);
                // Emit counter
                EmitExpression(e, forStatement.Counter, context.SymbolTable);
                //EmitAssignment(il, forStatement.Counter, context.SymbolTable);
                e.Br(loop);
                e.MarkLabel(exit);
            }
            else if (statement is Try ts)
            {
                // var exLocal = e.DefineLocal(_ts.FindType("System.Exception"));
                // e.BeginExceptionBlock();
                // EmitBody(e, ts.TryBlock, returnLabel, ref returnVariable, true);
                // e.BeginCatchBlock(_ts.FindType("System.Exception"));
                // e.StLoc(exLocal);
                // EmitBody(e, ts.CatchBlock, returnLabel, ref returnVariable, true);
                // e.EndExceptionBlock();
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