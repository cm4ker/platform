using System.Collections.Generic;
using System.Collections.Immutable;
using ZenPlatform.Compiler.Contracts;
using ZenPlatform.Compiler.Contracts.Symbols;
using ZenPlatform.Language.Ast.Definitions.Functions;

namespace ZenPlatform.Language.Ast.Definitions
{
    /// <summary>
    /// Описывает вызов метода
    /// </summary>
    public partial class Call : Expression
    {
        public bool IsStatement { get; set; }
    }

    public class ClrInternalCall : Expression
    {
        public ClrInternalCall(IMethod method, ArgumentList arguments) : base(null)
        {
            Arguments = arguments;
            Method = method;
        }

        public IMethod Method { get; }

        public ArgumentList Arguments { get; }
    }
}