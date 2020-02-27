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
using ZenPlatform.Language.Ast.Symbols;
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
                var propName = prop.Name;

                var propType = (prop.Types.Count() > 1)
                    ? sb.Object
                    : prop.Types.First().ConvertType(sb);

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
                prop.getMethod.Generator
                    .LdArg_0()
                    .LdFld(dtoPrivate)
                    .EmitCall(dtoTableProp.Getter)
                    .NewObj(t.FindConstructor(dtoTableProp.PropertyType))
                    .Ret();

                TypeBody.SymbolTable.AddProperty(new Property(null, table.Name, t.ToAstType()))
                    .Connect(prop.prop);
            }

            var saveBuilder = builder.DefineMethod("Save", true, false, false);

            var astMethod = new Function(null, null, null, null, null, saveBuilder.Name,
                saveBuilder.ReturnType.ToAstType());

            TypeBody.SymbolTable.AddMethod(astMethod)
                .ConnectOverload(astMethod, saveBuilder);
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
                SharedGenerators.EmitObjectProperty(builder, prop, sb, dtoType, dtoPrivate, ts, mrgGet, GetNamespace());
            }


            var saveBuilder = (IMethodBuilder) builder.FindMethod("Save");

            var sg = saveBuilder.Generator;

            sg
                .LdArg_0()
                .LdFld(dtoPrivate)
                .EmitCall(mrg.FindMethod("Save", dtoType))
                .Ret();
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