using System.Collections.Generic;
using System.Text;

namespace CodeGeneration.Syntax
{
    public class DocumentSyntax : Syntax
    {
        private List<UsingSyntax> _usings;
        private List<NamespaceSyntax> _namespases;
        private List<AttributeSyntax> _attributes;

        public DocumentSyntax()
        {
            _usings = new List<UsingSyntax>();
            _namespases = new List<NamespaceSyntax>();
            _attributes = new List<AttributeSyntax>();
        }

        public void AddUsing(UsingSyntax usingSyntax)
        {
            _usings.Add(usingSyntax);
        }

        public void AddNamespace(NamespaceSyntax namespaceSyntax)
        {
            _namespases.Add(namespaceSyntax);
        }

        public void AddAttribute(AttributeSyntax attr)
        {
            _attributes.Add(attr);
        }

        public override string ToString()
        {
            var sb = new StringBuilder();
            foreach (var usingSyntax in _usings)
            {
                sb.AppendLine(usingSyntax.ToString());
            }
            sb.AppendLine();

            foreach (var attr in _attributes)
            {
                sb.AppendLine(attr.ToString());
            }

            sb.AppendLine();

            foreach (var namespaceSyntax in _namespases)
            {
                sb.AppendLine(namespaceSyntax.ToString());
            }
            return sb.ToString();
        }
    }

}