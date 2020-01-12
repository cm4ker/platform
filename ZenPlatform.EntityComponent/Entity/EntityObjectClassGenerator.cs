using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using Mono.Cecil.Cil;
using MoreLinq.Extensions;
using ZenPlatform.Compiler;
using ZenPlatform.Compiler.Contracts;
using ZenPlatform.Compiler.Contracts.Symbols;
using ZenPlatform.Configuration.Contracts;
using ZenPlatform.Configuration.Contracts.Data;
using ZenPlatform.Configuration.Structure.Data.Types.Primitive;
using ZenPlatform.EntityComponent.Configuration;
using ZenPlatform.Language.Ast;
using ZenPlatform.Language.Ast.Definitions;
using ZenPlatform.Language.Ast.Definitions.Expressions;
using ZenPlatform.Language.Ast.Definitions.Statements;
using ZenPlatform.Language.Ast.Infrastructure;
using ZenPlatform.QueryBuilder;
using ZenPlatform.Shared.Tree;

namespace ZenPlatform.EntityComponent.Entity
{
    public class EntityObjectClassGenerator
    {
        private readonly IXCComponent _component;

        public EntityObjectClassGenerator(IXCComponent component)
        {
            _component = component;
        }

        public void GenerateAstTree(IXCObjectType type, Root root)
        {
            var className = type.Name;

            var @namespace = _component.GetCodeRule(CodeGenRuleType.NamespaceRule).GetExpression();

            var cls = new ComponentClass(CompilationMode.Server, _component, type, null, className,
                new TypeBody(new List<Member>()));
            cls.Bag = ObjectType.Object;

            cls.Namespace = @namespace;

            GenerateObjectClassUserModules(type, cls);

            var cu = new CompilationUnit(null, new List<NamespaceBase>(), new List<TypeEntity>() {cls});
            //end create dto class

            root.Add(cu);
        }

        public void EmitDetail(Node astTree, ITypeBuilder builder, SqlDatabaseType dbType, CompilationMode mode)
        {
            if (astTree is ComponentClass cc)
            {
                if (cc.Bag != null && ((ObjectType) cc.Bag) == ObjectType.Object)
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
            var set = cc.Type as XCSingleEntity ?? throw new Exception("This component can generate only SingleEntity");
            var ts = builder.Assembly.TypeSystem;
            var sb = ts.GetSystemBindings();
            var dtoClassName =
                $"{_component.GetCodeRuleExpression(CodeGenRuleType.DtoPreffixRule)}{type.Name}{_component.GetCodeRuleExpression(CodeGenRuleType.DtoPostfixRule)}";


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

            foreach (var prop in set.Properties)
            {
                bool propertyGenerated = false;

                var propName = prop.Name;

                var propType = (prop.Types.Count > 1)
                    ? sb.Object
                    : prop.Types[0].ConvertType(sb);


                var propBuilder = builder.DefineProperty(propType, propName, true, !prop.IsReadOnly, false);
                var getBuilder = propBuilder.getMethod.Generator;
                var setBuilder = propBuilder.setMethod?.Generator;


                // var valueParam = propBuilder.setMethod.Parameters[0];

                if (prop.Types.Count > 1)
                {
                    var typeField = prop.GetPropertySchemas(prop.Name)
                        .First(x => x.SchemaType == XCColumnSchemaType.Type);
                    var dtoTypeProp = dtoType.FindProperty(typeField.FullName);

                    foreach (var ctype in prop.Types)
                    {
                        XCColumnSchemaDefinition dtoPropSchema;

                        if (ctype is IXCLinkType)
                        {
                            dtoPropSchema = prop.GetPropertySchemas(prop.Name)
                                .First(x => x.SchemaType == XCColumnSchemaType.Ref);
                        }
                        else
                        {
                            dtoPropSchema = prop.GetPropertySchemas(prop.Name).First(x => x.PlatformType == ctype);
                        }

                        var dtoProp = dtoType.FindProperty(dtoPropSchema.FullName);

                        var compileType = ctype.ConvertType(sb);

                        var label = getBuilder.DefineLabel();

                        //GETTER
                        getBuilder
                            .LdArg_0()
                            .LdFld(dtoPrivate)
                            .EmitCall(dtoTypeProp.Getter)
                            .LdcI4((int) ctype.Id)
                            .BneUn(label)
                            .LdArg_0()
                            .LdFld(dtoPrivate)
                            .EmitCall(dtoProp.Getter);

                        if (ctype is IXCLinkType lt)
                        {
                            //Call Manager.Get(Id)
                            var mrg = ts.FindType($"{@namespace}.{lt.ParentType.Name}Manager");
                            var mrgGet = mrg.FindMethod("Get", sb.Guid);
                            getBuilder.EmitCall(mrgGet);
                        }
                        else if (compileType.IsValueType)
                            getBuilder.Box(compileType);

                        getBuilder
                            .Ret()
                            .MarkLabel(label);


                        if (!prop.IsReadOnly)
                        {
                            label = setBuilder.DefineLabel();
                            //SETTER
                            setBuilder
                                .LdArg(1)
                                .IsInst(compileType)
                                .BrFalse(label)
                                .LdArg_0()
                                .LdFld(dtoPrivate)
                                .LdArg(1)
                                .Unbox_Any(compileType)
                                .EmitCall(dtoProp.Setter)
                                .LdArg_0()
                                .LdFld(dtoPrivate)
                                .LdcI4((int) ctype.Id)
                                .EmitCall(dtoTypeProp.Setter)
                                .MarkLabel(label);
                        }
                    }

                    getBuilder.Throw(sb.Exception);
                    setBuilder.Throw(sb.Exception);

                    // getBuilder.Ret();
                    // setBuilder.Ret();
                }
                else
                {
                    if (!prop.IsSelfLink)
                    {
                        var schema = prop.GetPropertySchemas(prop.Name)
                            .First(x => x.SchemaType == XCColumnSchemaType.NoSpecial);

                        var dtofield = dtoType.FindProperty(schema.FullName);

                        getBuilder
                            .LdArg_0()
                            .LdFld(dtoPrivate)
                            .EmitCall(dtofield.Getter)
                            .Ret();
                    }
                    else
                    {
                        getBuilder
                            .LdNull()
                            .Ret();
                    }
                }
            }
        }

        private void GenerateObjectClassUserModules(IXCObjectType type, ComponentClass cls)
        {
            foreach (var module in type.GetProgramModules())
            {
                if (module.ModuleRelationType == XCProgramModuleRelationType.Object)
                {
                    var typeBody = ParserHelper.ParseTypeBody(module.ModuleText);


                    foreach (var func in typeBody.Functions)
                    {
                        func.SymbolScope = SymbolScopeBySecurity.User;
                        cls.AddFunction(func);
                    }
                }
            }
        }
    }
}