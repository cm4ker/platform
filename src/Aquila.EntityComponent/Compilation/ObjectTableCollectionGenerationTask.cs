using Aquila.Compiler.Contracts;
using Aquila.Compiler.Roslyn;
using Aquila.Compiler.Roslyn.RoslynBackend;
using Aquila.Core.Contracts;
using Aquila.Core.Contracts.Configuration;
using Aquila.Core.Contracts.TypeSystem;
using Aquila.EntityComponent.Entity;
using Aquila.Language.Ast;
using Aquila.QueryBuilder;

namespace Aquila.EntityComponent.Compilation
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

        public RoslynTypeBuilder Stage0(RoslynAssemblyBuilder asm)
        {
            var ts = asm.TypeSystem;
            _objectRow = ts.FindType(Table.GetObjectRowFullClassName());
            _dtoRowType = ts.FindType(Table.GetTableDtoRowClassFullName());

            var baseType = asm.TypeSystem.ResolveType(typeof(EntityTable<,>)).MakeGenericType(_dtoRowType, _objectRow);
            var type = asm.DefineInstanceType(this.GetNamespace(), Table.GetObjectRowCollectionClassName(), baseType);

            return type;
        }

        public void Stage1(RoslynTypeBuilder builder, SqlDatabaseType dbType, IEntryPointManager sm)
        {
            EmitStructure(builder, dbType);
        }

        public void Stage2(RoslynTypeBuilder builder, SqlDatabaseType dbType)
        {
            EmitBody(builder, dbType);
        }

        private RoslynType _objectRow;
        private RoslynType _dtoRowType;


        private void EmitStructure(RoslynTypeBuilder builder, SqlDatabaseType dbType)
        {
            var ts = builder.TypeSystem;
            var sb = ts.GetSystemBindings();

            var dtoListType = sb.List.MakeGenericType(_dtoRowType);

            var ctor = builder.DefineConstructor(false, dtoListType);

            var g = ctor.Body;

            g.LdArg_0()
                .LdArg(1)
                .Call(builder.BaseType.FindConstructor(dtoListType))
                ;


            var baseMethod = builder.BaseType.FindMethod("Add");
            var baseDtoProp = builder.BaseType.FindProperty("DtoRef");
            var baseObjProp = builder.BaseType.FindProperty("ObjRef");

            var addDtoMethod = sb.List.MakeGenericType(_dtoRowType).FindMethod(x => x.Name == "Add");
            var addObjMethod = sb.List.MakeGenericType(_objectRow).FindMethod(x => x.Name == "Add");


            var add = builder.DefineMethod("Add", true, false, false, baseMethod);
            add.WithReturnType(_objectRow);

            var g2 = add.Body;

            var dtoRowLocal = g2.DefineLocal(_dtoRowType);
            var objRowLocal = g2.DefineLocal(_objectRow);
            g2
                .NewObj(_dtoRowType.FindConstructor())
                .StLoc(dtoRowLocal)
                
                .LdLoc(dtoRowLocal)
                .NewObj(_objectRow.FindConstructor(_dtoRowType))
                .StLoc(objRowLocal)
                
                .LdArg_0()
                .LdProp(baseDtoProp)
                .LdLoc(dtoRowLocal)
                .Call(addDtoMethod)
                
                .LdArg_0()
                .LdProp(baseObjProp)
                .LdLoc(objRowLocal)
                .Call(addObjMethod)
                .LdLoc(objRowLocal)
                .Ret()
                ;
        }

        private void EmitBody(RoslynTypeBuilder builder, SqlDatabaseType dbType)
        {
        }
    }
}