using System;
using System.Linq;
using dnlib.DotNet;
using Aquila.Compiler.Contracts;
using Aquila.Compiler.Contracts.Extensions;
using Aquila.Compiler.Generation;
using Aquila.Compiler.Roslyn.RoslynBackend;
using Aquila.Configuration.Common.TypeSystem;
using Aquila.Core.Contracts;
using Aquila.Core.Contracts.Configuration;
using Aquila.Core.Contracts.Data;
using Aquila.Core.Contracts.TypeSystem;
using Aquila.EntityComponent.Compilation.UX;
using Aquila.EntityComponent.Configuration;
using Aquila.EntityComponent.Entity;
using Aquila.Language.Ast.Definitions;
using Aquila.QueryBuilder;
using Aquila.Shared.Tree;
using Root = Aquila.Language.Ast.Definitions.Root;
using TypeExtensions = Aquila.Compiler.Roslyn.TypeExtensions;
using TypeSystemExtensions = Aquila.Compiler.Roslyn.TypeSystemExtensions2;
using ConventionsHelper = Aquila.Compiler.Roslyn.ConventionsHelper;
using TypeAttributes = System.Reflection.TypeAttributes;

namespace Aquila.EntityComponent.Compilation
{
    public enum ObjectType
    {
        Dto,
        Object,
        Link,
        Manager
    }

    public class EntityPlatformGenerator : IPlatformGenerator
    {
        private readonly IComponent _component;
        private GeneratorRules _rules;


        public EntityPlatformGenerator(IComponent component)
        {
            _component = component;
            _rules = new GeneratorRules(component);
        }

        public RoslynTypeBuilder Stage0(RoslynAssemblyBuilder asm, Node task)
        {
            if (task is IEntityGenerationTask egt)
                return egt.Stage0(asm);
            else
                throw new Exception("Component doesn't support this task type");
        }

        public void Stage1(Node task, RoslynTypeBuilder builder, SqlDatabaseType dbType, CompilationMode mode,
            IEntryPointManager sm)
        {
            if (task is IEntityGenerationTask egt)
                egt.Stage1(builder, dbType, sm);
            else
                throw new Exception("Component doesn't support this task type");
        }

        public void Stage2(Node task, RoslynTypeBuilder builder, SqlDatabaseType dbType, CompilationMode mode)
        {
            if (task is IEntityGenerationTask egt)
                egt.Stage2(builder, dbType);
            else
                throw new Exception("Component doesn't support this task type");
        }

        /// <summary>
        ///  Генерация серверного кода
        /// </summary>
        /// <param name="ipType">Тип</param>
        /// <param name="root">Корень проекта</param>
        public void StageServer(IPType ipType, Node root)
        {
            var cu = new CompilationUnit(null, new UsingList(), new EntityList(),
                new NamespaceDeclarationList());

            var ns = new NamespaceDeclaration(null, "Entity",
                new UsingList(), new EntityList(), new NamespaceDeclarationList());

            cu.NamespaceDeclarations.Add(ns);

            var r = root as Root ?? throw new Exception("You must pass Root node to the generator");

            if (ipType.IsDto)
            {
                foreach (var table in ipType.Tables)
                {
                    ns.AddEntity(new DtoTableRowGenerationTask(ipType, table, CompilationMode.Server, _component,
                        table.Name, TypeBody.Empty));
                }

                ns.AddEntity(new DtoGenerationTask(ipType, CompilationMode.Server, _component, ipType.Name,
                    TypeBody.Empty));
            }

            if (ipType.IsManager)
                ns.AddEntity(new ManagerGenerationTask(ipType, CompilationMode.Server, _component, ipType.Name,
                    TypeBody.Empty));

            if (ipType.IsLink)
            {
                foreach (var table in ipType.Tables)
                {
                    ns.AddEntity(new LinkTableRowGenerationTask(ipType, table, CompilationMode.Server, _component,
                        table.Name, TypeBody.Empty));
                }

                ns.AddEntity(new LinkGenerationTask(ipType, CompilationMode.Server, _component, ipType.Name,
                    TypeBody.Empty));
            }

            if (ipType.IsObject)
            {
                foreach (var table in ipType.Tables)
                {
                    ns.AddEntity(new ObjectTableRowGenerationTask(ipType, table, CompilationMode.Server, _component,
                        table.Name, TypeBody.Empty));

                    ns.AddEntity(new ObjectTableCollectionGenerationTask(ipType, table, CompilationMode.Server,
                        _component,
                        table.Name + "Object", TypeBody.Empty));
                }

                ns.AddEntity(new ObjectGenerationTask(ipType, CompilationMode.Server, _component, ipType.Name,
                    TypeBody.Empty));

                var md = ipType.GetMD<MDEntity>();

                foreach (var cmd in md.Commands)
                {
                    ns.AddEntity(
                        new CommandGenerationTask(cmd, CompilationMode.Server, _component, $"__cmd_{cmd.Name}"));
                }

                foreach (var inf in md.Interfaces)
                {
                    ns.AddEntity(new UXFormGenerationTask(ipType, inf, CompilationMode.Server, _component, true,
                        inf.Name,
                        TypeBody.Empty));
                    ns.AddEntity(new FormStaticActionsGenerationTask(ipType, inf, CompilationMode.Server, _component,
                        true, inf.Name,
                        TypeBody.Empty));
                }
            }

            r.Units.Add(cu);
        }

