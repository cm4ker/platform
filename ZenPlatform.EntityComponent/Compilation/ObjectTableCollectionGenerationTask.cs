using ZenPlatform.Compiler.Contracts;
using ZenPlatform.Configuration.Contracts;
using ZenPlatform.Configuration.Contracts.TypeSystem;
using ZenPlatform.EntityComponent.Entity;
using ZenPlatform.Language.Ast;
using ZenPlatform.Language.Ast.Definitions;
using ZenPlatform.QueryBuilder;

namespace ZenPlatform.EntityComponent.Compilation
{
    public class ObjectTableCollectionGenerationTask : ComponentAstTask, IEntityGenerationTask
    {
        public ObjectTableCollectionGenerationTask(
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
            var ts = asm.TypeSystem;
            _objectRow = ts.FindType(Table.GetObjectRowFullClassName());
            _dtoRowType = ts.FindType(Table.GetTableDtoRowClassFullName());

            var baseType = asm.TypeSystem.FindType(typeof(EntityTable<,>)).MakeGenericType(_dtoRowType, _objectRow);
            var type = asm.DefineInstanceType(this.GetNamespace(), Table.GetObjectRowCollectionClassName(), baseType);

            return type;
        }

        public void Stage1(ITypeBuilder builder, SqlDatabaseType dbType, IAssemblyServiceManager sm)
        {
            EmitStructure(builder, dbType);
        }

        public void Stage2(ITypeBuilder builder, SqlDatabaseType dbType)
        {
            EmitBody(builder, dbType);
        }

        private IType _objectRow;
        private IType _dtoRowType;


        private void EmitStructure(ITypeBuilder builder, SqlDatabaseType dbType)
        {
            var ts = builder.TypeSystem;
            var sb = ts.GetSystemBindings();

            var dtoListType = sb.List.MakeGenericType(_dtoRowType);

            var ctor = builder.DefineConstructor(false, dtoListType);

            var g = ctor.Generator;

            g.LdArg_0()
                .LdArg(1)
                .EmitCall(builder.BaseType.FindConstructor(dtoListType))
                .Ret();


            var baseMethod = builder.BaseType.FindMethod("Add");
            var baseDtoProp = builder.BaseType.FindProperty("DtoRef");
            var baseObjProp = builder.BaseType.FindProperty("ObjRef");

            var addDtoMethod = sb.List.MakeGenericType(_dtoRowType).FindMethod(x => x.Name == "Add");
            var addObjMethod = sb.List.MakeGenericType(_objectRow).FindMethod(x => x.Name == "Add");


            var add = builder.DefineMethod("Add", true, false, false, baseMethod);
            add.WithReturnType(_objectRow);

            var g2 = add.Generator;

            var dtoRowLocal = g2.DefineLocal(_dtoRowType);
            var objRowLocal = g2.DefineLocal(_objectRow);
            g2
                .NewObj(_dtoRowType.FindConstructor())
                .StLoc(dtoRowLocal)
                .LdLoc(dtoRowLocal)
                .NewObj(_objectRow.FindConstructor(_dtoRowType))
                .StLoc(objRowLocal)
                .LdArg_0()
                .EmitCall(baseDtoProp.Getter)
                .LdLoc(dtoRowLocal)
                .EmitCall(addDtoMethod)
                .LdArg_0()
                .EmitCall(baseObjProp.Getter)
                .LdLoc(objRowLocal)
                .EmitCall(addObjMethod)
                .LdLoc(objRowLocal)
                .Ret();
        }

        private void EmitBody(ITypeBuilder builder, SqlDatabaseType dbType)
        {
        }
    }
}