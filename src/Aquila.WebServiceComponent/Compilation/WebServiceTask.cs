using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using Aquila.Compiler;
using Aquila.Compiler.Generation;
using Aquila.Compiler.Roslyn;
using Aquila.Compiler.Roslyn.RoslynBackend;
using Aquila.Component.Shared;
using Aquila.Configuration.Common.TypeSystem;
using Aquila.Core.Contracts;
using Aquila.Core.Contracts.Configuration;
using Aquila.Core.Contracts.Network;
using Aquila.Core.Contracts.TypeSystem;
using Aquila.Language.Ast;
using Aquila.Language.Ast.Misc;
using Aquila.Language.Ast.Symbols;
using Aquila.QueryBuilder;
using Aquila.ServerRuntime;
using Aquila.WebServiceComponent.Configuration;
using Microsoft.AspNetCore.Builder;
using AttributeSyntax = Aquila.Language.Ast.Definitions.AttributeSyntax;
using Name = Aquila.Language.Ast.Name;
using SingleTypeSyntax = Aquila.Language.Ast.SingleTypeSyntax;

namespace Aquila.WebServiceComponent.Compilation
{
    public class WebServiceTask : ComponentAstTask, IWsInternalGenTask
    {
        private readonly IPType _type;
        private RoslynTypeSystem _ts;
        private SystemTypeBindings _sb;

        public WebServiceTask(IPType type, CompilationMode compilationMode, IComponent component, string name)
            : base(compilationMode, component, false, name, TypeBody.Empty)
        {
            _type = type;

            GenerateObjectClassUserModules(_type);
        }


        public RoslynTypeBuilder Stage0(RoslynAssemblyBuilder asm)
        {
            _ts = asm.TypeSystem;
            _sb = _ts.GetSystemBindings();

            return asm.DefineInstanceType(GetNamespace(), Name);
        }

        public void Stage1(RoslynTypeBuilder builder, SqlDatabaseType dbType, IEntryPointManager sm)
        {
            var dataContractAttr = _ts.Factory.CreateAttribute(_ts, _ts.FindType<ServiceContractAttribute>());
            builder.SetCustomAttribute(dataContractAttr);

            var wsBuilder = sm.EntryPoint.DefineMethod($"{_type.Name}Builder", false, true, false);
            var iab = _ts.FindType<IApplicationBuilder>();
            wsBuilder.DefineParameter("appBuilder", iab, false, false);


            var method = _ts.FindType(typeof(WebService))
                .FindMethod(nameof(WebService.CreateWebService), iab, _sb.String)
                .MakeGenericMethod(new[] {builder});

            var registerMethod = _ts.FindType<IStartupService>()
                .FindMethod(x => x.Name == nameof(IStartupService.Register));

            var registerClassMethod = _ts.FindType<IStartupService>()
                .FindMethod(x => x.Name == nameof(IStartupService.RegisterWebServiceClass))
                .MakeGenericMethod(builder);

            wsBuilder.Body
                .LdArg_0()
                .LdLit(_type.Name)
                .Call(method);

            sm.Main.Body
                .LdSFld(sm.GetSSField())
                .LdFtn(wsBuilder)
                .Call(registerMethod)
                .LdSFld(sm.GetSSField())
                .Call(registerClassMethod);
        }


        private void GenerateObjectClassUserModules(IPType type)
        {
            var md = type.GetMD<MDWebService>();

            foreach (var module in md.Modules)
            {
                var typeBody = ParserHelper.ParseTypeBody(module.ModuleText);

                foreach (var func in typeBody.Functions)
                {
                    func.SymbolScope = SymbolScopeBySecurity.User;
                    func.Attributes.Add(new AttributeSyntax(null, ArgumentList.Empty,
                        new SingleTypeSyntax(null, "Server", TypeNodeKind.Type)));
                    this.AddFunction(func);
                }
            }
        }

        public void Stage2(RoslynTypeBuilder builder, SqlDatabaseType dbType)
        {
        }
    }
}