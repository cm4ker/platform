using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Aquila.Syntax.Text;

namespace Aquila.Syntax.Ast.Expressions
{
    public partial record LiteralEx
    {
        public object ObjectiveValue { get; set; }
    }
}


namespace Aquila.Syntax.Ast
{
    public partial record EmptyElement : LangElement
    {
        public EmptyElement() : base(Span.Empty, SyntaxKind.None)
        {
        }

        public override T Accept<T>(AstVisitorBase<T> visitor)
        {
            return default;
        }

        public override void Accept(AstVisitorBase visitor)
        {
        }

        public override IEnumerable<LangElement> GetChildren()
        {
            yield break;
        }
    }
}