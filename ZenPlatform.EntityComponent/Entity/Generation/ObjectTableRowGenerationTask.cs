using System.Linq;
using ZenPlatform.Compiler.Contracts;
using ZenPlatform.Compiler.Helpers;
using ZenPlatform.Configuration.Contracts;
using ZenPlatform.Configuration.Contracts.TypeSystem;
using ZenPlatform.Language.Ast;
using ZenPlatform.Language.Ast.Definitions;
using ZenPlatform.QueryBuilder;

namespace ZenPlatform.EntityComponent.Entity.Generation
{
    public class ObjectTableRowGenerationTask : ComponentAstTask, IEntityGenerationTask
    {
        public ObjectTableRowGenerationTask(
            IPType objectType,
            ITable table,
            CompilationMode compilationMode, IComponent component, string name, TypeBody tb)
            : base(compilationMode, component, false, name, tb)
        {
            ObjectType = objectType;
            Table = table;
        }

        public IPType ObjectType { get; }
        public ITable Table { get; }

        public ITypeBuilder Stage0(IAssemblyBuilder asm)
        {
            return asm.DefineInstanceType(this.GetNamespace(), Table.GetObjectRowClassName());
        }

        public void Stage1(ITypeBuilder builder, SqlDatabaseType dbType)
        {
            EmitStructure(builder, dbType);
        }

        public void Stage2(ITypeBuilder builder, SqlDatabaseType dbType)
        {
            EmitBody(builder, dbType);
        }

        private IType _dtoRowType;
        private IField _dtoPrivate;

        private void EmitStructure(ITypeBuilder builder, SqlDatabaseType dbType)
        {
            var ts = builder.TypeSystem;
            var sb = ts.GetSystemBindings();

            _dtoRowType = ts.FindType(Table.GetTableDtoRowClassFullName());
            var ctor = builder.DefineConstructor(false, _dtoRowType);

            var g = ctor.Generator;
            _dtoPrivate = builder.DefineField(_dtoRowType, "_dtoRow", false, false);

            g.LdArg_0()
                .EmitCall(builder.BaseType.FindConstructor())
                .LdArg_0()
                .LdArg(1)
                .StFld(_dtoPrivate)
                .Ret();

            foreach (var prop in Table.Properties)
            {
                bool propertyGenerated = false;

                var propName = prop.Name;

                var propType = (prop.Types.Count() > 1)
                    ? sb.Object
                    : prop.Types.First().ConvertType(sb);

                var hasSet = !prop.IsReadOnly;


                var codeObj = builder.DefineProperty(propType, propName, true, hasSet, false);
                TypeBody.SymbolTable.Add(new Property(null, propName, propType.ToAstType()), codeObj.prop);
            }
        }

        private void EmitBody(ITypeBuilder builder, SqlDatabaseType dbType)
        {
            var ts = builder.TypeSystem;
            var sb = builder.TypeSystem.GetSystemBindings();

            var mrg = ts.FindType($"{GetNamespace()}.{ObjectType.GetManagerType().Name}");
            var mrgGet = mrg.FindMethod("Get", sb.Guid);

            foreach (var prop in Table.Properties)
            {
                EmitProperty(builder, prop, sb, _dtoRowType, _dtoPrivate, ts, mrgGet);
            }
        }


        private void EmitProperty(ITypeBuilder builder, IPProperty prop, SystemTypeBindings sb, IType dtoType,
            IField dtoPrivate, ITypeSystem ts, IMethod mrgGet)
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
                        var mrgRemote = ts.FindType($"{GetNamespace()}.{ctype.GetManagerType().Name}");
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
                        var mrgRemote = ts.FindType($"{GetNamespace()}.{ctype.GetManagerType().Name}");
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
    }
}