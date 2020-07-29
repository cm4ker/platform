// using System.Linq;
// using Aquila.Compiler.Contracts;
// using Aquila.Compiler.Helpers;
// using Aquila.Language.Ast.Definitions;
// using Aquila.Language.Ast.Definitions.Functions;
// using Aquila.Core.Contracts;
// using Aquila.Core.Contracts.Network;
//
// namespace Aquila.Compiler.Generation
// {
//     public partial class Generator
//     {
//         private void EmitRemoteCall(Method method)
//         {
//             method.Builder.RemoteCall(_map.GetClrType(method.Type),
//                 $"{method.FirstParent<TypeEntity>().Name}.{method.Name}",
//                 e =>
//                 {
//                     e.LdLit(method.Parameters.Count);
//                     var b = e.NewArrAdv(_bindings.Object);
//                     foreach (var p in method.Parameters)
//                     {
//                         var iArg = method.Parameters.IndexOf(p);
//                         e.LdArg(iArg);
//                         b.PopArg();
//                     }
//
//                     b.EndBuild();
//                 });
//
//             if (!_map.GetClrType(method.Type).Equals(_bindings.Void))
//                 method.Builder.Ret();
//         }
//     }
// }