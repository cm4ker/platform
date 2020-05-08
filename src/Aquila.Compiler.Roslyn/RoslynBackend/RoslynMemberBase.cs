using System.IO;

namespace Aquila.Compiler.Roslyn.RoslynBackend
{
    public abstract class RoslynMemberBase
    {
        public virtual string Name { get; }

        public virtual void DumpRef(TextWriter tw)
        {
            tw.W(Name);
        }
    }
}