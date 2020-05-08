using System.Collections.Generic;
using System.Collections.Immutable;
using Aquila.Compiler.Contracts;
using Aquila.Compiler.Contracts.Symbols;
using Aquila.Language.Ast.Definitions.Functions;

namespace Aquila.Language.Ast.Definitions
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