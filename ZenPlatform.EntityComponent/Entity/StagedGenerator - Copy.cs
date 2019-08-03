using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Data.Common;
using System.Linq;
using System.Net.Mail;
using System.Security.Cryptography.X509Certificates;
using System.Threading;
using dnlib.DotNet.Resources;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Editing;
using Npgsql.TypeHandlers;
using ServiceStack;
using ZenPlatform.Compiler.Contracts;
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
using ZenPlatform.Language.Ast.Definitions;
using IType = ZenPlatform.Compiler.Contracts.IType;
using Property = ZenPlatform.Language.Ast.Definitions.Property;
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

        private IType GetTypeFromPlatformType(XCPremitiveType pt, ITypeSystem ts)
        {
            return pt switch
                {
                XCBinary b => ts.GetSystemBindings().Byte.MakeArrayType(),
                XCInt b => ts.GetSystemBindings().Int,
                XCString b => ts.GetSystemBindings().String,
                XCNumeric b => ts.GetSystemBindings().Double,
                XCBoolean b => ts.GetSystemBindings().Boolean,
                XCDateTime b => ts.GetSystemBindings().DateTime,
                XCGuid b => ts.GetSystemBindings().Guid,
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
                            new PrimitiveTypeSyntax(null, TypeNodeKind.Int), dbColName);

                        members.Add(astProp);
                    }
                }


                foreach (var ctype in prop.Types)
                {
                    if (ctype is XCPremitiveType pt)
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

                        TypeSyntax propType = pt switch
                            {
                            XCBinary b => (TypeSyntax) new ArrayTypeSyntax(null,
                                new PrimitiveTypeSyntax(null, TypeNodeKind.Byte)),
                            XCInt b => (TypeSyntax) new PrimitiveTypeSyntax(null, TypeNodeKind.Int),
                            XCString b => (TypeSyntax) new PrimitiveTypeSyntax(null, TypeNodeKind.String),
                            XCNumeric b => (TypeSyntax) new PrimitiveTypeSyntax(null, TypeNodeKind.Double),
                            XCBoolean b => (TypeSyntax) new PrimitiveTypeSyntax(null, TypeNodeKind.Boolean),
                            XCDateTime b => (TypeSyntax) new SingleTypeSyntax(null, nameof(DateTime), TypeNodeKind.Type)
                            ,
                            XCGuid b =>(TypeSyntax) new SingleTypeSyntax(null, nameof(Guid), TypeNodeKind.Type),
                            };

                        var astProp = new Property(null, propName, propType, dbColName);
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
                                new SingleTypeSyntax(null, nameof(Guid), TypeNodeKind.Type), dbColName);
                            members.Add(astProp);
                        }
                    }
                }
            }

            var cls = new Class(null, new TypeBody(members), dtoClassName, true);

            var cu = new CompilationUnit(null, null, new List<TypeEntity>() {cls});
            //end create dto class
            root.Add(cu);
        }

        public void Stage1(XCObjectTypeBase type, Root root)
        {
            var singleEntityType = type as XCSingleEntity ?? throw new InvalidOperationException(
                                       $"This component only can serve {nameof(XCSingleEntity)} objects");
            var className = type.Name;

            var @namespace = _component.GetCodeRule(CodeGenRuleType.NamespaceRule).GetExpression();

            List<Member> members = new List<Member>();

            foreach (var prop in singleEntityType.Properties)
            {
                bool propertyGenerated = false;

                if (prop.Types.Count > 1)
                {
                    {
                        var dbColName = prop.GetPropertySchemas(prop.DatabaseColumnName)
                            .First(x => x.SchemaType == XCColumnSchemaType.Type).Name;

                        var astProp = new Property(null, prop.Name + "_Type",
                            new PrimitiveTypeSyntax(null, TypeNodeKind.Int), dbColName);

                        members.Add(astProp);
                    }
                }


                foreach (var ctype in prop.Types)
                {
                    if (ctype is XCPremitiveType pt)
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

                        TypeSyntax propType = pt switch
                            {
                            XCBinary b => (TypeSyntax) new ArrayTypeSyntax(null,
                                new PrimitiveTypeSyntax(null, TypeNodeKind.Byte)),
                            XCInt b => (TypeSyntax) new PrimitiveTypeSyntax(null, TypeNodeKind.Int),
                            XCString b => (TypeSyntax) new PrimitiveTypeSyntax(null, TypeNodeKind.String),
                            XCNumeric b => (TypeSyntax) new PrimitiveTypeSyntax(null, TypeNodeKind.Double),
                            XCBoolean b => (TypeSyntax) new PrimitiveTypeSyntax(null, TypeNodeKind.Boolean),
                            XCDateTime b => (TypeSyntax) new SingleTypeSyntax(null, nameof(DateTime), TypeNodeKind.Type)
                            ,
                            XCGuid b =>(TypeSyntax) new SingleTypeSyntax(null, nameof(Guid), TypeNodeKind.Type),
                            };

                        var astProp = new Property(null, propName, propType, dbColName);
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
                                new SingleTypeSyntax(null, nameof(Guid), TypeNodeKind.Type), dbColName);
                            members.Add(astProp);
                        }
                    }
                }
            }

            var cls = new Class(null, new TypeBody(members), className, true);

            var cu = new CompilationUnit(null, null, new List<TypeEntity>() {cls});
            //end create dto class
            root.Add(cu);
        }
    }
}