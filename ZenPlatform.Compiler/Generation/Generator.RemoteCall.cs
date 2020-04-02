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
                    e.LdLit(function.Parameters.Count);
                    var b = e.NewArrAdv(_bindings.Object);
                    foreach (var p in function.Parameters)
                    {
                        var iArg = function.Parameters.IndexOf(p);
                        e.LdArg(iArg);
                        b.PopArg();
                    }

                    b.EndBuild();
                });

            if (!_map.GetClrType(function.Type).Equals(_bindings.Void))
                function.Builder.Ret().Statement();
        }
    }
}