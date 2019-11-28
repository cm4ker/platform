using System.Data.Common;
using ZenPlatform.Compiler.Contracts;
using ZenPlatform.Compiler.Contracts.Symbols;
using ZenPlatform.Compiler.Helpers;
using ZenPlatform.Language.Ast.Definitions;

namespace ZenPlatform.Compiler.Generation
{
    public partial class Generator
    {
        private void PrebuildProperties(TypeBody typeBody, ITypeBuilder tb)
        {
            foreach (var field in typeBody.Fields)
            {
                var fieldCodeObj = tb.DefineField(field.Type.ToClrType(_asm), field.Name, false, false);
                typeBody.SymbolTable.ConnectCodeObject(field, fieldCodeObj);
            }

            foreach (var property in typeBody.Properties)
            {
                var propBuilder = tb.DefineProperty(property.Type.ToClrType(_asm), property.Name, false);

                IField backField = null;

                if (property.Setter == null && property.Getter == null)
                {
                    backField = tb.DefineField(property.Type.ToClrType(_asm), $"{property.Name}_backingField", false,
                        false);
                }

                var getMethod = tb.DefineMethod($"get_{property.Name}", true, false, property.IsInterface);
                var setMethod = tb.DefineMethod($"set_{property.Name}", true, false, property.IsInterface);

                setMethod.WithReturnType(_bindings.Void);
                var valueArg = setMethod.DefineParameter("value", property.Type.ToClrType(_asm), false, false);

                getMethod.WithReturnType(property.Type.ToClrType(_asm));

                if (property.Getter != null)
                {
                    IEmitter emitter = getMethod.Generator;
                    emitter.InitLocals = true;

                    ILocal resultVar = null;

                    resultVar = emitter.DefineLocal(property.Type.ToClrType(_asm));

                    var returnLabel = emitter.DefineLabel();
                    EmitBody(emitter, property.Getter, returnLabel, ref resultVar);

                    emitter.MarkLabel(returnLabel);

                    if (resultVar != null)
                        emitter.LdLoc(resultVar);

                    emitter.Ret();
                }
                else
                {
                    getMethod.Generator.LdArg_0().LdFld(backField).Ret();
                }

                if (property.Setter != null)
                {
                    IEmitter emitter = setMethod.Generator;
                    emitter.InitLocals = true;

                    ILocal resultVar = null;

                    resultVar = emitter.DefineLocal(property.Type.ToClrType(_asm));

                    var valueSym = property.Setter.SymbolTable.Find("value", SymbolType.Variable, SymbolScopeBySecurity.Shared);
                    valueSym.CodeObject = valueArg;

                    var returnLabel = emitter.DefineLabel();
                    EmitBody(emitter, property.Setter, returnLabel, ref resultVar);

                    emitter.MarkLabel(returnLabel);
                    emitter.Ret();
                }
                else
                {
                    if (backField != null)
                        setMethod.Generator.LdArg_0().LdArg(1).StFld(backField).Ret();
                    else
                        setMethod.Generator.Ret();
                }

                propBuilder.WithGetter(getMethod).WithSetter(setMethod);
            }
        }
    }
}