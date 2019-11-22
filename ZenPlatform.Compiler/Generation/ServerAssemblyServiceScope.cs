using System.Reflection;
using ZenPlatform.Compiler.Contracts;

namespace ZenPlatform.Compiler.Generation
{
    public class ServerAssemblyServiceScope
    {
        private readonly IAssemblyBuilder _builder;
        private SystemTypeBindings _sb;
        private const string _serviceInitializerNamespace = "Service";
        private const string _serviceInitializerName = "ServerInitializer";
        private const string _invokeServiceFieldName = "_is";

        private const string _servInitMethod = "Init";

        public ServerAssemblyServiceScope(IAssemblyBuilder builder)
        {
            _builder = builder;

            _sb = _builder.TypeSystem.GetSystemBindings();

            ServiceInitializerType = builder.DefineType(_serviceInitializerNamespace, _serviceInitializerName,
                TypeAttributes.Class | TypeAttributes.Public | TypeAttributes.AnsiClass | TypeAttributes.RTSpecialName,
                _sb.Object);

            ServiceInitializerType.AddInterfaceImplementation(_sb.ServerInitializer);

            ServiceInitializerInitMethod = ServiceInitializerType.DefineMethod(_servInitMethod, true, false, true);

            ServiceInitializerConstructor = ServiceInitializerType.DefineConstructor(false, _sb.InvokeService);

            InvokeServiceField =
                ServiceInitializerType.DefineField(_sb.InvokeService, _invokeServiceFieldName, false, false);

            var e = ServiceInitializerConstructor.Generator;

            e.LdArg_0()
                .EmitCall(_sb.Object.Constructors[0])
                .LdArg_0()
                .LdArg(1)
                .StFld(InvokeServiceField)
                .Ret();
        }

        public ITypeBuilder ServiceInitializerType { get; }

        public IMethodBuilder ServiceInitializerInitMethod { get; }

        public IConstructorBuilder ServiceInitializerConstructor { get; }

        public IField InvokeServiceField { get; private set; }
    }


    public class GlobalVarManager
    {


        public void Register()
        {
            
        }

        public void Emit()
        {
            
        }
    }
}