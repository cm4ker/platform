using Aquila.Compiler.Contracts;
using Aquila.Core.Contracts.TypeSystem;

namespace Aquila.Compiler.Aqua.TypeSystem
{
    public class PLocal
    {
        public PLocal(PType type, int index)
        {
            Type = type;
            Index = index;
        }

        public PType Type { get; }

        public int Index { get; }

        public ILocal BackendLocal { get; set; }
    }
}