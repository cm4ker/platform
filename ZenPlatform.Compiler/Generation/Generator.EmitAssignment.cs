using ZenPlatform.Compiler.AST.Definitions.Symbols;
using ZenPlatform.Compiler.AST.Infrastructure;
using ZenPlatform.Compiler.Contracts;
using ZenPlatform.Compiler.Contracts.Symbols;
using ZenPlatform.Compiler.Infrastructure;
using ZenPlatform.Language.Ast.AST.Definitions;
using ZenPlatform.Language.Ast.AST.Definitions.Functions;
using ZenPlatform.Language.Ast.AST.Definitions.Statements;
using ZenPlatform.Language.Ast.AST.Infrastructure;


namespace ZenPlatform.Compiler.Generation
{
    public partial class Generator
    {
        private void EmitAssignment(IEmitter e, Assignment assignment, SymbolTable symbolTable)
        {
            var variable = symbolTable.Find(assignment.Name, SymbolType.Variable);
            if (variable == null)
                Error("Assignment variable " + assignment.Name + " unknown.");

            // Non-indexed assignment
            if (assignment.Index == null)
            {
                bool mtNode = ((ITypedNode) variable.SyntaxObject).Type is MultiTypeNode;

                if (variable.CodeObject is IParameter pd)
                {
                    Parameter p = variable.SyntaxObject as Parameter;
                    if (p.PassMethod == PassMethod.ByReference)
                        e.LdArg(pd);
                }
                else if (variable.CodeObject is IField fl)
                {
                    EmitLoadThis(e);
                }
                else if (variable.CodeObject is IProperty p)
                {
                    EmitLoadThis(e);
                }
                else if (variable.CodeObject is ILocal l && mtNode)
                    e.LdLocA(l);


                // Load value
                EmitExpression(e, assignment.Value, symbolTable);


                if (mtNode)
                {
                    if (!(assignment.Value.Type.Type.Equals(_bindings.Object)
                          || assignment.Value.Type.Type.Equals(_bindings.UnionTypeStorage)))
                    {
                        e.Box(assignment.Value.Type.Type);
                    }

                    e.EmitCall(_bindings.UnionTypeStorage.FindProperty("Value").Setter);
                    return;
                }

                // Store
                if (variable.CodeObject is ILocal vd)
                    e.StLoc(vd);
                else if (variable.CodeObject is IField fd)
                    e.StFld(fd);
                else if (variable.CodeObject is IParameter ppd)
                {
                    Parameter p = variable.SyntaxObject as Parameter;
                    if (p.PassMethod == PassMethod.ByReference)
                        e.StIndI4();
                    else
                        e.StArg(ppd);
                }
            }
            else
            {
                // Load array.
                if (variable.CodeObject is ILocal vd)
                    e.LdLoc(vd);
                else if (variable.CodeObject is IField fd)
                    e.LdsFld(fd);
                else if (variable.CodeObject is IParameter pd)
                    e.LdArg(pd.Sequence);
                // Load index.
                EmitExpression(e, assignment.Index, symbolTable);
                // Load value.
                EmitExpression(e, assignment.Value, symbolTable);
                // Set
                e.StElemI4();
            }
        }

        private void EmitLoadThis(IEmitter il)
        {
            il.LdArg_0();
        }

        private void EmitLoad(IEmitter e, object codeObject, bool stat, bool addr)
        {
            if (stat)
                if (!addr)
                    if (codeObject is IField f)
                        e.LdFld(f);
                    else if (codeObject is ILocal l)
                        e.LdLoc(l);
                    else if (codeObject is IParameter p)
                        e.LdArg(p);
        }
    }
}