using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Reflection;
using System.Security.Cryptography.X509Certificates;
using System.Threading;
using dnlib.DotNet.Resources;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Editing;
using Npgsql.TypeHandlers;
using ZenPlatform.Compiler.Contracts;
using ZenPlatform.Compiler.Platform;
using ZenPlatform.Configuration.Data.Contracts;
using ZenPlatform.Configuration.Structure.Data;
using ZenPlatform.Configuration.Structure.Data.Types.Complex;
using ZenPlatform.Configuration.Structure.Data.Types.Primitive;
using ZenPlatform.Contracts;
using ZenPlatform.Core.Language.QueryLanguage;
using ZenPlatform.EntityComponent.Configuration;
using ZenPlatform.Language.Ast.Definitions;

namespace ZenPlatform.EntityComponent.Entity
{
    public class Generator : IPlatformStagedAssemblyGenerator
    {
        private Dictionary<XCSingleEntity, IType> _dtoCollections;
        private readonly XCComponent _component;
        private GeneratorRules _rules;

        public Generator(XCComponent component)
        {
            _component = component;
            _rules = new GeneratorRules(component);
            _dtoCollections = new Dictionary<XCSingleEntity, IType>();
        }

        public void Stage0(XCObjectTypeBase type, IAssemblyBuilder builder)
        {
            var singleEntityType = type as XCSingleEntity ?? throw new InvalidOperationException(
                                       $"This component only can serve {nameof(XCSingleEntity)} objects");
            var dtoClassName =
                $"{_component.GetCodeRule(CodeGenRuleType.DtoPreffixRule)}{type.Name}{_component.GetCodeRule(CodeGenRuleType.DtoPostfixRule)}";

            var @namespace = _component.GetCodeRule(CodeGenRuleType.NamespaceRule).GetExpression();

            //Create dto class
            var dtoClass = builder.DefineType(@namespace, dtoClassName,
                TypeAttributes.Public | TypeAttributes.Class, null);

            _dtoCollections.Add(singleEntityType, dtoClass);

            foreach (var prop in singleEntityType.Properties)
            {
                bool hasManyObjectTypes = false;
                bool propertyGenerated = false;

                foreach (var ctype in prop.Types)
                {
                    if (ctype is XCPremitiveType pt)
                    {
                        var propType = builder.FindType(pt.CLRType.FullName);
                        dtoClass.DefineProperty(propType, prop.Name);
                    }
                    else if (ctype is XCObjectTypeBase ot)
                    {
                        if (hasManyObjectTypes) continue;

                        if (propertyGenerated)
                            hasManyObjectTypes = true;
                        if (!propertyGenerated)
                        {
                            propertyGenerated = true;
                            dtoClass.DefineProperty(builder.TypeSystem.GetSystemBindings().Guid, prop.Name);
                        }

                        if (hasManyObjectTypes)
                        {
                            dtoClass.DefineProperty(builder.TypeSystem.GetSystemBindings().Int, prop.Name + "_Type");
                        }
                    }
                }
            }

            //end create dto class
        }

        public ITypeBuilder Stage1(XCObjectTypeBase type, IAssemblyBuilder builder)
        {
            var set = type as XCSingleEntity ?? throw new InvalidOperationException(
                          $"This component only can serve {nameof(XCSingleEntity)} objects");
            var className =
                $"{_component.GetCodeRuleExpression(CodeGenRuleType.EntityClassPrefixRule)}{type.Name}{_component.GetCodeRuleExpression(CodeGenRuleType.EntityClassPostfixRule)}";

            var @namespace = _component.GetCodeRuleExpression(CodeGenRuleType.NamespaceRule);

            //Create entity class
            return builder.DefineType(@namespace, className, TypeAttributes.Public | TypeAttributes.Class, null);
        }

        public void Stage2(XCObjectTypeBase type, ITypeBuilder builder,
            ImmutableDictionary<XCObjectTypeBase, IType> platformTypes, IAssemblyBuilder asmBuilder)
        {
            var set = type as XCSingleEntity ?? throw new InvalidOperationException(
                          $"This component only can serve {nameof(XCSingleEntity)} objects");

            var dto = _dtoCollections[set];

            var dtoField = builder.DefineField(dto, "_dto", false, false);

            var constructor = builder.DefineConstructor(false, dto);

            constructor.Generator.LdArg_0().StFld(dtoField);

            foreach (var prop in set.Properties)
            {
                if (prop.Types.Count == 0)
                    throw new Exception($"Panic: property {prop.Name} from type {type.Name} has no types!");

                if (prop.Types.Count == 1)
                {
                    if (prop.Types[0] is XCPremitiveType pt)
                    {
                        var clrPropertyType = asmBuilder.FindType(pt.CLRType.FullName);
                        builder.DefineProperty(clrPropertyType, pt.Name, dtoField);
                    }
                    else if (prop.Types[0] is XCObjectTypeBase ot)
                    {
                        var objectType = platformTypes[ot];
                        builder.DefineProperty(objectType, ot.Name);
                    }
                }
                else
                {
                }
            }
        }
    }
}