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
            var dtoClassName = type.GetDtoName();

            var cls = new ComponentClass(CompilationMode.Shared, _component, type, null, dtoClassName,
                new TypeBody(new List<Member>())) {Namespace = type.GetNamespace()};

            cls.Bag = ObjectType.Dto;

            var cu = new CompilationUnit(null, new List<NamespaceBase>(), new List<TypeEntity>() {cls});
            root.Add(cu);
        }

        public void Stage1(Node astTree, ITypeBuilder builder, SqlDatabaseType dbType, CompilationMode mode)
        {
            if (astTree is ComponentClass cc && (ObjectType) cc.Bag == ObjectType.Dto)
            {
                if ((cc.CompilationMode & mode).HasFlag(CompilationMode.Server))
                {
                    EmitBody(cc, builder, dbType);
                    EmitVersionField(builder);
                    EmitMappingSupport(cc, builder);
                    
                }
                else if ((cc.CompilationMode & mode).HasFlag(CompilationMode.Client))
                {
                    EmitBody(cc, builder, dbType);
                }
            }
        }

        public void Stage2(Node astTree, ITypeBuilder builder, SqlDatabaseType dbType, CompilationMode mode)
        {
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
                        var clsSchema = prop.GetPropertySchemas(prop.Name)
                            .First(x => x.SchemaType == XCColumnSchemaType.Type);

                        var dbSchema = prop.GetPropertySchemas(prop.DatabaseColumnName)
                            .First(x => x.SchemaType == XCColumnSchemaType.Type);


                        var propBuilder = builder.DefinePropertyWithBackingField(sb.Int, clsSchema.FullName, false);

                        var attr = builder.CreateAttribute<MapToAttribute>(sb.String);
                        propBuilder.SetAttribute(attr);
                        attr.SetParameters(dbSchema.FullName);
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

        private void EmitVersionField(ITypeBuilder tb)
        {
            var _ts = tb.Assembly.TypeSystem;
            var _b = _ts.GetSystemBindings();
            var prop = tb.DefinePropertyWithBackingField(_b.Byte.MakeArrayType(), "Version", false);
        }
    }
}