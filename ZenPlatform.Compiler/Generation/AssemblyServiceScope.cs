using System.Reflection;
using ZenPlatform.Compiler.Contracts;

namespace ZenPlatform.Compiler.Generation
{
    public class AssemblyServiceScope
    {
        private readonly IAssemblyBuilder _builder;
        private SystemTypeBindings _sb;
        private const string _serviceInitializerNamespace = "Service";
        private const string _serviceInitializerName = "ServerInitializer";

        private const string _servInitMethod = "Init";

        public AssemblyServiceScope(IAssemblyBuilder builder)
        {
            _builder = builder;

            _sb = _builder.TypeSystem.GetSystemBindings();

            ServiceInitializerType = builder.DefineType(_serviceInitializerNamespace, _serviceInitializerName,
                TypeAttributes.Class, _sb.ServerInitializer);

            ServiceInitializerInitMethod = ServiceInitializerType.DefineMethod(_servInitMethod, true, false, true);

            ServiceInitializerConstructor = ServiceInitializerType.DefineConstructor(false, _sb.InvokeService);
        }

        public ITypeBuilder ServiceInitializerType { get; }

        public IMethodBuilder ServiceInitializerInitMethod { get; }

        public IConstructorBuilder ServiceInitializerConstructor { get; }
    }
}