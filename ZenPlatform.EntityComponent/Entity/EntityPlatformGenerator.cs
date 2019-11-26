using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Reflection;
using ZenPlatform.Compiler;
using ZenPlatform.Compiler.Contracts;
using ZenPlatform.Compiler.Contracts.Symbols;
using ZenPlatform.Compiler.Generation;
using ZenPlatform.Configuration.Compiler;
using ZenPlatform.Configuration.Contracts;
using ZenPlatform.Configuration.Contracts.Data;
using ZenPlatform.Configuration.Structure;
using ZenPlatform.Configuration.Structure.Data;
using ZenPlatform.Configuration.Structure.Data.Types;
using ZenPlatform.Configuration.Structure.Data.Types.Complex;
using ZenPlatform.Configuration.Structure.Data.Types.Primitive;
using ZenPlatform.Core.Querying;
using ZenPlatform.EntityComponent.Configuration;
using ZenPlatform.Language.Ast;
using ZenPlatform.Language.Ast.Definitions;
using ZenPlatform.Language.Ast.Definitions.Expressions;
using ZenPlatform.Language.Ast.Definitions.Functions;
using ZenPlatform.Language.Ast.Definitions.Statements;
using ZenPlatform.Language.Ast.Infrastructure;
using ZenPlatform.QueryBuilder;
using ZenPlatform.QueryBuilder.Visitor;
using ZenPlatform.Shared.Tree;
using ZenPlatform.UI.Ast;


namespace ZenPlatform.EntityComponent.Entity
{
    public class EntityPlatformGenerator : IPlatformGenerator
    {
        private Dictionary<XCSingleEntity, IType> _dtoCollections;
        private readonly IXCComponent _component;
        private GeneratorRules _rules;

        public EntityPlatformGenerator(IXCComponent component)
        {
            _component = component;
            _rules = new GeneratorRules(component);
            _dtoCollections = new Dictionary<XCSingleEntity, IType>();
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
                XCObjectTypeBase b => (TypeSyntax) new SingleTypeSyntax(null,
                    b.Parent.GetCodeRuleExpression(CodeGenRuleType.NamespaceRule) + "." + b.Name, TypeNodeKind.Type),
                XCGuid b => (TypeSyntax) new SingleTypeSyntax(null, nameof(Guid), TypeNodeKind.Type),
            };
        }

        private void GenerateServerDtoClass(IXCObjectType type, Root root)
        {
            var set = type as XCSingleEntity ?? throw new InvalidOperationException(
                          $"This component only can serve {nameof(XCSingleEntity)} objects");
            var dtoClassName =
                $"{_component.GetCodeRuleExpression(CodeGenRuleType.DtoPreffixRule)}{type.Name}{_component.GetCodeRuleExpression(CodeGenRuleType.DtoPostfixRule)}";

            var @namespace = _component.GetCodeRule(CodeGenRuleType.NamespaceRule).GetExpression();

            List<Member> members = new List<Member>();

            //Create dto class
            foreach (var prop in set.Properties)
            {
                bool propertyGenerated = false;

                if (string.IsNullOrEmpty(prop.DatabaseColumnName))
                {
                    throw new Exception(
                        $"Prop: {prop.Name} ObjectType: {typeof(XCSingleEntity)} Name: {set.Name}. Database column is empty!");
                }

                if (prop.Types.Count > 1)
                {
                    {
                        var dbColName = prop.GetPropertySchemas(prop.DatabaseColumnName)
                            .First(x => x.SchemaType == XCColumnSchemaType.Type).FullName;

                        var astProp = new Property(null, prop.Name + "_Type",
                            new PrimitiveTypeSyntax(null, TypeNodeKind.Int), true, true, dbColName);

                        members.Add(astProp);
                    }
                }


                foreach (var ctype in prop.Types)
                {
                    if (ctype is XCPrimitiveType pt)
                    {
                        var dbColName = prop
                            .GetPropertySchemas(prop.DatabaseColumnName)
                            .First(x => x.SchemaType == ((prop.Types.Count > 1)
                                            ? XCColumnSchemaType.Value
                                            : XCColumnSchemaType.NoSpecial) && x.PlatformType == pt).FullName;

                        var propName = prop
                            .GetPropertySchemas(prop.Name)
                            .First(x => x.SchemaType == ((prop.Types.Count > 1)
                                            ? XCColumnSchemaType.Value
                                            : XCColumnSchemaType.NoSpecial) && x.PlatformType == pt).FullName;

                        TypeSyntax propType = GetAstFromPlatformType(pt);

                        var astProp = new Property(null, propName, propType, true, true, dbColName);
                        members.Add(astProp);
                    }
                    else if (ctype is XCObjectTypeBase ot)
                    {
                        if (!propertyGenerated)
                        {
                            propertyGenerated = true;

                            var dbColName = prop
                                .GetPropertySchemas(prop.DatabaseColumnName)
                                .First(x => x.SchemaType == ((prop.Types.Count > 1)
                                                ? XCColumnSchemaType.Ref
                                                : XCColumnSchemaType.NoSpecial) /* && x.PlatformType == ot*/).FullName;

                            var propName = prop
                                .GetPropertySchemas(prop.Name)
                                .First(x => x.SchemaType == ((prop.Types.Count > 1)
                                                ? XCColumnSchemaType.Ref
                                                : XCColumnSchemaType.NoSpecial) /* && x.PlatformType == ot*/).FullName;

                            var astProp = new Property(null, propName,
                                new SingleTypeSyntax(null, nameof(Guid), TypeNodeKind.Type), true, true, dbColName);
                            members.Add(astProp);
                        }
                    }
                }
            }

            var cls = new ComponentClass(CompilationMode.Server, _component, set, null, dtoClassName,
                new TypeBody(members));

            cls.Namespace = @namespace;

            var cu = new CompilationUnit(null, new List<NamespaceBase>(), new List<TypeEntity>() {cls});
            //end create dto class
            root.Add(cu);
        }

