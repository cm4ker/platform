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
using Type = ZenPlatform.Compiler.AST.Definitions.Type;

namespace ZenPlatform.Compiler.Generation
{
    public partial class Generator
    {
        private void EmitBody(Emitter emitter, InstructionsBodyNode body, Instruction returnLabel,
            VariableDefinition returnVariable)
        {
            foreach (Statement statement in body.Statements)
            {
                //
                // Declare local variables.
                //
                EmitStatement(emitter, statement, body, returnLabel, returnVariable);

                var isLastStatement = body.Statements.Last() == statement;
            }
        }


        private void EmitStatement(Emitter emitter, Statement statement, InstructionsBodyNode context,
            Instruction returnLabel, VariableDefinition returnVariable)
        {
            if (statement is Variable)
            {
                Variable variable = statement as Variable;

                VariableDefinition local =
                    new VariableDefinition(ToCecilType(variable.Type.ToClrType()));

                emitter.Variable(local);

                context.SymbolTable.Add(variable.Name, SymbolType.Variable, variable, local);

                //
                // Initialize  variable.
                //

                if (variable.Type.VariableType == VariableType.Primitive)
                {
                    if (variable.Value != null && variable.Value is Expression)
                    {
                        EmitExpression(emitter, (Expression) variable.Value, context.SymbolTable);

                        emitter.StLoc(local);
                    }
                }
                else if (variable.Type.VariableType == VariableType.PrimitiveArray)
                {
                    // Empty array initialization.
                    if (variable.Value != null && variable.Value is Expression)
                    {
                        EmitExpression(emitter, (Expression) variable.Value, context.SymbolTable);
                        emitter.NewArr(ToCecilType(variable.Type.ToClrType()));
                        emitter.StLoc(local);
                    }
                    else if (variable.Value != null && variable.Value is ElementCollection)
                    {
                        ElementCollection elements = variable.Value as ElementCollection;

                        emitter.LdcI4(elements.Count);
                        emitter.NewArr(ToCecilType(variable.Type.ToClrType()));
                        emitter.StLoc(local);

                        for (int x = 0; x < elements.Count; x++)
                        {
                            // Load array
                            emitter.LdLoc(local);
                            // Load index
                            emitter.LdcI4(x);
                            // Load value
                            EmitExpression(emitter, elements[x].Expression, context.SymbolTable);
                            // Store
                            emitter.StElemI4();
                        }
                    }
                }
            }
            else if (statement is Assignment)
            {
                EmitAssignment(emitter, statement as Assignment, context.SymbolTable);
            }
            else if (statement is Return)
            {
                if (((Return) statement).Value != null)
                    EmitExpression(emitter, ((Return) statement).Value, context.SymbolTable);

                emitter.StLoc(returnVariable)
                    .Br(returnLabel);
            }
            else if (statement is CallStatement)
            {
                CallStatement call = statement as CallStatement;
                Symbol symbol = context.SymbolTable.Find(call.Name, SymbolType.Function);
                EmitCallStatement(emitter, statement as CallStatement, context.SymbolTable);

                if (symbol != null)
                {
                    if (((MethodDefinition) symbol.CodeObject).ReturnType != _dllModule.TypeSystem.Void)
                        emitter.Pop();
                }
                else
                {
                    if (call.Name == "Read")
                        emitter.Pop();
                }
            }
            else if (statement is If ifStatement)
            {
                // Eval condition
                EmitExpression(emitter, ifStatement.Condition, context.SymbolTable);

                if (ifStatement.IfInstructionsBody != null && ifStatement.ElseInstructionsBody == null)
                {
                    ifStatement.IfInstructionsBody.SymbolTable = new SymbolTable(context.SymbolTable);
                    var exit = new Label();
                    emitter.BrFalse(exit);
                    EmitBody(emitter, ifStatement.IfInstructionsBody, returnLabel, returnVariable);
                    emitter.Append(exit.Instruction);
                }
                else if (ifStatement.IfInstructionsBody != null && ifStatement.ElseInstructionsBody != null)
                {
                    ifStatement.IfInstructionsBody.SymbolTable = new SymbolTable(context.SymbolTable);
                    ifStatement.ElseInstructionsBody.SymbolTable = new SymbolTable(context.SymbolTable);


                    Label exit = new Label();
                    Label elseLabel = new Label();

                    emitter.BrFalse(elseLabel);
                    EmitBody(emitter, ifStatement.IfInstructionsBody, returnLabel, returnVariable);
                    if (emitter.MethodBody.Instructions.Last().OpCode != OpCodes.Br)
                        emitter.Br(exit);
                    emitter.Append(elseLabel.Instruction);
                    EmitBody(emitter, ifStatement.ElseInstructionsBody, returnLabel, returnVariable);
                    emitter.Append(exit.Instruction);
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

                emitter.Append(begin);
                // Eval condition
                EmitExpression(emitter, whileStatement.Condition, context.SymbolTable);
                emitter.BrFalse(exit);
                EmitBody(emitter, whileStatement.InstructionsBody, returnLabel, returnVariable);

                emitter.Br(begin)
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
                emitter.Append(loop);
                EmitBody(emitter, doStatement.InstructionsBody, returnLabel, returnVariable);
                EmitExpression(emitter, doStatement.Condition, context.SymbolTable);
                emitter.BrTrue(loop);
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
                EmitStatement(il, forStatement.Initializer, context, returnLabel, returnVariable);
                il.Append(loop.Instruction);
                // Emit condition
                EmitExpression(il, forStatement.Condition, context.SymbolTable);
                il.Emit(OpCodes.Brfalse, exit.Instruction);
                // Emit body
                EmitBody(il, forStatement.InstructionsBody, returnLabel, returnVariable);
                // Emit counter
                EmitStatement(il, forStatement.Counter, context, returnLabel, returnVariable);
                //EmitAssignment(il, forStatement.Counter, context.SymbolTable);
                il.Emit(OpCodes.Br, loop.Instruction);
                il.Append(exit.Instruction);
            }
            else if (statement is PostIncrementStatement pis)
            {
                var symbol = context.SymbolTable.Find(pis.Name.Value, SymbolType.Variable) ??
                             throw new Exception($"Variable {pis.Name} not found");

                Type opType = null;
                if (symbol.SyntaxObject is Parameter p)
                    opType = p.Type;
                else if (symbol.SyntaxObject is Variable v)
                    opType = v.Type;


                EmitExpression(il, pis.Name, context.SymbolTable);

                il.Emit(GetLdcCodeFromType(opType), 1);
                il.Emit(OpCodes.Add);

                if (symbol.CodeObject is ParameterDefinition pd)
                    il.Emit(OpCodes.Starg, pd);
                if (symbol.CodeObject is VariableDefinition vd)
                    il.Emit(OpCodes.Stloc, vd);
            }
        }
    }
}