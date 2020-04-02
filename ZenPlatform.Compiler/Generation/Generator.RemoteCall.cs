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
            function.Builder.RemoteCall(_map.GetClrType(function.Type),
                $"{function.FirstParent<TypeEntity>().Name}.{function.Name}",
                e =>
                {
                    e.LdcI4(function.Parameters.Count);
                    e.NewArr(_bindings.Object);
                    foreach (var p in function.Parameters)
                    {
                        e.Dup();
                        var iArg = function.Parameters.IndexOf(p);
                        e.LdcI4(iArg);
                        e.LdArg(iArg);
                        e.Box(_map.GetClrType(p.Type));
                        e.StElemRef();
                    }
                });

            if (!_map.GetClrType(function.Type).Equals(_bindings.Void))
                function.Builder.Ret();
        }
    }
}