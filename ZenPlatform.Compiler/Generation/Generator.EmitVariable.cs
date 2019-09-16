using System;
using ZenPlatform.Compiler.Contracts;
using ZenPlatform.Compiler.Contracts.Symbols;
using ZenPlatform.Compiler.Helpers;
using ZenPlatform.Language.Ast.Definitions;
using ZenPlatform.Language.Ast.Infrastructure;

namespace ZenPlatform.Compiler.Generation
{
    public partial class Generator
    {
        private void LoadValue(IEmitter e, Block context)
        {
        }

        private void EmitVariable(IEmitter e, SymbolTable symTable, Variable variable)
        {
            //load phase 

            if (variable.Value is Expression expr)
            {
                EmitExpression(e, expr, symTable);

                if (variable.Type.ToClrType(_asm).Equals(_bindings.Object))
                {
                    e.Box(expr.Type.ToClrType(_asm));
                }
            }
            else if (false) // variable.Value is ElementCollection ec)
            {
                // Empty array initialization.
                if (variable.Value != null && variable.Value is Expression value)
                {
                    EmitExpression(e, value, symTable);
                    e.NewArr(variable.Type.ToClrType(_asm).ArrayElementType);
                }
                else if (variable.Value != null)
                {
                    ElementCollection elements = null; //variable.Value as ElementCollection;

                    e.LdcI4(elements.Count);
                    e.NewArr(variable.Type.ToClrType(_asm).ArrayElementType);

                    for (int x = 0; x < elements.Count; x++)
                    {
                        // Load array
                        e.Dup();
                        // Load index
                        e.LdcI4(x);
                        // Load value
                        EmitExpression(e, elements[x].Expression, symTable);
                        // Store
                        e.StElemI4();
                    }
                }
            }

            //store phase
            ILocal local = e.DefineLocal(variable.Type.ToClrType(_asm));


            if (variable.Value is Expression ex)
            {
                if (variable.Value != null)
                {
                    e.StLoc(local);
                }
            }
            else if (false) //variable.Value is ElementCollection ec)
            {
                e.StLoc(local);
            }

            symTable.ConnectCodeObject(variable, local);
        }

        private void WrapMultitypeStackValue(IEmitter e, UnionTypeSyntax mtn, ILocal local, ILocal exp)
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

        private void WrapMultitypeNode(IEmitter e, UnionTypeSyntax mtn, ref ILocal local)
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