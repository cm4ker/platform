using System.Dynamic;
using System.Linq;
using ZenPlatform.Compiler.Contracts;
using ZenPlatform.Compiler.Helpers;
using ZenPlatform.Compiler.Roslyn;
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
            var e = _epManager.Main.Body;
            var invs = _epManager.GetISField();

            var dlgt = _epManager.EntryPoint.DefineMethod($"dlgt_{function.Name}", true, true, false);

            dlgt.DefineParameter("context", _ts.InvokeContext(), false, false);
            var argsParam = dlgt.DefineParameter("args", _bindings.Object.MakeArrayType(), false, false);
            dlgt.WithReturnType(_bindings.Object);

            var method = _stage1Methods[function];

            var dle = dlgt.Body;

            for (int i = 0; i < method.Parameters.Count; i++)
            {
                dle.LdArg(argsParam)
                    .LdLit(i)
                    .LdElem()
                    .Cast(method.Parameters[i].Type);
            }

            dle.Call(method);

            if (method.ReturnType == _bindings.Void)
            {
                dle.Null().Ret().Statement();
            }
            else
            {
                dle.Ret().Statement();
            }

            e.LdSFld(invs)
                .LdLit($"{function.FirstParent<TypeEntity>().Name}.{function.Name}")
                .NewObj(_ts.Route().Constructors.First())
                //.Null()
                .LdFtn(dlgt)
                //.NewObj(_bindings.ParametricMethod.Constructors.First())
                .Call(_ts.InvokeService().FindMethod(m => m.Name == "Register"))
                .Statement();
        }
    }
}