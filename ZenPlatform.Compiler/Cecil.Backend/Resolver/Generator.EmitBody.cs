using System;
using System.Linq;
using Mono.Cecil;
using Mono.Cecil.Cil;
using ZenPlatform.Compiler.AST.Definitions;
using ZenPlatform.Compiler.AST.Definitions.Functions;
using ZenPlatform.Compiler.AST.Definitions.Statements;
using ZenPlatform.Compiler.AST.Definitions.Symbols;
using ZenPlatform.Compiler.AST.Infrastructure;

namespace ZenPlatform.Compiler.Cecil.Backend.Resolver

{
    public partial class SyntaxAnalyser
    {
        private void ResolveBody(Emitter e, InstructionsBodyNode body)
        {
            foreach (Statement statement in body.Statements)
            {
                ResolveStatement(e, statement, body);
            }
        }


        private void ResolveStatement(Emitter e, Statement statement, InstructionsBodyNode context)
        {
            if (statement is Variable variable)
            {
                VariableDefinition local =
                    new VariableDefinition(_typeResolver.Resolve(variable.Type));

                e.Variable(local);

                context.SymbolTable.Add(variable.Name, SymbolType.Variable, variable, local);

                //
                // Initialize  variable.
                //

                if (variable.Type.IsSystem)
                {
                    if (variable.Value != null && variable.Value is Expression)
                    {
                        ResolveExpression(e, (Expression) variable.Value, context.SymbolTable);
                    }
                }
                else if (variable.Type is ZArray a)
                {
                    // Empty array initialization.
                    if (variable.Value != null && variable.Value is Expression value)
                    {
                        ResolveExpression(e, value, context.SymbolTable);
                    }
                    else if (variable.Value != null && variable.Value is ElementCollection)
                    {
                        ElementCollection elements = variable.Value as ElementCollection;
                    }
                }
            }
            else if (statement is Assignment)
            {
                EmitAssignment(e, statement as Assignment, context.SymbolTable);
            }
            else if (statement is Return)
            {
                if (((Return) statement).Value != null)
                    ResolveExpression(e, ((Return) statement).Value, context.SymbolTable);
            }
            else if (statement is CallStatement)
            {
                CallStatement call = statement as CallStatement;
                Symbol symbol = context.SymbolTable.Find(call.Name, SymbolType.Function);
                EmitCallStatement(e, statement as CallStatement, context.SymbolTable);
            }
            else if (statement is If ifStatement)
            {
                // Eval condition
                ResolveExpression(e, ifStatement.Condition, context.SymbolTable);

                if (ifStatement.IfInstructionsBody != null && ifStatement.ElseInstructionsBody == null)
                {
                    ifStatement.IfInstructionsBody.SymbolTable = new SymbolTable(context.SymbolTable);
                    var exit = new Label();
                    e.BrFalse(exit);
                    ResolveBody(e, ifStatement.IfInstructionsBody);
                    e.Append(exit.Instruction);
                }
                else if (ifStatement.IfInstructionsBody != null && ifStatement.ElseInstructionsBody != null)
                {
                    ifStatement.IfInstructionsBody.SymbolTable = new SymbolTable(context.SymbolTable);
                    ifStatement.ElseInstructionsBody.SymbolTable = new SymbolTable(context.SymbolTable);


                    Label exit = new Label();
                    Label elseLabel = new Label();

                    e.BrFalse(elseLabel);
                    ResolveBody(e, ifStatement.IfInstructionsBody);
                    if (e.MethodBody.Instructions.Last().OpCode != OpCodes.Br)
                        e.Br(exit);
                    e.Append(elseLabel.Instruction);
                    ResolveBody(e, ifStatement.ElseInstructionsBody);
                    e.Append(exit.Instruction);
                }
            }

            else if (statement is While)
            {
                //
                // Generate while statement.
                //

                While whileStatement = statement as While;
                whileStatement.InstructionsBody.SymbolTable = new SymbolTable(context.SymbolTable);
                Label begin = new Label();
                Label exit = new Label();

                e.Append(begin);
                // Eval condition
                ResolveExpression(e, whileStatement.Condition, context.SymbolTable);
                e.BrFalse(exit);
                ResolveBody(e, whileStatement.InstructionsBody);

                e.Br(begin)
                    .Append(exit);
            }
            else if (statement is Do)
            {
                //
                // Generate do statement.
                //

                Do doStatement = statement as Do;
                doStatement.InstructionsBody.SymbolTable = new SymbolTable(context.SymbolTable);

                Label loop = new Label();
                e.Append(loop);
                ResolveBody(e, doStatement.InstructionsBody);
                ResolveExpression(e, doStatement.Condition, context.SymbolTable);
                e.BrTrue(loop);
            }
            else if (statement is For)
            {
                //
                // Generate for statement.
                //

                For forStatement = statement as For;
                forStatement.InstructionsBody.SymbolTable = new SymbolTable(context.SymbolTable);

                Label loop = new Label();
                Label exit = new Label();

                // Emit initializer
                ResolveStatement(e, forStatement.Initializer, context);
                e.Append(loop);
                // Emit condition
                ResolveExpression(e, forStatement.Condition, context.SymbolTable);
                e.BrFalse(exit);
                // Emit body
                ResolveBody(e, forStatement.InstructionsBody);
                // Emit counter
                ResolveStatement(e, forStatement.Counter, context);
                //EmitAssignment(il, forStatement.Counter, context.SymbolTable);
                e.Br(loop);
                e.Append(exit);
            }
            else if (statement is PostIncrementStatement pis)
            {
                var symbol = context.SymbolTable.Find(pis.Name.Value, SymbolType.Variable) ??
                             throw new Exception($"Variable {pis.Name} not found");

                ZType opType = null;
                if (symbol.SyntaxObject is Parameter p)
                    opType = p.Type;
                else if (symbol.SyntaxObject is Variable v)
                    opType = v.Type;


                ResolveExpression(e, pis.Name, context.SymbolTable);

                EmitIncrement(e, opType);
                e.Add();

                if (symbol.CodeObject is ParameterDefinition pd)
                    e.StArg(pd);
                if (symbol.CodeObject is VariableDefinition vd)
                    e.StLoc(vd);
            }
            else if (statement is PostDecrementStatement pds)
            {
                var symbol = context.SymbolTable.Find(pds.Name.Value, SymbolType.Variable) ??
                             throw new Exception($"Variable {pds.Name} not found");

                ZType opType = null;
                if (symbol.SyntaxObject is Parameter p)
                    opType = p.Type;
                else if (symbol.SyntaxObject is Variable v)
                    opType = v.Type;


                ResolveExpression(e, pds.Name, context.SymbolTable);

                EmitDecrement(e, opType);
                e.Add();

                if (symbol.CodeObject is ParameterDefinition pd)
                    e.StArg(pd);
                if (symbol.CodeObject is VariableDefinition vd)
                    e.StLoc(vd);
            }
        }
    }
}