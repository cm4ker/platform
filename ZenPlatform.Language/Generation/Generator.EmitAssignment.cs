using Mono.Cecil;
using Mono.Cecil.Cil;
using ZenPlatform.Language.AST.Definitions.Functions;
using ZenPlatform.Language.AST.Definitions.Symbols;
using ZenPlatform.Language.AST.Infrastructure;

namespace ZenPlatform.Language.Generation
{
    public partial class Generator
    {
        private void EmitAssignment(ILProcessor il, Assignment assignment, SymbolTable symbolTable)
        {
            Symbol variable = symbolTable.Find(assignment.Name, SymbolType.Variable);
            if (variable == null)
                Error("Assignment variable " + assignment.Name + " unknown.");

            // Non-indexed assignment
            if (assignment.Index == null)
            {
                if (variable.CodeObject is ParameterDefinition)
                {
                    Parameter p = variable.SyntaxObject as Parameter;
                    if (p.PassMethod == PassMethod.ByReference)
                        il.Emit(Mono.Cecil.Cil.OpCodes.Ldarg_S,
                            ((ParameterDefinition) variable.CodeObject).Sequence - 1);
                }

                // Load value
                EmitExpression(il, assignment.Value, symbolTable);

                // Store
                if (variable.CodeObject is VariableDefinition)
                    il.Emit(Mono.Cecil.Cil.OpCodes.Stloc, (VariableDefinition) variable.CodeObject);
                else if (variable.CodeObject is FieldDefinition)
                    il.Emit(Mono.Cecil.Cil.OpCodes.Stsfld, (FieldDefinition) variable.CodeObject);
                else if (variable.CodeObject is ParameterDefinition)
                {
                    Parameter p = variable.SyntaxObject as Parameter;
                    if (p.PassMethod == PassMethod.ByReference)
                        il.Emit(Mono.Cecil.Cil.OpCodes.Stind_I4);
                    else
                        il.Emit(Mono.Cecil.Cil.OpCodes.Starg, ((ParameterDefinition) variable.CodeObject).Sequence - 1);
                }
            }
            else
            {
                // Load array.
                if (variable.CodeObject is VariableDefinition)
                    il.Emit(Mono.Cecil.Cil.OpCodes.Ldloc, (VariableDefinition) variable.CodeObject);
                else if (variable.CodeObject is FieldDefinition)
                    il.Emit(Mono.Cecil.Cil.OpCodes.Ldsfld, (FieldDefinition) variable.CodeObject);
                // Load index.
                EmitExpression(il, assignment.Index, symbolTable);
                // Load value.
                EmitExpression(il, assignment.Value, symbolTable);
                // Set
                il.Emit(Mono.Cecil.Cil.OpCodes.Stelem_I4);
            }
        }
    }
}