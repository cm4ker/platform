using System.Linq;
using System.Runtime.Serialization;
using Aquila.Compiler.Roslyn;
using Aquila.Compiler.Roslyn.RoslynBackend;
using Aquila.Component.Shared;
using Aquila.Core.Contracts;
using Aquila.Core.Contracts.Configuration;
using Aquila.Core.Contracts.TypeSystem;
using Aquila.Language.Ast;
using Aquila.Language.Ast.Definitions;
using Aquila.QueryBuilder;
using Name = Aquila.Language.Ast.Name;

namespace Aquila.SerializableTypeComponent.Compilation
{
    internal interface IInternalGenTask
    {
        RoslynTypeBuilder Stage0(RoslynAssemblyBuilder asm);
        void Stage1(RoslynTypeBuilder builder, SqlDatabaseType dbType, IEntryPointManager sm);
        void Stage2(RoslynTypeBuilder builder, SqlDatabaseType dbType);
    }

    public class TypeTask : ComponentAstTask, IInternalGenTask
    {
        private readonly IPType _type;
        private RoslynTypeSystem _ts;
        private SystemTypeBindings _sb;

        public TypeTask(IPType type, CompilationMode compilationMode, IComponent component, string name)
            : base(compilationMode, component, false, name, TypeBody.Empty)
        {
            _type = type;
        }


        public RoslynTypeBuilder Stage0(RoslynAssemblyBuilder asm)
        {
            _ts = asm.TypeSystem;
            _sb = _ts.GetSystemBindings();

            return asm.DefineInstanceType(GetNamespace(), Name);
        }

        public void Stage1(RoslynTypeBuilder builder, SqlDatabaseType dbType, IEntryPointManager sm)
        {
            var dataContractAttr = _ts.Factory.CreateAttribute(_ts, _ts.FindType<DataContractAttribute>());
            var dataMemberAttr = _ts.Factory.CreateAttribute(_ts, _ts.FindType<DataMemberAttribute>());
            builder.SetCustomAttribute(dataContractAttr);

            foreach (var prop in _type.Properties)
            {
                var type = prop.Type; //here can't contains set of types
                var clrType = type.ConvertType(_sb);

                var propBuilder = builder.DefinePropertyWithBackingField(clrType, prop.Name, false);
                propBuilder.SetAttribute(dataMemberAttr);
            }
        }

        public void Stage2(RoslynTypeBuilder builder, SqlDatabaseType dbType)
        {
        }
    }
}