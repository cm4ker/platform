using System;
using System.Reflection;
using System.Security.Cryptography.X509Certificates;
using Microsoft.CodeAnalysis.Diagnostics;
using ZenPlatform.Compiler.Contracts;
using ZenPlatform.Compiler.Roslyn;
using ZenPlatform.Compiler.Roslyn.RoslynBackend;
using ZenPlatform.Configuration.Contracts;
using ZenPlatform.Core.Contracts;
using ZenPlatform.Core.Network;
using ZenPlatform.Language.Ast.Definitions.Functions;
using SystemTypeBindings = ZenPlatform.Compiler.Roslyn.SystemTypeBindings;

namespace ZenPlatform.Compiler.Generation
{
    public class EntryPointAssemblyManager : IEntryPointManager
    {
        private readonly RoslynAssemblyBuilder _builder;
        private SystemTypeBindings _sb;
        private RoslynTypeBuilder _ep;
        private RoslynMethodBuilder _main;

        private const string _classNamespace = "";
        private const string _className = "EntryPoint";

        private const string _mainMethodName = "Main";

        public EntryPointAssemblyManager(RoslynAssemblyBuilder builder)
        {
            _builder = builder;

            _sb = _builder.TypeSystem.GetSystemBindings();
            _ep = builder.DefineStaticType(_classNamespace, _className);

            _main = _ep.DefineMethod(_mainMethodName, true, true, false);
            _main.DefineParameter("args", _sb.Object.MakeArrayType(), false, false);
        }


        public RoslynTypeBuilder EntryPoint => _ep;

        public RoslynMethodBuilder Main => _main;

        public void EndBuild()
        {
            var e = _main.Body;
            // e.Ret();
        }
    }

    public static class EntryPotinExtensions
    {
        private const string _invokeServiceFieldName = "_is";
        private const string _linkFactoryFieldName = "_lf";

        public static RoslynType InvokeService(this RoslynTypeSystem ts)
        {
            return ts.FindType<IInvokeService>();
        }

        public static RoslynType LinkFactory(this RoslynTypeSystem ts)
        {
            return ts.FindType<ILinkFactory>();
        }

        public static void InitService(this IEntryPointManager am)
        {
            var ep = am.EntryPoint;
            var sb = ep.TypeSystem;
            var field = ep.DefineField(sb.InvokeService(), _invokeServiceFieldName, false, true);
            var lf = ep.DefineField(sb.LinkFactory(), _linkFactoryFieldName, false, true);

            am.Main.Body
                .LdArg_0()
                .LdLit(0)
                .LdElem()
                .Cast(sb.InvokeService())
                .StSFld(field)
                .LdArg_0()
                .LdLit(1)
                .LdElem()
                .Cast(sb.LinkFactory())
                .StSFld(lf);
        }

        public static RoslynField GetISField(this IEntryPointManager am)
        {
            return am.EntryPoint.FindField(_invokeServiceFieldName) ??
                   throw new Exception($"Platform not isnitialized you must invoke {nameof(InitService)} method first");
        }

        public static RoslynField GetLFField(this IEntryPointManager am)
        {
            return am.EntryPoint.FindField(_linkFactoryFieldName) ??
                   throw new Exception($"Platform not isnitialized you must invoke {nameof(InitService)} method first");
        }
    }
}