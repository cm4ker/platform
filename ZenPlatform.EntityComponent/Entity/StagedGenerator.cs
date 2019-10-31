using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Data.Common;
using System.Linq;
using ZenPlatform.Compiler.Contracts;
using ZenPlatform.Configuration.Structure;
using ZenPlatform.Configuration.Structure.Data;
using ZenPlatform.Configuration.Structure.Data.Types.Complex;
using ZenPlatform.Configuration.Structure.Data.Types.Primitive;
using ZenPlatform.Contracts;
using ZenPlatform.Core.Sessions;
using ZenPlatform.EntityComponent.Configuration;
using ZenPlatform.Language.Ast.Definitions;
using IType = ZenPlatform.Compiler.Contracts.IType;
using TypeAttributes = System.Reflection.TypeAttributes;

namespace ZenPlatform.EntityComponent.Entity
{
    /*
     * Пишу тут, так как это напрямую связано с генерацией кода
     *
     * Этапы:
     *     1) Создать полностью каркас приложения. Сгенерировать классы свойсва и поля
     *     2) Необходимо создать Юниты компиляции и зарегистрировать видимость всех нужных вещей, это
     *        необходимо для того, чтобы можно было компилировать правильно локальный код.
     *
     *       * Мы НЕ должны видеть внутренних переменных
     *       * Мы НЕ должны видеть лишние классы окружения (вообще нужно вырезать оператор new,
     *         платформа сама занимается созданием объектов)
     *       * Мы должны видеть переменные, которые МЫ объявили
     *       * Мы должны видеть разрешенные нам классы (например класс менеджер сущностей)
     *
     *         Все классы генерируются на уровне инструкций потому что это API удобно предоставить, на него легально
     *         можно накладывать отладочную информацию (когда именно это необходимо)
     */

    public class CompileInformation
    {
        public IAssemblyBuilder Assembly { get; }

        public List<CompilationUnit> Units { get; }

        public XCObjectTypeBase CurrentConf { get; }

        public ITypeBuilder CurrentType { get; }

        public ImmutableDictionary<XCObjectTypeBase, IType> PlatformTypes { get; set; }
    }


    public class StagedGenerator
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

        private IType GetTypeFromPlatformType(XCPrimitiveType pt, ITypeSystem ts)
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

        private void Stage0EmitMap(IEmitter rg, IParameter readerParam, IType readerType, IType propertyType,
            SystemTypeBindings ts, IMethod setter, string propName)
        {
            rg
                .LdArg_0()
                .LdArg(readerParam.ArgIndex)
                .LdStr(propName)
                .EmitCall(readerType.FindMethod("get_Item", ts.String))
                .Unbox_Any(propertyType)
                .EmitCall(setter);
        }

