using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Data.Common;
using System.Linq;
using System.Linq.Expressions;
using System.Net.Mail;
using System.Security.Cryptography.X509Certificates;
using System.Threading;
using dnlib.DotNet;
using dnlib.DotNet.Resources;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Editing;
using NLog.LayoutRenderers;
using Npgsql.NameTranslation;
using Npgsql.TypeHandlers;
using ServiceStack;
using ZenPlatform.Compiler;
using ZenPlatform.Compiler.Contracts;
using ZenPlatform.Compiler.Contracts.Symbols;
using ZenPlatform.Compiler.Generation;
using ZenPlatform.Compiler.Platform;
using ZenPlatform.Configuration.Data.Contracts;
using ZenPlatform.Configuration.Structure;
using ZenPlatform.Configuration.Structure.Data;
using ZenPlatform.Configuration.Structure.Data.Types;
using ZenPlatform.Configuration.Structure.Data.Types.Complex;
using ZenPlatform.Configuration.Structure.Data.Types.Primitive;
using ZenPlatform.Contracts;
using ZenPlatform.Core.Language.QueryLanguage;
using ZenPlatform.Core.Sessions;
using ZenPlatform.EntityComponent.Configuration;
using ZenPlatform.Language.Ast;
using ZenPlatform.Language.Ast.Definitions;
using ZenPlatform.Language.Ast.Definitions.Expressions;
using ZenPlatform.Language.Ast.Definitions.Statements;
using ZenPlatform.Language.Ast.Infrastructure;
using BinaryExpression = ZenPlatform.Language.Ast.Definitions.Expressions.BinaryExpression;
using IType = ZenPlatform.Compiler.Contracts.IType;
using Parameter = ZenPlatform.Language.Ast.Definitions.Functions.Parameter;
using Property = ZenPlatform.Language.Ast.Definitions.Property;
using PublicKey = System.Security.Cryptography.X509Certificates.PublicKey;
using TypeAttributes = System.Reflection.TypeAttributes;

namespace ZenPlatform.EntityComponent.Entity
{
    public class StagedGeneratorAst
    {
        private Dictionary<XCSingleEntity, IType> _dtoCollections;
        private readonly XCComponent _component;
        private GeneratorRules _rules;

        public StagedGeneratorAst(XCComponent component)
        {
            _component = component;
            _rules = new GeneratorRules(component);
            _dtoCollections = new Dictionary<XCSingleEntity, IType>();
        }

