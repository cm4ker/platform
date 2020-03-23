using System.Data.Common;
using ZenPlatform.Compiler.Contracts;
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

        public ITypeBuilder Stage0(IAssemblyBuilder asm)
        {
            var type = asm.DefineInstanceType(this.GetNamespace(), DtoType.Name);

            type.DefineDefaultConstructor(false);

            return type;
        }

        public void Stage1(ITypeBuilder builder, SqlDatabaseType dbType)
        {
            EmitBody(builder, dbType);
            EmitVersionField(builder);

            if (CompilationMode == CompilationMode.Server)
            {
                EmitMappingSupport(builder);
            }
        }

        public void Stage2(ITypeBuilder builder, SqlDatabaseType dbType)
        {
        }

        private void EmitBody(ITypeBuilder builder, SqlDatabaseType dbType)
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

                builder.DefinePropertyWithBackingField(listType, table.Name, false);
            }
        }

        private void EmitMappingSupport(ITypeBuilder tb)
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

        private void EmitDefaultValues(ITypeBuilder builder, SqlDatabaseType dbType)
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