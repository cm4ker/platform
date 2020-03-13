using System;
using System.Linq;
using ZenPlatform.Compiler.Contracts;
using ZenPlatform.Configuration.Contracts.Data;
using ZenPlatform.Configuration.Contracts.TypeSystem;
using ZenPlatform.EntityComponent.Entity;
using ZenPlatform.Language.Ast;
using ZenPlatform.Language.Ast.Definitions;
using ZenPlatform.QueryBuilder;

namespace ZenPlatform.EntityComponent.Compilation
{
    public class LinkGenerationTask : ComponentAstTask, IEntityGenerationTask
    {
        public IPType LinkType { get; }

        public LinkGenerationTask(IPType linkType, CompilationMode compilationMode, IComponent component,
            string name, TypeBody tb) : base(compilationMode, component, false, name, tb)
        {
            LinkType = linkType;
        }

        public ITypeBuilder Stage0(IAssemblyBuilder asm)
        {
            return asm.DefineInstanceType(GetNamespace(), Name, asm.FindType("Entity.EntityLink"));
        }

        public void Stage1(ITypeBuilder builder, SqlDatabaseType dbType)
        {
            EmitStructure(builder, dbType);
        }

        public void Stage2(ITypeBuilder builder, SqlDatabaseType dbType)
        {
            EmitBody(builder, dbType);
        }

        private void EmitBody(ITypeBuilder builder, SqlDatabaseType dbType)
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

            var mrg = ts.FindType($"{@namespace}.{set.GetManagerType().Name}");
            var mrgGet = mrg.FindMethod("Get", sb.Guid);

            foreach (var prop in type.Properties)
            {
                SharedGenerators.EmitLinkProperty(builder, prop, sb, dtoType, dtoPrivate, ts, mrgGet, GetNamespace());
            }
        }

        public void EmitStructure(ITypeBuilder builder, SqlDatabaseType dbType)
        {
            var type = LinkType;
            var ts = builder.Assembly.TypeSystem;
            var sb = ts.GetSystemBindings();
            var dtoClassName = type.GetDtoType().Name;


            var @namespace = Component.GetCodeRule(CodeGenRuleType.NamespaceRule).GetExpression();

            var dtoType = ts.FindType($"{@namespace}.{dtoClassName}");

            var c = builder.DefineConstructor(false, dtoType);
            var g = c.Generator;

            var dtoPrivate = builder.DefineField(dtoType, "_dto", false, false);


            g.LdArg_0()
                .EmitCall(builder.BaseType.FindConstructor())
                .LdArg_0()
                .LdArg(1)
                .StFld(dtoPrivate)
                .Ret();

            foreach (var prop in type.Properties)
            {
                bool propertyGenerated = false;

                var propName = prop.Name;

                var propType = (prop.Types.Count() > 1)
                    ? sb.Object
                    : prop.Types.First().ConvertType(sb);

                IProperty baseProp = null;

                if (propName == "Id")
                {
                    baseProp = builder.BaseType.FindProperty("Id");
                }

                builder.DefineProperty(propType, propName, true, false, false, baseProp);
            }
        }
    }
}