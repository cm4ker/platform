using System;
using System.Linq;
using Aquila.Compiler.Aqua;
using Aquila.Compiler.Contracts;
using Aquila.Compiler.Roslyn;
using Aquila.Compiler.Roslyn.RoslynBackend;
using Aquila.Component.Shared;
using Aquila.Core.Contracts.Configuration;
using Aquila.Core.Contracts.TypeSystem;
using Aquila.EntityComponent.Entity;
using SystemTypeBindings = Aquila.Compiler.Roslyn.SystemTypeBindings;

namespace Aquila.EntityComponent.Compilation
{
    public static class SharedGenerators
    {
        public static void EmitDtoProperty(RoslynTypeBuilder builder, IPProperty prop, SystemTypeBindings sb)
        {
            bool propertyGenerated = false;
            if (prop.IsSelfLink) return;


            if (string.IsNullOrEmpty(prop.GetSettings().DatabaseName))
            {
                throw new Exception(
                    $"Prop: {prop.Name} ObjectType: {"Empty"}. Database column is empty!");
            }

            if (prop.Type.IsTypeSet)
            {
                var clsSchema = prop.GetObjSchema()
                    .First(x => x.SchemaType == ColumnSchemaType.Type);

                var dbSchema = prop.GetDbSchema()
                    .First(x => x.SchemaType == ColumnSchemaType.Type);


                var propBuilder = builder.DefinePropertyWithBackingField(clsSchema.PlatformIpType.ConvertType(sb),
                    clsSchema.FullName, false);

                var attr = builder.CreateAttribute<MapToAttribute>(sb.String);
                attr.SetParameters(dbSchema.FullName);
                propBuilder.SetAttribute(attr);
            }


            foreach (var ctype in prop.GetTypes())
            {
                if (ctype.IsPrimitive)
                {
                    var dbColName = prop
                        .GetDbSchema()
                        .First(x => x.SchemaType == ((prop.Type.IsTypeSet)
                            ? ColumnSchemaType.Value
                            : ColumnSchemaType.NoSpecial) && x.PlatformIpType == ctype).FullName;

                    var propName = prop
                        .GetObjSchema()
                        .First(x => x.SchemaType == ((prop.Type.IsTypeSet)
                            ? ColumnSchemaType.Value
                            : ColumnSchemaType.NoSpecial) && x.PlatformIpType == ctype).FullName;

                    RoslynType propType = ctype.ConvertType(sb);

                    var propBuilder = builder.DefinePropertyWithBackingField(propType, propName, false);

                    var attr = builder.CreateAttribute<MapToAttribute>(sb.String);
                    attr.SetParameters(dbColName);
                    propBuilder.SetAttribute(attr);


                    if (!prop.IsUnique)
                    {
                        var initAttr = builder.CreateAttribute<NeedInitAttribute>();
                        propBuilder.SetAttribute(initAttr);
                    }
                }
                else if (ctype.IsLink)
                {
                    if (!propertyGenerated)
                    {
                        propertyGenerated = true;

                        var dbColName = prop
                            .GetDbSchema()
                            .First(x => x.SchemaType == ((prop.Type.IsTypeSet)
                                ? ColumnSchemaType.Ref
                                : ColumnSchemaType.NoSpecial)).FullName;

                        var propName = prop
                            .GetObjSchema()
                            .First(x => x.SchemaType == ((prop.Type.IsTypeSet)
                                ? ColumnSchemaType.Ref
                                : ColumnSchemaType.NoSpecial)).FullName;

                        var propBuilder = builder.DefinePropertyWithBackingField(sb.Guid, propName, false);

                        var attr = builder.CreateAttribute<MapToAttribute>(sb.String);
                        attr.SetParameters(dbColName);
                        propBuilder.SetAttribute(attr);


                        if (!prop.IsUnique)
                        {
                            var initAttr = builder.CreateAttribute<NeedInitAttribute>();
                            propBuilder.SetAttribute(initAttr);
                        }
                    }
                }
            }
        }


