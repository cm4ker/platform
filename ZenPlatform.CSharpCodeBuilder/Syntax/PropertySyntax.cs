using System.Collections.Generic;
using System.Text;

namespace ZenPlatform.CSharpCodeBuilder.Syntax
{
    public class PropertySyntax : Syntax
    {
        private readonly string _name;
        private readonly TypeSyntax _typeSyntax;
        private List<AttributeSyntax> _attributes;


        public PropertySyntax(string name, TypeSyntax typeSyntax)
        {
            _name = name;
            _typeSyntax = typeSyntax;
            _attributes = new List<AttributeSyntax>();
        }

        public void AddAttribute(AttributeSyntax attr)
        {
            _attributes.Add(attr);
        }

        public override string ToString()
        {
            var sb = new StringBuilder();
            foreach (var attr in _attributes)
            {
                sb.AppendFormat("{0}\n", attr);
            }
            sb.AppendLine($"public {_typeSyntax} {_name} {{ get; set; }}\n");
            return sb.ToString();
        }
    }
}