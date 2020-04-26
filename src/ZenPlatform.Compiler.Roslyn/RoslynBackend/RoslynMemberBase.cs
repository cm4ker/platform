using System.IO;

namespace ZenPlatform.Compiler.Roslyn.RoslynBackend
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