        public static void EmitObjectProperty(RoslynTypeBuilder builder, IPProperty prop, SystemTypeBindings sb,
            RoslynType dtoType, RoslynField dtoPrivate, RoslynTypeSystem ts, RoslynMethod mrgGet, string ns)
        {
            var propName = prop.Name;

            var propType = (prop.Type.IsTypeSet)
                ? sb.Object
                : prop.Type.ConvertType(sb);

            var propBuilder = (RoslynPropertyBuilder) builder.FindProperty(propName);
            var getBuilder = ((RoslynMethodBuilder) propBuilder.Getter).Body;
            var setBuilder = ((RoslynMethodBuilder) propBuilder.Setter)?.Body;

            if (prop.Type.IsTypeSet)
            {
                var typeField = prop.GetObjSchema()
                    .First(x => x.SchemaType == ColumnSchemaType.Type);
                var dtoTypeProp = dtoType.FindProperty(typeField.FullName);

                foreach (var ctype in prop.GetTypes())
                {
                    ColumnSchemaDefinition dtoPropSchema;

                    if (ctype.IsLink)
                    {
                        dtoPropSchema = prop.GetObjSchema()
                            .First(x => x.SchemaType == ColumnSchemaType.Ref);
                    }
                    else
                    {
                        dtoPropSchema = prop.GetObjSchema().First(x =>
                            Equals(x.PlatformIpType, ctype) && x.SchemaType != ColumnSchemaType.Type);
                    }

                    var dtoProp = dtoType.FindProperty(dtoPropSchema.FullName);

                    var compileType = ctype.ConvertType(sb);

                    //var label = getBuilder.DefineLabel();

                    //GETTER
                    getBuilder
                        .LdArg_0()
                        .LdFld(dtoPrivate)
                        .Call(dtoTypeProp.Getter)
                        .LdLit((int) ctype.GetSettings().SystemId)
                        .Ceq()
                        .Block()
                        .LdArg_0()
                        .LdFld(dtoPrivate)
                        .Call(dtoProp.Getter)
                        .Ret()
                        .EndBlock()
                        .Nothing()
                        .If()
                        ;


                    // if (ctype.IsLink)
                    // {
                    //     var mrgRemote = ts.FindType($"{ns}.{ctype.GetManagerType().Name}");
                    //     var mrgRemoteGet = mrgRemote.FindMethod("Get", sb.Guid);
                    //     getBuilder.Call(mrgRemoteGet);
                    // }

                    if (setBuilder != null)
                    {
                        //label = setBuilder.DefineLabel();
                        //SETTER
                        var block = setBuilder
                            .LdArg(1)
                            .IsInst(compileType)
                            .Block()
                            .LdArg_0()
                            .LdFld(dtoPrivate)
                            .LdArg(1)
                            .Cast(compileType);

                        if (ctype.IsLink)
                            block.Call(compileType.FindProperty("Id").Getter);

                        block
                            .Call(dtoProp.Setter)
                            .LdArg_0()
                            .LdFld(dtoPrivate)
                            .LdLit((int) ctype.GetSettings().SystemId)
                            .Call(dtoTypeProp.Setter)
                            .Ret()
                            .EndBlock()
                            .Nothing()
                            .If()
                            ;


                        // setBuilder.Call(dtoProp.Setter)
                        //     .LdArg_0()
                        //     .LdFld(dtoPrivate)
                        //     .LdLit((int) ctype.GetSettings().SystemId)
                        //     .Call(dtoTypeProp.Setter)
                        //     .Statement();
                    }
                }

                getBuilder.Throw(sb.Exception);
                setBuilder?.Throw(sb.Exception);

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

                    var ctype = prop.Type;

                    var compileType = ctype.ConvertType(sb);

                    //GETTER
                    getBuilder
                        .LdArg_0()
                        .LdFld(dtoPrivate)
                        .Call(dtoProp.Getter);

                    if (ctype.IsLink)
                    {
                        var mrgRemote = ts.FindType($"{ns}.{ctype.GetManagerType().Name}");
                        var mrgRemoteGet = mrgRemote.FindMethod("Get", sb.Guid);
                        getBuilder.Call(mrgRemoteGet);
                    }

                    getBuilder
                        .Ret()
                        ;

                    if (setBuilder != null)
                    {
                        setBuilder
                            .LdArg_0()
                            .LdFld(dtoPrivate)
                            .LdArg(1);

                        if (ctype.IsLink)
                            setBuilder.Call(compileType.FindProperty("Id").Getter);

                        setBuilder.Call(dtoProp.Setter)
                            ;
                    }
                }
                else
                {
                    getBuilder
                        .LdArg_0()
                        .Call(builder.FindProperty("Id").Getter)
                        .Call(mrgGet)
                        .Ret();
                }
            }
        }


        public static void EmitLinkProperty(RoslynTypeBuilder builder, IPProperty prop, SystemTypeBindings sb,
            RoslynType dtoType,
            RoslynField dtoPrivate, RoslynTypeSystem ts, RoslynMethod mrgGet, string ns, RoslynMethod reload)
        {
            var propName = prop.Name;

            var propType = (prop.Type.IsTypeSet)
                ? sb.Object
                : prop.Type.ConvertType(sb);

            var propBuilder = (RoslynPropertyBuilder) builder.FindProperty(propName);
            var getBuilder = ((RoslynMethodBuilder) propBuilder.Getter).Body;

            // var valueParam = propBuilder.setMethod.Parameters[0];

            if (reload != null)
                getBuilder
                    .LdArg_0()
                    .LdFld(dtoPrivate)
                    .LdNull()
                    .Ceq()
                    .Block()
                    .LdArg_0()
                    .Call(reload)
                    .EndBlock()
                    .Nothing()
                    .If();

            if (prop.Type.IsTypeSet)
            {
                var typeField = prop.GetObjSchema()
                    .First(x => x.SchemaType == ColumnSchemaType.Type);
                var dtoTypeProp = dtoType.FindProperty(typeField.FullName);

                foreach (var ctype in prop.GetTypes())
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

                    //var label = getBuilder.DefineLabel();


                    //GETTER
                    getBuilder
                        .LdArg_0()
                        .LdFld(dtoPrivate)
                        .Call(dtoTypeProp.Getter)
                        .LdLit((int) ctype.GetSettings().SystemId)
                        .Ceq()
                        .Block()
                        .LdArg_0()
                        .LdFld(dtoPrivate)
                        .Call(dtoProp.Getter)
                        .Ret()
                        .EndBlock()
                        .Nothing()
                        .If();
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
                        .Call(dtofield.Getter)
                        .Ret();
                }
                else
                {
                    getBuilder
                        .LdArg_0()
                        .Call(builder.FindProperty("Id").Getter)
                        .Call(mrgGet)
                        .Ret();
                }
            }
        }
    }
}