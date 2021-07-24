namespace Aquila.Syntax.Ast.Functions
{
    public partial record MethodDecl
    {
        private MemberModifiers? _memberModifers;

        public MemberModifiers GetModifiers()
        {
            if (_memberModifers == null)
            {
                _memberModifers = MemberModifiers.None;

                foreach (var modifier in modifiers)
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
    }
}