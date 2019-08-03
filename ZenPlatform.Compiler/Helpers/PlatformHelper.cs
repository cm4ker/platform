using System;
using System.Linq;
using System.Runtime.CompilerServices;
using Antlr4.Runtime;
using Mono.Cecil;
using ZenPlatform.AsmClientInfrastructure;
using ZenPlatform.Compiler.AST;
using ZenPlatform.Compiler.Contracts;
using ZenPlatform.Compiler.Contracts.Symbols;
using ZenPlatform.Language.Ast;
using ZenPlatform.Language.Ast.AST;
using ZenPlatform.Language.Ast.Definitions;
using ZenPlatform.ServerClientShared.Network;

namespace ZenPlatform.Compiler.Helpers
{
    public static class PlatformHelper
    {
        public static IMethod ClientInvoke(this SystemTypeBindings b)
        {
            return b.Client.Methods.FirstOrDefault(x => x.Name == nameof(Client.Invoke)) ??
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

        public static IType ToClrType(this TypeSyntax typeSyntax, IAssembly context)
        {
            if (typeSyntax is SingleTypeSyntax sts)
            {
                return ToClrType(typeSyntax, context.TypeSystem) ?? context.FindType(sts.TypeName);
            }

            return ToClrType(typeSyntax, context.TypeSystem);
        }

        public static IType ToClrType(this TypeSyntax typeSyntax, ITypeSystem context)
        {
            var _stb = context.GetSystemBindings();

            if (typeSyntax is SingleTypeSyntax stn)
            {
                return context.FindType(stn.TypeName) ?? context.FindType("System." + stn.TypeName);
            }

            else if (typeSyntax is PrimitiveTypeSyntax ptn)
            {
                return ptn.Kind switch
                    {
                    TypeNodeKind.Boolean => _stb.Boolean,
                    TypeNodeKind.Int => _stb.Int,
                    TypeNodeKind.Char => _stb.Char,
                    TypeNodeKind.Double => _stb.Double,
                    TypeNodeKind.String => _stb.String,
                    TypeNodeKind.Byte => _stb.Byte,
                    TypeNodeKind.Object => _stb.Object
                    };
            }

            else if (typeSyntax is ArrayTypeSyntax atn)
            {
                return ToClrType(atn.ElementType, context).MakeArrayType();
            }

            else if (typeSyntax is UnionTypeSyntax utn)
            {
                throw new NotImplementedException();
            }

            return null;
        }
    }

    public static class Helper
    {
        public static ILineInfo ToLineInfo(this IToken token)
        {
            return new LineInfo {Line = token.Line, Position = token.Column};
        }
    }
}