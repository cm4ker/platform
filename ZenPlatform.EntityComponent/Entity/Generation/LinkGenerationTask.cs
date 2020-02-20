using System;
using System.Linq;
using ZenPlatform.Compiler.Contracts;
using ZenPlatform.Configuration.Contracts;
using ZenPlatform.Configuration.Contracts.Data;
using ZenPlatform.Configuration.Contracts.TypeSystem;
using ZenPlatform.Language.Ast;
using ZenPlatform.Language.Ast.Definitions;
using ZenPlatform.QueryBuilder;

namespace ZenPlatform.EntityComponent.Entity.Generation
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

            foreach (var prop in type.Properties)
            {
                bool propertyGenerated = false;

                var propName = prop.Name;

                var propType = (prop.Types.Count() > 1)
                    ? sb.Object
                    : prop.Types.First().ConvertType(sb);

                var propBuilder = (IPropertyBuilder) builder.FindProperty(propName);
                var getBuilder = ((IMethodBuilder) propBuilder.Getter).Generator;

                // var valueParam = propBuilder.setMethod.Parameters[0];

                if (prop.Types.Count() > 1)
                {
                    var typeField = prop.GetObjSchema()
                        .First(x => x.SchemaType == ColumnSchemaType.Type);
                    var dtoTypeProp = dtoType.FindProperty(typeField.FullName);

                    foreach (var ctype in prop.Types)
                    {
                        ColumnSchemaDefinition dtoPropSchema;

                        if (ctype.IsLink)
                        {
                            dtoPropSchema = prop.GetObjSchema()
                                .First(x => x.SchemaType == ColumnSchemaType.Ref);
                        }
                        else
                        {
                            dtoPropSchema = prop.GetObjSchema().First(x => x.PlatformIpType == ctype);
                        }

                        var dtoProp = dtoType.FindProperty(dtoPropSchema.FullName);

                        var compileType = ctype.ConvertType(sb);

                        var label = getBuilder.DefineLabel();

                        //GETTER
                        getBuilder
                            .LdArg_0()
                            .LdFld(dtoPrivate)
                            .EmitCall(dtoTypeProp.Getter)
                            .LdcI4((int) ctype.GetSettings().SystemId)
                            .BneUn(label)
                            .LdArg_0()
                            .LdFld(dtoPrivate)
                            .EmitCall(dtoProp.Getter);

                        if (ctype.IsLink)
                        {
                            //Call Manager.Get(Id)
                            //Мы не можем ссылаться на методы, когда они ещё не готовы.
                            //нужно либо разбивать все на стадии, либо вводить понятие шаблона
                            var mrgRemote = ts.FindType($"{GetNamespace()}.{ctype.GetManagerType().Name}");
                            var mrgRemoteGet = mrgRemote.FindMethod("Get", sb.Guid);
                            getBuilder.EmitCall(mrgRemoteGet);
                        }
                        else if (compileType.IsValueType)
                            getBuilder.Box(compileType);

                        getBuilder
                            .Ret()
                            .MarkLabel(label);
                    }

                    getBuilder.Throw(sb.Exception);


                    // getBuilder.Ret();
                    // setBuilder.Ret();
                }
                else
                {
                    if (!prop.IsSelfLink)
                    {
                        var schema = prop.GetObjSchema()
                            .First(x => x.SchemaType == ColumnSchemaType.NoSpecial);

                        var dtofield = dtoType.FindProperty(schema.FullName);

                        getBuilder
                            .LdArg_0()
                            .LdFld(dtoPrivate)
                            .EmitCall(dtofield.Getter)
                            .Ret();
                    }
                    else
                    {
                        var mrg = ts.FindType($"{@namespace}.{mrgType.Name}");
                        var mrgGet = mrg.FindMethod("Get", sb.Guid);

                        getBuilder
                            .LdArg_0()
                            .EmitCall(builder.FindProperty("Id").Getter)
                            .EmitCall(mrgGet)
                            .Ret();
                    }
                }
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