        /// <summary>
        /// Генерация клиентского кода
        /// </summary>
        /// <param name="ipType">Тип</param>
        /// <param name="node"></param>
        /// <param name="dbType"></param>
        public void StageClient(IPType ipType, Node root)
        {
            var cu = new CompilationUnit(null, new UsingList(), new EntityList(),
                new NamespaceDeclarationList());

            var ns = new NamespaceDeclaration(null, "Entity",
                new UsingList(), new EntityList(), new NamespaceDeclarationList());

            cu.NamespaceDeclarations.Add(ns);

            var r = root as Root ?? throw new Exception("You must pass Root node to the generator");

            if (ipType.IsDto)
            {
                foreach (var table in ipType.Tables)
                {
                    ns.AddEntity(new DtoTableRowGenerationTask(ipType, table, CompilationMode.Client, _component,
                        table.Name, TypeBody.Empty));
                }

                ns.AddEntity(new DtoGenerationTask(ipType, CompilationMode.Client, _component, ipType.Name,
                    TypeBody.Empty));
            }

            if (ipType.IsObject)
            {
                ns.AddEntity(new ObjectGenerationTask(ipType, CompilationMode.Client, _component, ipType.Name,
                    TypeBody.Empty));

                var md = ipType.GetMD<MDEntity>();

                foreach (var cmd in md.Commands)
                {
                    ns.AddEntity(
                        new CommandGenerationTask(cmd, CompilationMode.Client, _component, $"__cmd_{cmd.Name}"));
                }

                foreach (var inf in md.Interfaces)
                {
                    ns.AddEntity(new UXFormClientGenerationTask(ipType, inf, CompilationMode.Client, _component, true,
                        inf.Name,
                        TypeBody.Empty));
                }
            }

            r.Units.Add(cu);
        }

        public void StageUI(IPType ipType, Node node)
        {
            throw new NotImplementedException();
        }

        public void StageGlobalVar(IGlobalVarManager manager)
        {
            var ts = manager.TypeSystem;

            var root = new GlobalVarTreeItem(VarTreeLeafType.Prop, CompilationMode.Shared, "Entity", (n, e) => { });

            foreach (var type in _component.GetTypes().Where(x => x.IsManager))
            {
                var mrgName = $"{type.GetNamespace()}.{type.Name}";
                var objectName = $"{type.GetNamespace()}.{type.GetObjectType().Name}";
                var mrg = ts.FindType(mrgName);

                var mrgLeaf = new GlobalVarTreeItem(VarTreeLeafType.Prop, CompilationMode.Shared,
                    type.GetObjectType().Name,
                    (n, e) => { });

                root.Attach(mrgLeaf);

                var createMethod = new GlobalVarTreeItem(VarTreeLeafType.Func, CompilationMode.Shared,
                    "Create", (n, e) =>
                    {
                        var call = n as Call ?? throw new Exception("Can't emit function if it is not a call");
                        e.Call(TypeExtensions.FindMethod(mrg, "Create"));
                    }, new SingleTypeSyntax(null, objectName, TypeNodeKind.Object));

                mrgLeaf.Attach(createMethod);
            }

            manager.Register(root);

            StageGlobalVarUX(manager);
        }

