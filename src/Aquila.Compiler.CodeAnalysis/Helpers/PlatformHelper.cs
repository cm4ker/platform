// using System;
// using System.Collections.Generic;
// using System.Linq;
// using Antlr4.Runtime;
// using Aquila.ClientRuntime;
// using Aquila.Compiler.Contracts;
// using Aquila.Compiler.Roslyn;
// using Aquila.Compiler.Roslyn.RoslynBackend;
// using Aquila.Core;
// using Aquila.Core.Contracts;
// using Aquila.Core.Contracts.Network;
// using Aquila.Core.Network;
// using Aquila.Language.Ast;
// using Aquila.Language.Ast.Definitions;
// using Aquila.Data;
// using Aquila.Language.Ast.Definitions.Functions;
// using Aquila.Language.Ast.Misc;
// using ArrayTypeSyntax = Aquila.Language.Ast.ArrayTypeSyntax;
// using SingleTypeSyntax = Aquila.Language.Ast.SingleTypeSyntax;
// using SystemTypeBindings = Aquila.Compiler.Roslyn.SystemTypeBindings;
// using TypeSyntax = Aquila.Language.Ast.TypeSyntax;
//
// namespace Aquila.Compiler.Helpers
// {
//     public static class PlatformHelper
//     {
//         public static RBlockBuilder LdContext(this RBlockBuilder e)
//         {
//             var m = e.TypeSystem.Resolve<ContextHelper>().FindMethod(nameof(ContextHelper.GetContext));
//             e.Call(m);
//             return e;
//         }
//
//         public static RBlockBuilder LdDataContext(this RBlockBuilder e)
//         {
//             var p = e.TypeSystem.Resolve<PlatformContext>().FindProperty(nameof(PlatformContext.DataContext));
//             e
//                 .LdContext()
//                 .LdProp(p);
//             return e;
//         }
//
//         public static RBlockBuilder NewDbCmdFromContext(this RBlockBuilder e)
//         {
//             var o = e.TypeSystem.Resolve<DataContext>().FindMethod(nameof(DataContext.CreateCommand));
//             e
//                 .LdDataContext()
//                 .Call(o);
//             return e;
//         }
//
//
//         public static RBlockBuilder DbTypes(this RBlockBuilder e)
//         {
//             var o = e.TypeSystem.Resolve<DataContext>().FindProperty(nameof(DataContext.Types));
//             e
//                 .LdDataContext()
//                 .LdProp(o);
//             return e;
//         }
//
//
//         public static RBlockBuilder LdDefaultDateTime(this RBlockBuilder e)
//         {
//             var o = e.TypeSystem.Resolve<IDbTypesContract>().FindProperty(nameof(IDbTypesContract.DateTime));
//             var o1 = e.TypeSystem.Resolve<IDateTime>().FindProperty(nameof(IDateTime.MinValue));
//
//             e
//                 .DbTypes()
//                 .LdProp(o)
//                 .LdProp(o1);
//             return e;
//         }
//
//         public static RoslynType Client(this RoslynTypeSystem ts)
//         {
//             return ts.FindType<IProtocolClient>();
//         }
//
//
//         public static RoslynType InvokeContext(this RoslynTypeSystem ts)
//         {
//             return ts.FindType<InvokeContext>();
//         }
//
//         public static RoslynType Route(this RoslynTypeSystem ts)
//         {
//             return ts.FindType<Route>();
//         }
//
//         public static RoslynMethod ClientInvoke(this RoslynTypeSystem b)
//         {
//             return b.Client().Methods.FirstOrDefault(x => x.Name == nameof(IPlatformClient.Invoke)) ??
//                    throw new NotSupportedException();
//         }
//
//         public static RoslynMethod ClientInvoke(this RoslynTypeSystem b, params RoslynType[] genParams)
//         {
//             return b.Client().Methods.FirstOrDefault(x => x.Name == "Invoke")?.MakeGenericMethod(genParams) ??
//                    throw new NotSupportedException();
//         }
//
//         public static RoslynType AsmInf(this SystemTypeBindings b)
//         {
//             return b.TypeSystem.FindType($"{typeof(GlobalScope).Namespace}.{nameof(GlobalScope)}",
//                 typeof(GlobalScope).Assembly.GetName().FullName);
//         }
//
//         /// <summary>
//         /// Свойство клиента
//         /// </summary>
//         /// <param name="b"></param>
//         /// <returns></returns>
//         public static RoslynProperty? AIClient(this SystemTypeBindings b)
//         {
//             return b.AsmInf().Properties.First(x => x.Name == nameof(GlobalScope.Client));
//         }
//
//
//         public static RBlockBuilder RemoteCall(this RBlockBuilder e, RoslynType result, string route,
//             Action<RBlockBuilder> paramHandler)
//         {
//             RBlockBuilder emitter = e;
//             var ts = e.TypeSystem;
//             var sb = ts.GetSystemBindings();
//             var type = ts.Client();
//
//             var client = emitter.DefineLocal(type);
//             emitter.LdProp(sb.AIClient());
//             emitter.StLoc(client);
//
//             var routeType = ts.FindType($"{typeof(Route).Namespace}.{nameof(Route)}",
//                 typeof(Route).Assembly.GetName().FullName);
//
//             var method = ts.ClientInvoke(result);
//
//             emitter.LdLoc(client);
//
//             //First parameter
//             emitter.LdLit(route);
//             emitter.NewObj(routeType.Constructors.First());
//
//             paramHandler(e);
//
//             emitter.Call(method);
//
//             return emitter;
//         }
//
//         public static TypeSyntax ToAstType(this IType type)
//         {
//             SingleTypeSyntax GetSingle(IType elementType)
//             {
//                 var singleType = new SingleTypeSyntax(null, elementType.FullName, TypeNodeKind.Unknown);
//
//                 if (elementType.IsPrimitive)
//                 {
//                     singleType.ChangeKind(elementType.Name switch
//                     {
//                         "String" => TypeNodeKind.String,
//                         "Int32" => TypeNodeKind.Int,
//                         "Byte" => TypeNodeKind.Byte,
//                         "Boolean" => TypeNodeKind.Boolean,
//                         _ => throw new Exception($"New unknown primitive type {elementType.Name}")
//                     });
//                 }
//                 else
//                 {
//                     singleType.ChangeKind(TypeNodeKind.Object);
//                 }
//
//                 return singleType;
//             }
//
//             TypeSyntax GetArray(IType elementType)
//             {
//                 if (elementType.IsArray)
//                     return GetArray(elementType.ArrayElementType);
//
//                 return new ArrayTypeSyntax(GetSingle(elementType));
//             }
//
//             if (type.IsArray)
//             {
//                 return GetArray(type.ArrayElementType);
//             }
//
//             return GetSingle(type);
//         }
//
//         public static TypeSyntax ToAstType(this RoslynType type)
//         {
//             SingleTypeSyntax GetSingle(RoslynType elementType)
//             {
//                 var singleType = new SingleTypeSyntax(null, elementType.FullName, TypeNodeKind.Unknown);
//
//                 if (elementType.IsPrimitive)
//                 {
//                     singleType.ChangeKind(elementType.Name switch
//                     {
//                         "String" => TypeNodeKind.String,
//                         "Int32" => TypeNodeKind.Int,
//                         "Byte" => TypeNodeKind.Byte,
//                         "Boolean" => TypeNodeKind.Boolean,
//                         _ => throw new Exception($"New unknown primitive type {elementType.Name}")
//                     });
//                 }
//                 else
//                 {
//                     singleType.ChangeKind(TypeNodeKind.Object);
//                 }
//
//                 return singleType;
//             }
//
//             TypeSyntax GetArray(RoslynType elementType)
//             {
//                 if (elementType.IsArray)
//                     return GetArray(elementType.ArrayElementType);
//
//                 return new ArrayTypeSyntax(GetSingle(elementType));
//             }
//
//             if (type.IsArray)
//             {
//                 return GetArray(type.ArrayElementType);
//             }
//
//             return GetSingle(type);
//         }
//     }
//

