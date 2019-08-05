using ZenPlatform.Compiler.Contracts.Symbols;

namespace ZenPlatform.Language.Ast.Definitions.Statements
{
    public partial class Match
    {
    }

    public partial class MatchAtom
    {
        public MatchAtom(ILineInfo li, Block block, Expression exp) : this(li, block, exp, null)
        {
        }

        public MatchAtom(ILineInfo li, Block block, TypeSyntax ts) : this(li, block, null, ts)
        {
        }
    }
}