        private void GenerateClientDtoClass(IXCObjectType type, Root root)
        {
            var set = type as XCSingleEntity ?? throw new InvalidOperationException(
                          $"This component only can serve {nameof(XCSingleEntity)} objects");
            var dtoClassName =
                $"{_component.GetCodeRuleExpression(CodeGenRuleType.DtoPreffixRule)}{type.Name}{_component.GetCodeRuleExpression(CodeGenRuleType.DtoPostfixRule)}";

            var @namespace = _component.GetCodeRule(CodeGenRuleType.NamespaceRule).GetExpression();

            List<Member> members = new List<Member>();

            //Create dto class
            foreach (var prop in set.Properties)
            {
                bool propertyGenerated = false;

                if (string.IsNullOrEmpty(prop.DatabaseColumnName))
                {
                    throw new Exception(
                        $"Prop: {prop.Name} ObjectType: {typeof(XCSingleEntity)} Name: {set.Name}. Database column is empty!");
                }

                if (prop.Types.Count > 1)
                {
                    {
                        var dbColName = prop.GetPropertySchemas(prop.DatabaseColumnName)
                            .First(x => x.SchemaType == XCColumnSchemaType.Type).FullName;

                        var astProp = new Property(null, prop.Name + "_Type",
                            new PrimitiveTypeSyntax(null, TypeNodeKind.Int), true, true);

                        members.Add(astProp);
                    }
                }

                foreach (var ctype in prop.Types)
                {
                    if (ctype is XCPrimitiveType pt)
                    {
                        var propName = prop
                            .GetPropertySchemas(prop.Name)
                            .First(x => x.SchemaType == ((prop.Types.Count > 1)
                                            ? XCColumnSchemaType.Value
                                            : XCColumnSchemaType.NoSpecial) && x.PlatformType == pt).FullName;

                        TypeSyntax propType = GetAstFromPlatformType(pt);

                        var astProp = new Property(null, propName, propType, true, true);
                        members.Add(astProp);
                    }
                    else if (ctype is XCObjectTypeBase ot)
                    {
                        if (!propertyGenerated)
                        {
                            propertyGenerated = true;


                            var dbColName = prop
                                .GetPropertySchemas(prop.DatabaseColumnName)
                                .First(x => x.SchemaType == ((prop.Types.Count > 1)
                                                ? XCColumnSchemaType.Ref
                                                : XCColumnSchemaType.NoSpecial) && (prop.Types.Count > 1)
                                    ? x.PlatformType == PlatformTypesFactory.Guid
                                    : x.PlatformType == ot).FullName;

                            var propName = prop
                                .GetPropertySchemas(prop.Name)
                                .First(x => x.SchemaType == ((prop.Types.Count > 1)
                                                ? XCColumnSchemaType.Ref
                                                : XCColumnSchemaType.NoSpecial) && (prop.Types.Count > 1)
                                    ? x.PlatformType == PlatformTypesFactory.Guid
                                    : x.PlatformType == ot).FullName;

                            var astProp = new Property(null, propName,
                                new SingleTypeSyntax(null, nameof(Guid), TypeNodeKind.Type), true, true, dbColName);
                            members.Add(astProp);
                        }
                    }
                }
            }

            var cls = new ComponentClass(CompilationMode.Client, _component, set, null, dtoClassName,
                new TypeBody(members));

            cls.Namespace = @namespace;

            var cu = new CompilationUnit(null, new List<NamespaceBase>(), new List<TypeEntity>() {cls});

            //end create dto class
            root.Add(cu);
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


                        var schemaTyped = ctype switch
                        {
                            XCObjectTypeBase obj => prop.GetPropertySchemas(prop.Name)
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
                    var schema = prop.GetPropertySchemas(prop.Name)
                        .First(x => x.SchemaType == XCColumnSchemaType.NoSpecial);
                    var fieldExpression = new GetFieldExpression(new Name(null, "_dto"), schema.FullName);
                    var ret = new Return(null, fieldExpression);
                    get.Add(ret);
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

        private void EmitMappingSupport(ComponentClass cls, ITypeBuilder tb)
        {
            var _ts = tb.Assembly.TypeSystem;
            var _bindings = _ts.GetSystemBindings();


            tb.AddInterfaceImplementation(_ts.FindType<ICanMap>());

            var readerMethod = tb.DefineMethod(nameof(ICanMap.Map), true, false, true);
            var rg = readerMethod.Generator;

            var readerType = _ts.FindType<DbDataReader>();

            var readerParam =
                readerMethod.DefineParameter("reader", readerType, false, false);


            foreach (var property in cls.TypeBody.Properties)
            {
                if (string.IsNullOrEmpty(property.MapTo)) continue;

                var prop = tb.FindProperty(property.Name);

                rg
                    .LdArg_0()
                    .LdArg(readerParam.ArgIndex)
                    .LdStr(property.MapTo)
                    .EmitCall(readerType.FindMethod("get_Item", _bindings.String))
                    .Unbox_Any(prop.PropertyType)
                    .EmitCall(prop.Setter);
            }

            rg.Ret();
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
            if (root is Root r)
            {
                GenerateServerDtoClass(type, r);
                GenerateServerObjectClass(type, r);
                GenerateCommands(type, r);
            }
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
                GenerateCommands(type, root);
                GenerateClientDtoClass(type, root);
            }
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


        public void Stage0(Node astTree, ITypeBuilder builder, SqlDatabaseType dbType)
        {
        }

        public void Stage1(Node astTree, ITypeBuilder builder, SqlDatabaseType dbType)
        {
            if (astTree is ComponentClass cc)
            {
                if (cc.CompilationMode == CompilationMode.Server)
                {
                    EmitMappingSupport(cc, builder);
                    EmitSavingSupport(cc, builder);
                }

                BuildVersionField(builder);
            }
        }

        public void StageInfrastructure(IAssemblyBuilder builder)
        {
            var ts = builder.TypeSystem;
            var b = ts.GetSystemBindings();

            var linkType = builder.DefineType(_component.GetCodeRuleExpression(CodeGenRuleType.NamespaceRule),
                "EntityLink",
                TypeAttributes.Class | TypeAttributes.AnsiClass | TypeAttributes.BeforeFieldInit,
                b.Object);

            linkType.AddInterfaceImplementation(ts.FindType<ILink>());

            var idBack = linkType.DefineField(b.Guid, СonventionsHelper.GetBackingFieldName("Id"), false, false);
            linkType.DefineProperty(b.Guid, "Id", idBack, true, false);

            var typeBack = linkType.DefineField(b.Int, СonventionsHelper.GetBackingFieldName("Type"), false, false);
            linkType.DefineProperty(b.Int, "Type", typeBack, true, false);

            var presentationBack = linkType.DefineField(b.String, СonventionsHelper.GetBackingFieldName("Presentation"),
                false, false);
            linkType.DefineProperty(b.String, "Presentation", presentationBack, true, false);

            var ctor = linkType.DefineConstructor(false, b.Int, b.Guid);

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
            var prop = tb.DefinePropertyWithBackingField(_b.Byte.MakeArrayType(), "Version");
        }

        private void EmitSavingSupport(ComponentClass cls, ITypeBuilder tb, SqlDatabaseType dbType)
        {
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

            QueryMachine qm = new QueryMachine();

            foreach (var props in cls.TypeBody.Properties)
            {
                
            }

            var updateQuery = "";
            var insertQuery = "";

            var p_loc = rg.DefineLocal(_ts.FindType<DbParameter>());

            rg
                .LdArg(cmdParam.ArgIndex)
                .LdStr("SELECT * FROM Table")
                .EmitCall(cmdType.FindProperty(nameof(DbCommand.CommandText)).Setter);

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
    }
}