using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Diagnostics;
using System.Linq;
using Newtonsoft.Json.Bson;
using ZenPlatform.Compiler.Contracts;
using ZenPlatform.Configuration.Contracts;
using ZenPlatform.Configuration.Contracts.Data;
using ZenPlatform.Configuration.Structure.Data.Types.Complex;
using ZenPlatform.Configuration.Structure.Data.Types.Primitive;
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

        public void GenerateDraft(IXCObjectType type, Root root)
        {
            var cls = new ComponentClass(CompilationMode.Shared, _component, type, null, type.Name,
                new TypeBody(new List<Member>())) {Base = "Documents.EntityLink", Namespace = "Documents"};

            cls.Bag = ObjectType.Dto;

            var cu = new CompilationUnit(null, new List<NamespaceBase>(), new List<TypeEntity>() {cls});
            root.Add(cu);
        }

        public void GenerateDetail(Node astTree, ITypeBuilder builder, SqlDatabaseType dbType)
        {
            if (astTree is ComponentClass cc)
            {
                if (cc.Bag != null && ((ObjectType) cc.Bag) == ObjectType.Dto)
                {
                    if (cc.CompilationMode == CompilationMode.Server)
                    {
                        EmitMappingSupport(cc, builder);
                        EmitSavingSupport(cc, builder, dbType);
                    }
                }
            }
        }

        private IType GetTypeFromPlatform(IXCType pt, SystemTypeBindings sb)
        {
            return pt switch
            {
                XCBinary b => sb.Byte.MakeArrayType(),
                XCInt b => sb.Int,
                XCString b => sb.String,
                XCNumeric b => sb.Double,
                XCBoolean b => sb.Boolean,
                XCDateTime b => sb.DateTime,
                XCGuid b => sb.Guid,

                IXCLinkType b => sb.TypeSystem.FindType(
                    b.Parent.GetCodeRuleExpression(CodeGenRuleType.NamespaceRule) + "." + b.Name),
                IXCObjectType b => sb.TypeSystem.FindType(
                    b.Parent.GetCodeRuleExpression(CodeGenRuleType.NamespaceRule) + "." + b.Name),
            };
        }

        private void EmitGeneral(ComponentClass cc, ITypeBuilder builder, SqlDatabaseType dbType)
        {
            var set = cc.Type as XCSingleEntity ?? throw new InvalidOperationException(
                          $"This component only can serve {nameof(XCSingleEntity)} objects");

            var ts = builder.Assembly.TypeSystem;
            var sb = ts.GetSystemBindings();

            //Create dto class
            foreach (var prop in set.Properties)
            {
                bool propertyGenerated = false;

                if (!prop.IsLink && string.IsNullOrEmpty(prop.DatabaseColumnName))
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

                        IType propType = GetTypeFromPlatform(pt, sb);

                        builder.DefinePropertyWithBackingField(propType, propName, false);
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

                            builder.DefinePropertyWithBackingField(sb.Guid, propName, false);
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

            foreach (var property in cls.TypeBody.Properties)
            {
                if (string.IsNullOrEmpty(property.MapTo)) continue;

                var prop = tb.FindProperty(property.Name);

                rg
                    .LdArg_0()
                    .LdArg(readerParam.ArgIndex)
                    .LdStr(property.MapTo)
                    .EmitCall(readerType.FindMethod("get_Item", _bindings.String))
                    .Unbox_Any(prop.PropertyType)
                    .EmitCall(prop.Setter);
            }

            rg.Ret();
        }


        private SSyntaxNode GetInsertQuery(XCSingleEntity se)
        {
            QueryMachine qm = new QueryMachine();
            qm.bg_query()
                .m_values();

            var pIndex = 0;

            var columns = se.Properties.Where(x => !x.IsLink)
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
                //if Version != null
                rg
                    .LdArg(cmdParam.ArgIndex)
                    .LdStr(compiler.Compile(GetUpdateQuery(set)))
                    .EmitCall(cmdType.FindProperty(nameof(DbCommand.CommandText)).Setter)
                    .Br(end);
                //if Version == null
                rg
                    .MarkLabel(narg)
                    .LdArg(cmdParam.ArgIndex)
                    .LdStr(compiler.Compile(GetInsertQuery(set)))
                    .EmitCall(cmdType.FindProperty(nameof(DbCommand.CommandText)).Setter)
                    .MarkLabel(end);
            }

            foreach (var property in cls.TypeBody.Properties)
            {
                if (string.IsNullOrEmpty(property.MapTo)) continue;

                var prop = tb.FindProperty(property.Name);

                rg.LdArg(cmdParam.ArgIndex)
                    .EmitCall(cmdType.FindMethod(nameof(DbCommand.CreateParameter)))
                    .StLoc(p_loc)
                    .LdLoc(p_loc)
                    .LdStr($"P_{indexp}")
                    .EmitCall(parameterType.FindProperty(nameof(DbParameter.ParameterName)).Setter)
                    .LdLoc(p_loc)
                    .LdArg_0()
                    .EmitCall(prop.Getter)
                    .Box(prop.PropertyType)
                    .EmitCall(parameterType.FindProperty(nameof(DbParameter.Value)).Setter);

                indexp++;
            }

            rg.Ret();
        }
    }
}