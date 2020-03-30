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
            var e = _serviceScope.ServiceInitializerInitMethod.Generator;
            var invs = _serviceScope.InvokeServiceField;

            var dlgt = _serviceScope.ServiceInitializerType.DefineMethod($"dlgt_{function.Name}", true, false, false);

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

            e.LdArg_0()
                .LdFld(invs)
                .LdStr($"{function.FirstParent<TypeEntity>().Name}.{function.Name}")
                .NewObj(_bindings.Route.Constructors.First())
                .LdArg_0()
                .LdFtn(dlgt)
                .NewObj(_bindings.ParametricMethod.Constructors.First())
                .EmitCall(_bindings.InvokeService.FindMethod((m) => m.Name == "Register"));
        }

        private void EmitEndRegisterServerFunction()
        {
            var e = _serviceScope.ServiceInitializerInitMethod.Generator;
            e.Ret();
        }
    }
}