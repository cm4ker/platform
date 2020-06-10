using System;
using Aquila.Compiler.Aqua.TypeSystem;
using Aquila.Compiler.Aqua.TypeSystem.Builders;
using Aquila.Compiler.Contracts;
using Aquila.Core.Contracts;
using Aquila.Core.Contracts.Configuration;
using Aquila.Core.Contracts.Network;
using Aquila.Core.Contracts.TypeSystem;

namespace Aquila.Compiler.Generation
{
    public class EntryPointAssemblyManager : IEntryPointManager
    {
        private readonly TypeManager _builder;
        private SystemTypeBindings _sb;
        private PTypeBuilder _ep;
        private PMethodBuilder _main;

        private const string _classNamespace = "";
        private const string _className = "EntryPoint";

        private const string _mainMethodName = "Main";

        public EntryPointAssemblyManager(TypeManager builder)
        {
            _builder = builder;

            _ep = builder.DefineStaticType(_classNamespace, _className);

            _main = _ep.DefineMethod(_mainMethodName, true, true, false);
            _main.SetName(_mainMethodName);

            _main.DefineParameter("args", _sb.Object.MakeArrayType(), false, false);
        }


        public PTypeBuilder EntryPoint => _ep;

        public PMethodBuilder Main => _main;

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
        private const string _startupServiceFieldName = "_ss";

        public static PType InvokeService(this TypeManager ts)
        {
            return ts.FindType<IInvokeService>();
        }

        public static PType LinkFactory(this TypeManager ts)
        {
            return ts.FindType<ILinkFactory>();
        }

        public static void InitService(this IEntryPointManager am)
        {
            var ep = am.EntryPoint;
            var sb = ep.TypeManager;

            var field = ep.DefineField(sb.InvokeService(), _invokeServiceFieldName, false, true);
            var lf = ep.DefineField(sb.LinkFactory(), _linkFactoryFieldName, false, true);
            var ss = ep.DefineField(sb.FindType<IStartupService>(), _startupServiceFieldName, false, true);

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
                .StSFld(lf)
                .LdArg_0()
                .LdLit(2)
                .LdElem()
                .Cast(sb.FindType<IStartupService>())
                .StSFld(ss);
        }

        public static PField GetISField(this IEntryPointManager am)
        {
            return am.EntryPoint.FindField(_invokeServiceFieldName) ??
                   throw new Exception(
                       $"Platform isn't initialized you must invoke {nameof(InitService)} method first");
        }

        public static PField GetLFField(this IEntryPointManager am)
        {
            return am.EntryPoint.FindField(_linkFactoryFieldName) ??
                   throw new Exception(
                       $"Platform isn't initialized you must invoke {nameof(InitService)} method first");
        }

        public static PField GetSSField(this IEntryPointManager am)
        {
            return am.EntryPoint.FindField(_startupServiceFieldName) ??
                   throw new Exception(
                       $"Platform isn't initialized you must invoke {nameof(InitService)} method first");
        }
    }
}