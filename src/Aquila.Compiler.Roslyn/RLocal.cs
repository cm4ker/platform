using System.IO;
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


    public class RLabel : ILabel
    {
        private readonly int _index;

        public RLabel(int index)
        {
            _index = index;
        }

        public void Dump(TextWriter tw)
        {
            tw.W($"lbl_{_index}");
        }
    }
}