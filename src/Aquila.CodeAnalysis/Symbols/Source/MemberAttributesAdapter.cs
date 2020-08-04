﻿using Microsoft.CodeAnalysis;

namespace Aquila.CodeAnalysis.Symbols.Source
{
    static class MemberAttributesAdapter
    {
        public static Accessibility GetAccessibility(this PhpMemberAttributes member)
        {
            if ((member & PhpMemberAttributes.Private) != 0)
                return Accessibility.Private;

            if ((member & PhpMemberAttributes.Protected) != 0)
                return Accessibility.Protected;

            return Accessibility.Public;
        }

        public static bool IsStatic(this PhpMemberAttributes member) => (member & PhpMemberAttributes.Static) != 0;
        public static bool IsSealed(this PhpMemberAttributes member) => (member & PhpMemberAttributes.Final) != 0;
        public static bool IsAbstract(this PhpMemberAttributes member) => (member & PhpMemberAttributes.Abstract) != 0;
    }
}
