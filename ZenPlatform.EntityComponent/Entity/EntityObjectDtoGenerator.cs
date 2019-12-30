using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Diagnostics;
using System.Linq;
using Newtonsoft.Json.Bson;
using ZenPlatform.Compiler;
using ZenPlatform.Compiler.Contracts;
using ZenPlatform.Compiler.Contracts.Symbols;
using ZenPlatform.Configuration.Contracts;
using ZenPlatform.Configuration.Contracts.Data;
using ZenPlatform.Configuration.Structure.Data.Types.Complex;
using ZenPlatform.EntityComponent.Configuration;
using ZenPlatform.Language.Ast;
using ZenPlatform.Language.Ast.Definitions;
using ZenPlatform.QueryBuilder;
using ZenPlatform.QueryBuilder.Model;
using ZenPlatform.Shared.Tree;

namespace ZenPlatform.EntityComponent.Entity
{
    public class EntityObjectDtoGenerator
    {
        private readonly IXCComponent _component;

        public EntityObjectDtoGenerator(IXCComponent component)
        {
            _component = component;
        }

        public void GenerateAstTree(IXCObjectType type, Root root)
        {
            var dtoClassName =
                $"{_component.GetCodeRuleExpression(CodeGenRuleType.DtoPreffixRule)}{type.Name}{_component.GetCodeRuleExpression(CodeGenRuleType.DtoPostfixRule)}";

            var cls = new ComponentClass(CompilationMode.Shared, _component, type, null, dtoClassName,
                new TypeBody(new List<Member>())) {Namespace = "Documents"};

            cls.Bag = ObjectType.Dto;

            var cu = new CompilationUnit(null, new List<NamespaceBase>(), new List<TypeEntity>() {cls});
            root.Add(cu);
        }

        public void EmitDetail(Node astTree, ITypeBuilder builder, SqlDatabaseType dbType, CompilationMode mode)
        {
            if (astTree is ComponentClass cc)
            {
                if (cc.Bag != null && ((ObjectType) cc.Bag) == ObjectType.Dto)
                {
                    if (cc.CompilationMode.HasFlag(CompilationMode.Server) && mode.HasFlag(CompilationMode.Server))
                    {
                        EmitBody(cc, builder, dbType);
                        EmitVersionField(builder);
                        EmitMappingSupport(cc, builder);
                        EmitSavingSupport(cc, builder, dbType);
                    }
                    else if (cc.CompilationMode.HasFlag(CompilationMode.Client) && mode.HasFlag(CompilationMode.Client))
                    {
                        EmitBody(cc, builder, dbType);
                    }
                }
            }
        }

        private void EmitBody(ComponentClass cc, ITypeBuilder builder, SqlDatabaseType dbType)
        {
            var set = cc.Type as XCSingleEntity ?? throw new InvalidOperationException(
                          $"This component only can serve {nameof(XCSingleEntity)} objects");

            var ts = builder.Assembly.TypeSystem;
            var sb = ts.GetSystemBindings();

            //Create dto class
            foreach (var prop in set.Properties)
            {
                bool propertyGenerated = false;
                if (prop.IsSelfLink) continue;


                if (string.IsNullOrEmpty(prop.DatabaseColumnName))
                {
                    throw new Exception(
                        $"Prop: {prop.Name} ObjectType: {typeof(XCSingleEntity)} Name: {set.Name}. Database column is empty!");
                }

                if (prop.Types.Count > 1)
                {
                    {
                        builder.DefinePropertyWithBackingField(sb.Int, prop.Name + "_Type", false);
                    }
                }


                foreach (var ctype in prop.Types)
                {
                    if (ctype is IXCPrimitiveType pt)
                    {
                        var dbColName = prop
                            .GetPropertySchemas(prop.DatabaseColumnName)
                            .First(x => x.SchemaType == ((prop.Types.Count > 1)
                                            ? XCColumnSchemaType.Value
                                            : XCColumnSchemaType.NoSpecial) && x.PlatformType == pt).FullName;

                        var propName = prop
                            .GetPropertySchemas(prop.Name)
                            .First(x => x.SchemaType == ((prop.Types.Count > 1)
                                            ? XCColumnSchemaType.Value
                                            : XCColumnSchemaType.NoSpecial) && x.PlatformType == pt).FullName;

                        IType propType = pt.ConvertType(sb);

                        var propBuilder = builder.DefinePropertyWithBackingField(propType, propName, false);

                        var attr = builder.CreateAttribute<MapToAttribute>(sb.String);
                        propBuilder.SetAttribute(attr);
                        attr.SetParameters(dbColName);
                    }
                    else if (ctype is IXCLinkType ot)
                    {
                        if (!propertyGenerated)
                        {
                            propertyGenerated = true;

                            var dbColName = prop
                                .GetPropertySchemas(prop.DatabaseColumnName)
                                .First(x => x.SchemaType == ((prop.Types.Count > 1)
                                                ? XCColumnSchemaType.Ref
                                                : XCColumnSchemaType.NoSpecial)).FullName;

                            var propName = prop
                                .GetPropertySchemas(prop.Name)
                                .First(x => x.SchemaType == ((prop.Types.Count > 1)
                                                ? XCColumnSchemaType.Ref
                                                : XCColumnSchemaType.NoSpecial)).FullName;

                            var propBuilder = builder.DefinePropertyWithBackingField(sb.Guid, propName, false);

                            var attr = builder.CreateAttribute<MapToAttribute>(sb.String);
                            propBuilder.SetAttribute(attr);
                            attr.SetParameters(dbColName);
                        }
                    }
                }
            }
        }

