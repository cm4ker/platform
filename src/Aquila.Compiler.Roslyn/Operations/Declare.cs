using System.IO;
using Aquila.Compiler.Contracts;
using Aquila.Compiler.Roslyn.RoslynBackend;

namespace Aquila.Compiler.Roslyn
{
    public class Declare : Expression
    {
        private readonly IType _type;
        private readonly NameExpression _name;
        private readonly Expression _initializer;

        public Declare(Expression initializer, NameExpression name, IType type)
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