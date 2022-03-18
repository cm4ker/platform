using Aquila.CodeAnalysis.Syntax;
using Aquila.Syntax.Ast;
using Microsoft.CodeAnalysis;

namespace Aquila.CodeAnalysis.Symbols.Source
{
    static class MemberAttributesAdapter
    {
        public static Accessibility GetAccessibility(this MemberModifiers member)
        {
            if ((member & MemberModifiers.Public) != 0)
                return Accessibility.Public;

            return Accessibility.Private;
        }

        public static bool IsStatic(this MemberModifiers member) => (member & MemberModifiers.Static) != 0;

        public static bool IsPartial(this MemberModifiers member) => (member & MemberModifiers.Partial) != 0;
    }
}