using System.Collections.Generic;
using System.Linq;
using Antlr4.Runtime;
using Antlr4.Runtime.Tree;
using Aquila.Compiler.Syntax;
using Aquila.Core.Contracts;
using Aquila.Language.Ast;
using Aquila.Language.Ast.Definitions;
using Aquila.Language.Ast.Definitions.Functions;
using Aquila.Language.Ast.Misc;

public static class Helper
{
    public static ILineInfo ToLineInfo(this ParserRuleContext context)
    {
        return new SpanInfo
        {
            Line = context.start.Line,
            Position = context.start.Column,
            StartIndex = context.start.StartIndex,
            StopIndex = context.stop.StopIndex
        };
    }

    public static ILineInfo ToLineInfo(this IToken context)
    {
        return new SpanInfo
        {
            Line = context.Line,
            Position = context.Column,
            StartIndex = context.StartIndex,
            StopIndex = context.StopIndex
        };
    }

    public static IdentifierToken Identifier(this ITerminalNode node)
    {
        return new IdentifierToken(node.Symbol.ToLineInfo(), SyntaxKind.IdentifierToken, node.GetText());
    }


    // public static IEnumerable<Method> FilterFunc(this IEnumerable<Method> list, CompilationMode mode)
    // {
    //     return list.Where(x => ((int) x.Flags & (int) mode) != 0);
    // }
}
// }