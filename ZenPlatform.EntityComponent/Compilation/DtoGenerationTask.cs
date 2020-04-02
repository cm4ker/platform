using System.Data.Common;
using ZenPlatform.Compiler.Contracts;
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
    public class DtoGenerationTask : ComponentAstTask, IEntityGenerationTask
    {
        public DtoGenerationTask(
            IPType dtoType,
            CompilationMode compilationMode, IComponent component, string name, TypeBody tb)
            : base(compilationMode, component, false, name, tb)
        {
            DtoType = dtoType;
        }

        public IPType DtoType { get; }

        public SreTypeBuilder Stage0(SreAssemblyBuilder asm)
        {
            var type = asm.DefineInstanceType(this.GetNamespace(), DtoType.Name);
            type.DefineDefaultConstructor(false);

            return type;
        }

        public void Stage1(SreTypeBuilder builder, SqlDatabaseType dbType, IEntryPointManager sm)
        {
            EmitBody(builder, dbType);
            EmitVersionField(builder);

            if (CompilationMode == CompilationMode.Server)
            {
                EmitMappingSupport(builder);
            }
        }

        public void Stage2(SreTypeBuilder builder, SqlDatabaseType dbType)
        {
        }

        private void EmitBody(SreTypeBuilder builder, SqlDatabaseType dbType)
        {
            var type = DtoType;

            var ts = builder.Assembly.TypeSystem;
            var sb = ts.GetSystemBindings();

            //Create dto class
            foreach (var prop in type.Properties)
            {
                SharedGenerators.EmitDtoProperty(builder, prop, sb);
            }

            //Create table property
            foreach (var table in type.Tables)
            {
                var tableRow = ts.GetType(table.GetTableDtoRowClassFullName());
                var listType = sb.List.MakeGenericType(tableRow);

                //builder.DefinePropertyWithBackingField(listType, table.Name, false);
            }
        }

        private void EmitMappingSupport(SreTypeBuilder tb)
        {
            var _ts = tb.Assembly.TypeSystem;
            var _bindings = _ts.GetSystemBindings();
            tb.AddInterfaceImplementation(_ts.Resolve<ICanMap>());

            var readerMethod = tb.DefineMethod(nameof(ICanMap.Map), true, false, true);
            var rg = readerMethod.Body;

            var readerType = _ts.Resolve<DbDataReader>();

            var readerParam =
                readerMethod.DefineParameter("reader", readerType, false, false);

            foreach (var property in tb.Properties)
            {
                var mt = property.FindCustomAttribute<MapToAttribute>();
                if (mt is null) continue;

                rg
                    .LdArg_0()
                    .LdArg(readerParam)
                    .LdLit(mt.Parameters[0].ToString())
                    .LdElem()
                    .Cast(property.PropertyType)
                    .StProp(property)
                    .Statement();
            }

            //rg.Ret();
        }

        private void EmitVersionField(SreTypeBuilder tb)
        {
            var _ts = tb.Assembly.TypeSystem;
            var _b = _ts.GetSystemBindings();
            var prop = tb.DefinePropertyWithBackingField(_b.Byte.MakeArrayType(), "Version", false);
        }

        private void EmitDefaultValues(SreTypeBuilder builder, SqlDatabaseType dbType)
        {
            var set = DtoType;

            var ts = builder.Assembly.TypeSystem;
            var sb = ts.GetSystemBindings();

            foreach (var prop in set.Properties)
            {
                if (prop.IsSelfLink) continue;
            }
        }
    }
}