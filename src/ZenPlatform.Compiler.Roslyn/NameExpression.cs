using System.IO;

namespace ZenPlatform.Compiler.Roslyn
{
    public class NameExpression : Expression
    {
        private readonly string _name;

        public NameExpression(string name)
        {
            _name = name;
        }

        public override void Dump(TextWriter tw)
        {
            tw.W(_name);
        }
    }
}