using System;
using System.Collections.Generic;
using System.Linq;
using Mono.Cecil;
using MoreLinq.Extensions;
using ZenPlatform.Compiler;
using ZenPlatform.Compiler.Contracts;
using ZenPlatform.Compiler.Contracts.Symbols;
using ZenPlatform.Configuration.Contracts;
using ZenPlatform.Configuration.Contracts.Data;
using ZenPlatform.Configuration.Contracts.TypeSystem;
using ZenPlatform.EntityComponent.Configuration;
using ZenPlatform.Language.Ast;
using ZenPlatform.Language.Ast.Definitions;
using ZenPlatform.Language.Ast.Definitions.Statements;
using ZenPlatform.QueryBuilder;
using ZenPlatform.Shared.Tree;
using IProperty = ZenPlatform.Compiler.Contracts.IProperty;
using IType = ZenPlatform.Compiler.Contracts.IType;
using Root = ZenPlatform.Language.Ast.Definitions.Root;

namespace ZenPlatform.EntityComponent.Entity
{
    public static class EntityComponentHelper
    {
        public static IPType GetDtoType(this IPType ipType)
        {
            return ipType.TypeManager.Types.FirstOrDefault(x => x.IsDto && x.GroupId == ipType.GroupId);
        }

        public static IPType GetManagerType(this IPType ipType)
        {
            return ipType.TypeManager.Types.FirstOrDefault(x => x.IsManager && x.GroupId == ipType.GroupId);
        }

        public static IPType GetLinkType(this IPType ipType)
        {
            return ipType.TypeManager.Types.FirstOrDefault(x => x.IsLink && x.GroupId == ipType.GroupId);
        }

        public static IPType GetObjectType(this IPType ipType)
        {
            return ipType.TypeManager.Types.FirstOrDefault(x => x.IsObject && x.GroupId == ipType.GroupId);
        }

        public static T GetMD<T>(this IPType type)
        {
            return (T) type.TypeManager.Metadatas.FirstOrDefault(x => x.Id == type.Id)?.Metadata;
        }
    }

    public class EntityLinkClassGenerator
    {
        private readonly IComponent _component;

        public EntityLinkClassGenerator(IComponent component)
        {
            _component = component;
        }

        public void GenerateAstTree(ZenPlatform.Configuration.Contracts.TypeSystem.IPType ipType, Root root)
        {
            var className = ipType.Name;

            var @namespace = _component.GetCodeRule(CodeGenRuleType.NamespaceRule).GetExpression();

            var cls = new ComponentClass(CompilationMode.Shared, _component, ipType, null, className,
                    new TypeBody(new List<Member>()))
                {Base = "Entity.EntityLink", Namespace = ipType.GetNamespace()};

            cls.Namespace = @namespace;
            cls.Bag = ObjectType.Link;


            var cu = new CompilationUnit(null, new List<NamespaceBase>(), new List<TypeEntity>() {cls});
            root.Add(cu);
        }

        public void Stage1(Node astTree, ITypeBuilder builder, SqlDatabaseType dbType, CompilationMode mode)
        {
            if (astTree is ComponentClass cc)
            {
                if (cc.Bag != null && ((ObjectType) cc.Bag) == ObjectType.Link)
                {
                    if (cc.CompilationMode.HasFlag(CompilationMode.Server) && mode.HasFlag(CompilationMode.Server))
                    {
                        EmitStructure(cc, builder, dbType);
                    }
                    else if (cc.CompilationMode.HasFlag(CompilationMode.Client))
                    {
                    }
                }
            }
        }

        public void Stage2(Node astTree, ITypeBuilder builder, SqlDatabaseType dbType, CompilationMode mode)
        {
            if (astTree is ComponentClass cc)
            {
                if (cc.Bag != null && ((ObjectType) cc.Bag) == ObjectType.Link)
                {
                    if (cc.CompilationMode.HasFlag(CompilationMode.Server) && mode.HasFlag(CompilationMode.Server))
                    {
                        EmitBody(cc, builder, dbType);
                    }
                    else if (cc.CompilationMode.HasFlag(CompilationMode.Client))
                    {
                    }
                }
            }
        }

        private void EmitBody(ComponentClass cc, ITypeBuilder builder, SqlDatabaseType dbType)
        {
            var type = cc.Type;
            var set = cc.Type ?? throw new Exception("This component can generate only SingleEntity");
            var mrgType = type.GetManagerType();
            var ts = builder.Assembly.TypeSystem;
            var sb = ts.GetSystemBindings();
            var dtoClassName = type.GetDtoType().Name;


            var @namespace = _component.GetCodeRule(CodeGenRuleType.NamespaceRule).GetExpression();

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
                            .LdcI4((int) ctype.SystemId)
                            .BneUn(label)
                            .LdArg_0()
                            .LdFld(dtoPrivate)
                            .EmitCall(dtoProp.Getter);

                        if (compileType.IsValueType)
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

        public void EmitStructure(ComponentClass cc, ITypeBuilder builder, SqlDatabaseType dbType)
        {
            var type = cc.Type;
            var ts = builder.Assembly.TypeSystem;
            var sb = ts.GetSystemBindings();
            var dtoClassName = type.GetDtoType().Name;


            var @namespace = _component.GetCodeRule(CodeGenRuleType.NamespaceRule).GetExpression();

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