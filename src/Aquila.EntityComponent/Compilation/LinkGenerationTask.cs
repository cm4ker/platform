using System;
using System.Linq;
using Aquila.Compiler.Contracts;
using Aquila.Compiler.Generation;
using Aquila.Compiler.Roslyn;
using Aquila.Compiler.Roslyn.RoslynBackend;
using Aquila.Component.Shared;
using Aquila.Core.Contracts;
using Aquila.Core.Contracts.Configuration;
using Aquila.Core.Contracts.Data;
using Aquila.Core.Contracts.TypeSystem;
using Aquila.EntityComponent.Entity;
using Aquila.Language.Ast;
using Aquila.Language.Ast.Definitions;
using Aquila.QueryBuilder;
using Name = Aquila.Language.Ast.Name;

namespace Aquila.EntityComponent.Compilation
{
    public class LinkGenerationTask : ComponentAstTask, IEntityGenerationTask
    {
        private RoslynTypeBuilder _linkType;
        private RoslynConstructorBuilder _linkConstructor;
        private RoslynConstructorBuilder _linkConst2;
        private RoslynMethodBuilder _reload;
        private RoslynField _dtoField;
        private RoslynType _mrg;
        private RoslynMethod _mrgGet;
        private RoslynMethod _mrgGetDto;

        public IPType LinkType { get; }

        public LinkGenerationTask(IPType linkType, CompilationMode compilationMode, IComponent component,
            string name, TypeBody tb) : base(compilationMode, component, false, name, tb)
        {
            LinkType = linkType;
        }

        public RoslynTypeBuilder Stage0(RoslynAssemblyBuilder asm)
        {
            _linkType = asm.DefineInstanceType(GetNamespace(), Name, asm.FindType("Entity.EntityLink"));

            return _linkType;
        }

        public void Stage1(RoslynTypeBuilder builder, SqlDatabaseType dbType, IEntryPointManager sm)
        {
            EmitStructure(builder, dbType);
            RegisterFactory(sm);
        }

        public void RegisterFactory(IEntryPointManager sm)
        {
            var ts = _linkType.TypeSystem;

            var methodBuilder = sm.EntryPoint.DefineMethod($"fac_{LinkType.Name}", true, true, false, null, false);
            var p1 = methodBuilder.DefineParameter("p1", ts.GetSystemBindings().Guid, false, false);
            var p2 = methodBuilder.DefineParameter("p2", ts.GetSystemBindings().String, false, false);
            methodBuilder.WithReturnType(ts.Resolve<ILink>());

            methodBuilder
                .Body
                .LdArg(p1)
                .LdArg(p2)
                .NewObj(_linkConstructor)
                .Ret();

            sm.Main.Body
                .LdSFld(sm.GetLFField())
                .LdLit(LinkType.GetSettings().SystemId)
                .LdFtn(methodBuilder)
                .Call(_linkType.TypeSystem.LinkFactory().FindMethod(x => x.Name == nameof(ILinkFactory.Register)))
                ;
        }

        public void Stage2(RoslynTypeBuilder builder, SqlDatabaseType dbType)
        {
            EmitBody(builder, dbType);
        }

        private void EmitBody(RoslynTypeBuilder builder, SqlDatabaseType dbType)
        {
            var type = LinkType;
            var set = LinkType ?? throw new Exception("This component can generate only SingleEntity");
            var mrgType = type.GetManagerType();
            var ts = builder.Assembly.TypeSystem;
            var sb = ts.GetSystemBindings();
            var dtoClassName = type.GetDtoType().Name;


            var @namespace = Component.GetCodeRule(CodeGenRuleType.NamespaceRule).GetExpression();

            var dtoType = ts.FindType($"{@namespace}.{dtoClassName}");

            var dtoPrivate = builder.FindField("_dto") ?? throw new Exception("You must declare private field _dto");

            _mrg = ts.FindType($"{@namespace}.{set.GetManagerType().Name}");
            _mrgGet = _mrg.FindMethod("Get", sb.Guid);
            _mrgGetDto = _mrg.FindMethod("GetDto", sb.Guid);

            foreach (var prop in type.Properties)
            {
                if (prop.Name == "Id") continue;
                SharedGenerators.EmitLinkProperty(builder, prop, sb, dtoType, dtoPrivate, ts, _mrgGet, GetNamespace(), _reload);
            }

            EmitReload(builder);
        }

        public void EmitStructure(RoslynTypeBuilder builder, SqlDatabaseType dbType)
        {
            var type = LinkType;
            var ts = builder.Assembly.TypeSystem;
            var sb = ts.GetSystemBindings();
            var dtoClassName = type.GetDtoType().Name;


            var @namespace = Component.GetCodeRule(CodeGenRuleType.NamespaceRule).GetExpression();

            var dtoType = ts.FindType($"{@namespace}.{dtoClassName}");

            _linkConstructor = builder.DefineConstructor(false, sb.Guid, sb.String);
            _linkConst2 = builder.DefineConstructor(false, sb.Guid);

            var g = _linkConstructor.Body;
            var g2 = _linkConst2.Body;

            _dtoField = builder.DefineField(dtoType, "_dto", false, false);

            g.LdArg_0()
                .LdArg(1)
                .LdLit(type.GetSettings().SystemId)
                .LdArg(2)
                .Call(builder.BaseType.Constructors.First())
                ;

            g2.LdArg_0()
                .LdArg(1)
                .LdLit(type.GetSettings().SystemId)
                .LdLit("Link: [{0}]")
                .LdArg(1)
                .Call(sb.Methods.Format)
                .Call(builder.BaseType.Constructors.First())
                ;

            foreach (var prop in type.Properties)
            {
                bool propertyGenerated = false;

                var propName = prop.Name;

                var propType = (prop.Type.IsTypeSet)
                    ? sb.Object
                    : prop.Type.ConvertType(sb);

                RoslynProperty baseProp = null;

                if (propName == "Id")
                {
                    continue;
                    baseProp = builder.BaseType.FindProperty("Id");
                }

                builder.DefineProperty(propType, propName, true, false, false, baseProp);
            }

            _reload = builder.DefineMethod("Reload", true, false, false, null, false);
        }

        public void EmitReload(RoslynTypeBuilder builder)
        {
            _reload.Body
                .LdArg_0()
                .Dup()
                .LdProp(builder.FindProperty("Id"))
                .Call(_mrgGetDto)
                .StFld(_dtoField);
        }
    }
}