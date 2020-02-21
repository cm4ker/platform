using System;
using System.Linq;
using ZenPlatform.Compiler.Contracts;
using ZenPlatform.Configuration.Contracts;
using ZenPlatform.Configuration.Contracts.TypeSystem;

namespace ZenPlatform.EntityComponent.Entity.Generation
{
    public static class SharedGenerators
    {
        public static void EmitDtoProperty(ITypeBuilder builder, IPProperty prop, SystemTypeBindings sb)
        {
            bool propertyGenerated = false;
            if (prop.IsSelfLink) return;


            if (string.IsNullOrEmpty(prop.GetSettings().DatabaseName))
            {
                throw new Exception(
                    $"Prop: {prop.Name} ObjectType: {"Empty"}. Database column is empty!");
            }

            if (prop.Types.Count() > 1)
            {
                var clsSchema = prop.GetObjSchema()
                    .First(x => x.SchemaType == ColumnSchemaType.Type);

                var dbSchema = prop.GetDbSchema()
                    .First(x => x.SchemaType == ColumnSchemaType.Type);


                var propBuilder = builder.DefinePropertyWithBackingField(clsSchema.PlatformIpType.ConvertType(sb),
                    clsSchema.FullName, false);

                var attr = builder.CreateAttribute<MapToAttribute>(sb.String);
                propBuilder.SetAttribute(attr);
                attr.SetParameters(dbSchema.FullName);
            }


            foreach (var ctype in prop.Types)
            {
                if (ctype.IsPrimitive)
                {
                    var dbColName = prop
                        .GetDbSchema()
                        .First(x => x.SchemaType == ((prop.Types.Count() > 1)
                            ? ColumnSchemaType.Value
                            : ColumnSchemaType.NoSpecial) && x.PlatformIpType == ctype).FullName;

                    var propName = prop
                        .GetObjSchema()
                        .First(x => x.SchemaType == ((prop.Types.Count() > 1)
                            ? ColumnSchemaType.Value
                            : ColumnSchemaType.NoSpecial) && x.PlatformIpType == ctype).FullName;

                    IType propType = ctype.ConvertType(sb);

                    var propBuilder = builder.DefinePropertyWithBackingField(propType, propName, false);

                    var attr = builder.CreateAttribute<MapToAttribute>(sb.String);
                    propBuilder.SetAttribute(attr);
                    attr.SetParameters(dbColName);

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
                            .First(x => x.SchemaType == ((prop.Types.Count() > 1)
                                ? ColumnSchemaType.Ref
                                : ColumnSchemaType.NoSpecial)).FullName;

                        var propName = prop
                            .GetObjSchema()
                            .First(x => x.SchemaType == ((prop.Types.Count() > 1)
                                ? ColumnSchemaType.Ref
                                : ColumnSchemaType.NoSpecial)).FullName;

                        var propBuilder = builder.DefinePropertyWithBackingField(sb.Guid, propName, false);

                        var attr = builder.CreateAttribute<MapToAttribute>(sb.String);
                        propBuilder.SetAttribute(attr);
                        attr.SetParameters(dbColName);

                        if (!prop.IsUnique)
                        {
                            var initAttr = builder.CreateAttribute<NeedInitAttribute>();
                            propBuilder.SetAttribute(initAttr);
                        }
                    }
                }
            }
        }


        public static void EmitObjectProperty(ITypeBuilder builder, IPProperty prop, SystemTypeBindings sb,
            IType dtoType,
            IField dtoPrivate, ITypeSystem ts, IMethod mrgGet, string ns)
        {
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
                        dtoPropSchema = prop.GetObjSchema().First(x => Equals(x.PlatformIpType, ctype));
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
                        var mrgRemote = ts.FindType($"{ns}.{ctype.GetManagerType().Name}");
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
                            .LdcI4((int) ctype.GetSettings().SystemId)
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
                        var mrgRemote = ts.FindType($"{ns}.{ctype.GetManagerType().Name}");
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


        public static void EmitLinkProperty(ITypeBuilder builder, IPProperty prop, SystemTypeBindings sb,
            IType dtoType,
            IField dtoPrivate, ITypeSystem ts, IMethod mrgGet, string ns)
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
                    getBuilder
                        .LdArg_0()
                        .EmitCall(builder.FindProperty("Id").Getter)
                        .EmitCall(mrgGet)
                        .Ret();
                }
            }
        }
    }
}