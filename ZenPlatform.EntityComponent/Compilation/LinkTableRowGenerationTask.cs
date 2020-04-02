using System.Linq;
using ZenPlatform.Compiler.Contracts;
using ZenPlatform.Compiler.Helpers;
using ZenPlatform.Compiler.Roslyn;
using ZenPlatform.Compiler.Roslyn.DnlibBackend;
using ZenPlatform.Configuration.Contracts;
using ZenPlatform.Configuration.Contracts.TypeSystem;
using ZenPlatform.EntityComponent.Entity;
using ZenPlatform.Language.Ast;
using ZenPlatform.Language.Ast.Definitions;
using ZenPlatform.QueryBuilder;

namespace ZenPlatform.EntityComponent.Compilation
{
    public class LinkTableRowGenerationTask : ComponentAstTask, IEntityGenerationTask
    {
        public LinkTableRowGenerationTask(
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

        public SreTypeBuilder Stage0(SreAssemblyBuilder asm)
        {
            return asm.DefineInstanceType(this.GetNamespace(), Table.GetLinkRowClassName());
        }

        public void Stage1(SreTypeBuilder builder, SqlDatabaseType dbType, IEntryPointManager sm)
        {
            EmitStructure(builder, dbType);
        }

        public void Stage2(SreTypeBuilder builder, SqlDatabaseType dbType)
        {
            EmitBody(builder, dbType);
        }

        private IType _dtoRowType;
        private IField _dtoPrivate;

        private void EmitStructure(SreTypeBuilder builder, SqlDatabaseType dbType)
        {
            // var ts = builder.TypeSystem;
            // var sb = ts.GetSystemBindings();
            //
            // _dtoRowType = ts.FindType(Table.GetTableDtoRowClassFullName());
            // var ctor = builder.DefineConstructor(false, _dtoRowType);
            //
            // var g = ctor.Generator;
            // _dtoPrivate = builder.DefineField(_dtoRowType, "_dtoRow", false, false);
            //
            // g.LdArg_0()
            //     .EmitCall(builder.BaseType.FindConstructor())
            //     .LdArg_0()
            //     .LdArg(1)
            //     .StFld(_dtoPrivate)
            //     .Ret();
            //
            // foreach (var prop in Table.Properties)
            // {
            //     bool propertyGenerated = false;
            //
            //     var propName = prop.Name;
            //
            //     var propType = (prop.Types.Count() > 1)
            //         ? sb.Object
            //         : prop.Types.First().ConvertType(sb);
            //
            //     var hasSet = !prop.IsReadOnly;
            //
            //
            //     var codeObj = builder.DefineProperty(propType, propName, true, hasSet, false);
            //     TypeBody.SymbolTable.AddProperty(new Property(null, propName, propType.ToAstType()))
            //         .Connect(codeObj.prop);
            // }
        }

        private void EmitBody(SreTypeBuilder builder, SqlDatabaseType dbType)
        {
            // var ts = builder.TypeSystem;
            // var sb = builder.TypeSystem.GetSystemBindings();
            //
            // var mrg = ts.FindType($"{GetNamespace()}.{ObjectType.GetManagerType().Name}");
            // var mrgGet = mrg.FindMethod("Get", sb.Guid);
            //
            // foreach (var prop in Table.Properties)
            // {
            //     SharedGenerators.EmitLinkProperty(builder, prop, sb, _dtoRowType, _dtoPrivate, ts, mrgGet,
            //         GetNamespace());
            // }
        }
    }
}