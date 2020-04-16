using ZenPlatform.Compiler.Roslyn.DnlibBackend;

namespace ZenPlatform.Compiler.Roslyn
{
    public class RLocal
    {
        public RLocal(string name, RoslynType type)
        {
            Name = name;
            Type = type;
        }

        public string Name { get; }

        public RoslynType Type { get; }
    }
}