using System.IO;
using Aquila.Compiler.Contracts;
using Aquila.Compiler.Roslyn.RoslynBackend;

namespace Aquila.Compiler.Roslyn
{
    public class TypeToken : Expression
    {
        private readonly IType _name;

        public TypeToken(IType name)
        {
            _name = name;
        }

        public override void Dump(TextWriter tw)
        {
            _name.DumpRef(tw);
        }
    }
}