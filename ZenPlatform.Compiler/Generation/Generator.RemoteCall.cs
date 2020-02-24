using System.Linq;
using ZenPlatform.Compiler.Contracts;
using ZenPlatform.Compiler.Helpers;
using ZenPlatform.Language.Ast.Definitions;
using ZenPlatform.Language.Ast.Definitions.Functions;
using ZenPlatform.Core.Contracts;
using ZenPlatform.Core.Contracts.Network;

namespace ZenPlatform.Compiler.Generation
{
    public partial class Generator
    {
        private void EmitRemoteCall(Function function)
        {
            IEmitter emitter = function.Builder;
            var type = _bindings.Client;
            var client = emitter.DefineLocal(type);
            emitter.PropGetValue(_bindings.AIClient());
            emitter.StLoc(client);

            var route = _ts.FindType($"{typeof(Route).Namespace}.{nameof(Route)}",
                typeof(Route).Assembly.GetName().FullName);

            var method = _bindings.ClientInvoke(_map.GetType(function.Type));

            emitter.LdLoc(client);

            //First parameter
            emitter.LdStr($"{function.FirstParent<TypeEntity>().Name}.{function.Name}");
            emitter.NewObj(route.Constructors.First());

            //Second parameter
            emitter.LdcI4(function.Parameters.Count);
            emitter.NewArr(_bindings.Object);
            foreach (var p in function.Parameters)
            {
                emitter.Dup();
                var iArg = function.Parameters.IndexOf(p);
                emitter.LdcI4(iArg);
                emitter.LdArg(iArg);
                emitter.Box(_map.GetType(p.Type));
                emitter.StElemRef();
            }

            emitter.EmitCall(method);
            //            emitter.LdcI4(0);
            //            emitter.LdElemI4();

            if (!_map.GetType(function.Type).Equals(_bindings.Void))
                emitter.Ret();
        }
    }
}