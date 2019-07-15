using System;
using System.Linq;
using ZenPlatform.Compiler.AST.Definitions;
using ZenPlatform.Compiler.AST.Definitions.Symbols;
using ZenPlatform.Compiler.AST.Infrastructure;
using ZenPlatform.Compiler.Contracts;
using ZenPlatform.Compiler.Contracts.Symbols;
using ZenPlatform.Language.Ast.AST.Definitions;
using ZenPlatform.Language.Ast.AST.Definitions.Functions;
using ZenPlatform.Language.Ast.AST.Definitions.Statements;
using ZenPlatform.Language.Ast.AST.Infrastructure;

namespace ZenPlatform.Compiler.Generation
{
    public partial class Generator
    {
        private void EmitBody(IEmitter e, BlockNode body, ILabel returnLabel,
            ref ILocal returnVariable, bool inTry = false)
        {
            foreach (Statement statement in body.Statements)
            {
                //
                // Declare local variables.
                //
                EmitStatement(e, statement, body, returnLabel, ref returnVariable, inTry);

                var isLastStatement = body.Statements.Last() == statement;
            }
        }


        private void EmitStatement(IEmitter e, Statement statement, BlockNode context,
            ILabel returnLabel, ref ILocal returnVariable, bool inTry = false)
        {
            if (statement is Variable variable)
            {
                EmitVariable(e, context, variable);
            }
            else if (statement is Assignment)
            {
                EmitAssignment(e, statement as Assignment, context.SymbolTable);
            }
            else if (statement is Return ret)
            {
                if (ret.Value != null)
                {
                    EmitExpression(e, ret.Value, context.SymbolTable);
                }

                if (ret.GetParent<Function>().Type is UnionTypeNode mtn)
                {
                    var exp = e.DefineLocal(ret.Value.Type.Type);
                    e.StLoc(exp);
                    WrapMultitypeStackValue(e, mtn, returnVariable, exp);
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
            else if (statement is CallStatement)
            {
                CallStatement call = statement as CallStatement;
                ISymbol symbol = context.SymbolTable.Find(call.Name, SymbolType.Function);
                EmitCallStatement(e, statement as CallStatement, context.SymbolTable);

                if (symbol != null)
                {
                    if (((IMethod) symbol.CodeObject).ReturnType != _bindings.Void)
                        e.Pop();
                }
                else
                {
                    if (call.Name == "Read")
                        e.Pop();
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
            else if (statement is Do)
            {
                //
                // Generate do statement.
                //

                Do doStatement = statement as Do;
                doStatement.Block.SymbolTable = new SymbolTable(context.SymbolTable);

                ILabel loop = e.DefineLabel();
                e.MarkLabel(loop);
                EmitBody(e, doStatement.Block, returnLabel, ref returnVariable);
                EmitExpression(e, doStatement.Condition, context.SymbolTable);
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
                EmitStatement(e, forStatement.Initializer, context, returnLabel, ref returnVariable);
                e.MarkLabel(loop);
                // Emit condition
                EmitExpression(e, forStatement.Condition, context.SymbolTable);
                e.BrFalse(exit);
                // Emit body
                EmitBody(e, forStatement.Block, returnLabel, ref returnVariable);
                // Emit counter
                EmitStatement(e, forStatement.Counter, context, returnLabel, ref returnVariable);
                //EmitAssignment(il, forStatement.Counter, context.SymbolTable);
                e.Br(loop);
                e.MarkLabel(exit);
            }
            else if (statement is PostIncrementStatement pis)
            {
                var symbol = context.SymbolTable.Find(pis.Name.Value, SymbolType.Variable) ??
                             throw new Exception($"Variable {pis.Name} not found");

                IType opType = null;
                if (symbol.SyntaxObject is Parameter p)
                    opType = p.Type.Type;
                else if (symbol.SyntaxObject is Variable v)
                    opType = v.Type.Type;


                EmitExpression(e, pis.Name, context.SymbolTable);

                EmitIncrement(e, opType);
                e.Add();

                if (symbol.CodeObject is IParameter pd)
                    e.StArg(pd);

                if (symbol.CodeObject is ILocal vd)
                    e.StLoc(vd);
            }
            else if (statement is PostDecrementStatement pds)
            {
                var symbol = context.SymbolTable.Find(pds.Name.Value, SymbolType.Variable) ??
                             throw new Exception($"Variable {pds.Name} not found");

                IType opType = null;
                if (symbol.SyntaxObject is Parameter p)
                    opType = p.Type.Type;
                else if (symbol.SyntaxObject is Variable v)
                    opType = v.Type.Type;


                EmitExpression(e, pds.Name, context.SymbolTable);

                EmitDecrement(e, opType);
                e.Add();

                if (symbol.CodeObject is IParameter pd)
                    e.StArg(pd);

                if (symbol.CodeObject is ILocal vd)
                    e.StLoc(vd);
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
        }
    }
}