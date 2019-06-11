using System;
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
            if (variable.Type is MultiTypeNode mtn)
            {
                local = e.DefineLocal(_bindings.MultiTypeDataStorage);
                var mt = _asm.FindType("PlatformCustom.DefinedMultitypes");

                e.LdLocA(local)
                    .LdsFld(mt.FindField(mtn.DeclName));
            }
            else
                local = e.DefineLocal(variable.Type.Type);

            context.SymbolTable.ConnectCodeObject(variable, local);

            //
            // Initialize  variable.
            //


            if (variable.Value is Expression ex)
            {
                if (variable.Value != null)
                {
                    EmitExpression(e, ex, context.SymbolTable);

                    if (ex.Type is MultiTypeNode)
                    {
                        e.EmitCall(_bindings.MultiTypeDataStorage.FindProperty("Value").Getter);
                    }

                    if (variable.Type is MultiTypeNode && !(ex.Type is MultiTypeNode))
                        e.Box(ex.Type.Type);

                    //e.StLoc(local);
                }
            }
//            else if (variable.Value is Expression || variable.Value is ElementCollection)
//            {
//                // Empty array initialization.
//                if (variable.Value != null && variable.Value is Expression value)
//                {
//                    EmitExpression(e, value, context.SymbolTable);
//                    e.NewArr(variable.Type.Type.ArrayElementType);
//                    e.StLoc(local);
//                }
//                else if (variable.Value != null && variable.Value is ElementCollection)
//                {
//                    ElementCollection elements = variable.Value as ElementCollection;
//
//                    e.LdcI4(elements.Count);
//                    e.NewArr(variable.Type.Type.ArrayElementType);
//                    e.StLoc(local);
//
//                    for (int x = 0; x < elements.Count; x++)
//                    {
//                        // Load array
//                        e.LdLoc(local);
//                        // Load index
//                        e.LdcI4(x);
//                        // Load value
//                        EmitExpression(e, elements[x].Expression, context.SymbolTable);
//                        // Store
//                        e.StElemI4();
//                    }
//                }
//            }

            if (variable.Type is MultiTypeNode)
            {
                e.EmitCall(_bindings.MultiTypeDataStorage.FindConstructor(_bindings.MultiType, _bindings.Object));
            }
        }
    }
}