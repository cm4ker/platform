using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace ZenPlatform.Compiler.Roslyn.InfoProviders
{
    public class RuntimeModule
    {
        private readonly RuntimeAssembly _asm;
        private readonly Module _module;
        private IEnumerable<RuntimeTypeInfo> _types;

        public RuntimeModule(RuntimeAssembly asm, Module module)
        {
            _asm = asm;
            _module = module;
            _types = _module.GetTypes().Select(x => new RuntimeTypeInfo(this, x.GetTypeInfo()));
        }

        public IEnumerable<RuntimeTypeInfo> Types => _types;
    }
}