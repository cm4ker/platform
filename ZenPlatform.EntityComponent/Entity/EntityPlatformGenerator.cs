using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using ZenPlatform.Compiler;
using ZenPlatform.Compiler.Contracts;
using ZenPlatform.Compiler.Contracts.Symbols;
using ZenPlatform.Compiler.Generation;
using ZenPlatform.Configuration.Contracts;
using ZenPlatform.Configuration.Contracts.Data;
using ZenPlatform.Configuration.Contracts.TypeSystem;
using ZenPlatform.EntityComponent.Configuration;
using ZenPlatform.EntityComponent.Entity.Generation;
using ZenPlatform.Language.Ast;
using ZenPlatform.Language.Ast.Definitions;
using ZenPlatform.QueryBuilder;
using ZenPlatform.Shared.Tree;
using IType = ZenPlatform.Compiler.Contracts.IType;
using Root = ZenPlatform.Language.Ast.Definitions.Root;

namespace ZenPlatform.EntityComponent.Entity
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

        private void GenerateLink(IPType ipType, Root root)
        {
            // var cls = new ComponentClass(CompilationMode.Shared, _component, type, null, type.Name,
            //     new TypeBody(new List<Member>())) {Base = "Entity.EntityLink", Namespace = "Entity"};
            //
            // cls.Bag = ObjectType.Link;
            //
            // var cu = new CompilationUnit(null, new List<NamespaceBase>(), new List<TypeEntity>() {cls});
            // root.Add(cu);
        }

        public ITypeBuilder Stage0(IAssemblyBuilder asm, Node task)
        {
            if (task is IEntityGenerationTask egt)
                return egt.Stage0(asm);
            else
                throw new Exception("Component doesn't support this task type");
        }


        public void Stage1(Node task, ITypeBuilder builder, SqlDatabaseType dbType, CompilationMode mode)
        {
            if (task is IEntityGenerationTask egt)
                egt.Stage1(builder, dbType);
            else
                throw new Exception("Component doesn't support this task type");
        }

        public void Stage2(Node task, ITypeBuilder builder, SqlDatabaseType dbType, CompilationMode mode)
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
                        table.Name+"Object", TypeBody.Empty));
                }

                ns.AddEntity(new ObjectGenerationTask(ipType, CompilationMode.Server, _component, ipType.Name,
                    TypeBody.Empty));

                var md = ipType.GetMD<MDEntity>();

                foreach (var cmd in md.Commands)
                {
                    ns.AddEntity(
                        new CommandGenerationTask(cmd, CompilationMode.Server, _component, $"__cmd_{cmd.Name}"));
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
        public void StageClient(IPType ipType, Node node)
        {
            // if (node is Root root)
            // {
            //     _egDto.GenerateAstTree(type, root);
            //     GenerateCommands(type, root);
            //     GenerateLink(type.GetLink(), root);
            //     GenerateClientObjectClass(type, root);
            // }
        }

        private void GenerateClientObjectClass(ZenPlatform.Configuration.Contracts.TypeSystem.IPType ipType, Root root)
        {
            // var singleEntityType = type as XCSingleEntity ?? throw new InvalidOperationException(
            //                            $"This component only can serve {nameof(XCSingleEntity)} objects");
            // var className = type.Name;
            // var dtoClassName =
            //     $"{_component.GetCodeRuleExpression(CodeGenRuleType.DtoPreffixRule)}{type.Name}{_component.GetCodeRuleExpression(CodeGenRuleType.DtoPostfixRule)}";
            //
            // var @namespace = _component.GetCodeRule(CodeGenRuleType.NamespaceRule).GetExpression();
            // var intType = new PrimitiveTypeSyntax(null, TypeNodeKind.Int);
            // List<Member> members = new List<Member>();
            //
            // var sessionType = new PrimitiveTypeSyntax(null, TypeNodeKind.Session);
            // var dtoType = new SingleTypeSyntax(null, $"{@namespace}.{dtoClassName}", TypeNodeKind.Type);
            //
            // var sessionParameter = new Parameter(null, "session", sessionType
            //     , PassMethod.ByValue);
            //
            // var dtoParameter = new Parameter(null, "dto", dtoType
            //     , PassMethod.ByValue);
            //
            // var block = new Block(null,
            //     new[]
            //         {
            //             new Assignment(null, new Name(null, "session"), null, new Name(null, "_session")).ToStatement(),
            //             new Assignment(null, new Name(null, "dto"), null, new Name(null, "_dto")).ToStatement()
            //         }
            //         .ToList());
            //
            // var constructor =
            //     new Constructor(null, block, new List<Parameter>() {sessionParameter, dtoParameter}, null, className);
            //
            // var field = new Field(null, "_dto", dtoType) {SymbolScope = SymbolScopeBySecurity.System};
            //
            // var fieldSession = new Field(null, "_session", sessionType) {SymbolScope = SymbolScopeBySecurity.System};
            //
            // members.Add(constructor);
            //
            // members.Add(field);
            // members.Add(fieldSession);
            //
            // foreach (var prop in singleEntityType.Properties)
            // {
            //     bool propertyGenerated = false;
            //
            //     var propName = prop.Name;
            //
            //     var propType = (prop.Types.Count > 1)
            //         ? new PrimitiveTypeSyntax(null, TypeNodeKind.Object)
            //         : GetAstFromPlatformType(prop.Types[0]);
            //
            //
            //     var astProp = new Property(null, propName, propType, true, !prop.IsReadOnly);
            //
            //     members.Add(astProp);
            //     var get = new List<Statement>();
            //     var set = new List<Statement>();
            //
            //     if (prop.Types.Count > 1)
            //     {
            //         var typeField = prop.GetPropertySchemas(prop.Name)
            //             .First(x => x.SchemaType == XCColumnSchemaType.Type);
            //
            //         var matchAtomList = new List<MatchAtom>();
            //
            //         var valExp = new Name(null, "value");
            //
            //         foreach (var ctype in prop.Types)
            //         {
            //             var typeLiteral = new Literal(null, ctype.Id.ToString(), intType);
            //
            //             var schema = prop.GetPropertySchemas(prop.Name)
            //                 .First(x => x.SchemaType == XCColumnSchemaType.Type);
            //
            //             var fieldExpression = new GetFieldExpression(new Name(null, "_dto"), schema.FullName);
            //
            //             var expr = new BinaryExpression(null,
            //                 typeLiteral
            //                 , fieldExpression
            //                 , BinaryOperatorType.Equal);
            //
            //
            //             var schemaTyped = ctype switch
            //             {
            //                 IXCLinkType obj => prop.GetPropertySchemas(prop.Name)
            //                     .First(x => x.SchemaType == XCColumnSchemaType.Ref),
            //                 _ => prop.GetPropertySchemas(prop.Name).First(x => x.PlatformType == ctype),
            //             };
            //
            //             var feTypedProp = new GetFieldExpression(new Name(null, "_dto"), schemaTyped.FullName);
            //
            //             var ret = new Return(null, feTypedProp);
            //
            //             var @if = new If(null, null, ret.ToBlock(), expr);
            //             get.Add(@if);
            //
            //
            //             var afe = new AssignFieldExpression(null, new Name(null, "_dto"), schemaTyped.FullName);
            //             var afe2 = new AssignFieldExpression(null, new Name(null, "_dto"), typeField.FullName);
            //             var dtoAssignment = new Assignment(null, valExp, null, afe);
            //             var typeAssignment = new Assignment(null, new Literal(null, ctype.Id.ToString(), intType), null,
            //                 afe2);
            //             TypeSyntax matchAtomType = null;
            //
            //             if (ctype is XCPrimitiveType pt)
            //             {
            //                 matchAtomType = GetAstFromPlatformType(pt);
            //             }
            //             else if (ctype is XCObjectTypeBase ot)
            //             {
            //                 matchAtomType = new SingleTypeSyntax(null,
            //                     ot.Parent.GetCodeRuleExpression(CodeGenRuleType.NamespaceRule) + "." + ot.Name + "Link",
            //                     TypeNodeKind.Type);
            //             }
            //
            //             var atomBlock =
            //                 new Block(new[] {dtoAssignment.ToStatement(), typeAssignment.ToStatement()}.ToList());
            //
            //             var matchAtom = new MatchAtom(null, atomBlock, matchAtomType);
            //
            //             matchAtomList.Add(matchAtom);
            //         }
            //
            //         var match = new Match(null, matchAtomList, valExp);
            //
            //         get.Add(new Throw(null,
            //                 new Literal(null, "The type not found", new PrimitiveTypeSyntax(null, TypeNodeKind.String)))
            //             .ToStatement());
            //
            //         set.Add(match);
            //     }
            //     else
            //     {
            //         if (!prop.IsSelfLink)
            //         {
            //             var schema = prop.GetPropertySchemas(prop.Name)
            //                 .First(x => x.SchemaType == XCColumnSchemaType.NoSpecial);
            //             var fieldExpression = new GetFieldExpression(new Name(null, "_dto"), schema.FullName);
            //             var ret = new Return(null, fieldExpression);
            //             get.Add(ret);
            //         }
            //         else
            //         {
            //             //TODO: Link gen
            //         }
            //     }
            //
            //     if (astProp.HasGetter)
            //         astProp.Getter = new Block(get);
            //
            //     if (astProp.HasSetter)
            //         astProp.Setter = new Block(set);
            // }
            //
            // //IReferenceImpl
            //
            //
            // var tprop = new Property(null, "Type", intType, true, false) {IsInterface = true};
            // tprop.Getter = new Block(new[]
            // {
            //     (Statement) new Return(null, new Literal(null, singleEntityType.Id.ToString(), intType))
            // }.ToList());
            // //
            //
            // members.Add(tprop);
            //
            // var cls = new ComponentClass(CompilationMode.Server, _component, singleEntityType, null, className,
            //     new TypeBody(members));
            // cls.Namespace = @namespace;
            //
            // GenerateObjectClassUserModules(type, cls);
            //
            // var cu = new CompilationUnit(null, new List<NamespaceBase>(), new List<TypeEntity>() {cls});
            // //end create dto class
            // root.Add(cu);
        }

        public void StageUI(ZenPlatform.Configuration.Contracts.TypeSystem.IPType ipType, Node node)
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

                var mrg = ts.FindType(mrgName);

                var mrgLeaf = new GlobalVarTreeItem(VarTreeLeafType.Prop, CompilationMode.Shared,
                    type.GetObjectType().Name,
                    (n, e) => { });

                root.Attach(mrgLeaf);

                var createMethod = new GlobalVarTreeItem(VarTreeLeafType.Func, CompilationMode.Shared,
                    "Create", (n, e) =>
                    {
                        var call = n as Call ?? throw new Exception("Can't emit function if it is not a call");
                        e.EmitCall(mrg.FindMethod("Create"), call.IsStatement);
                    });

                mrgLeaf.Attach(createMethod);
            }

            manager.Register(root);

            /*
             * $.Document.Invoice.Create();
             * $.SomeFunction()
             *
             * MyGM.StaticFunction()
             */
        }

        public void StageInfrastructure(IAssemblyBuilder builder, SqlDatabaseType dbType, CompilationMode mode)
        {
            CreateMainLink(builder);
        }

        private void CreateMainLink(IAssemblyBuilder builder)
        {
            var ts = builder.TypeSystem;
            var b = ts.GetSystemBindings();

            var linkType = builder.DefineType(_component.GetCodeRuleExpression(CodeGenRuleType.NamespaceRule),
                "EntityLink",
                TypeAttributes.Class | TypeAttributes.AnsiClass | TypeAttributes.BeforeFieldInit,
                b.Object);

            linkType.AddInterfaceImplementation(ts.FindType<ILink>());

            var idBack = linkType.DefineField(b.Guid, ConventionsHelper.GetBackingFieldName("Id"), false, false);
            linkType.DefineProperty(b.Guid, "Id", idBack, true, false, true);

            var typeBack = linkType.DefineField(b.Int, ConventionsHelper.GetBackingFieldName("Type"), false, false);
            linkType.DefineProperty(b.Int, "Type", typeBack, true, false, true);

            var presentationBack = linkType.DefineField(b.String, ConventionsHelper.GetBackingFieldName("Presentation"),
                false, false);
            linkType.DefineProperty(b.String, "Presentation", presentationBack, true, false, true);

            var ctor = linkType.DefineConstructor(false);

            var e = ctor.Generator;

            e.LdArg_0()
                .EmitCall(b.Object.Constructors[0])
                .Ret();
        }
    }
}