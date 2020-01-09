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

namespace ZenPlatform.EntityComponent.Entity
{
    public enum ObjectType
    {
        Dto,
        Object,
        Link
    }

    public class EntityPlatformGenerator : IPlatformGenerator
    {
        private Dictionary<XCSingleEntity, IType> _dtoCollections;
        private readonly IXCComponent _component;
        private GeneratorRules _rules;
        private EntityObjectDtoGenerator _egDto;
        private EntityObjectClassGenerator _egClass;
        private EntityLinkClassGenerator _egLink;

        public EntityPlatformGenerator(IXCComponent component)
        {
            _component = component;
            _rules = new GeneratorRules(component);
            _dtoCollections = new Dictionary<XCSingleEntity, IType>();

            _egDto = new EntityObjectDtoGenerator(component);
            _egClass = new EntityObjectClassGenerator(component);
            _egLink = new EntityLinkClassGenerator(component);
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

        private void GenerateServerObjectClass(IXCObjectType type, Root root)
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

                        XCColumnSchemaDefinition schemaTyped;

                        if (ctype is IXCLinkType)
                        {
                            schemaTyped = prop.GetPropertySchemas(prop.Name)
                                .First(x => x.SchemaType == XCColumnSchemaType.Ref);
                        }
                        else
                        {
                            schemaTyped = prop.GetPropertySchemas(prop.Name).First(x => x.PlatformType == ctype);
                        }


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
                        else if (ctype is IXCLinkType ot)
                        {
                            matchAtomType = new SingleTypeSyntax(null,
                                ot.Parent.GetCodeRuleExpression(CodeGenRuleType.NamespaceRule) + "." + ot.Name,
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
                new TypeBody(new List<Member>())) {Base = "Documents.EntityLink", Namespace = "Documents"};

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
                    $"__cmd_{command.Name}", typeBody);

                var clientModule = new ComponentModule(CompilationMode.Client, _component, set, null,
                    $"__cmd_{command.Name}", typeBody);


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
            //GenerateServerObjectClass(type, r);
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
            manager.Register(new GlobalVarTreeItem(VarTreeLeafType.Prop, CompilationMode.Shared, "Test", (e) => { }));

            /*
             *
             * 
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
            if (astTree is ComponentClass cc)
            {
                if (cc.Bag != null && ((ObjectType) cc.Bag) == ObjectType.Dto)
                {
                    _egDto.EmitDetail(cc, builder, dbType, mode);
                }
                else if (cc.Bag != null && ((ObjectType) cc.Bag) == ObjectType.Object)
                {
                    _egClass.EmitDetail(cc, builder, dbType, mode);
                }
                else if (cc.Bag != null && ((ObjectType) cc.Bag) == ObjectType.Link)
                {
                    _egLink.EmitDetail(cc, builder, dbType, mode);
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

            var idBack = linkType.DefineField(b.Guid, СonventionsHelper.GetBackingFieldName("Id"), false, false);
            linkType.DefineProperty(b.Guid, "Id", idBack, true, false, true);

            var typeBack = linkType.DefineField(b.Int, СonventionsHelper.GetBackingFieldName("Type"), false, false);
            linkType.DefineProperty(b.Int, "Type", typeBack, true, false, true);

            var presentationBack = linkType.DefineField(b.String, СonventionsHelper.GetBackingFieldName("Presentation"),
                false, false);
            linkType.DefineProperty(b.String, "Presentation", presentationBack, true, false, true);

            var ctor = linkType.DefineConstructor(false);

            var e = ctor.Generator;

            e.LdArg_0()
                .EmitCall(b.Object.Constructors[0])
                .LdArg_0()
                .LdArg(1)
                .StFld(typeBack)
                .LdArg_0()
                .LdArg(2)
                .StFld(idBack)
                .Ret();
        }

        private void BuildVersionField(ITypeBuilder tb)
        {
            var _ts = tb.Assembly.TypeSystem;
            var _b = _ts.GetSystemBindings();
            var prop = tb.DefinePropertyWithBackingField(_b.Byte.MakeArrayType(), "Version", false);
        }

        private SSyntaxNode GetInsertQuery(XCSingleEntity se)
        {
            QueryMachine qm = new QueryMachine();
            qm.bg_query()
                .m_values();

            var pIndex = 0;

            var columns = se.Properties.Where(x => !x.IsSelfLink)
                .SelectMany(x => x.GetPropertySchemas());

            foreach (var column in columns)
            {
                qm.ld_param($"P{pIndex}");
                pIndex++;
            }

            qm.m_insert()
                .ld_table(se.RelTableName);

            foreach (var col in columns)
            {
                qm.ld_column(col.FullName);
            }

            qm.st_query();

            return (SSyntaxNode) qm.pop();
        }

        private SSyntaxNode GetUpdateQuery(XCSingleEntity se)
        {
            QueryMachine qm = new QueryMachine();

            var pIndex = 0;

            var columns = se.Properties.Where(x => !x.Unique).SelectMany(x => x.GetPropertySchemas());

            qm.bg_query()
                .m_where()
                .ld_column(se.GetPropertyByName("Id").DatabaseColumnName, "T0")
                .ld_param($"P_{pIndex++}")
                .eq();

            qm.m_set();
            Debug.Assert(columns.Any());

            foreach (var column in columns)
            {
                qm.ld_column(column.FullName, "T0")
                    .ld_param($"P_{pIndex++}")
                    .assign();
            }

            qm.m_update()
                .ld_table(se.RelTableName)
                .@as("T0")
                .st_query();

            return (SSyntaxNode) qm.pop();
        }

        private void EmitSavingSupport(ComponentClass cls, ITypeBuilder tb, SqlDatabaseType dbType)
        {
            var set = cls.Type as XCSingleEntity ??
                      throw new Exception($"This component can't serve this type {cls.Type}");

            var _ts = tb.Assembly.TypeSystem;
            var _bindings = _ts.GetSystemBindings();

            var compiler = SqlCompillerBase.FormEnum(dbType);

            tb.AddInterfaceImplementation(_ts.FindType<ICanSave>());

            var readerMethod = tb.DefineMethod(nameof(ICanSave.Save), true, false, true);

            var rg = readerMethod.Generator;

            var cmdType = _ts.FindType<DbCommand>();
            var parameterType = _ts.FindType<DbParameter>();

            var cmdParam =
                readerMethod.DefineParameter("cmd", cmdType, false, false);

            var indexp = 0;


            var p_loc = rg.DefineLocal(_ts.FindType<DbParameter>());

            var versionF = tb.Properties.First(x => x.Name == "Version");

            if (versionF != null)
            {
                var narg = rg.DefineLabel();
                var end = rg.DefineLabel();
                rg.LdArg_0()
                    .EmitCall(versionF.Getter)
                    .LdNull()
                    .Ceq()
                    .BrTrue(narg);
                //if Version != null
                rg
                    .LdArg(cmdParam.ArgIndex)
                    .LdStr(compiler.Compile(GetUpdateQuery(set)))
                    .EmitCall(cmdType.FindProperty(nameof(DbCommand.CommandText)).Setter)
                    .Br(end);
                //if Version == null
                rg
                    .MarkLabel(narg)
                    .LdArg(cmdParam.ArgIndex)
                    .LdStr(compiler.Compile(GetInsertQuery(set)))
                    .EmitCall(cmdType.FindProperty(nameof(DbCommand.CommandText)).Setter)
                    .MarkLabel(end);
            }

            foreach (var property in cls.TypeBody.Properties)
            {
                if (string.IsNullOrEmpty(property.MapTo)) continue;

                var prop = tb.FindProperty(property.Name);

                rg.LdArg(cmdParam.ArgIndex)
                    .EmitCall(cmdType.FindMethod(nameof(DbCommand.CreateParameter)))
                    .StLoc(p_loc)
                    .LdLoc(p_loc)
                    .LdStr($"P_{indexp}")
                    .EmitCall(parameterType.FindProperty(nameof(DbParameter.ParameterName)).Setter)
                    .LdLoc(p_loc)
                    .LdArg_0()
                    .EmitCall(prop.Getter)
                    .Box(prop.PropertyType)
                    .EmitCall(parameterType.FindProperty(nameof(DbParameter.Value)).Setter);

                indexp++;
            }

            rg.Ret();
        }

        private void EmitLink()
        {
        }
    }


    /*
     
     class EntityLink
     {
        StoreLink _store;
        double _sum;
        string _name;
        Guid _id;        
        bool _isLoaded;        
        
        ViewBagEntity _vb;
                
        public EntityLink(ViewBag vb)
        {
            //Required
            if(vb.HasName("Name"))
                _name = vb.Name;
            
            //Required
            if(vb.HasName("Id"))
                _id = vb.Id;
            else
                throw new Exception();
            
            if(vb.Has("Sum"))
                _sum = (double)vb.Sum;
                
            if(vb.Has("Store"))
                _store = StoreManager.GetLink(vb.Store);
                
        }   
        
        public string Name => _name;
        
        public EntityLink Link => this;
        
        public StoreLink Store => <k_platform_prefix>GetPropertyStore();
        
        public double Sum => <k_platform_prefix>GetPropertySum();
       
       public object CompositeProperty => 
        
        private object <k_platform_prefix>GetPropertyCompositeProperty()
        {
            if(_isLoaded)
                FetchFromServer();
        }
        
        private double <k_platform_prefix>GetPropertySum()
        {
            if(_isLoaded)
                FetchFromServer();
                
            return _sum;    
        }
        
        private StoreLink <k_platform_prefix>GetPropertyStore()
        {
            if(_isLoaded)
                FetchFromServer();
        
            _store ??= StoreManager.GetLink(Service.GetProperty(TypeId: 5, "Store", _id));
            return _store;
        }
        
        private void FetchFromServer()
        {
            //fetching base layer from server
            var props = Service.GetProperties(TypeId: 5, "Store", "Sum", _id);
            
            _store = StoreManager.GetLink(props["_store"]);
            _sum = (double)props["Sum"];
            _name = (string)props["Name"];
            ...     
            
            _isLoaded = true;
        }        
        
        public void Reload()
        {
            FetchFromServer();
        }
                
        public override ToString()
        {
            return Name;
        }      
     }
     
     */
}