using System.Dynamic;
using ZenPlatform.Language.Ast.Definitions.Functions;

namespace ZenPlatform.Compiler.Generation
{
    public partial class Generator
    {
        private void EmitRegisterServerFunction(Function function)
        {
            //IInvokeService.Register("test", (context, list) => { a(list[0], list[1], list[2]); });

            var e = _serviceScope.ServiceInitializerInitMethod.Generator;
            
        }
    }
}