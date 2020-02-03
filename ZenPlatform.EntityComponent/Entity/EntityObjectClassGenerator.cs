using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using Mono.Cecil.Cil;
using MoreLinq.Extensions;
using Npgsql.NameTranslation;
using ZenPlatform.Compiler;
using ZenPlatform.Compiler.Contracts;
using ZenPlatform.Compiler.Contracts.Symbols;
using ZenPlatform.Compiler.Helpers;
using ZenPlatform.Configuration.Contracts;
using ZenPlatform.Configuration.Contracts.Data;
using ZenPlatform.Configuration.Contracts.TypeSystem;
using ZenPlatform.Configuration.Structure.Data.Types.Primitive;
using ZenPlatform.Core.Authentication;
using ZenPlatform.EntityComponent.Configuration;
using ZenPlatform.Language.Ast;
using ZenPlatform.Language.Ast.Definitions;
using ZenPlatform.Language.Ast.Definitions.Expressions;
using ZenPlatform.Language.Ast.Definitions.Functions;
using ZenPlatform.Language.Ast.Definitions.Statements;
using ZenPlatform.Language.Ast.Infrastructure;
using ZenPlatform.QueryBuilder;
using ZenPlatform.Shared.Tree;
using IType = ZenPlatform.Compiler.Contracts.IType;
using Root = ZenPlatform.Language.Ast.Definitions.Root;

namespace ZenPlatform.EntityComponent.Entity
{
    public class EntityObjectClassGenerator
    {
        private readonly IComponent _component;

        public EntityObjectClassGenerator(IComponent component)
        {
            _component = component;
        }

        public void GenerateAstTree(ZenPlatform.Configuration.Contracts.TypeSystem.IType type, Root root)
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

