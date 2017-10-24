using System.Collections.Generic;
using System.Text;

namespace ZenPlatform.CSharpCodeBuilder.Syntax
{
    public class ClassSyntax : Syntax
    {
        private readonly TypeSyntax _baseType;
        private readonly string _name;
        private List<PropertySyntax> _propertyes;
        private List<AttributeSyntax> _attributes;

        public ClassSyntax(string name)
        {
            _name = name;
            _propertyes = new List<PropertySyntax>();
            _attributes = new List<AttributeSyntax>();
        }

        public ClassSyntax(string name, TypeSyntax baseType) : this(name)
        {
            _baseType = baseType;
        }


        public void AddProperty(PropertySyntax prop)
        {
            _propertyes.Add(prop);
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
                sb.AppendLine(attr.ToString());
            }
            sb.Append($"public class {_name}");
            if (_baseType != null)
                sb.AppendFormat(": {0}", _baseType);
            sb.AppendLine("\n{");
            foreach (var prop in _propertyes)
            {

                sb.AppendFormat("\t{0}", prop.ToString().Replace("\n", "\n\t").Trim('\t'));
            }

            sb.AppendLine("}");

            return sb.ToString();
        }
    }
}