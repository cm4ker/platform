using System;

namespace ZenPlatform.CSharpCodeBuilder.Syntax
{
    public class TypeSyntax : Syntax
    {
        private readonly string _typeName;

        public TypeSyntax(string typeName)
        {
            _typeName = typeName;
        }

        public TypeSyntax(Type type)
        {
            _typeName = type.CSharpName();
        }

        public override string ToString()
        {
            return _typeName;
        }
    }
}