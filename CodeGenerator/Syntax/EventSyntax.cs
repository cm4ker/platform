using System.Collections.Generic;
using System.Text;

namespace CodeGeneration.Syntax
{
    public class EventSyntax : Syntax
    {
        private readonly string _name;
        private readonly TypeSyntax _typeSyntax;
        private List<AttributeSyntax> _attributes;


        public EventSyntax(string name, TypeSyntax delegateTypeSyntax)
        {
            _name = name;
            _typeSyntax = delegateTypeSyntax;
            _attributes = new List<AttributeSyntax>();
        }

        public override string ToString()
        {
            var sb = new StringBuilder();
            foreach (var attr in _attributes)
            {
                sb.AppendFormat("{0}\n", attr);
            }
            sb.AppendLine($"public event {_typeSyntax} {_name}\n");
            return sb.ToString();
        }
    }
}