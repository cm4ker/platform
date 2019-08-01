using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
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
using ZenPlatform.Core.Sessions;
using ZenPlatform.EntityComponent.Configuration;
using ZenPlatform.Language.Ast.Definitions;
using IType = ZenPlatform.Compiler.Contracts.IType;
using TypeAttributes = System.Reflection.TypeAttributes;

namespace ZenPlatform.EntityComponent.Entity
{
    public class StagedGenerator : IPlatformStagedAssemblyGenerator
    {
        private Dictionary<XCSingleEntity, IType> _dtoCollections;
        private readonly XCComponent _component;
        private GeneratorRules _rules;

        public StagedGenerator(XCComponent component)
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

        public void Stage0(XCObjectTypeBase type, IAssemblyBuilder builder)
        {
            var singleEntityType = type as XCSingleEntity ?? throw new InvalidOperationException(
                                       $"This component only can serve {nameof(XCSingleEntity)} objects");
            var dtoClassName =
                $"{_component.GetCodeRuleExpression(CodeGenRuleType.DtoPreffixRule)}{type.Name}{_component.GetCodeRuleExpression(CodeGenRuleType.DtoPostfixRule)}";

            var @namespace = _component.GetCodeRule(CodeGenRuleType.NamespaceRule).GetExpression();

            //Create dto class
            var dtoClass = builder.DefineType(@namespace, dtoClassName,
                TypeAttributes.Public | TypeAttributes.Class);

            _dtoCollections.Add(singleEntityType, dtoClass);

            foreach (var prop in singleEntityType.Properties)
            {
                bool propertyGenerated = false;

                if (prop.Types.Count > 1)
                {
                    dtoClass.DefinePropertyWithBackingField(builder.TypeSystem.GetSystemBindings().Int,
                        prop.Name + "_Type");
                }

                foreach (var ctype in prop.Types)
                {
                    if (ctype is XCPremitiveType pt)
                    {
                        var propType = pt switch
                            {
                            XCBinary b => builder.TypeSystem.GetSystemBindings().Byte.MakeArrayType(),
                            XCInt b => builder.TypeSystem.GetSystemBindings().Int,
                            XCString b => builder.TypeSystem.GetSystemBindings().String,
                            XCNumeric b => builder.TypeSystem.GetSystemBindings().Double,
                            XCBoolean b => builder.TypeSystem.GetSystemBindings().Boolean,
                            XCDateTime b => builder.TypeSystem.GetSystemBindings().DateTime,
                            XCGuid b => builder.TypeSystem.GetSystemBindings().Guid,
                            };

                        //var propType = builder.FindType(pt.CLRType.FullName);
                        dtoClass.DefinePropertyWithBackingField(propType, $"{prop.Name}_{pt.Name}");
                    }
                    else if (ctype is XCObjectTypeBase ot)
                    {
                        if (!propertyGenerated)
                        {
                            propertyGenerated = true;
                            dtoClass.DefinePropertyWithBackingField(builder.TypeSystem.GetSystemBindings().Guid,
                                $"{prop.Name}_{ot.Name}");
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
            return builder.DefineType(@namespace, className, TypeAttributes.Public | TypeAttributes.Class);
        }

        public void Stage2(XCObjectTypeBase type, ITypeBuilder builder,
            ImmutableDictionary<XCObjectTypeBase, IType> platformTypes, IAssemblyBuilder asmBuilder)
        {
            var set = type as XCSingleEntity ?? throw new InvalidOperationException(
                          $"This component only can serve {nameof(XCSingleEntity)} objects");

            var dto = _dtoCollections[set];

            var dtoField = builder.DefineField(dto, "_dto", false, false);

            var constructor = builder.DefineConstructor(false, dto);

            var g = constructor.Generator
                .LdArg_0()
                .EmitCall(asmBuilder.TypeSystem.GetSystemBindings().Object.Constructors[0])
                .LdArg_0()
                .LdArg(1)
                .StFld(dtoField);

            builder.AddSessionSupport(constructor);

            g.Ret();

            Stage2GeneragteProperties(set, dtoField, dto, builder, asmBuilder, platformTypes);
        }


        private void Stage2GeneragteProperties(XCSingleEntity set, IField dtoField, IType dto, ITypeBuilder builder,
            IAssemblyBuilder asmBuilder, ImmutableDictionary<XCObjectTypeBase, IType> platformTypes)
        {
            foreach (var prop in set.Properties)
            {
                if (prop.Types.Count == 0)
                    throw new Exception($"Panic: property {prop.Name} from type {set.Name} has no types!");

                if (prop.Types.Count == 1)
                {
                    if (prop.Types[0] is XCPremitiveType pt)
                    {
                        var clrPropertyType = GetTypeFromPlatformType(pt, asmBuilder.TypeSystem);
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
                    var objectClrType = asmBuilder.TypeSystem.GetSystemBindings().Object;
                    var clrProperty =
                        builder.DefineProperty(objectClrType, prop.Name);

                    var getter = builder.DefineMethod($"{prop.Name}_get", true, false, false)
                        .WithReturnType(objectClrType);

                    var gen = getter.Generator;


                    foreach (var propType in prop.Types)
                    {
                        ILabel ni = gen.DefineLabel();
                        IType primitiveType = null;

                        var typeProp = dto.Properties.FirstOrDefault(x => x.Name == $"{prop.Name}_Type");


                        gen
                            .LdArg_0()
                            .LdFld(dtoField)
                            .EmitCall(typeProp.Getter)
                            .LdcI4((int) propType.Id)
                            .BneUn(ni);

                        if (propType is XCPremitiveType)
                        {
                            var dataField =
                                dto.Properties.FirstOrDefault(x => x.Name == $"{prop.Name}_{propType.Name}") ??
                                throw new Exception("Field not found");

                            gen
                                .LdArg_0()
                                .LdFld(dtoField)
                                .EmitCall(dataField.Getter)
                                .Box(dataField.PropertyType)
                                .Ret();
                        }
                        else if (propType is XCObjectTypeBase ot)
                        {
                            //TODO: Нужно обратиться к серверу кэша
                        }

                        gen.MarkLabel(ni);

                        //gen.Ret();
                    }

                    clrProperty.WithGetter(getter);


                    /*
                     * public object Document
                     * {
                     *      get
                     *      {
                     *          if (_dto.CompositeProperty_Type == 1)
	                            {
		                            return _dto.CompositeProperty_Binary;
	                            }
	                            if (_dto.CompositeProperty_Type == 2)
	                            {
		                            return _dto.CompositeProperty_Boolean;
	                            }
	                            if (_dto.CompositeProperty_Type == 6)
	                            {
		                            return _dto.CompositeProperty_String;
	                            }
	                            if (_dto.CompositeProperty_Type == 3)
	                            {
		                            return _dto.CompositeProperty_DateTime;
	                            }
	                            if (_dto.CompositeProperty_Type == 100)
	                            {
	                                reutrn _cacheService.Return(_dto.CompositeProperty_Type, _dto.CompositeProperty_Ref);
	                            }
                     
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


        private void Stage2GenerageSaveMethod(XCSingleEntity set, ITypeBuilder builder, IType dto, IField dtoField)
        {
            var save = builder.DefineMethod("Save", true, false, false);
        }
    }


    public static class GenHelper
    {
        public static void AddSessionSupport(this ITypeBuilder builder, IConstructorBuilder constructor)
        {
            var sessionType = builder.Assembly.TypeSystem.FindType<Session>();
            var field = builder.DefineField(sessionType, "_session", false, false);
            var param = constructor.DefineParameter(sessionType);

            var g = constructor.Generator;

            g
                .LdArg_0()
                .LdArg(param.ArgIndex)
                .StFld(field);
        }

        public static IField GetSessionField(this IType type)
        {
            return type.FindField("_session");
        }
    }
}