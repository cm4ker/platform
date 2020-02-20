using System;
using System.Linq;
using System.Reflection;
using ZenPlatform.Compiler;
using ZenPlatform.Compiler.Contracts;
using ZenPlatform.Compiler.Contracts.Symbols;
using ZenPlatform.Compiler.Helpers;
using ZenPlatform.Configuration.Contracts;
using ZenPlatform.Configuration.Contracts.TypeSystem;
using ZenPlatform.EntityComponent.Configuration;
using ZenPlatform.Language.Ast;
using ZenPlatform.Language.Ast.Definitions;
using ZenPlatform.Language.Ast.Definitions.Functions;
using ZenPlatform.QueryBuilder;
using ZenPlatform.Shared.Tree;

namespace ZenPlatform.EntityComponent.Entity.Generation
{
    public class ObjectGenerationTask : ComponentAstTask, IEntityGenerationTask
    {
        public ObjectGenerationTask(
            IPType objectType,
            CompilationMode compilationMode, IComponent component, string name, TypeBody tb)
            : base(compilationMode, component, false, name, tb)
        {
            ObjectType = objectType;
            DtoType = objectType.GetDtoType();
        }

        public IPType ObjectType { get; }
        public IPType DtoType { get; }

        public ITypeBuilder Stage0(IAssemblyBuilder asm)
        {
            return asm.DefineInstanceType(GetNamespace(), Name);
        }

        public void Stage1(ITypeBuilder builder, SqlDatabaseType dbType)
        {
            EmitStructure(builder, dbType);
        }

        public void Stage2(ITypeBuilder builder, SqlDatabaseType dbType)
        {
            EmitBody(builder, dbType);
        }

        public void EmitStructure(ITypeBuilder builder, SqlDatabaseType dbType)
        {
            var type = ObjectType;
            var set = ObjectType;
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
                TypeBody.SymbolTable.Add(new Property(null, propName, propType.ToAstType()), codeObj.prop);
            }

            var saveBuilder = builder.DefineMethod("Save", true, false, false);

            TypeBody.SymbolTable.Add(
                new Function(null, null, null, null, saveBuilder.Name, saveBuilder.ReturnType.ToAstType()),
                saveBuilder);
        }

        private void EmitBody(ITypeBuilder builder, SqlDatabaseType dbType)
        {
            var type = ObjectType;
            var set = ObjectType;
            var ts = builder.Assembly.TypeSystem;
            var sb = ts.GetSystemBindings();
            var dtoClassName = set.GetDtoType().Name;

            var @namespace = set.GetNamespace();

            var dtoType = ts.FindType($"{@namespace}.{dtoClassName}");


            var dtoPrivate = builder.FindField("_dto");

            var mrg = ts.FindType($"{@namespace}.{set.GetManagerType().Name}");
            var mrgGet = mrg.FindMethod("Get", sb.Guid);

            foreach (var prop in set.Properties)
            {
                EmitProperty(builder, prop, sb, dtoType, dtoPrivate, ts, mrgGet);
            }

            foreach (var table in set.Tables)
            {
                var full = $"{GetNamespace()}.{table.GetObjectRowCollectionClassName()}";
                var t = ts.FindType(full);
                var dtoTableProp = dtoType.FindProperty(table.Name);

                var prop = builder.DefineProperty(t, table.Name, true, false, false);
                prop.getMethod.Generator
                    .LdArg_0()
                    .LdFld(dtoPrivate)
                    .EmitCall(dtoTableProp.Getter)
                    .NewObj(t.FindConstructor(dtoTableProp.PropertyType))
                    .Ret();
            }

            var saveBuilder = (IMethodBuilder) builder.FindMethod("Save");

            var sg = saveBuilder.Generator;

            sg
                .LdArg_0()
                .LdFld(dtoPrivate)
                .EmitCall(mrg.FindMethod("Save", dtoType))
                .Ret();
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

        private void GenerateObjectClassUserModules(IPType type)
        {
            var md = type.GetMD<MDEntity>();

            foreach (var module in md.Modules)
            {
                if (module.ModuleRelationType == ProgramModuleRelationType.Object)
                {
                    var typeBody = ParserHelper.ParseTypeBody(module.ModuleText);

                    foreach (var func in typeBody.Functions)
                    {
                        func.SymbolScope = SymbolScopeBySecurity.User;
                        this.AddFunction(func);
                    }
                }
            }
        }
    }
}