        private TypeSyntax GetAstFromPlatformType(XCTypeBase pt)
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
                XCObjectTypeBase b => (TypeSyntax) new SingleTypeSyntax(null, b.Name, TypeNodeKind.Type),
                XCGuid b => (TypeSyntax) new SingleTypeSyntax(null, nameof(Guid), TypeNodeKind.Type),
            };
        }

        public void Stage0(XCObjectTypeBase type, Root root)
        {
            var singleEntityType = type as XCSingleEntity ?? throw new InvalidOperationException(
                                       $"This component only can serve {nameof(XCSingleEntity)} objects");
            var dtoClassName =
                $"{_component.GetCodeRuleExpression(CodeGenRuleType.DtoPreffixRule)}{type.Name}{_component.GetCodeRuleExpression(CodeGenRuleType.DtoPostfixRule)}";

            var @namespace = _component.GetCodeRule(CodeGenRuleType.NamespaceRule).GetExpression();

            List<Member> members = new List<Member>();

            //Create dto class
            foreach (var prop in singleEntityType.Properties)
            {
                bool propertyGenerated = false;

                if (prop.DatabaseColumnName.IsNullOrEmpty())
                {
                    throw new Exception(
                        $"Prop: {prop.Name} ObjectType: {typeof(XCSingleEntity)} Name: {singleEntityType.Name}. Database column is empty!");
                }

                if (prop.Types.Count > 1)
                {
                    {
                        var dbColName = prop.GetPropertySchemas(prop.DatabaseColumnName)
                            .First(x => x.SchemaType == XCColumnSchemaType.Type).Name;

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
                                            : XCColumnSchemaType.NoSpecial) && x.PlatformType == pt).Name;

                        var propName = prop
                            .GetPropertySchemas(prop.Name)
                            .First(x => x.SchemaType == ((prop.Types.Count > 1)
                                            ? XCColumnSchemaType.Value
                                            : XCColumnSchemaType.NoSpecial) && x.PlatformType == pt).Name;

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
                                                : XCColumnSchemaType.NoSpecial) && x.PlatformType == ot).Name;

                            var propName = prop
                                .GetPropertySchemas(prop.Name)
                                .First(x => x.SchemaType == ((prop.Types.Count > 1)
                                                ? XCColumnSchemaType.Ref
                                                : XCColumnSchemaType.NoSpecial) && x.PlatformType == ot).Name;

                            var astProp = new Property(null, propName,
                                new SingleTypeSyntax(null, nameof(Guid), TypeNodeKind.Type), true, true, dbColName);
                            members.Add(astProp);
                        }
                    }
                }
            }

            var cls = new Class(null, new TypeBody(members), dtoClassName, true);

            cls.Namespace = @namespace;

            var cu = new CompilationUnit(null, new List<NamespaceBase>(), new List<TypeEntity>() {cls});
            //end create dto class
            root.Add(cu);
        }

        public void Stage1(XCObjectTypeBase type, Root root)
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

            var field = new Field(null, "_dto", dtoType) {SymbolScope = SymbolScope.System};

            var fieldSession = new Field(null, "_session", sessionType) {SymbolScope = SymbolScope.System};

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

                        var fieldExpression = new GetFieldExpression(new Name(null, "_dto"), schema.Name);

                        var expr = new BinaryExpression(null,
                            typeLiteral
                            , fieldExpression
                            , BinaryOperatorType.Equal);


                        var schemaTyped = prop.GetPropertySchemas(prop.Name)
                            .First(x => x.PlatformType == ctype);

                        var feTypedProp = new GetFieldExpression(new Name(null, "_dto"), schemaTyped.Name);

                        var ret = new Return(null, feTypedProp);

                        var @if = new If(null, null, ret.ToBlock(), expr);
                        get.Add(@if);


                        var afe = new AssignFieldExpression(null, new Name(null, "_dto"), schemaTyped.Name);
                        var afe2 = new AssignFieldExpression(null, new Name(null, "_dto"), typeField.Name);
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
                    var fieldExpression = new GetFieldExpression(new Name(null, "_dto"), schema.Name);
                    var ret = new Return(null, fieldExpression);
                    get.Add(ret);
                }

                if (astProp.HasGetter)
                    astProp.Getter = new Block(get);

                if (astProp.HasSetter)
                    astProp.Setter = new Block(set);
            }

            //IReferenceImpl


            var tprop = new Property(null, "Type", intType, true, false);
            tprop.Getter = new Block(new[]
            {
                (Statement) new Return(null, new Literal(null, singleEntityType.Id.ToString(), intType))
            }.ToList());
            //

            members.Add(tprop);

            var cls = new Class(null, new TypeBody(members), className);
            cls.ImplementsReference = true;
            cls.Namespace = @namespace;

            Stage1Modules(type, cls);

            var cu = new CompilationUnit(null, new List<NamespaceBase>(), new List<TypeEntity>() {cls});
            //end create dto class
            root.Add(cu);
        }

        public void Stage1Modules(XCObjectTypeBase type, Class cls)
        {
            foreach (var module in type.GetProgramModules())
            {
                if (module.ModuleRelationType == XCProgramModuleRelationType.Object)
                {
                    var typeBody = ParserHelper.ParseTypeBody(module.ModuleText);

                    foreach (var func in typeBody.Functions)
                    {
                        func.SymbolScope = SymbolScope.User;
                        cls.AddFunction(func);
                    }
                }
            }
        }

        public void Stage3()
        {
        }
    }
}