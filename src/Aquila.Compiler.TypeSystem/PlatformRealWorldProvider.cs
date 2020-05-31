using Aquila.Compiler.Aqua.TypeSystem;
using Aquila.Compiler.Contracts;

namespace Aquila.Compiler.Aqua
{
    public class PlatformRealWorldProvider
    {
        private readonly TypeManager _tm;
        private readonly ITypeSystem _ts;

        public PlatformRealWorldProvider(TypeManager tm, ITypeSystem ts)
        {
            _tm = tm;
            _ts = ts;
        }

        public void Find(string typeName)
        {
            // 1. Find the external type
            var type = _ts.FindType(typeName);

            //2. Register it in the manager
            _tm.ExportType(type);
        }
    }
}