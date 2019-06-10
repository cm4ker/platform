using System;
using System.Linq;
using System.Runtime.CompilerServices;
using Antlr4.Runtime;
using Mono.Cecil;
using ZenPlatform.AsmInfrastructure;
using ZenPlatform.Compiler.AST;
using ZenPlatform.Compiler.Contracts;
using ZenPlatform.Language.Ast.AST;
using ZenPlatform.ServerClientShared.Network;

namespace ZenPlatform.Compiler.Helpers
{
    public static class PlatformHelper
    {
        public static IType ClientType(this SystemTypeBindings b)
        {
            return b.TypeSystem.FindType($"{typeof(Client).Namespace}.{nameof(Client)}",
                typeof(Client).Assembly.GetName().FullName);
        }

        public static IMethod ClientInvoke(this SystemTypeBindings b)
        {
            return b.ClientType().Methods.FirstOrDefault(x => x.Name == nameof(Client.Invoke)) ??
                   throw new NotSupportedException();
        }

        public static IMethod ClientInvoke(this SystemTypeBindings b, params IType[] genParams)
        {
            return b.ClientType().Methods.FirstOrDefault(x => x.Name == "Invoke")?.MakeGenericMethod(genParams) ??
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
    }

    public static class Helper
    {
        public static ILineInfo ToLineInfo(this IToken token)
        {
            return new LineInfo {Line = token.Line, Position = token.Column};
        }
    }
}