using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using Antlr4.Runtime;
using Mono.Cecil;
using ZenPlatform.ClientRuntime;
using ZenPlatform.Compiler.AST;
using ZenPlatform.Compiler.Contracts;
using ZenPlatform.Compiler.Contracts.Symbols;
using ZenPlatform.Core;
using ZenPlatform.Language.Ast;
using ZenPlatform.Language.Ast.AST;
using ZenPlatform.Language.Ast.Definitions;
using ZenPlatform.Core.Network;
using ZenPlatform.Core.Network.Contracts;
using ZenPlatform.Data;
using ZenPlatform.Language.Ast.Definitions.Functions;

namespace ZenPlatform.Compiler.Helpers
{
    public static class PlatformHelper
    {
        public static IEmitter LdContext(this IEmitter e)
        {
            var m = e.TypeSystem.FindType<ContextHelper>().FindMethod(nameof(ContextHelper.GetContext));
            e.EmitCall(m);
            return e;
        }

        public static IEmitter LdDataContext(this IEmitter e)
        {
            var p = e.TypeSystem.FindType<PlatformContext>().FindProperty(nameof(PlatformContext.DataContext));
            e
                .LdContext()
                .EmitCall(p.Getter);
            return e;
        }

        public static IEmitter NewDbCmdFromContext(this IEmitter e)
        {
            var o = e.TypeSystem.FindType<DataContext>().FindMethod(nameof(DataContext.CreateCommand));
            e
                .LdDataContext()
                .EmitCall(o);
            return e;
        }


        public static IEmitter DbTypes(this IEmitter e)
        {
            var o = e.TypeSystem.FindType<DataContext>().FindProperty(nameof(DataContext.Types));
            e
                .LdDataContext()
                .EmitCall(o.Getter);
            return e;
        }


        public static IEmitter LdDefaultDateTime(this IEmitter e)
        {
            var o = e.TypeSystem.FindType<IDbTypesContract>().FindProperty(nameof(IDbTypesContract.DateTime));
            var o1 = e.TypeSystem.FindType<IDateTime>().FindProperty(nameof(IDateTime.MinValue));

            e
                .DbTypes()
                .EmitCall(o.Getter)
                .EmitCall(o1.Getter);
            return e;
        }


        public static IMethod ClientInvoke(this SystemTypeBindings b)
        {
            return b.Client.Methods.FirstOrDefault(x => x.Name == nameof(IPlatformClient.Invoke)) ??
                   throw new NotSupportedException();
        }

        public static IMethod ClientInvoke(this SystemTypeBindings b, params IType[] genParams)
        {
            return b.Client.Methods.FirstOrDefault(x => x.Name == "Invoke")?.MakeGenericMethod(genParams) ??
                   throw new NotSupportedException();
        }

        public static IType AsmInf(this SystemTypeBindings b)
        {
            return b.TypeSystem.FindType($"{typeof(GlobalScope).Namespace}.{nameof(GlobalScope)}",
                typeof(GlobalScope).Assembly.GetName().FullName);
        }

        /// <summary>
        /// Свойство клиента
        /// </summary>
        /// <param name="b"></param>
        /// <returns></returns>
        public static IProperty? AIClient(this SystemTypeBindings b)
        {
            return b.AsmInf().Properties.First(x => x.Name == nameof(GlobalScope.Client));
        }

        // public static IType ToClrType(this TypeSyntax typeSyntax, IAssembly context)
        // {
        //     if (typeSyntax is SingleTypeSyntax sts)
        //     {
        //         return ToClrType(typeSyntax, context.TypeSystem) ?? context.FindType(sts.TypeName);
        //     }
        //
        //     return ToClrType(typeSyntax, context.TypeSystem);
        // }

        // public static IType ToClrType(this TypeSyntax typeSyntax, ITypeSystem context, List<UsingBase> usings)
        // {
        //     if (typeSyntax is SingleTypeSyntax sts)
        //     {
        //         var type = ToClrType(typeSyntax, context) ?? context.FindType(sts.TypeName);
        //
        //         if (type == null)
        //             foreach (var @using in usings)
        //             {
        //                 if (@using is UsingDeclaration)
        //                     type = context.FindType(@using + sts.TypeName);
        //                 if (@using is UsingAliasDeclaration ad && ad.Alias == sts.TypeName)
        //                     return context.FindType(ad.ClassName);
        //             }
        //
        //         return type;
        //     }
        //
        //     return ToClrType(typeSyntax, context);
        // }

        // public static IType ToClrType(this TypeSyntax typeSyntax, ITypeSystem context)
        // {
        //     var _stb = context.GetSystemBindings();
        //
        //     if (typeSyntax is SingleTypeSyntax stn)
        //     {
        //         return context.FindType(stn.TypeName) ?? context.FindType("System." + stn.TypeName);
        //     }
        //     else if (typeSyntax is GenericTypeSyntax gts)
        //     {
        //         IType[] args = new IType[gts.Args.Count];
        //
        //         for (int i = 0; i < gts.Args.Count; i++)
        //         {
        //             args[i] = gts.Args[i].ToClrType(context);
        //         }
        //
        //         return context.FindType($@"{gts.TypeName}`{gts.Args.Count}").MakeGenericType(args);
        //     }
        //     else if (typeSyntax is PrimitiveTypeSyntax ptn)
        //     {
        //         return ptn.Kind switch
        //         {
        //             TypeNodeKind.Boolean => _stb.Boolean,
        //             TypeNodeKind.Int => _stb.Int,
        //             TypeNodeKind.Char => _stb.Char,
        //             TypeNodeKind.Double => _stb.Double,
        //             TypeNodeKind.String => _stb.String,
        //             TypeNodeKind.Byte => _stb.Byte,
        //             TypeNodeKind.Object => _stb.Object,
        //             TypeNodeKind.Void => _stb.Void,
        //             TypeNodeKind.Session => _stb.Session,
        //             TypeNodeKind.Context => context.FindType<PlatformContext>(),
        //             _ => throw new Exception($"This type is not primitive {ptn.Kind}")
        //         };
        //     }
        //
        //     else if (typeSyntax is ArrayTypeSyntax atn)
        //     {
        //         return ToClrType(atn.ElementType, context).MakeArrayType();
        //     }
        //
        //     else if (typeSyntax is UnionTypeSyntax utn)
        //     {
        //         throw new NotImplementedException();
        //     }
        //
        //     return null;
        // }

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