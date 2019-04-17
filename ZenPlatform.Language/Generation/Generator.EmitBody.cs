using Mono.Cecil;
using Mono.Cecil.Cil;
using ZenPlatform.Language.AST.Definitions;
using ZenPlatform.Language.AST.Definitions.Functions;
using ZenPlatform.Language.AST.Definitions.Statements;
using ZenPlatform.Language.AST.Definitions.Symbols;
using ZenPlatform.Language.AST.Infrastructure;

namespace ZenPlatform.Language.Generation
{
    public partial class Generator
    {
        private void EmitBody(ILProcessor il, InstructionsBody body)
        {
            foreach (Statement statement in body.Statements)
            {
                //
                // Declare local variables.
                //

                if (statement is Variable)
                {
                    Variable variable = statement as Variable;

                    VariableDefinition local =
                        new VariableDefinition(ToCecilType(variable.Type.ToSystemType()));
                    il.Body.Variables.Add(local);

                    body.SymbolTable.Add(variable.Name, SymbolType.Variable, variable, local);

                    //
                    // Initialize  variable.
                    //

                    if (variable.Type.VariableType == VariableType.Primitive)
                    {
                        if (variable.Value != null && variable.Value is Expression)
                        {
                            EmitExpression(il, (Expression) variable.Value, body.SymbolTable);

                            il.Emit(Mono.Cecil.Cil.OpCodes.Stloc, local);
                        }
                    }
                    else if (variable.Type.VariableType == VariableType.PrimitiveArray)
                    {
                        // Empty array initialization.
                        if (variable.Value != null && variable.Value is Expression)
                        {
                            EmitExpression(il, (Expression) variable.Value, body.SymbolTable);
                            il.Emit(Mono.Cecil.Cil.OpCodes.Newarr,
                                ToCecilType(variable.Type.ToSystemType()));

                            il.Emit(Mono.Cecil.Cil.OpCodes.Stloc, local);
                        }
                        else if (variable.Value != null && variable.Value is ElementCollection)
                        {
                            ElementCollection elements = variable.Value as ElementCollection;

                            il.Emit(Mono.Cecil.Cil.OpCodes.Ldc_I4, elements.Count);
                            il.Emit(Mono.Cecil.Cil.OpCodes.Newarr,
                                ToCecilType(variable.Type.ToSystemType()));

                            il.Emit(Mono.Cecil.Cil.OpCodes.Stloc, local);

                            for (int x = 0; x < elements.Count; x++)
                            {
                                // Load array
                                il.Emit(Mono.Cecil.Cil.OpCodes.Ldloc, local);
                                // Load index
                                il.Emit(Mono.Cecil.Cil.OpCodes.Ldc_I4, x);
                                // Load value
                                EmitExpression(il, elements[x].Expression, body.SymbolTable);
                                // Store
                                il.Emit(Mono.Cecil.Cil.OpCodes.Stelem_I4);
                            }
                        }
                    }
                }
                else if (statement is Assignment)
                {
                    EmitAssignment(il, statement as Assignment, body.SymbolTable);
                }
                else if (statement is Return)
                {
                    if (((Return) statement).Value != null)
                        EmitExpression(il, ((Return) statement).Value, body.SymbolTable);
                    il.Emit(Mono.Cecil.Cil.OpCodes.Ret);
                }
                else if (statement is CallStatement)
                {
                    CallStatement call = statement as CallStatement;
                    Symbol symbol = body.SymbolTable.Find(call.Name, SymbolType.Function);
                    EmitCallStatement(il, statement as CallStatement, body.SymbolTable);

                    if (symbol != null)
                    {
                        if (((MethodDefinition) symbol.CodeObject).ReturnType != _dllModule.TypeSystem.Void)
                            il.Emit(Mono.Cecil.Cil.OpCodes.Pop);
                    }
                    else
                    {
                        if (call.Name == "Read")
                            il.Emit(Mono.Cecil.Cil.OpCodes.Pop);
                    }
                }
                else if (statement is If)
                {
                    //
                    // Genereate if statement.
                    //

                    If ifStatement = statement as If;

                    // Eval condition
                    EmitExpression(il, ifStatement.Condition, body.SymbolTable);

                    if (ifStatement.IfInstructionsBody != null && ifStatement.ElseInstructionsBody == null)
                    {
                        ifStatement.IfInstructionsBody.SymbolTable = new SymbolTable(body.SymbolTable);


                        var exit = new Label();
                        il.Emit(Mono.Cecil.Cil.OpCodes.Brfalse, exit.Instruction);
                        EmitBody(il, ifStatement.IfInstructionsBody);
                        il.Append(exit.Instruction);
                    }
                    else if (ifStatement.IfInstructionsBody != null && ifStatement.ElseInstructionsBody != null)
                    {
                        ifStatement.IfInstructionsBody.SymbolTable = new SymbolTable(body.SymbolTable);
                        ifStatement.ElseInstructionsBody.SymbolTable = new SymbolTable(body.SymbolTable);
                        Label exit = new Label();
                        Label elseLabel = new Label();
                        il.Emit(Mono.Cecil.Cil.OpCodes.Brfalse, elseLabel.Instruction);
                        EmitBody(il, ifStatement.IfInstructionsBody);
                        il.Emit(Mono.Cecil.Cil.OpCodes.Br, exit.Instruction);
                        il.Append(elseLabel.Instruction);
                        EmitBody(il, ifStatement.ElseInstructionsBody);
                        il.Append(exit.Instruction);
                    }
                }

                else if (statement is While)
                {
                    //
                    // Generate while statement.
                    //

                    While whileStatement = statement as While;
                    whileStatement.InstructionsBody.SymbolTable = new SymbolTable(body.SymbolTable);
                    Label begin = new Label();
                    Label exit = new Label();
                    il.Append(begin.Instruction);
                    // Eval condition
                    EmitExpression(il, whileStatement.Condition, body.SymbolTable);
                    il.Emit(Mono.Cecil.Cil.OpCodes.Brfalse, exit.Instruction);
                    EmitBody(il, whileStatement.InstructionsBody);
                    il.Emit(Mono.Cecil.Cil.OpCodes.Br, begin.Instruction);
                    il.Append(exit.Instruction);
                }
                else if (statement is Do)
                {
                    //
                    // Generate do statement.
                    //

                    Do doStatement = statement as Do;
                    doStatement.InstructionsBody.SymbolTable = new SymbolTable(body.SymbolTable);

                    Label loop = new Label();
                    il.Append(loop.Instruction);
                    EmitBody(il, doStatement.InstructionsBody);
                    EmitExpression(il, doStatement.Condition, body.SymbolTable);
                    il.Emit(Mono.Cecil.Cil.OpCodes.Brtrue, loop.Instruction);
                }
                else if (statement is For)
                {
                    //
                    // Generate for statement.
                    //

                    For forStatement = statement as For;
                    forStatement.InstructionsBody.SymbolTable = new SymbolTable(body.SymbolTable);

                    Label loop = new Label();
                    Label exit = new Label();

                    // Emit initializer
                    EmitAssignment(il, forStatement.Initializer, body.SymbolTable);
                    il.Append(loop.Instruction);
                    // Emit condition
                    EmitExpression(il, forStatement.Condition, body.SymbolTable);
                    il.Emit(Mono.Cecil.Cil.OpCodes.Brfalse, exit.Instruction);
                    // Emit body
                    EmitBody(il, forStatement.InstructionsBody);
                    // Emit counter
                    EmitAssignment(il, forStatement.Counter, body.SymbolTable);
                    il.Emit(Mono.Cecil.Cil.OpCodes.Br, loop.Instruction);
                    il.Append(exit.Instruction);
                }
            }
        }
    }
}