using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace ZenPlatform.Compiler.Roslyn.InfoProviders
{
    public class RuntimeAssembly
    {
        private readonly Assembly _asm;
        private readonly List<RuntimeModule> _modules;

        public RuntimeAssembly(Assembly asm)
        {
            _asm = asm;
            _modules = _asm.Modules.Select(x => new RuntimeModule(this, x)).ToList();
        }

        public IEnumerable<RuntimeModule> Modules => _modules;
    }
}