using System.Linq;
using Aquila.Compiler.Aqua.TypeSystem;
using Aquila.Compiler.Contracts;
using Aquila.Compiler.Helpers;
using Aquila.Language.Ast.Definitions;
using Aquila.Language.Ast.Definitions.Functions;
using Aquila.Core.Contracts;
using Aquila.Core.Contracts.Network;

namespace Aquila.Compiler.Generation
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
                    var b = e.NewArrAdv(_tm.Object);
                    foreach (var p in function.Parameters)
                    {
                        var iArg = function.Parameters.IndexOf(p);
                        e.LdArg(iArg);
                        b.PopArg();
                    }

                    b.EndBuild();
                });

            if (!_map.GetClrType(function.Type).Equals(_tm.Void))
                function.Builder.Ret();
        }
    }
}