        private void EmitMappingSupport(ComponentClass cls, ITypeBuilder tb)
        {
            var _ts = tb.Assembly.TypeSystem;
            var _bindings = _ts.GetSystemBindings();

            tb.AddInterfaceImplementation(_ts.FindType<ICanMap>());

            var readerMethod = tb.DefineMethod(nameof(ICanMap.Map), true, false, true);
            var rg = readerMethod.Generator;

            var readerType = _ts.FindType<DbDataReader>();

            var readerParam =
                readerMethod.DefineParameter("reader", readerType, false, false);

            foreach (var property in tb.Properties)
            {
                var mt = property.FindCustomAttribute<MapToAttribute>();
                if (mt is null) continue;

                rg
                    .LdArg_0()
                    .LdArg(readerParam.ArgIndex)
                    .LdStr(mt.Parameters[0].ToString())
                    .EmitCall(readerType.FindMethod("get_Item", _bindings.String))
                    .Unbox_Any(property.PropertyType)
                    .EmitCall(property.Setter);
            }

            rg.Ret();
        }

        private SSyntaxNode GetInsertQuery(XCSingleEntity se)
        {
            QueryMachine qm = new QueryMachine();
            qm.bg_query()
                .m_values();

            var pIndex = 0;

            var columns = se.Properties.Where(x => !x.IsSelfLink)
                .SelectMany(x => x.GetPropertySchemas());

            foreach (var column in columns)
            {
                qm.ld_param($"P{pIndex}");
                pIndex++;
            }

            qm.m_insert()
                .ld_table(se.RelTableName);

            foreach (var col in columns)
            {
                qm.ld_column(col.FullName);
            }

            qm.st_query();

            return (SSyntaxNode) qm.pop();
        }

        private SSyntaxNode GetUpdateQuery(XCSingleEntity se)
        {
            QueryMachine qm = new QueryMachine();

            var pIndex = 0;

            var columns = se.Properties.Where(x => !x.Unique).SelectMany(x => x.GetPropertySchemas());

            qm.bg_query()
                .m_where()
                .ld_column(se.GetPropertyByName("Id").DatabaseColumnName, "T0")
                .ld_param($"P_{pIndex++}")
                .eq();

            qm.m_set();
            Debug.Assert(columns.Any());

            foreach (var column in columns)
            {
                qm.ld_column(column.FullName, "T0")
                    .ld_param($"P_{pIndex++}")
                    .assign();
            }

            qm.m_update()
                .ld_table(se.RelTableName)
                .@as("T0")
                .st_query();

            return (SSyntaxNode) qm.pop();
        }

