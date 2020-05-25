using Aquila.Compiler.Contracts;
using Aquila.Compiler.Roslyn.RoslynBackend;

namespace Aquila.Compiler.Roslyn
{
    public class RLocal : ILocal
    {
        public RLocal(int index, IType type)
        {
            Name = $"loc{index}";
            Index = index;
            Type = type;
        }

        public string Name { get; }

        public int Index { get; }

        public IType Type { get; }
    }
}