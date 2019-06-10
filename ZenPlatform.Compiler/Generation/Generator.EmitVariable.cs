using ZenPlatform.Compiler.Contracts;
using ZenPlatform.Compiler.Helpers;
using ZenPlatform.Language.Ast.AST.Definitions;
using ZenPlatform.Language.Ast.AST.Infrastructure;

namespace ZenPlatform.Compiler.Generation
{
    public partial class Generator
    {
        private void EmitVariable(IEmitter e, InstructionsBodyNode context, Variable variable)
        {
            ILocal local;
//            if (variable.Type.Type.Equals(_bindings.MultiTypeDataStorage))
//            {
//                local = e.DefineLocal((variable.Type.Type);
//
//                e.Newobj(variable.Type.Type.FindConstructor());
//                e.EmitCall(_bindings.ClientInvoke())
//            }
//            else
            local = e.DefineLocal(variable.Type.Type);

            context.SymbolTable.ConnectCodeObject(variable, local);

            //
            // Initialize  variable.
            //

            if (variable.Type.Type.IsValueType)
            {
                if (variable.Value != null && variable.Value is Expression)
                {
                    EmitExpression(e, (Expression) variable.Value, context.SymbolTable);

                    e.StLoc(local);
                }
            }
            else if (variable.Type.Type.IsArray)
            {
                // Empty array initialization.
                if (variable.Value != null && variable.Value is Expression value)
                {
                    EmitExpression(e, value, context.SymbolTable);
                    e.NewArr(variable.Type.Type.ArrayElementType);
                    e.StLoc(local);
                }
                else if (variable.Value != null && variable.Value is ElementCollection)
                {
                    ElementCollection elements = variable.Value as ElementCollection;

                    e.LdcI4(elements.Count);
                    e.NewArr(variable.Type.Type.ArrayElementType);
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
    }
}