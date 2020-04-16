using System.IO;
using ZenPlatform.Compiler.Roslyn.RoslynBackend;

namespace ZenPlatform.Compiler.Roslyn
{
    public class TypeToken : Expression
    {
        private readonly RoslynType _name;

        public TypeToken(RoslynType name)
        {
            _name = name;
        }

        public override void Dump(TextWriter tw)
        {
            _name.DumpRef(tw);
        }
    }
}