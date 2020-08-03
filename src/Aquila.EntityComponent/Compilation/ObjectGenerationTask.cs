using System.Linq;
using Aquila.Compiler;
using Aquila.Compiler.Contracts;
using Aquila.Compiler.Helpers;
using Aquila.Compiler.Roslyn;
using Aquila.Compiler.Roslyn.RoslynBackend;
using Aquila.Component.Shared;
using Aquila.Configuration.Common.TypeSystem;
using Aquila.Core.Contracts;
using Aquila.Core.Contracts.Configuration;
using Aquila.Core.Contracts.TypeSystem;
using Aquila.EntityComponent.Configuration;
using Aquila.EntityComponent.Entity;
using Aquila.Language.Ast;
using Aquila.Language.Ast.Definitions.Functions;
using Aquila.Language.Ast.Misc;
using Aquila.Language.Ast.Symbols;
using Aquila.QueryBuilder;
using Aquila.Serializer;
using Name = Aquila.Language.Ast.Name;
using Property = Aquila.Language.Ast.Property;

namespace Aquila.EntityComponent.Compilation
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

            GenerateObjectClassUserModules(ObjectType);
        }

        public IPType ObjectType { get; }
        public IPType DtoType { get; }

        public RoslynTypeBuilder Stage0(RoslynAssemblyBuilder asm)
        {
            return asm.DefineInstanceType(GetNamespace(), Name);
        }

        public void Stage1(RoslynTypeBuilder builder, SqlDatabaseType dbType, IEntryPointManager sm)
        {
            EmitStructure(builder, dbType);
        }

        public void Stage2(RoslynTypeBuilder builder, SqlDatabaseType dbType)
        {
            EmitBody(builder, dbType);
        }

        public void EmitStructure(RoslynTypeBuilder builder, SqlDatabaseType dbType)
        {
            var set = ObjectType;
            var ts = builder.Assembly.TypeSystem;
            var sb = ts.GetSystemBindings();
            var dtoClassName = set.GetDtoType().Name;
            var @namespace = set.GetNamespace();

            var dtoType = ts.FindType($"{@namespace}.{dtoClassName}");

            var c = builder.DefineConstructor(false, dtoType);
            var g = c.Body;

            var dtoPrivate = builder.DefineField(dtoType, "_dto", false, false);

            g.LdArg_0().LdArg(c.Parameters.First())
                .StFld(dtoPrivate)
                ;

            if (CompilationMode.HasFlag(CompilationMode.Server))
            {
                foreach (var prop in set.Properties)
                {
                    var propName = prop.Name;

                    var propType = (prop.Type.IsTypeSet)
                        ? sb.Object
                        : prop.Type.ConvertType(sb);

                    var hasSet = !prop.IsReadOnly;


                    var codeObj = builder.DefineProperty(propType, propName, true, hasSet, false);
                    TypeBody.SymbolTable.AddProperty(new Property(null, propName, propType.ToAstType()))
                        .Connect(codeObj.prop);
                }


                foreach (var table in set.Tables)
                {
                    var full = $"{GetNamespace()}.{table.GetObjectRowCollectionClassName()}";
                    var t = ts.FindType(full);
                    var dtoTableProp = dtoType.FindProperty(table.Name);

                    var prop = builder.DefineProperty(t, table.Name, true, false, false);

                    prop.getMethod.Body
                        .LdArg_0()
                        .LdFld(dtoPrivate)
                        .Call(dtoTableProp.Getter)
                        .NewObj(t.FindConstructor(dtoTableProp.PropertyType))
                        .Ret()
                        ;

                    TypeBody.SymbolTable.AddProperty(new Property(null, table.Name, t.ToAstType()))
                        .Connect(prop.prop);
                }

                var saveBuilder = builder.DefineMethod("Save", true, false, false);

                var astMethod = new Method(null, null, new ParameterList(), new GenericParameterList(),
                    new AttributeList(), saveBuilder.Name, saveBuilder.ReturnType.ToAstType());

                TypeBody.SymbolTable.AddMethod(astMethod)
                    .ConnectOverload(astMethod, saveBuilder);
            }

            EmitIDtoObject(builder, dtoPrivate);
        }

        private void EmitIDtoObject(RoslynTypeBuilder b, RoslynField dtoPrivate)
        {
            // var ts = b.Assembly.TypeSystem;
            // b.AddInterfaceImplementation(ts.FindType<IDtoObject>());
            //
            // var m = b.DefineMethod(nameof(IDtoObject.GetDto), true, false, true);
            // m.WithReturnType(ts.GetSystemBindings().Object);
            // var g = m.Generator;
            // g.LdArg_0()
            //     .LdFld(dtoPrivate)
            //     .Ret();
        }

        private void EmitBody(RoslynTypeBuilder builder, SqlDatabaseType dbType)
        {
            var set = ObjectType;
            var ts = builder.Assembly.TypeSystem;
            var sb = ts.GetSystemBindings();
            var dtoClassName = set.GetDtoType().Name;

            var @namespace = set.GetNamespace();

            var dtoType = ts.FindType($"{@namespace}.{dtoClassName}");

            var dtoPrivate = builder.FindField("_dto");

            if (CompilationMode.HasFlag(CompilationMode.Server))
            {
                var mrg = ts.FindType($"{@namespace}.{set.GetManagerType().Name}");
                var mrgGet = mrg.FindMethod("Get", sb.Guid);

                foreach (var prop in set.Properties)
                {
                    SharedGenerators.EmitObjectProperty(builder, prop, sb, dtoType, dtoPrivate, ts, mrgGet,
                        GetNamespace());
                }

                var saveBuilder = (RoslynMethodBuilder) builder.FindMethod("Save");

                var sg = saveBuilder.Body;

                sg
                    .LdArg_0()
                    .LdFld(dtoPrivate)
                    .Call(mrg.FindMethod("Save", dtoType))
                    ;
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