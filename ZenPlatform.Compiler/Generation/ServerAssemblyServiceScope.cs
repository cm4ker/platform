using System;
using System.Reflection;
using System.Security.Cryptography.X509Certificates;
using Microsoft.CodeAnalysis.Diagnostics;
using ZenPlatform.Compiler.Roslyn;
using ZenPlatform.Compiler.Roslyn.DnlibBackend;
using ZenPlatform.Configuration.Contracts;
using ZenPlatform.Language.Ast.Definitions.Functions;

namespace ZenPlatform.Compiler.Generation
{
    // public class ServerAssemblyServiceScope
    // {
    //     private readonly SreAssemblyBuilder _builder;
    //     private SystemTypeBindings _sb;
    //
    //     private const string _serviceInitializerNamespace = "Service";
    //     private const string _serviceInitializerName = "ServerInitializer";
    //     private const string _invokeServiceFieldName = "_is";
    //
    //     private const string _servInitMethod = "Init";
    //
    //     public ServerAssemblyServiceScope(SreAssemblyBuilder builder)
    //     {
    //         _builder = builder;
    //
    //        // _sb = _builder.TypeSystem.GetSystemBindings();
    //
    //         ServiceInitializerType = builder.DefineType(_serviceInitializerNamespace, _serviceInitializerName,
    //             TypeAttributes.Class | TypeAttributes.Public | TypeAttributes.AnsiClass
    //             | TypeAttributes.AutoClass | TypeAttributes.BeforeFieldInit,
    //             _sb.Object);
    //
    //         ServiceInitializerType.AddInterfaceImplementation(_sb.ServerInitializer);
    //
    //         ServiceInitializerInitMethod = ServiceInitializerType.DefineMethod(_servInitMethod, true, false, true);
    //
    //         ServiceInitializerConstructor = ServiceInitializerType.DefineConstructor(false, _sb.InvokeService);
    //
    //         InvokeServiceField =
    //             ServiceInitializerType.DefineField(_sb.InvokeService, _invokeServiceFieldName, false, false);
    //
    //         var e = ServiceInitializerConstructor.Generator;
    //
    //         e.LdArg_0()
    //             .EmitCall(_sb.Object.Constructors[0])
    //             .LdArg_0()
    //             .LdArg(1)
    //             .StFld(InvokeServiceField)
    //             .Ret();
    //     }
    //
    //     public SreTypeBuilder ServiceInitializerType { get; }
    //
    //     public SreMethodBuilder ServiceInitializerInitMethod { get; }
    //
    //     public SreConstructorBuilder ServiceInitializerConstructor { get; }
    //
    //     public SreField InvokeServiceField { get; private set; }
    //
    //     // public void EndBuild()
    //     // {
    //     //     var e = ServiceInitializerInitMethod.Generator;
    //     //     e.Ret();
    //     // }
    // }

    public class EntryPointAssemblyManager : IEntryPointManager
    {
        private readonly SreAssemblyBuilder _builder;
        private SystemTypeBindings _sb;
        private SreTypeBuilder _ep;
        private SreMethodBuilder _main;

        private const string _classNamespace = "";
        private const string _className = "EntryPoint";

        private const string _mainMethodName = "Main";

        public EntryPointAssemblyManager(SreAssemblyBuilder builder)
        {
            _builder = builder;

            _sb = _builder.TypeSystem.GetSystemBindings();
            _ep = builder.DefineStaticType(_classNamespace, _className);

            _main = _ep.DefineMethod(_mainMethodName, true, true, false);
            _main.DefineParameter("args", _sb.Object.MakeArrayType(), false, false);
        }


        public SreTypeBuilder EntryPoint => _ep;

        public SreMethodBuilder Main => _main;

        public void EndBuild()
        {
            var e = _main.Body;
            // e.Ret();
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

            am.Main.Body
                .LdArg_0()
                .LdLit(0)
                .LdElem()
                .Cast(sb.InvokeService)
                .StSFld(field)
                .Statement();
        }

        public static SreField GetISField(this IEntryPointManager am)
        {
            return am.EntryPoint.FindField(_invokeServiceFieldName) ??
                   throw new Exception($"Platform not isnitialized you must invoke {nameof(InitService)} method first");
        }
    }
}