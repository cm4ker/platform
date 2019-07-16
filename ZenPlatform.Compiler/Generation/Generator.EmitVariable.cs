using System;
using ZenPlatform.Compiler.Contracts;
using ZenPlatform.Compiler.Helpers;
using ZenPlatform.Language.Ast.AST.Definitions;
using ZenPlatform.Language.Ast.AST.Infrastructure;

namespace ZenPlatform.Compiler.Generation
{
    public partial class Generator
    {
        private void LoadValue(IEmitter e, BlockNode context)
        {
        }

        private void EmitVariable(IEmitter e, BlockNode context, Variable variable)
        {
            //load phase 

            if (variable.Value is Expression expr)
            {
                EmitExpression(e, expr, context.SymbolTable);

//                if (variable.Type.Type.Equals(_bindings.Object))
//                {
//                    e.Box(expr.Type.Type);
//                }
            }
//            else if (variable.Value is ElementCollection ec)
//            {
//                // Empty array initialization.
//                if (variable.Value != null && variable.Value is Expression value)
//                {
//                    EmitExpression(e, value, context.SymbolTable);
//                    e.NewArr(variable.Type.Type.ArrayElementType);
//                }
//                else if (variable.Value != null)
//                {
//                    ElementCollection elements = variable.Value as ElementCollection;
//
//                    e.LdcI4(elements.Count);
//                    e.NewArr(variable.Type.Type.ArrayElementType);
//
//                    for (int x = 0; x < elements.Count; x++)
//                    {
//                        // Load array
//                        e.Dup();
//                        // Load index
//                        e.LdcI4(x);
//                        // Load value
//                        EmitExpression(e, elements[x].Expression, context.SymbolTable);
//                        // Store
//                        e.StElemI4();
//                    }
//                }
//            }

            //store phase
            ILocal local;
//            if (variable.Type is UnionTypeNode)
//            {
//                local = e.DefineLocal(_bindings.Object);
//            }
//            else
//                local = e.DefineLocal(variable.Type.Type);
//
//
//            if (variable.Value is Expression ex)
//            {
//                if (variable.Value != null)
//                {
//                    if (variable.Type is UnionTypeNode && !(ex.Type is UnionTypeNode))
//                        e.Box(ex.Type.Type);
//                    e.StLoc(local);
//                }
//            }
//            else if (variable.Value is ElementCollection ec)
//            {
//                e.StLoc(local);
//            }

//            if (variable.Type is UnionTypeNode mtn)
//            {
//                WrapMultitypeNode(e, mtn, ref local);
//            }
//
//            context.SymbolTable.ConnectCodeObject(variable, local);
        }

        private void WrapMultitypeStackValue(IEmitter e, UnionTypeNode mtn, ILocal local, ILocal exp)
        {
            var mt = _asm.FindType("PlatformCustom.DefinedMultitypes");
            e.LdLocA(local);
            e.Dup();
            e.LdsFld(mt.FindField(mtn.DeclName));
            e.EmitCall(_bindings.UnionTypeStorage.FindConstructor(_bindings.MultiType));
            e.LdLoc(local);
            e.EmitCall(_bindings.UnionTypeStorage.FindProperty("Value").Setter);
            e.LdLoc(local);
        }

        private void WrapMultitypeNode(IEmitter e, UnionTypeNode mtn, ref ILocal local)
        {
            var localWrap = e.DefineLocal(_bindings.UnionTypeStorage);
            var mt = _asm.FindType("PlatformCustom.DefinedMultitypes");
            e.LdLocA(localWrap);
            e.Dup();
            e.LdsFld(mt.FindField(mtn.DeclName));
            e.EmitCall(_bindings.UnionTypeStorage.FindConstructor(_bindings.MultiType));
            e.LdLoc(local);
            e.EmitCall(_bindings.UnionTypeStorage.FindProperty("Value").Setter);
            local = localWrap;
        }
    }
}