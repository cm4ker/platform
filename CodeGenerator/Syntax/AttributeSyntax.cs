using System.Collections.Generic;
using System.Text;

namespace CodeGeneration.Syntax
{
    public class AttributeSyntax : Syntax
    {
        private readonly string _attributeName;
        private Dictionary<string, ValueSyntax> _properties;
        private List<ValueSyntax> _constructorParams;
        private string _attrPreffix;


        public AttributeSyntax(string attributeName)
        {
            _attributeName = attributeName;
            _properties = new Dictionary<string, ValueSyntax>();
            _constructorParams = new List<ValueSyntax>();
        }

        public AttributeSyntax(string attributeName, string preffix)
        {
            _attributeName = attributeName;
            _properties = new Dictionary<string, ValueSyntax>();
            _constructorParams = new List<ValueSyntax>();
            _attrPreffix = preffix;
        }

        public void AddConstrunctorParam(ValueSyntax value)
        {
            _constructorParams.Add(value);
        }

        public void AddPropertyInitializer(string propName, ValueSyntax value)
        {
            _properties.Add(propName, value);
        }



        public override string ToString()
        {
            var sb = new StringBuilder();
            if (_properties.Count > 0 || _constructorParams.Count > 0)
            {
                if (string.IsNullOrEmpty(_attrPreffix))
                    sb.Append($"[{_attributeName}(");
                else
                    sb.Append($"[{_attrPreffix}: {_attributeName}(");
                var i = 0;

                foreach (var constructorParam in _constructorParams)
                {
                    if (i > 0) sb.Append(",");
                    sb.Append(constructorParam);
                    i++;
                }

                foreach (var value in _properties)
                {
                    if (i > 0)
                    {
                        sb.Append(",");
                    }
                    sb.AppendFormat("{0} = {1}", value.Key, value.Value);
                    i++;
                }
                sb.Append(")]");
            }
            else
            {
                sb.Append($"[{_attributeName}]");

            }
            return sb.ToString();
        }
    }
}