        public void StageGlobalVarUX(IGlobalVarManager manager)
        {
            var ts = manager.TypeSystem;

            var root = new GlobalVarTreeItem(VarTreeLeafType.Prop, CompilationMode.Shared, "UX", (n, e) => { });

            foreach (var type in _component.GetTypes().Where(x => x.IsManager))
            {
                var mrgName = $"{type.GetNamespace()}.{type.Name}";

                var mrg = ts.FindType(mrgName);

                var mrgLeaf = new GlobalVarTreeItem(VarTreeLeafType.Prop, CompilationMode.Shared,
                    type.GetObjectType().Name,
                    (n, e) => { });

                root.Attach(mrgLeaf);

                var getMethod = new GlobalVarTreeItem(VarTreeLeafType.Func, CompilationMode.Shared,
                    "Get", (n, e) =>
                    {
                        var call = n as Call ?? throw new Exception("Can't emit function if it is not a call");
                        //need add some constant values

                        //e.EmitCall(mrg.FindMethod("Create"), call.IsStatement);
                    });

                mrgLeaf.Attach(getMethod);
            }

            manager.Register(root);

            /*
             * $.Document.Invoice.Create();
             * $.SomeFunction()
             *
             * MyGM.StaticFunction()
             */
        }


        public void StageInfrastructure(RoslynAssemblyBuilder builder, SqlDatabaseType dbType, CompilationMode mode)
        {
            CreateMainLink(builder);
        }

        private void CreateEntryPoint(RoslynAssemblyBuilder b)
        {
            var ep = TypeSystemExtensions.DefineStaticType(b, "", "EntryPoint");
            var run = ep.DefineMethod("Run", true, true, false);

            // run.Generator.Ret();
        }

        private void CreateMainLink(RoslynAssemblyBuilder builder)
        {
            var ts = builder.TypeSystem;
            var b = ts.GetSystemBindings();

            var linkType = builder.DefineType(_component.GetCodeRuleExpression(CodeGenRuleType.NamespaceRule),
                "EntityLink",
                TypeAttributes.Class | TypeAttributes.AnsiClass | TypeAttributes.BeforeFieldInit |
                TypeAttributes.Public,
                b.Object);

            linkType.AddInterfaceImplementation(ts.FindType<ILink>());

            var idBack = linkType.DefineField(b.Guid, ConventionsHelper.GetBackingFieldName("i"), false, false);

            TypeExtensions.DefineProperty(linkType, b.Guid, nameof(ILink.Id), idBack, true, false, true);

            var typeBack = linkType.DefineField(b.Int, ConventionsHelper.GetBackingFieldName("t"), false, false);
            TypeExtensions.DefineProperty(linkType, b.Int, nameof(ILink.TypeId), typeBack, true, false, true);

            var presentationBack = linkType.DefineField(b.String, ConventionsHelper.GetBackingFieldName("p"),
                false, false);
            TypeExtensions.DefineProperty(linkType, b.String, nameof(ILink.Presentation), presentationBack, true, false,
                true);

            var ctor = linkType.DefineConstructor(false);

            var p1 = ctor.DefineParameter(b.Guid);
            var p2 = ctor.DefineParameter(b.Int);
            var p3 = ctor.DefineParameter(b.String);


            ctor.Body
                .LdArg_0()
                .LdArg(p1)
                .StFld(idBack)
                .LdArg_0()
                .LdArg(p2)
                .StFld(typeBack)
                .LdArg_0()
                .LdArg(p3)
                .StFld(presentationBack);
        }
    }
}