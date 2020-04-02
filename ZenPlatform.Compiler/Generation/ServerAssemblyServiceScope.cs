using System;
using System.Reflection;
using System.Security.Cryptography.X509Certificates;
using Microsoft.CodeAnalysis.Diagnostics;
using ZenPlatform.Compiler.Contracts;
using ZenPlatform.Configuration.Contracts;
using ZenPlatform.Language.Ast.Definitions.Functions;

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
                TypeAttributes.Class | TypeAttributes.Public | TypeAttributes.AnsiClass
                | TypeAttributes.AutoClass | TypeAttributes.BeforeFieldInit,
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

        public void EndBuild()
        {
            var e = ServiceInitializerInitMethod.Generator;
            e.Ret();
        }
    }

    public class EntryPointAssemblyManager : IEntryPointManager
    {
        private readonly IAssemblyBuilder _builder;
        private SystemTypeBindings _sb;
        private ITypeBuilder _ep;
        private IMethodBuilder _main;

        private const string _classNamespace = "";
        private const string _className = "EntryPoint";

        private const string _mainMethodName = "Main";

        public EntryPointAssemblyManager(IAssemblyBuilder builder)
        {
            _builder = builder;

            _sb = _builder.TypeSystem.GetSystemBindings();

            _ep = builder.DefineStaticType(_classNamespace, _className);

            _main = _ep.DefineMethod(_mainMethodName, true, true, false);
            _main.DefineParameter("args", _sb.Object.MakeArrayType(), false, false);
        }


        public ITypeBuilder EntryPoint => _ep;

        public IMethodBuilder Main => _main;

        public void EndBuild()
        {
            var e = _main.Generator;
            e.Ret();
        }
    }

    public static class EntryPotinExtensions
    {
        private const string _invokeServiceFieldName = "_is";

        public static void InitService(this IEntryPointManager am)
        {
            var ep = am.EntryPoint;
            var sb = ep.TypeSystem.GetSystemBindings();
            var field = ep.DefineField(sb.InvokeService, _invokeServiceFieldName, false, true);

            am.Main.Generator
                .LdArg_0()
                .LdcI4(0)
                .LdElemRef()
                .CastClass(sb.InvokeService)
                .StSFld(field);
        }

        public static IField GetISField(this IEntryPointManager am)
        {
            return am.EntryPoint.FindField(_invokeServiceFieldName) ??
                   throw new Exception($"Platform not isnitialized you must invoke {nameof(InitService)} method first");
        }
    }
}