using System;
using Hyperion.Internal;
using ZenPlatform.Compiler.Contracts.Symbols;
using ZenPlatform.Language.Ast.AST.Definitions.Statements;
using ZenPlatform.Language.Ast.AST.Infrastructure;

namespace ZenPlatform.Language.Ast.AST.Definitions.Functions
{
    /// <summary>
    /// Describes a function call statement.
    /// </summary>
    public class CallStatement : Statement
    {
        /// <summary>
        /// Function to call.
        /// </summary>
        public string Name;

        /// <summary>
        /// Function arguments to pass.
        /// </summary>
        public ArgumentCollection Arguments { get; }

        /// <summary>
        /// Creates a function call object.
        /// </summary>
        public CallStatement(ILineInfo li, ArgumentCollection arguments, string name) : base(li)
        {
            Arguments = arguments ?? throw new ArgumentNullException(nameof(Arguments));
            Name = name;
        }

        public override void Accept<T>(IVisitor<T> visitor)
        {
            throw new System.NotImplementedException();
        }
    }
}