        public void Stage0(CompileInformation ci)
        {
            var type = ci.CurrentConf;
            var builder = ci.Assembly;

            var singleEntityType = type as XCSingleEntity ?? throw new InvalidOperationException(
                                       $"This component only can serve {nameof(XCSingleEntity)} objects");
            var dtoClassName =
                $"{_component.GetCodeRuleExpression(CodeGenRuleType.DtoPreffixRule)}{type.Name}{_component.GetCodeRuleExpression(CodeGenRuleType.DtoPostfixRule)}";

            var @namespace = _component.GetCodeRule(CodeGenRuleType.NamespaceRule).GetExpression();

            //Create dto class
            var dtoClass = builder.DefineType(@namespace, dtoClassName,
                TypeAttributes.Public | TypeAttributes.Class);

            dtoClass.AddInterfaceImplementation(builder.TypeSystem.FindType<ICanMapSelfFromDataReader>());
            var readerMethod = dtoClass.DefineMethod("Map", true, false, true);
            var rg = readerMethod.Generator;

            var readerType = builder.TypeSystem.FindType<DbDataReader>();

            var readerParam =
                readerMethod.DefineParameter("reader", readerType, false, false);


            _dtoCollections.Add(singleEntityType, dtoClass);

            foreach (var prop in singleEntityType.Properties)
            {
                bool propertyGenerated = false;

                if (string.IsNullOrEmpty(prop.DatabaseColumnName))
                {
                    throw new Exception(
                        $"Prop: {prop.Name} ObjectType: {typeof(XCSingleEntity)} Name: {singleEntityType.Name}. Database column is empty!");
                }

                var ts = builder.TypeSystem.GetSystemBindings();
                if (prop.Types.Count > 1)
                {
                    {
                        var tProperty = dtoClass.DefinePropertyWithBackingField(
                            ts.Int,
                            prop.Name + "_Type");

                        Stage0EmitMap(rg, readerParam, readerType, ts.Int, ts, tProperty.Setter, prop
                            .GetPropertySchemas(prop.DatabaseColumnName)
                            .First(x => x.SchemaType == XCColumnSchemaType.Type).Name);
                    }
                }

                foreach (var ctype in prop.Types)
                {
                    var propName = $"{prop.Name}_{ctype.Name}";
                    var propRef = $"{prop.Name}_Ref";

                    if (ctype is XCPrimitiveType pt)
                    {
                        var propType = pt switch
                        {
                            XCBinary b => ts.Byte.MakeArrayType(),
                            XCInt b => ts.Int,
                            XCString b => ts.String,
                            XCNumeric b => ts.Double,
                            XCBoolean b => ts.Boolean,
                            XCDateTime b => ts.DateTime,
                            XCGuid b => ts.Guid,
                        };

                        //var propType = builder.FindType(pt.CLRType.FullName);
                        var ptProperty = dtoClass.DefinePropertyWithBackingField(propType, propName);

                        Stage0EmitMap(rg, readerParam, readerType, propType, ts, ptProperty.Setter, prop
                            .GetPropertySchemas(prop.DatabaseColumnName)
                            .First(x => x.SchemaType == ((prop.Types.Count > 1)
                                            ? XCColumnSchemaType.Value
                                            : XCColumnSchemaType.NoSpecial) && x.PlatformType == pt).Name);
                    }
                    else if (ctype is XCObjectTypeBase ot)
                    {
                        if (!propertyGenerated)
                        {
                            propertyGenerated = true;
                            var otProperty = dtoClass.DefinePropertyWithBackingField(
                                ts.Guid,
                                propRef);

                            Stage0EmitMap(rg, readerParam, readerType, ts.Guid, ts, otProperty.Setter, prop
                                .GetPropertySchemas(prop.DatabaseColumnName)
                                .First(x => x.SchemaType == XCColumnSchemaType.Ref).Name);
                        }
                    }
                }
            }


            rg.Ret();
            //end create dto class
        }

        public ITypeBuilder Stage1(CompileInformation ci)
        {
            var type = ci.CurrentConf;
            var builder = ci.Assembly;

            var set = type as XCSingleEntity ?? throw new InvalidOperationException(
                          $"This component only can serve {nameof(XCSingleEntity)} objects");
            var className =
                $"{_component.GetCodeRuleExpression(CodeGenRuleType.EntityClassPrefixRule)}{type.Name}{_component.GetCodeRuleExpression(CodeGenRuleType.EntityClassPostfixRule)}";

            var @namespace = _component.GetCodeRuleExpression(CodeGenRuleType.NamespaceRule);

            //Create entity class
            return builder.DefineType(@namespace, className, TypeAttributes.Public | TypeAttributes.Class);
        }

        public void Stage2(CompileInformation ci)
        {
            var type = ci.CurrentConf;
            var asmBuilder = ci.Assembly;
            var builder = ci.CurrentType;
            var platformTypes = ci.PlatformTypes;

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

        public void Stage3(XCObjectTypeBase type, ITypeBuilder builder,
            ImmutableDictionary<XCObjectTypeBase, IType> platformTypes, IAssemblyBuilder asmBuilderd)
        {
            foreach (var pm in type.GetProgramModules())
            {
                if (pm.ModuleRelationType == XCProgramModuleRelationType.Object)
                {
                    var script = pm.ModuleText;
                }
            }
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
                    if (prop.Types[0] is XCPrimitiveType pt)
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

                        if (propType is XCPrimitiveType)
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

    /*
     * MAPPER
     *
     *  void Map(DbReader reader, _Invoice dto)
     *  {
     *     dto.FirstProp = reader["FirstProp"];
     *     dto.SecondProp = reader["SecondProp"];
     *
     *     dto.ThirdProp = reader["ThirdProp"];
     *
     *  }
     *
     *  SQL_BUILDER
     *
     *  void BuildSql()
     *  {
     *     Select FisrtProp, SecondProp, ThirdProp from Tbl_231 WHERE Id = "1231234-2341-2341-3434-3123455612"; 
     *  }
     * 
     */
}