using System.Linq;
using Aquila.Compiler.Aqua.TypeSystem;
using Aquila.Language.Ast.Definitions;
using Aquila.Language.Ast.Definitions.Functions;

namespace Aquila.Compiler.Generation
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

            dlgt.DefineParameter("context", _tm.InvokeContext(), false, false);
            var argsParam = dlgt.DefineParameter("args", _tm.Object.MakeArrayType(), false, false);
            dlgt.SetReturnType(_tm.Object);

            var method = _stage1Methods[function];

            var dle = dlgt.Body;

            for (int i = 0; i < method.Parameters.Count(); i++)
            {
                dle.LdArg(argsParam)
                    .LdLit(i)
                    .LdElem()
                    .Cast(method.Parameters[i].Type);
            }

            dle.Call(method);

            if (method.ReturnType == _tm.Unknown)
            {
                dle.LdNull().Ret();
            }
            else
            {
                dle.Ret();
            }

            e.LdSFld(invs)
                .LdLit($"{function.FirstParent<TypeEntity>().Name}.{function.Name}")
                .NewObj(_tm.Route().Constructors.First())
                //.Null()
                .LdFtn(dlgt)
                //.NewObj(_bindings.ParametricMethod.Constructors.First())
                .Call(_tm.InvokeService().FindMethod(m => m.Name == "Register"));
        }
    }
}