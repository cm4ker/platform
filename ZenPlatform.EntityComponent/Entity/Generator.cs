using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Threading;
using dnlib.DotNet;
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
using IType = ZenPlatform.Compiler.Contracts.IType;
using TypeAttributes = System.Reflection.TypeAttributes;

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

            constructor.Generator
                .LdArg_0()
                .StFld(dtoField);

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
                    var clrProperty =
                        builder.DefineProperty(asmBuilder.TypeSystem.GetSystemBindings().Object, prop.Name);

                    var getter = builder.DefineMethod($"{prop.Name}_get", true, false, false);


                    var gen = getter.Generator;

                    ILabel ni = gen.DefineLabel();

                    foreach (var propType in prop.Types)
                    {
                        IType primitiveType = null;

                        var typeField = dto.Fields.FirstOrDefault(x => x.Name == $"{prop.Name}_Type");


                        gen
                            .LdArg_0()
                            .LdFld(dtoField)
                            .LdFld(typeField)
                            .LdcI4((int) propType.Id)
                            .BneUn(ni);

                        if (propType is XCPremitiveType)
                        {
                            var dataField = dto.Fields.FirstOrDefault(x => x.Name == $"{prop.Name}_{propType.Name}") ??
                                            throw new Exception("Field not found");

                            gen
                                .LdArg_0()
                                .LdFld(dtoField)
                                .LdFld(dataField)
                                .Box(dataField.FieldType)
                                .Ret();
                        }
                        else if (propType is XCObjectTypeBase ot)
                        {
                            //TODO: Нужно обратиться к серверу кэша
                        }

                        gen.MarkLabel(ni);

                        gen.Ret();
                    }

                    /*
                     * public object Document
                     * {
                     *      get
                     *      {
                     *             if(_dto.Prop1_type = 1_dto.Prop1_guid is not default) reutrn _dto.Prop1_guid;
                     *             if(_dto.Prop1_int is not default) reutrn _dto.Prop1_int;
                     *
                     *             if(_dto.Prop1_type is not default) reutrn _cacheService.Return(_dto.Prop1_type, _dto.Prop1_ref);
                     *      }
                     *
                     *      set
                     *      {
                     *            if(value is guid) {_dto.Prop1_guid = value; _dto.Prop1_int = default; _dto.Prop1_type = 0; _dto.Prop1_ref = null;}
                     *            if(value is int) {_dto.Prop1_guid = deafult; _dto.Prop1_int = value; _dto.Prop1_type = 0; _dto.Prop1_ref = null;}
                     *            if(value is Invoice i) {_dto.Prop1_guid = deafult; _dto.Prop1_int = default; _dto.Prop1_type = 1; _dto.Prop1_ref = i.GetRef();}
                     *      }
                     * }
                     * 
                     */

                    //TODO: Сделать генерацию управляющего кода выбора типа свойства
                }
            }
        }
    }
}