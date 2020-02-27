using System;
using System.Linq;
using System.Reflection.Emit;
using ZenPlatform.Compiler.Contracts;
using ZenPlatform.Compiler.Contracts.Symbols;
using ZenPlatform.Compiler.Helpers;
using ZenPlatform.Language.Ast.Definitions;
using ZenPlatform.Language.Ast.Definitions.Functions;
using ZenPlatform.Language.Ast.Definitions.Statements;
using ZenPlatform.Language.Ast.Symbols;

namespace ZenPlatform.Compiler.Generation
{
    public partial class Generator
    {
        private void EmitStatement(IEmitter e, Statement statement, Block context,
            ILabel returnLabel, ref ILocal returnVariable, bool inTry = false)
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

                    if (returnVariable.Type == _bindings.Object)
                    {
                        var clrType = _map.GetClrType(ret.Expression.Type);
                        if (clrType.IsValueType && !clrType.IsArray)
                            e.Box(clrType);
                    }
                }

                if (inTry)
                {
                    e.StLoc(returnVariable);
                    e.Leave(returnLabel);
                }
                else
                {
                    e.StLoc(returnVariable)
                        .Br(returnLabel);
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
                    ifStatement.IfBlock.SymbolTable = new SymbolTable(context.SymbolTable);
                    ifStatement.ElseBlock.SymbolTable = new SymbolTable(context.SymbolTable);

                    ILabel elseLabel = e.DefineLabel();

                    e.BrFalse(elseLabel);
                    EmitBody(e, ifStatement.IfBlock, returnLabel, ref returnVariable);
                    e.Br(exit);
                    e.MarkLabel(elseLabel);
                    EmitBody(e, ifStatement.ElseBlock, returnLabel, ref returnVariable);
                }

                e.MarkLabel(exit);
            }

            else if (statement is While)
            {
                //
                // Generate while statement.
                //

                While whileStatement = statement as While;
                whileStatement.Block.SymbolTable = new SymbolTable(context.SymbolTable);
                ILabel begin = e.DefineLabel();
                ILabel exit = e.DefineLabel();

                e.MarkLabel(begin);
                // Eval condition
                EmitExpression(e, whileStatement.Condition, context.SymbolTable);
                e.BrFalse(exit);
                EmitBody(e, whileStatement.Block, returnLabel, ref returnVariable);

                e.Br(begin)
                    .MarkLabel(exit);
            }
            else if (statement is DoWhile)
            {
                //
                // Generate do statement.
                //

                DoWhile doWhileStatement = statement as DoWhile;
                doWhileStatement.Block.SymbolTable = new SymbolTable(context.SymbolTable);

                ILabel loop = e.DefineLabel();
                e.MarkLabel(loop);
                EmitBody(e, doWhileStatement.Block, returnLabel, ref returnVariable);
                EmitExpression(e, doWhileStatement.Condition, context.SymbolTable);
                e.BrTrue(loop);
            }
            else if (statement is For)
            {
                //
                // Generate for statement.
                //

                For forStatement = statement as For;
                forStatement.Block.SymbolTable = new SymbolTable(context.SymbolTable);

                ILabel loop = e.DefineLabel();
                ILabel exit = e.DefineLabel();

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
                var exLocal = e.DefineLocal(_ts.FindType("System.Exception"));
                e.BeginExceptionBlock();
                EmitBody(e, ts.TryBlock, returnLabel, ref returnVariable, true);
                e.BeginCatchBlock(_ts.FindType("System.Exception"));
                e.StLoc(exLocal);
                EmitBody(e, ts.CatchBlock, returnLabel, ref returnVariable, true);
                e.EndExceptionBlock();
            }
            else if (statement is Match mt)
            {
                foreach (var matchAtom in mt.Matches)
                {
                    var label = e.DefineLabel();
                    //Load value
                    EmitExpression(e, mt.Expression, context.SymbolTable);
                    //Check is instance of the value
                    e.IsInst(_map.GetClrType(matchAtom.Type));
                    e.BrFalse(label);

                    EmitBody(e, matchAtom.Block, returnLabel, ref returnVariable, false);

                    e.MarkLabel(label);
                }
            }
        }
    }
}