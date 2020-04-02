using System.Reflection;

namespace ZenPlatform.Compiler.Roslyn.InfoProviders
{
    public class RuntimeTypeInfo
    {
        private readonly TypeInfo _ti;

        public RuntimeTypeInfo(RuntimeModule module, TypeInfo ti)
        {
            _ti = ti;
            Module = module;
        }

        public string Name => _ti.Name;

        public string FullName => _ti.FullName;

        public string Namespace => _ti.Namespace;

        public bool IsPublic => _ti.IsPublic;

        public bool IsNestedAssembly => _ti.IsNestedAssembly;

        public RuntimeModule Module { get; }
    }
}