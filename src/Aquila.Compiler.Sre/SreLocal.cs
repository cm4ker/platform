using System.Reflection.Emit;
using Aquila.Compiler.Contracts;

namespace Aquila.Compiler.Sre
{
    class SreLocal : ILocal
    {
        private readonly SreTypeSystem _ts;
        public LocalBuilder Local { get; }

        public SreLocal(SreTypeSystem ts, LocalBuilder local)
        {
            _ts = ts;
            Local = local;
        }

        public int Index => Local.LocalIndex;
        public IType Type => _ts.ResolveType(Local.LocalType);
    }
}