        public void Stage1(Node astTree, ITypeBuilder builder, SqlDatabaseType dbType, CompilationMode mode)
        {
            if (astTree is ComponentClass cc)
            {
                if (cc.Bag != null && ((ObjectType) cc.Bag) == ObjectType.Object)
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

        public void EmitStructure(ComponentClass cc, ITypeBuilder builder, SqlDatabaseType dbType)
        {
            var type = cc.Type;
            var set = cc.Type;
            var ts = builder.Assembly.TypeSystem;
            var sb = ts.GetSystemBindings();
            var dtoClassName = set.GetDtoType().Name;
            var @namespace = set.GetNamespace();

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

                var propType = (prop.Types.Count() > 1)
                    ? sb.Object
                    : prop.Types.First().ConvertType(sb);

                var hasSet = !prop.IsReadOnly;


                var codeObj = builder.DefineProperty(propType, propName, true, hasSet, false);
                cc.TypeBody.SymbolTable.Add(new Property(null, propName, propType.ToAstType()), codeObj.prop);
            }

            var saveBuilder = builder.DefineMethod("Save", true, false, false);

            cc.TypeBody.SymbolTable.Add(
                new Function(null, null, null, null, saveBuilder.Name, saveBuilder.ReturnType.ToAstType()),
                saveBuilder);
        }

        private void EmitBody(ComponentClass cc, ITypeBuilder builder, SqlDatabaseType dbType)
        {
            var type = cc.Type;
            var set = cc.Type;
            var ts = builder.Assembly.TypeSystem;
            var sb = ts.GetSystemBindings();
            var dtoClassName =
                $"{_component.GetCodeRuleExpression(CodeGenRuleType.DtoPreffixRule)}{type.Name}{_component.GetCodeRuleExpression(CodeGenRuleType.DtoPostfixRule)}";

            var @namespace = _component.GetCodeRule(CodeGenRuleType.NamespaceRule).GetExpression();

            var dtoType = ts.FindType($"{@namespace}.{dtoClassName}");


            var dtoPrivate = builder.FindField("_dto");

            var mrg = ts.FindType($"{@namespace}.{set.Name}Manager");
            var mrgGet = mrg.FindMethod("Get", sb.Guid);

            foreach (var prop in set.Properties)
            {
                bool propertyGenerated = false;

                var propName = prop.Name;

                var propType = (prop.Types.Count() > 1)
                    ? sb.Object
                    : prop.Types.First().ConvertType(sb);

                var propBuilder = (IPropertyBuilder) builder.FindProperty(propName);
                var getBuilder = ((IMethodBuilder) propBuilder.Getter).Generator;
                var setBuilder = ((IMethodBuilder) propBuilder.Setter)?.Generator;

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
                            dtoPropSchema = prop.GetObjSchema().First(x => x.PlatformType == ctype);
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

                        if (ctype.IsLink)
                        {
                            //Call Manager.Get(Id)
                            //Мы не можем ссылаться на методы, когда они ещё не готовы.
                            //нужно либо разбивать все на стадии, либо вводить понятие шаблона
                            var mrgRemote = ts.FindType($"{@namespace}.{ctype.GetManagerType().Name}");
                            var mrgRemoteGet = mrgRemote.FindMethod("Get", sb.Guid);
                            getBuilder.EmitCall(mrgRemoteGet);
                        }
                        else if (compileType.IsValueType)
                            getBuilder.Box(compileType);

                        getBuilder
                            .Ret()
                            .MarkLabel(label);


                        if (setBuilder != null)
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
                                .Unbox_Any(compileType);

                            if (ctype.IsLink)
                                setBuilder.EmitCall(compileType.FindProperty("Id").Getter);

                            setBuilder.EmitCall(dtoProp.Setter)
                                .LdArg_0()
                                .LdFld(dtoPrivate)
                                .LdcI4((int) ctype.SystemId)
                                .EmitCall(dtoTypeProp.Setter)
                                .Ret()
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
                        var dtoPropSchema = prop.GetObjSchema()
                            .First(x => x.SchemaType == ColumnSchemaType.NoSpecial);

                        var dtoProp = dtoType.FindProperty(dtoPropSchema.FullName);

                        var ctype = prop.Types.First();

                        var compileType = ctype.ConvertType(sb);

                        //GETTER
                        getBuilder
                            .LdArg_0()
                            .LdFld(dtoPrivate)
                            .EmitCall(dtoProp.Getter);

                        if (ctype.IsLink)
                        {
                            //Call Manager.Get(Id)
                            //Мы не можем ссылаться на методы, когда они ещё не готовы.
                            //нужно либо разбивать все на стадии, либо вводить понятие шаблона
                            var mrgRemote = ts.FindType($"{@namespace}.{ctype.GetManagerType().Name}");
                            var mrgRemoteGet = mrgRemote.FindMethod("Get", sb.Guid);
                            getBuilder.EmitCall(mrgRemoteGet);
                        }

                        getBuilder
                            .Ret();

                        if (setBuilder != null)
                        {
                            setBuilder
                                .LdArg_0()
                                .LdFld(dtoPrivate)
                                .LdArg(1);

                            if (ctype.IsLink)
                                setBuilder.EmitCall(compileType.FindProperty("Id").Getter);

                            setBuilder.EmitCall(dtoProp.Setter)
                                .Ret();
                        }
                    }
                    else
                    {
                        getBuilder
                            .LdArg_0()
                            .EmitCall(builder.FindProperty("Id").Getter)
                            .EmitCall(mrgGet)
                            .Ret();
                    }
                }
            }

            var saveBuilder = (IMethodBuilder) builder.FindMethod("Save");

            var sg = saveBuilder.Generator;

            sg
                .LdArg_0()
                .LdFld(dtoPrivate)
                .EmitCall(mrg.FindMethod("Save", dtoType))
                .Ret();
        }

        private void GenerateObjectClassUserModules(ZenPlatform.Configuration.Contracts.TypeSystem.IType type,
            ComponentClass cls)
        {
            //TODO: Нужно добавить поддержку программных модулей в метаданные
            throw new NotImplementedException();

            // foreach (var module in type.GetProgramModules())
            // {
            //     if (module.ModuleRelationType == XCProgramModuleRelationType.Object)
            //     {
            //         var typeBody = ParserHelper.ParseTypeBody(module.ModuleText);
            //
            //
            //         foreach (var func in typeBody.Functions)
            //         {
            //             func.SymbolScope = SymbolScopeBySecurity.User;
            //             cls.AddFunction(func);
            //         }
            //     }
            // }
        }
    }
}