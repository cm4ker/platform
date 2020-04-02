using System.IO;
using ZenPlatform.Compiler.Roslyn.DnlibBackend;

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

    public class TypeToken : Expression
    {
        private readonly SreType _name;

        public TypeToken(SreType name)
        {
            _name = name;
        }

        public override void Dump(TextWriter tw)
        {
            _name.DumpRef(tw);
        }
    }
}