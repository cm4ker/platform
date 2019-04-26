using System;
using System.Linq;
using Mono.Cecil;
using Mono.Cecil.Cil;
using ZenPlatform.Compiler.AST.Definitions;
using ZenPlatform.Compiler.AST.Definitions.Functions;
using ZenPlatform.Compiler.AST.Definitions.Statements;
using ZenPlatform.Compiler.AST.Definitions.Symbols;
using ZenPlatform.Compiler.AST.Infrastructure;
using Type = ZenPlatform.Compiler.AST.Definitions.Type;

namespace ZenPlatform.Compiler.Generation
{
    public partial class Generator
    {
        private void EmitBody(ILProcessor il, InstructionsBody body, Instruction returnLabel,
            VariableDefinition returnVariable)
        {
            foreach (Statement statement in body.Statements)
            {
                //
                // Declare local variables.
                //
                EmitStatement(il, statement, body, returnLabel, returnVariable);

                var isLastStatement = body.Statements.Last() == statement;
            }
        }


        private void EmitStatement(ILProcessor il, Statement statement, InstructionsBody context,
            Instruction returnLabel, VariableDefinition returnVariable)
        {
            if (statement is Variable)
            {
                Variable variable = statement as Variable;

                VariableDefinition local =
                    new VariableDefinition(ToCecilType(variable.Type.ToClrType()));

                il.Body.Variables.Add(local);

                context.SymbolTable.Add(variable.Name, SymbolType.Variable, variable, local);

                //
                // Initialize  variable.
                //

                if (variable.Type.VariableType == VariableType.Primitive)
                {
                    if (variable.Value != null && variable.Value is Expression)
                    {
                        EmitExpression(il, (Expression) variable.Value, context.SymbolTable);

                        if (local.Index > 255)
                            il.Emit(OpCodes.Stloc, local);
                        else
                            il.Emit(OpCodes.Stloc_S, local);
                    }
                }
                else if (variable.Type.VariableType == VariableType.PrimitiveArray)
                {
                    // Empty array initialization.
                    if (variable.Value != null && variable.Value is Expression)
                    {
                        EmitExpression(il, (Expression) variable.Value, context.SymbolTable);
                        il.Emit(OpCodes.Newarr,
                            ToCecilType(variable.Type.ToClrType()));

                        if (local.Index > 255)
                            il.Emit(OpCodes.Stloc, local);
                        else
                            il.Emit(OpCodes.Stloc_S, local);
                    }
                    else if (variable.Value != null && variable.Value is ElementCollection)
                    {
                        ElementCollection elements = variable.Value as ElementCollection;

                        il.Emit(OpCodes.Ldc_I4, elements.Count);
                        il.Emit(OpCodes.Newarr,
                            ToCecilType(variable.Type.ToClrType()));

                        if (local.Index > 255)
                            il.Emit(OpCodes.Stloc, local);
                        else
                            il.Emit(OpCodes.Stloc_S, local);

                        for (int x = 0; x < elements.Count; x++)
                        {
                            // Load array
                            il.Emit(OpCodes.Ldloc, local);
                            // Load index
                            il.Emit(OpCodes.Ldc_I4, x);
                            // Load value
                            EmitExpression(il, elements[x].Expression, context.SymbolTable);
                            // Store
                            il.Emit(OpCodes.Stelem_I4);
                        }
                    }
                }
            }
            else if (statement is Assignment)
            {
                EmitAssignment(il, statement as Assignment, context.SymbolTable);
            }
            else if (statement is Return)
            {
                if (((Return) statement).Value != null)
                    EmitExpression(il, ((Return) statement).Value, context.SymbolTable);
                il.Emit(OpCodes.Stloc, returnVariable);
                il.Emit(OpCodes.Br, returnLabel);
            }
            else if (statement is CallStatement)
            {
                CallStatement call = statement as CallStatement;
                Symbol symbol = context.SymbolTable.Find(call.Name, SymbolType.Function);
                EmitCallStatement(il, statement as CallStatement, context.SymbolTable);

                if (symbol != null)
                {
                    if (((MethodDefinition) symbol.CodeObject).ReturnType != _dllModule.TypeSystem.Void)
                        il.Emit(OpCodes.Pop);
                }
                else
                {
                    if (call.Name == "Read")
                        il.Emit(OpCodes.Pop);
                }
            }
            else if (statement is If ifStatement)
            {
                // Eval condition
                EmitExpression(il, ifStatement.Condition, context.SymbolTable);

                if (ifStatement.IfInstructionsBody != null && ifStatement.ElseInstructionsBody == null)
                {
                    ifStatement.IfInstructionsBody.SymbolTable = new SymbolTable(context.SymbolTable);
                    var exit = new Label();
                    il.Emit(OpCodes.Brfalse, exit.Instruction);
                    EmitBody(il, ifStatement.IfInstructionsBody, returnLabel, returnVariable);
                    il.Append(exit.Instruction);
                }
                else if (ifStatement.IfInstructionsBody != null && ifStatement.ElseInstructionsBody != null)
                {
                    ifStatement.IfInstructionsBody.SymbolTable = new SymbolTable(context.SymbolTable);
                    ifStatement.ElseInstructionsBody.SymbolTable = new SymbolTable(context.SymbolTable);


                    Label exit = new Label();
                    Label elseLabel = new Label();

                    il.Emit(OpCodes.Brfalse, elseLabel.Instruction);
                    EmitBody(il, ifStatement.IfInstructionsBody, returnLabel, returnVariable);
                    if (il.Body.Instructions.Last().OpCode != OpCodes.Br)
                        il.Emit(OpCodes.Br, exit.Instruction);
                    il.Append(elseLabel.Instruction);
                    EmitBody(il, ifStatement.ElseInstructionsBody, returnLabel, returnVariable);
                    il.Append(exit.Instruction);
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
                il.Append(begin.Instruction);
                // Eval condition
                EmitExpression(il, whileStatement.Condition, context.SymbolTable);
                il.Emit(OpCodes.Brfalse, exit.Instruction);
                EmitBody(il, whileStatement.InstructionsBody, returnLabel, returnVariable);
                il.Emit(OpCodes.Br, begin.Instruction);
                il.Append(exit.Instruction);
            }
            else if (statement is Do)
            {
                //
                // Generate do statement.
                //

                Do doStatement = statement as Do;
                doStatement.InstructionsBody.SymbolTable = new SymbolTable(context.SymbolTable);

                Label loop = new Label();
                il.Append(loop.Instruction);
                EmitBody(il, doStatement.InstructionsBody, returnLabel, returnVariable);
                EmitExpression(il, doStatement.Condition, context.SymbolTable);
                il.Emit(OpCodes.Brtrue, loop.Instruction);
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