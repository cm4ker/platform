using System.Dynamic;
using System.Linq;
using ZenPlatform.Compiler.Contracts;
using ZenPlatform.Compiler.Helpers;
using ZenPlatform.Language.Ast.Definitions;
using ZenPlatform.Language.Ast.Definitions.Functions;

namespace ZenPlatform.Compiler.Generation
{
    public partial class Generator
    {
        /// <summary>
        /// Зарегистрировать маршрут функции на сервере
        /// </summary>
        /// <param name="function"></param>
        private void EmitRegisterServerFunction(Function function)
        {
            var e = _epManager.Main.Generator;
            var invs = _epManager.GetISField();

            var dlgt = _epManager.EntryPoint.DefineMethod($"dlgt_{function.Name}", true, true, false);

            dlgt.DefineParameter("context", _bindings.InvokeContext, false, false);
            var argsParam = dlgt.DefineParameter("args", _bindings.Object.MakeArrayType(), false, false);
            dlgt.WithReturnType(_bindings.Object);

            var method = _stage1Methods[function];

            var dle = dlgt.Generator;

            for (int i = 0; i < method.Parameters.Count; i++)
            {
                dle.LdArg(argsParam)
                    .LdcI4(i)
                    .LdElemRef()
                    .Unbox_Any(method.Parameters[i].Type);
            }

            dle.EmitCall(method);

            if (method.ReturnType == _bindings.Void)
            {
                dle.LdNull().Ret();
            }
            else
            {
                dle.Box(method.ReturnType).Ret();
            }

            e.LdSFld(invs)
                .LdStr($"{function.FirstParent<TypeEntity>().Name}.{function.Name}")
                .NewObj(_bindings.Route.Constructors.First())
                .LdNull()
                .LdFtn(dlgt)
                .NewObj(_bindings.ParametricMethod.Constructors.First())
                .EmitCall(_bindings.InvokeService.FindMethod(m => m.Name == "Register"));
        }
    }
}