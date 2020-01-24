using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using dnlib.DotNet.Resources;
using ZenPlatform.Compiler;
using ZenPlatform.Compiler.Contracts;
using ZenPlatform.Compiler.Contracts.Symbols;
using ZenPlatform.Compiler.Generation;
using ZenPlatform.Configuration.Contracts;
using ZenPlatform.Configuration.Contracts.Data;
using ZenPlatform.Configuration.Contracts.TypeSystem;
using ZenPlatform.Configuration.Structure.Data.Types;
using ZenPlatform.Configuration.Structure.Data.Types.Complex;
using ZenPlatform.Configuration.Structure.Data.Types.Primitive;
using ZenPlatform.EntityComponent.Configuration;
using ZenPlatform.Language.Ast;
using ZenPlatform.Language.Ast.Definitions;
using ZenPlatform.Language.Ast.Definitions.Expressions;
using ZenPlatform.Language.Ast.Definitions.Functions;
using ZenPlatform.Language.Ast.Definitions.Statements;
using ZenPlatform.Language.Ast.Infrastructure;
using ZenPlatform.QueryBuilder;
using ZenPlatform.QueryBuilder.Model;
using ZenPlatform.QueryBuilder.Visitor;
using ZenPlatform.Shared.Tree;
using IType = ZenPlatform.Compiler.Contracts.IType;

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
        private Dictionary<XCSingleEntity, IType> _dtoCollections;
        private readonly IComponent _component;
        private GeneratorRules _rules;
        private EntityObjectDtoGenerator _egDto;
        private EntityObjectClassGenerator _egClass;
        private EntityLinkClassGenerator _egLink;
        private EntityManagerGenerator _egManager;

        public EntityPlatformGenerator(IComponent component)
        {
            _component = component;
            _rules = new GeneratorRules(component);
            _dtoCollections = new Dictionary<XCSingleEntity, IType>();

            _egDto = new EntityObjectDtoGenerator(component);
            _egClass = new EntityObjectClassGenerator(component);
            _egLink = new EntityLinkClassGenerator(component);
            _egManager = new EntityManagerGenerator(component);
        }

        private TypeSyntax GetAstFromPlatformType(IXCType pt)
        {
            return pt switch
            {
                XCBinary b => (TypeSyntax) new ArrayTypeSyntax(null,
                    new PrimitiveTypeSyntax(null, TypeNodeKind.Byte)),
                XCInt b => (TypeSyntax) new PrimitiveTypeSyntax(null, TypeNodeKind.Int),
                XCString b => (TypeSyntax) new PrimitiveTypeSyntax(null, TypeNodeKind.String),
                XCNumeric b => (TypeSyntax) new PrimitiveTypeSyntax(null, TypeNodeKind.Double),
                XCBoolean b => (TypeSyntax) new PrimitiveTypeSyntax(null, TypeNodeKind.Boolean),
                XCDateTime b => (TypeSyntax) new SingleTypeSyntax(null, nameof(DateTime), TypeNodeKind.Type),
                XCLinkTypeBase b => (TypeSyntax) new SingleTypeSyntax(null,
                    b.Parent.GetCodeRuleExpression(CodeGenRuleType.NamespaceRule) + "." + b.Name,
                    TypeNodeKind.Type),
                XCObjectTypeBase b => (TypeSyntax) new SingleTypeSyntax(null,
                    b.Parent.GetCodeRuleExpression(CodeGenRuleType.NamespaceRule) + "." + b.Name + "Link",
                    TypeNodeKind.Type),
                XCGuid b => (TypeSyntax) new SingleTypeSyntax(null, nameof(Guid), TypeNodeKind.Type),
            };
        }

        private void GenerateObjectClassUserModules(IXCObjectType type, ComponentClass cls)
        {
            foreach (var module in type.GetProgramModules())
            {
                if (module.ModuleRelationType == XCProgramModuleRelationType.Object)
                {
                    var typeBody = ParserHelper.ParseTypeBody(module.ModuleText);


                    foreach (var func in typeBody.Functions)
                    {
                        func.SymbolScope = SymbolScopeBySecurity.User;
                        cls.AddFunction(func);
                    }
                }
            }
        }

        private void GenerateLink(IXCLinkType type, Root root)
        {
            var cls = new ComponentClass(CompilationMode.Shared, _component, type, null, type.Name,
                new TypeBody(new List<Member>())) {Base = "Entity.EntityLink", Namespace = "Entity"};

            cls.Bag = ObjectType.Link;

            var cu = new CompilationUnit(null, new List<NamespaceBase>(), new List<TypeEntity>() {cls});
            root.Add(cu);
        }


        private void GenerateCommands(IXCObjectType type, Root root)
        {
            var set = type as XCSingleEntity ?? throw new ArgumentException(nameof(type));

            foreach (var command in type.GetCommands())
            {
                var typeBody = ParserHelper.ParseTypeBody(command.Module.ModuleText);
                var serverModule = new ComponentModule(CompilationMode.Server, _component, set, null,
                    $"__cmd_{command.Name}", typeBody) {Namespace = type.GetNamespace()};

                var clientModule = new ComponentModule(CompilationMode.Client, _component, set, null,
                    $"__cmd_{command.Name}", typeBody) {Namespace = type.GetNamespace()};


                foreach (var func in typeBody.Functions)
                {
                    func.SymbolScope = SymbolScopeBySecurity.User;
                }

                var cu = new CompilationUnit(null, new List<NamespaceBase>(),
                    new List<TypeEntity>() {serverModule, clientModule});

                root.Add(cu);
            }
        }

        /// <summary>
        ///  Генерация серверного кода
        /// </summary>
        /// <param name="type">Тип</param>
        /// <param name="root">Корень проекта</param>
        /// <param name="dbType"></param>
        public void StageServer(IXCObjectType type, Node root, SqlDatabaseType dbType)
        {
            var r = root as Root ?? throw new Exception("You must pass Root node to the generator");

            _egDto.GenerateAstTree(type, r);
            _egLink.GenerateAstTree(type.GetLink(), r);
            _egClass.GenerateAstTree(type, r);
            _egManager.GenerateAstTree(type, r);

            GenerateCommands(type, r);
        }

        /// <summary>
        /// Генерация клиентского кода
        /// </summary>
        /// <param name="type">Тип</param>
        /// <param name="node"></param>
        /// <param name="dbType"></param>
        public void StageClient(IXCObjectType type, Node node, SqlDatabaseType dbType)
        {
            if (node is Root root)
            {
                _egDto.GenerateAstTree(type, root);
                GenerateCommands(type, root);
                GenerateLink(type.GetLink(), root);
                GenerateClientObjectClass(type, root);
            }
        }

        private void GenerateClientObjectClass(IXCObjectType type, Root root)
        {
            var singleEntityType = type as XCSingleEntity ?? throw new InvalidOperationException(
                                       $"This component only can serve {nameof(XCSingleEntity)} objects");
            var className = type.Name;
            var dtoClassName =
                $"{_component.GetCodeRuleExpression(CodeGenRuleType.DtoPreffixRule)}{type.Name}{_component.GetCodeRuleExpression(CodeGenRuleType.DtoPostfixRule)}";

            var @namespace = _component.GetCodeRule(CodeGenRuleType.NamespaceRule).GetExpression();
            var intType = new PrimitiveTypeSyntax(null, TypeNodeKind.Int);
            List<Member> members = new List<Member>();

            var sessionType = new PrimitiveTypeSyntax(null, TypeNodeKind.Session);
            var dtoType = new SingleTypeSyntax(null, $"{@namespace}.{dtoClassName}", TypeNodeKind.Type);

            var sessionParameter = new Parameter(null, "session", sessionType
                , PassMethod.ByValue);

            var dtoParameter = new Parameter(null, "dto", dtoType
                , PassMethod.ByValue);

            var block = new Block(null,
                new[]
                    {
                        new Assignment(null, new Name(null, "session"), null, new Name(null, "_session")).ToStatement(),
                        new Assignment(null, new Name(null, "dto"), null, new Name(null, "_dto")).ToStatement()
                    }
                    .ToList());

            var constructor =
                new Constructor(null, block, new List<Parameter>() {sessionParameter, dtoParameter}, null, className);

            var field = new Field(null, "_dto", dtoType) {SymbolScope = SymbolScopeBySecurity.System};

            var fieldSession = new Field(null, "_session", sessionType) {SymbolScope = SymbolScopeBySecurity.System};

            members.Add(constructor);

            members.Add(field);
            members.Add(fieldSession);

            foreach (var prop in singleEntityType.Properties)
            {
                bool propertyGenerated = false;

                var propName = prop.Name;

                var propType = (prop.Types.Count > 1)
                    ? new PrimitiveTypeSyntax(null, TypeNodeKind.Object)
                    : GetAstFromPlatformType(prop.Types[0]);


                var astProp = new Property(null, propName, propType, true, !prop.IsReadOnly);

                members.Add(astProp);
                var get = new List<Statement>();
                var set = new List<Statement>();

                if (prop.Types.Count > 1)
                {
                    var typeField = prop.GetPropertySchemas(prop.Name)
                        .First(x => x.SchemaType == XCColumnSchemaType.Type);

                    var matchAtomList = new List<MatchAtom>();

                    var valExp = new Name(null, "value");

                    foreach (var ctype in prop.Types)
                    {
                        var typeLiteral = new Literal(null, ctype.Id.ToString(), intType);

                        var schema = prop.GetPropertySchemas(prop.Name)
                            .First(x => x.SchemaType == XCColumnSchemaType.Type);

                        var fieldExpression = new GetFieldExpression(new Name(null, "_dto"), schema.FullName);

                        var expr = new BinaryExpression(null,
                            typeLiteral
                            , fieldExpression
                            , BinaryOperatorType.Equal);


                        var schemaTyped = ctype switch
                        {
                            IXCLinkType obj => prop.GetPropertySchemas(prop.Name)
                                .First(x => x.SchemaType == XCColumnSchemaType.Ref),
                            _ => prop.GetPropertySchemas(prop.Name).First(x => x.PlatformType == ctype),
                        };

                        var feTypedProp = new GetFieldExpression(new Name(null, "_dto"), schemaTyped.FullName);

                        var ret = new Return(null, feTypedProp);

                        var @if = new If(null, null, ret.ToBlock(), expr);
                        get.Add(@if);


                        var afe = new AssignFieldExpression(null, new Name(null, "_dto"), schemaTyped.FullName);
                        var afe2 = new AssignFieldExpression(null, new Name(null, "_dto"), typeField.FullName);
                        var dtoAssignment = new Assignment(null, valExp, null, afe);
                        var typeAssignment = new Assignment(null, new Literal(null, ctype.Id.ToString(), intType), null,
                            afe2);
                        TypeSyntax matchAtomType = null;

                        if (ctype is XCPrimitiveType pt)
                        {
                            matchAtomType = GetAstFromPlatformType(pt);
                        }
                        else if (ctype is XCObjectTypeBase ot)
                        {
                            matchAtomType = new SingleTypeSyntax(null,
                                ot.Parent.GetCodeRuleExpression(CodeGenRuleType.NamespaceRule) + "." + ot.Name + "Link",
                                TypeNodeKind.Type);
                        }

                        var atomBlock =
                            new Block(new[] {dtoAssignment.ToStatement(), typeAssignment.ToStatement()}.ToList());

                        var matchAtom = new MatchAtom(null, atomBlock, matchAtomType);

                        matchAtomList.Add(matchAtom);
                    }

                    var match = new Match(null, matchAtomList, valExp);

                    get.Add(new Throw(null,
                            new Literal(null, "The type not found", new PrimitiveTypeSyntax(null, TypeNodeKind.String)))
                        .ToStatement());

                    set.Add(match);
                }
                else
                {
                    if (!prop.IsSelfLink)
                    {
                        var schema = prop.GetPropertySchemas(prop.Name)
                            .First(x => x.SchemaType == XCColumnSchemaType.NoSpecial);
                        var fieldExpression = new GetFieldExpression(new Name(null, "_dto"), schema.FullName);
                        var ret = new Return(null, fieldExpression);
                        get.Add(ret);
                    }
                    else
                    {
                        //TODO: Link gen
                    }
                }

                if (astProp.HasGetter)
                    astProp.Getter = new Block(get);

                if (astProp.HasSetter)
                    astProp.Setter = new Block(set);
            }

            //IReferenceImpl


            var tprop = new Property(null, "Type", intType, true, false) {IsInterface = true};
            tprop.Getter = new Block(new[]
            {
                (Statement) new Return(null, new Literal(null, singleEntityType.Id.ToString(), intType))
            }.ToList());
            //

            members.Add(tprop);

            var cls = new ComponentClass(CompilationMode.Server, _component, singleEntityType, null, className,
                new TypeBody(members));
            cls.Namespace = @namespace;

            GenerateObjectClassUserModules(type, cls);

            var cu = new CompilationUnit(null, new List<NamespaceBase>(), new List<TypeEntity>() {cls});
            //end create dto class
            root.Add(cu);
        }

        public void StageUI(IXCObjectType type, Node node)
        {
            throw new NotImplementedException();
        }

        public void StageGlobalVar(IGlobalVarManager manager)
        {
            var ts = manager.TypeSystem;

            var root = new GlobalVarTreeItem(VarTreeLeafType.Prop, CompilationMode.Shared, "Entity", (n, e) => { });

            foreach (var type in _component.MDTypes)
            {
                var mrgName = $"{type.GetNamespace()}.{type.GetManagerName()}";

                var mrg = ts.FindType(mrgName);

                var mrgLeaf = new GlobalVarTreeItem(VarTreeLeafType.Prop, CompilationMode.Shared, type.GetObjectName(),
                    (n, e) => { });

                mrgLeaf.Attach(root);

                var createMethod = new GlobalVarTreeItem(VarTreeLeafType.Func, CompilationMode.Shared,
                    "Create", (n, e) =>
                    {
                        var call = n as Call ?? throw new Exception("Can't emit function if it is not a call");
                        e.EmitCall(mrg.FindMethod("Create"), call.IsStatement);
                    });

                createMethod.Attach(mrgLeaf);
            }

            manager.Register(root);

            /*
             * $.Document.Invoice.Create();
             * $.SomeFunction()
             *
             * MyGM.StaticFunction()
             */
        }

        public void Stage0(Node astTree, ITypeBuilder builder, SqlDatabaseType dbType, CompilationMode mode)
        {
        }

        public void Stage1(Node astTree, ITypeBuilder builder, SqlDatabaseType dbType, CompilationMode mode)
        {
            if (astTree is ComponentAstBase cc)
            {
                if (cc.Bag != null && ((ObjectType) cc.Bag) == ObjectType.Dto)
                {
                    _egDto.Stage1(cc, builder, dbType, mode);
                }
                else if (cc.Bag != null && ((ObjectType) cc.Bag) == ObjectType.Object)
                {
                    _egClass.Stage1(cc, builder, dbType, mode);
                }
                else if (cc.Bag != null && ((ObjectType) cc.Bag) == ObjectType.Link)
                {
                    _egLink.Stage1(cc, builder, dbType, mode);
                }
                else if (cc.Bag != null && ((ObjectType) cc.Bag) == ObjectType.Manager)
                {
                    _egManager.Stage1(cc, builder, dbType, mode);
                }
            }
        }

        public void Stage2(Node astTree, ITypeBuilder builder, SqlDatabaseType dbType, CompilationMode mode)
        {
            if (astTree is ComponentAstBase cc && cc.Bag != null)
            {
                if ((ObjectType) cc.Bag == ObjectType.Dto)
                {
                    _egDto.Stage2(cc, builder, dbType, mode);
                }
                else if ((ObjectType) cc.Bag == ObjectType.Object)
                {
                    _egClass.Stage2(cc, builder, dbType, mode);
                }
                else if ((ObjectType) cc.Bag == ObjectType.Link)
                {
                    _egLink.Stage2(cc, builder, dbType, mode);
                }
                else if ((ObjectType) cc.Bag == ObjectType.Manager)
                {
                    _egManager.Stage2(cc, builder, dbType, mode);
                }
            }
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