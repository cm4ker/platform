using System.IO;
using ZenPlatform.Compiler.Roslyn.DnlibBackend;

namespace ZenPlatform.Compiler.Roslyn
{
    public class Declare : Expression
    {
        private readonly SreType _type;
        private readonly NameExpression _name;
        private readonly Expression _initializer;

        public Declare(Expression initializer, NameExpression name, SreType type)
        {
            _type = type;
            _name = name;
            _initializer = initializer;
        }

        public override void Dump(TextWriter tw)
        {
            _type.DumpRef(tw);
            tw.Space();
            _name.Dump(tw);

            if (_initializer != null)
            {
                tw.W(" = ");
                _initializer.Dump(tw);
            }
        }
    }
}