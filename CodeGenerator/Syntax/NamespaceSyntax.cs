using System.Collections.Generic;
using System.Text;

namespace CodeGeneration.Syntax
{
    public class NamespaceSyntax : Syntax
    {
        private readonly string _name;
        private List<ClassSyntax> _classes;

        public NamespaceSyntax(string name)
        {
            _name = name;
            _classes = new List<ClassSyntax>();
        }

        public void AddClass(ClassSyntax cls)
        {
            _classes.Add(cls);
        }

        public override string ToString()
        {
            var sb = new StringBuilder();
            sb.AppendLine($"namespace {_name}");
            sb.AppendLine("{");
            foreach (var @class in _classes)
            {
                sb.AppendFormat("\t{0}", @class.ToString().Replace("\n", "\n\t").Trim('\t'));
            }
            sb.AppendLine("}");

            return sb.ToString();
        }
    }
}