        private void EmitSavingSupport(ComponentClass cls, ITypeBuilder tb, SqlDatabaseType dbType)
        {
            var set = cls.Type as XCSingleEntity ??
                      throw new Exception($"This component can't serve this type {cls.Type}");

            var _ts = tb.Assembly.TypeSystem;
            var _bindings = _ts.GetSystemBindings();

            var compiler = SqlCompillerBase.FormEnum(dbType);

            tb.AddInterfaceImplementation(_ts.FindType<ICanSave>());

            var readerMethod = tb.DefineMethod(nameof(ICanSave.Save), true, false, true);

            var rg = readerMethod.Generator;

            var cmdType = _ts.FindType<DbCommand>();
            var parameterType = _ts.FindType<DbParameter>();

            var cmdParam =
                readerMethod.DefineParameter("cmd", cmdType, false, false);

            var indexp = 0;


            var p_loc = rg.DefineLocal(_ts.FindType<DbParameter>());

            var versionF = tb.Properties.First(x => x.Name == "Version");

            if (versionF != null)
            {
                var narg = rg.DefineLabel();
                var end = rg.DefineLabel();
                rg.LdArg_0()
                    .EmitCall(versionF.Getter)
                    .LdNull()
                    .Ceq()
                    .BrTrue(narg);

                rg
                    .LdArg(cmdParam.ArgIndex)
                    .LdStr(compiler.Compile(GetUpdateQuery(set)))
                    .EmitCall(cmdType.FindProperty(nameof(DbCommand.CommandText)).Setter)
                    .Br(end);

                rg
                    .MarkLabel(narg)
                    .LdArg(cmdParam.ArgIndex)
                    .LdStr(compiler.Compile(GetInsertQuery(set)))
                    .EmitCall(cmdType.FindProperty(nameof(DbCommand.CommandText)).Setter)
                    .MarkLabel(end);
            }

            foreach (var property in tb.Properties)
            {
                var m = property.FindCustomAttribute<MapToAttribute>();
                if (m is null) continue;

                rg.LdArg(cmdParam.ArgIndex)
                    .EmitCall(cmdType.FindMethod(nameof(DbCommand.CreateParameter)))
                    .StLoc(p_loc)
                    .LdLoc(p_loc)
                    .LdStr($"P_{indexp}")
                    .EmitCall(parameterType.FindProperty(nameof(DbParameter.ParameterName)).Setter)
                    .LdLoc(p_loc)
                    .LdArg_0()
                    .EmitCall(property.Getter)
                    .Box(property.PropertyType)
                    .EmitCall(parameterType.FindProperty(nameof(DbParameter.Value)).Setter);

                indexp++;
            }

            rg.Ret();
        }

        private void EmitVersionField(ITypeBuilder tb)
        {
            var _ts = tb.Assembly.TypeSystem;
            var _b = _ts.GetSystemBindings();
            var prop = tb.DefinePropertyWithBackingField(_b.Byte.MakeArrayType(), "Version", false);
        }
    }

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
            cls.Namespace = @namespace;

            GenerateObjectClassUserModules(type, cls);

            var cu = new CompilationUnit(null, new List<NamespaceBase>(), new List<TypeEntity>() {cls});
            //end create dto class
            root.Add(cu);
        }

        public void EmitDetail(Node astTree, ITypeBuilder builder, SqlDatabaseType dbType)
        {
            if (astTree is ComponentClass cc)
            {
                if (cc.Bag != null && ((ObjectType) cc.Bag) == ObjectType.Dto)
                {
                    if (cc.CompilationMode.HasFlag(CompilationMode.Server))
                    {
                        EmitBody(cc, builder, dbType);
                    }
                    else if (cc.CompilationMode.HasFlag(CompilationMode.Client))
                    {
                        EmitBody(cc, builder, dbType);
                    }
                }
            }
        }

        private void EmitBody(ComponentClass cc, ITypeBuilder builder, SqlDatabaseType dbType)
        {
            var type = cc.Type;
            var ts = builder.Assembly.TypeSystem;
            var dtoClassName =
                $"{_component.GetCodeRuleExpression(CodeGenRuleType.DtoPreffixRule)}{type.Name}{_component.GetCodeRuleExpression(CodeGenRuleType.DtoPostfixRule)}";


            var @namespace = _component.GetCodeRule(CodeGenRuleType.NamespaceRule).GetExpression();

            var dtoType = ts.FindType($"{@namespace}.{dtoClassName}");
            var session = ts.GetSystemBindings().Session;

            var c = builder.DefineConstructor(false, dtoType, session);
            var g = c.Generator;

            var dtoPrivate = builder.DefineField(dtoType, "_dto", false, false);
            var sessionPrivate = builder.DefineField(session, "_session", false, false);

            g.LdArg_0()
                .EmitCall(builder.BaseType.FindConstructor())
                .LdArg(1)
                .StFld(dtoPrivate)
                .LdArg(2)
                .StFld(sessionPrivate);
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