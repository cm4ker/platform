using System;
using System.Linq;
using Mono.Cecil;
using Mono.Cecil.Cil;
using ZenPlatform.Compiler.AST.Definitions;
using ZenPlatform.Compiler.AST.Definitions.Functions;
using ZenPlatform.Compiler.AST.Definitions.Statements;
using ZenPlatform.Compiler.AST.Definitions.Symbols;
using ZenPlatform.Compiler.AST.Infrastructure;
using ZenPlatform.Compiler.Cecil.Backend;

namespace ZenPlatform.Compiler.Generation
{
    public partial class Generator
    {
        private void EmitBody(Emitter e, InstructionsBodyNode body, Label returnLabel,
            VariableDefinition returnVariable)
        {
            foreach (Statement statement in body.Statements)
            {
                //
                // Declare local variables.
                //
                EmitStatement(e, statement, body, returnLabel, returnVariable);

                var isLastStatement = body.Statements.Last() == statement;
            }
        }


        private void EmitStatement(Emitter e, Statement statement, InstructionsBodyNode context,
            Label returnLabel, VariableDefinition returnVariable)
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
                        EmitExpression(e, (Expression) variable.Value, context.SymbolTable);

                        e.StLoc(local);
                    }
                }
                else if (variable.Type is ZArray a)
                {
                    // Empty array initialization.
                    if (variable.Value != null && variable.Value is Expression value)
                    {
                        EmitExpression(e, value, context.SymbolTable);
                        e.NewArr(_typeResolver.Resolve(variable.Type));
                        e.StLoc(local);
                    }
                    else if (variable.Value != null && variable.Value is ElementCollection)
                    {
                        ElementCollection elements = variable.Value as ElementCollection;

                        e.LdcI4(elements.Count);
                        e.NewArr(_typeResolver.Resolve(variable.Type));
                        e.StLoc(local);

                        for (int x = 0; x < elements.Count; x++)
                        {
                            // Load array
                            e.LdLoc(local);
                            // Load index
                            e.LdcI4(x);
                            // Load value
                            EmitExpression(e, elements[x].Expression, context.SymbolTable);
                            // Store
                            e.StElemI4();
                        }
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
                    EmitExpression(e, ((Return) statement).Value, context.SymbolTable);

                e.StLoc(returnVariable)
                    .Br(returnLabel);
            }
            else if (statement is CallStatement)
            {
                CallStatement call = statement as CallStatement;
                Symbol symbol = context.SymbolTable.Find(call.Name, SymbolType.Function);
                EmitCallStatement(e, statement as CallStatement, context.SymbolTable);

                if (symbol != null)
                {
                    if (((MethodDefinition) symbol.CodeObject).ReturnType != _dllModule.TypeSystem.Void)
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

                if (ifStatement.IfInstructionsBody != null && ifStatement.ElseInstructionsBody == null)
                {
                    ifStatement.IfInstructionsBody.SymbolTable = new SymbolTable(context.SymbolTable);
                    var exit = new Label();
                    e.BrFalse(exit);
                    EmitBody(e, ifStatement.IfInstructionsBody, returnLabel, returnVariable);
                    e.Append(exit.Instruction);
                }
                else if (ifStatement.IfInstructionsBody != null && ifStatement.ElseInstructionsBody != null)
                {
                    ifStatement.IfInstructionsBody.SymbolTable = new SymbolTable(context.SymbolTable);
                    ifStatement.ElseInstructionsBody.SymbolTable = new SymbolTable(context.SymbolTable);


                    Label exit = new Label();
                    Label elseLabel = new Label();

                    e.BrFalse(elseLabel);
                    EmitBody(e, ifStatement.IfInstructionsBody, returnLabel, returnVariable);
                    if (e.MethodBody.Instructions.Last().OpCode != OpCodes.Br)
                        e.Br(exit);
                    e.Append(elseLabel.Instruction);
                    EmitBody(e, ifStatement.ElseInstructionsBody, returnLabel, returnVariable);
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
                EmitExpression(e, whileStatement.Condition, context.SymbolTable);
                e.BrFalse(exit);
                EmitBody(e, whileStatement.InstructionsBody, returnLabel, returnVariable);

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
                EmitBody(e, doStatement.InstructionsBody, returnLabel, returnVariable);
                EmitExpression(e, doStatement.Condition, context.SymbolTable);
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
                EmitStatement(e, forStatement.Initializer, context, returnLabel, returnVariable);
                e.Append(loop);
                // Emit condition
                EmitExpression(e, forStatement.Condition, context.SymbolTable);
                e.BrFalse(exit);
                // Emit body
                EmitBody(e, forStatement.InstructionsBody, returnLabel, returnVariable);
                // Emit counter
                EmitStatement(e, forStatement.Counter, context, returnLabel, returnVariable);
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


                EmitExpression(e, pis.Name, context.SymbolTable);

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


                EmitExpression(e, pds.Name, context.SymbolTable);

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