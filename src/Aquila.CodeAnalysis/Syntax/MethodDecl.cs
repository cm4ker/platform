using System;

namespace Aquila.CodeAnalysis.Syntax
{
    [Flags]
    public enum MemberModifiers
    {
        None = 0,
        Private = 0,
        Public = 0x1,
        Static = 0x2,
    }

    public partial class MethodDecl
    {
        private MemberModifiers? _memberModifers;

        public MemberModifiers GetModifiers()
        {
            if (_memberModifers == null)
            {
                _memberModifers = MemberModifiers.None;

                foreach (var modifier in Modifiers)
                {
                    _memberModifers |= modifier.Text switch
                    {
                        "public" => MemberModifiers.Public,
                        "static" => MemberModifiers.Static,
                        "private" => MemberModifiers.Private
                    };
                }
            }

            return _memberModifers.Value;
        }


        public bool IsGlobal => Parent is CompilationUnitSyntax;
    }
}