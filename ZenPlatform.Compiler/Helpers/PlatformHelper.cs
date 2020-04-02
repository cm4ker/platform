using System;
using System.Collections.Generic;
using System.Linq;
using Antlr4.Runtime;
using ZenPlatform.ClientRuntime;
using ZenPlatform.Compiler.Contracts;
using ZenPlatform.Compiler.Contracts.Symbols;
using ZenPlatform.Compiler.Roslyn;
using ZenPlatform.Compiler.Roslyn.DnlibBackend;
using ZenPlatform.Core;
using ZenPlatform.Core.Contracts;
using ZenPlatform.Language.Ast;
using ZenPlatform.Language.Ast.Definitions;
using ZenPlatform.Core.Network.Contracts;
using ZenPlatform.Data;
using ZenPlatform.Language.Ast.Definitions.Functions;
using SystemTypeBindings = ZenPlatform.Compiler.Roslyn.SystemTypeBindings;

namespace ZenPlatform.Compiler.Helpers
{
    public static class PlatformHelper
    {
        public static RBlockBuilder LdContext(this RBlockBuilder e)
        {
            var m = e.TypeSystem.Resolve<ContextHelper>().FindMethod(nameof(ContextHelper.GetContext));
            e.Call(m);
            return e;
        }

        public static RBlockBuilder LdDataContext(this RBlockBuilder e)
        {
            var p = e.TypeSystem.Resolve<PlatformContext>().FindProperty(nameof(PlatformContext.DataContext));
            e
                .LdContext()
                .LdProp(p);
            return e;
        }

        public static RBlockBuilder NewDbCmdFromContext(this RBlockBuilder e)
        {
            var o = e.TypeSystem.Resolve<DataContext>().FindMethod(nameof(DataContext.CreateCommand));
            e
                .LdDataContext()
                .Call(o);
            return e;
        }


        public static RBlockBuilder DbTypes(this RBlockBuilder e)
        {
            var o = e.TypeSystem.Resolve<DataContext>().FindProperty(nameof(DataContext.Types));
            e
                .LdDataContext()
                .LdProp(o);
            return e;
        }


        public static RBlockBuilder LdDefaultDateTime(this RBlockBuilder e)
        {
            var o = e.TypeSystem.Resolve<IDbTypesContract>().FindProperty(nameof(IDbTypesContract.DateTime));
            var o1 = e.TypeSystem.Resolve<IDateTime>().FindProperty(nameof(IDateTime.MinValue));

            e
                .DbTypes()
                .LdProp(o)
                .LdProp(o1);
            return e;
        }


        public static SreMethod ClientInvoke(this SystemTypeBindings b)
        {
            return b.Client.Methods.FirstOrDefault(x => x.Name == nameof(IPlatformClient.Invoke)) ??
                   throw new NotSupportedException();
        }

        public static SreMethod ClientInvoke(this SystemTypeBindings b, params SreType[] genParams)
        {
            return b.Client.Methods.FirstOrDefault(x => x.Name == "Invoke")?.MakeGenericMethod(genParams) ??
                   throw new NotSupportedException();
        }

        public static SreType AsmInf(this SystemTypeBindings b)
        {
            return b.TypeSystem.FindType($"{typeof(GlobalScope).Namespace}.{nameof(GlobalScope)}",
                typeof(GlobalScope).Assembly.GetName().FullName);
        }

        /// <summary>
        /// Свойство клиента
        /// </summary>
        /// <param name="b"></param>
        /// <returns></returns>
        public static SreProperty? AIClient(this SystemTypeBindings b)
        {
            return b.AsmInf().Properties.First(x => x.Name == nameof(GlobalScope.Client));
        }


        public static RBlockBuilder RemoteCall(this RBlockBuilder e, SreType result, string route,
            Action<RBlockBuilder> paramHandler)
        {
            RBlockBuilder emitter = e;
            var ts = e.TypeSystem;
            var sb = ts.GetSystemBindings();
            var type = sb.Client;

            var client = emitter.DefineLocal(type);
            emitter.LdProp(sb.AIClient());
            emitter.StLoc(client)
                .Statement();

            var routeType = ts.FindType($"{typeof(Route).Namespace}.{nameof(Route)}",
                typeof(Route).Assembly.GetName().FullName);

            var method = sb.ClientInvoke(result);

            emitter.LdLoc(client);

            //First parameter
            emitter.LdLit(route);
            emitter.NewObj(routeType.Constructors.First());

            paramHandler(e);

            emitter.Call(method);

            return emitter;
        }

        public static TypeSyntax ToAstType(this IType type)
        {
            SingleTypeSyntax GetSingle(IType elementType)
            {
                var singleType = new SingleTypeSyntax(null, elementType.FullName, TypeNodeKind.Unknown);

                if (elementType.IsPrimitive)
                {
                    singleType.ChangeKind(elementType.Name switch
                    {
                        "String" => TypeNodeKind.String,
                        "Int32" => TypeNodeKind.Int,
                        "Byte" => TypeNodeKind.Byte,
                        "Boolean" => TypeNodeKind.Boolean,
                        _ => throw new Exception($"New unknown primitive type {elementType.Name}")
                    });
                }
                else
                {
                    singleType.ChangeKind(TypeNodeKind.Object);
                }

                return singleType;
            }

            TypeSyntax GetArray(IType elementType)
            {
                if (elementType.IsArray)
                    return GetArray(elementType.ArrayElementType);

                return new ArrayTypeSyntax(GetSingle(elementType));
            }

            if (type.IsArray)
            {
                return GetArray(type.ArrayElementType);
            }

            return GetSingle(type);
        }

        public static TypeSyntax ToAstType(this SreType type)
        {
            SingleTypeSyntax GetSingle(SreType elementType)
            {
                var singleType = new SingleTypeSyntax(null, elementType.FullName, TypeNodeKind.Unknown);

                if (elementType.IsPrimitive)
                {
                    singleType.ChangeKind(elementType.Name switch
                    {
                        "String" => TypeNodeKind.String,
                        "Int32" => TypeNodeKind.Int,
                        "Byte" => TypeNodeKind.Byte,
                        "Boolean" => TypeNodeKind.Boolean,
                        _ => throw new Exception($"New unknown primitive type {elementType.Name}")
                    });
                }
                else
                {
                    singleType.ChangeKind(TypeNodeKind.Object);
                }

                return singleType;
            }

            TypeSyntax GetArray(SreType elementType)
            {
                if (elementType.IsArray)
                    return GetArray(elementType.ArrayElementType);

                return new ArrayTypeSyntax(GetSingle(elementType));
            }

            if (type.IsArray)
            {
                return GetArray(type.ArrayElementType);
            }

            return GetSingle(type);
        }
    }

    public static class Helper
    {
        public static ILineInfo ToLineInfo(this IToken token)
        {
            return new LineInfo {Line = token.Line, Position = token.Column};
        }

        public static IEnumerable<Function> FilterFunc(this IEnumerable<Function> list, CompilationMode mode)
        {
            return list.Where(x => ((int) x.Flags & (int) mode) != 0);
        }
    }
}