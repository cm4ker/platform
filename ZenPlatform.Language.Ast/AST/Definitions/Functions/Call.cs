using System.Collections.Generic;
using System.Collections.Immutable;
using System.Collections.Specialized;
using System.Linq;
using ZenPlatform.Compiler.Contracts.Symbols;
using ZenPlatform.Language.Ast.AST.Infrastructure;

namespace ZenPlatform.Language.Ast.AST.Definitions.Functions
{
    /// <summary>
    /// Описывает вызов метода
    /// </summary>
    public class Call : Expression
    {
        /// <summary>
        /// Название вызываемого метода
        /// </summary>
        public string Name { get; }

        /// <summary>
        /// Передаваемые аргументы функции
        /// </summary>
        public IReadOnlyList<Argument> Arguments { get; }

        /// <summary>
        /// Creates a function call object.
        /// </summary>
        public Call(ILineInfo li, ImmutableList<Argument> arguments, string name) : base(li)
        {
            Arguments = arguments;
            Name = name;

            var slot = 0;

            foreach (var argument in arguments)
            {
                Children.SetSlot(argument, slot);
                slot++;
            }
        }

        public override T Accept<T>(AstVisitorBase<T> visitor)
        {
            return visitor.VisitCall(this);
        }
    }
}