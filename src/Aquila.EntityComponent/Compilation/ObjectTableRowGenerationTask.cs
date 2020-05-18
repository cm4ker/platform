using System.Linq;
using Aquila.Compiler.Contracts;
using Aquila.Compiler.Helpers;
using Aquila.Compiler.Roslyn;
using Aquila.Compiler.Roslyn.RoslynBackend;
using Aquila.Component.Shared;
using Aquila.Core.Contracts;
using Aquila.Core.Contracts.Configuration;
using Aquila.Core.Contracts.TypeSystem;
using Aquila.EntityComponent.Entity;
using Aquila.Language.Ast;
using Aquila.Language.Ast.Definitions;
using Aquila.QueryBuilder;

namespace Aquila.EntityComponent.Compilation
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

        public RoslynTypeBuilder Stage0(RoslynAssemblyBuilder asm)
        {
            return asm.DefineInstanceType(this.GetNamespace(), Table.GetObjectRowClassName());
        }

        public void Stage1(RoslynTypeBuilder builder, SqlDatabaseType dbType, IEntryPointManager sm)
        {
            EmitStructure(builder, dbType);
        }

        public void Stage2(RoslynTypeBuilder builder, SqlDatabaseType dbType)
        {
            EmitBody(builder, dbType);
        }

        private RoslynType _dtoRowType;
        private RoslynField _dtoPrivate;

        private void EmitStructure(RoslynTypeBuilder builder, SqlDatabaseType dbType)
        {
            var ts = builder.TypeSystem;
            var sb = ts.GetSystemBindings();

            _dtoRowType = ts.FindType(Table.GetTableDtoRowClassFullName());
            var ctor = builder.DefineConstructor(false, _dtoRowType);

            var g = ctor.Body;
            _dtoPrivate = builder.DefineField(_dtoRowType, "_dtoRow", false, false);

            g.LdArg_0()
                .Call(builder.BaseType.FindConstructor())
                .LdArg_0()
                .LdArg(1)
                .StFld(_dtoPrivate)
                .Ret();

            foreach (var prop in Table.Properties)
            {
                bool propertyGenerated = false;

                var propName = prop.Name;

                var propType = (prop.Type.IsTypeSet)
                    ? sb.Object
                    : prop.Type.ConvertType(sb);

                var hasSet = !prop.IsReadOnly;


                var codeObj = builder.DefineProperty(propType, propName, true, hasSet, false);
                TypeBody.SymbolTable.AddProperty(new Property(null, propName, propType.ToAstType()))
                    .Connect(codeObj.prop);
            }
        }

        private void EmitBody(RoslynTypeBuilder builder, SqlDatabaseType dbType)
        {
            var ts = builder.TypeSystem;
            var sb = builder.TypeSystem.GetSystemBindings();

            var mrg = ts.FindType($"{GetNamespace()}.{ObjectType.GetManagerType().Name}");
            var mrgGet = mrg.FindMethod("Get", sb.Guid);

            foreach (var prop in Table.Properties)
            {
                SharedGenerators.EmitObjectProperty(builder, prop, sb, _dtoRowType, _dtoPrivate, ts, mrgGet,
                    GetNamespace());
            }
        }
    }
}