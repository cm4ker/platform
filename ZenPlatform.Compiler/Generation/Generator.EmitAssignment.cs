using ZenPlatform.Compiler.AST.Definitions.Symbols;
using ZenPlatform.Compiler.AST.Infrastructure;
using ZenPlatform.Compiler.Contracts;
using ZenPlatform.Compiler.Contracts.Symbols;
using ZenPlatform.Language.Ast.AST.Definitions.Functions;
using ZenPlatform.Language.Ast.AST.Infrastructure;


namespace ZenPlatform.Compiler.Generation
{
    public class GeneratorMethodContext
    {
        public bool InsideTheClass { get; set; }

        public int GetParameterSequence(IParameter parameter)
        {
            return parameter.Sequence + (InsideTheClass ? 1 : 0);
        }
    }

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
                if (variable.CodeObject is IParameter pd)
                {
                    Parameter p = variable.SyntaxObject as Parameter;
                    if (p.PassMethod == PassMethod.ByReference)
                        il.LdArg(pd.Sequence - 1);
                }
                else if (variable.CodeObject is IField fl)
                {
                    //load "this"
                    il.LdArg_0();
                }

                // Load value
                EmitExpression(il, assignment.Value, symbolTable);

                // Store
                if (variable.CodeObject is ILocal vd)
                    il.StLoc(vd);
                else if (variable.CodeObject is IField fd)
                    il.StFld(fd);
                else if (variable.CodeObject is IParameter ppd)
                {
                    Parameter p = variable.SyntaxObject as Parameter;
                    if (p.PassMethod == PassMethod.ByReference)
                        il.StIndI4();
                    else
                        il.StArg(ppd);
                }
            }
            else
            {
                // Load array.
                if (variable.CodeObject is ILocal vd)
                    il.LdLoc(vd);
                else if (variable.CodeObject is IField fd)
                    il.LdsFld(fd);
                else if (variable.CodeObject is IParameter pd)
                    il.LdArg(pd.Sequence);
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