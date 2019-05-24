using Mono.Cecil;
using Mono.Cecil.Cil;
using ZenPlatform.Compiler.AST.Definitions.Functions;
using ZenPlatform.Compiler.AST.Definitions.Symbols;
using ZenPlatform.Compiler.AST.Infrastructure;
using ZenPlatform.Compiler.Cecil.Backend;
using ZenPlatform.Compiler.Contracts;

namespace ZenPlatform.Compiler.Generation
{
    public partial class Generator
    {
        private void EmitAssignment(IEmitter il, Assignment assignment, SymbolTable symbolTable)
        {
            Symbol variable = symbolTable.Find(assignment.Name, SymbolType.Variable);
            if (variable == null)
                Error("Assignment variable " + assignment.Name + " unknown.");

            // Non-indexed assignment
            if (assignment.Index == null)
            {
                if (variable.CodeObject is ParameterDefinition pd)
                {
                    Parameter p = variable.SyntaxObject as Parameter;
                    if (p.PassMethod == PassMethod.ByReference)
                        il.LdArg(pd.Sequence - 1);
                }

                // Load value
                EmitExpression(il, assignment.Value, symbolTable);

                // Store
                if (variable.CodeObject is VariableDefinition vd)
                    il.StLoc(vd);
                else if (variable.CodeObject is FieldDefinition fd)
                    il.LdsFld(fd);
                else if (variable.CodeObject is ParameterDefinition)
                {
                    Parameter p = variable.SyntaxObject as Parameter;
                    if (p.PassMethod == PassMethod.ByReference)
                        il.StIndI4();
                    else
                        il.StArg((ParameterDefinition) variable.CodeObject);
                }
            }
            else
            {
                // Load array.
                if (variable.CodeObject is VariableDefinition vd)
                    il.LdLoc(vd);
                else if (variable.CodeObject is FieldDefinition fd)
                    il.LdsFld(fd);
                // Load index.
                EmitExpression(il, assignment.Index, symbolTable);
                // Load value.
                EmitExpression(il, assignment.Value, symbolTable);
                // Set
                il.StElemI4();
            }
        }
    }
}