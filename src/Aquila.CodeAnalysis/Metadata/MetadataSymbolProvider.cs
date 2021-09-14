using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Reflection.Metadata;
using Aquila.CodeAnalysis;
using Aquila.CodeAnalysis.CodeGen;
using Aquila.CodeAnalysis.Public;
using Aquila.CodeAnalysis.Symbols;
using Aquila.CodeAnalysis.Symbols.Attributes;
using Aquila.CodeAnalysis.Symbols.Synthesized;
using Aquila.CodeAnalysis.Syntax.Parser;
using Aquila.Metadata;
using Aquila.Syntax.Syntax;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeGen;
using MoreLinq.Extensions;
using Roslyn.Utilities;
using SpecialTypeExtensions = Microsoft.CodeAnalysis.SpecialTypeExtensions;

namespace Aquila.Syntax.Metadata
{
    internal class MetadataSymbolProvider
    {
        private readonly AquilaCompilation _declaredCompilation;
        private CoreTypes _ct;
        private CoreMethods _cm;
        private PlatformSymbolCollection _ps;
        private SynthesizedNamespaceSymbol _entityNamespaceSymbol;

        private const string ObjectPostfix = "Object";
        private const string DtoPostfix = "Dto";
        private const string ManagerPostfix = "Manager";
        private const string LinkPostfix = "Link";
        private const string Namespace = "Entity";

        public MetadataSymbolProvider(AquilaCompilation declaredCompilation)
        {
            _declaredCompilation = declaredCompilation;

            //force fill the core types
            _declaredCompilation.GetBoundReferenceManager();

            _ct = _declaredCompilation.CoreTypes;
            _cm = _declaredCompilation.CoreMethods;
            _ps = _declaredCompilation.PlatformSymbolCollection;
        }

        public INamespaceSymbol EntityNamespace => _entityNamespaceSymbol;

        public void PopulateNamespaces(IEnumerable<SMEntity> mds)
        {
            _entityNamespaceSymbol = _ps.SynthesizeNamespace(_declaredCompilation.GlobalNamespace, Namespace);
        }

        public void PopulateTypes(IEnumerable<SMEntity> mds)
        {
            foreach (var md in mds)
            {
                //Dto
                _ps.SynthesizeType(_entityNamespaceSymbol, $"{md.Name}{DtoPostfix}")
                    .SetAccess(Accessibility.Public);

                //Object
                var oType = _ps.SynthesizeType(_entityNamespaceSymbol, $"{md.Name}{ObjectPostfix}")
                    .SetAccess(Accessibility.Public)
                    .AddAttribute(new SynthesizedAttributeData(_ct.EntityAttribute.Ctor(),
                        ImmutableArray<TypedConstant>.Empty,
                        ImmutableArray<KeyValuePair<string, TypedConstant>>.Empty));

                _ps.RegisterAlias(md.Name, oType, _entityNamespaceSymbol);

                //Manager
                _ps.SynthesizeType(_entityNamespaceSymbol, $"{md.Name}{ManagerPostfix}")
                    .SetIsStatic(true);

                //Link
                _ps.SynthesizeType(_entityNamespaceSymbol, $"{md.Name}{LinkPostfix}")
                    .SetAccess(Accessibility.Public)
                    .AddAttribute(new SynthesizedAttributeData(_ct.LinkAttribute.Ctor(),
                        ImmutableArray<TypedConstant>.Empty,
                        ImmutableArray<KeyValuePair<string, TypedConstant>>.Empty));
            }
        }

        public void PopulateMembers(IEnumerable<SMEntity> mds)
        {
            foreach (var md in mds)
            {
                var dtoType = PopulateDtoType(md);
                var objectType = PopulateObjectType(md, dtoType);
                var managerType = PopulateManagerType(md, dtoType, objectType);
                var linkType = PopulateLinkType(md);
            }
        }

        private IEnumerable<(string propName, TypeSymbol type)> GetDtoPropertySchema(SMProperty prop)
        {
            var isComplexType = prop.Types.Count() > 1;

            TypeSymbol propType;
            var hasLinkProperty = false;

            foreach (var info in prop.GetOrderedFlattenTypes())
            {
                propType = MetadataTypeProvider.Resolve(_declaredCompilation, info.type);

                if (info.type.IsReference && hasLinkProperty)
                    continue;

                if (info.type.IsReference)
                {
                    hasLinkProperty = true;
                    yield return new($"{prop.Name}{info.postfix}", _ct.Guid);
                }
                else
                    yield return new($"{prop.Name}{info.postfix}", propType);
            }
        }

        private NamedTypeSymbol PopulateDtoType(SMEntity md)
        {
            var dtoType = _ps.GetSynthesizedType(QualifiedName.Parse($"{Namespace}.{md.Name}{DtoPostfix}", false));
            var thisPlace = new ThisArgPlace(dtoType);
            var ctor = _ps.SynthesizeConstructor(dtoType);
            ctor
                .SetMethodBuilder((m, d) => (il) =>
                {
                    thisPlace.EmitLoad(il);
                    il.EmitCall(m, d, ILOpCode.Call, _ct.Object.Ctor());

                    il.EmitRet(true);
                });


            foreach (var prop in md.Properties)
            {
                foreach (var schema in GetDtoPropertySchema(prop))
                {
                    _ps.CreatePropertyWithBackingField(dtoType, schema.type, schema.propName);
                }
            }

            dtoType.AddMember(ctor);
            return dtoType;
        }

        private NamedTypeSymbol PopulateObjectType(SMEntity md, NamedTypeSymbol dtoType)
        {
            var objectType =
                _ps.GetSynthesizedType(QualifiedName.Parse($"{Namespace}.{md.Name}{ObjectPostfix}", false));

            #region Fields

            //Internal field
            var dtoField = _ps.SynthesizeField(objectType);
            dtoField
                .SetName("_dto")
                .SetAccess(Accessibility.Private)
                .SetType(dtoType);

            var ctxField = _ps.SynthesizeField(objectType);
            ctxField
                .SetName(SpecialParameterSymbol.ContextName)
                .SetAccess(Accessibility.Private)
                .SetType(_ct.AqContext);

            #endregion

            #region Constructor

            var ctor = _ps.SynthesizeConstructor(objectType);
            var ctxParam = new SpecialParameterSymbol(ctor, _ct.AqContext, SpecialParameterSymbol.ContextName, 0);
            var dtoParam = new SynthesizedParameterSymbol(ctor, dtoType, 1, RefKind.None, "dto");


            var thisPlace = new ArgPlace(objectType, 0);
            var dtoPS = new ParamPlace(dtoParam);
            var ctxPS = new ParamPlace(ctxParam);

            var dtoFieldPlace = new FieldPlace(dtoField, thisPlace);
            var ctxFieldPlace = new FieldPlace(ctxField, thisPlace);

            ctor
                .SetParameters(ctxParam, dtoParam)
                .SetMethodBuilder((m, d) => (il) =>
                {
                    thisPlace.EmitLoad(il);
                    il.EmitCall(m, d, ILOpCode.Call, _ct.Object.Ctor());

                    thisPlace.EmitLoad(il);
                    ctxPS.EmitLoad(il);
                    ctxFieldPlace.EmitStore(il);

                    thisPlace.EmitLoad(il);
                    dtoPS.EmitLoad(il);
                    dtoFieldPlace.EmitStore(il);

                    il.EmitRet(true);
                });

            #endregion

            #region Props

            foreach (var prop in md.Properties)
            {
                var isComplexType = prop.Types.Count() > 1;

                var getter = _ps.SynthesizeMethod(objectType);
                var setter = _ps.SynthesizeMethod(objectType);
                var property = _ps.SynthesizeProperty(objectType);

                var propType = (isComplexType)
                    ? _ct.Object
                    : MetadataTypeProvider.Resolve(_declaredCompilation, prop.Types.First());

                getter.SetAccess(Accessibility.Public)
                    .SetName($"get_{prop.Name}")
                    .SetReturn(propType);

                setter.SetAccess(Accessibility.Public)
                    .SetName($"set_{prop.Name}");


                property.SetType(propType)
                    .SetGetMethod(getter)
                    .SetSetMethod(setter)
                    .SetName(prop.Name);

                var param = new SynthesizedParameterSymbol(setter, propType, 0, RefKind.None);
                setter.SetParameters(param);

                var setValueParam = new ParamPlace(param);

                if (isComplexType)
                {
                    getter.SetMethodBuilder((m, d) =>
                    {
                        return (il) =>
                        {
                            var types = prop.Types.GetOrderedFlattenTypes().OrderBy(x => x.type.Kind)
                                .ToImmutableArray();

                            foreach (var type in types)
                            {
                                if (type.isType)
                                    continue;

                                var underlyingPropType = MetadataTypeProvider.Resolve(_declaredCompilation, type.type);

                                var dtoMemberName = $"{prop.Name}{type.postfix}";
                                var dtoTypeMemberName = prop.Name + types.FirstOrDefault(x => x.isType).postfix;

                                var dtoMember = dtoType.GetMembers(dtoMemberName).OfType<PropertySymbol>().First();
                                var dtoTypeMember = dtoType.GetMembers(dtoTypeMemberName).OfType<PropertySymbol>()
                                    .First();

                                thisPlace.EmitLoad(il);
                                dtoFieldPlace.EmitLoad(il);
                                il.EmitCall(m, d, ILOpCode.Call, dtoTypeMember.GetMethod);

                                if (type.type.IsPrimitive)
                                {
                                    il.EmitIntConstant((int)type.type.Kind);
                                }
                                else
                                {
                                    var managerType =
                                        _ps.GetType(QualifiedName.Parse(
                                            $"{Namespace}.{type.type.GetSemantic().Name}{ManagerPostfix}",
                                            true));
                                    var typeIDField = managerType.GetMembers("TypeId").OfType<FieldSymbol>()
                                        .FirstOrDefault();
                                    var fieldPlace = new FieldPlace(typeIDField, m);
                                    fieldPlace.EmitLoad(il);
                                }

                                var lbl = new NamedLabel("<return>");
                                il.EmitBranch(ILOpCode.Bne_un_s, lbl);

                                thisPlace.EmitLoad(il);
                                dtoFieldPlace.EmitLoad(il);

                                //TODO: Here we need create link (or get it from the cache)
                                il.EmitCall(m, d, ILOpCode.Call, dtoMember.GetMethod);
                                il.EmitOpCode(ILOpCode.Box);
                                il.EmitSymbolToken(m, d, dtoMember.GetMethod.ReturnType, null);
                                il.EmitRet(false);
                                il.MarkLabel(lbl);
                            }

                            il.EmitCall(m, d, ILOpCode.Newobj, _ct.Exception.Ctor());
                            il.EmitThrow(false);
                        };
                    });

                    setter.SetMethodBuilder((m, d) => (il) =>
                    {
                        var types = prop.Types.GetOrderedFlattenTypes().ToImmutableArray();

                        foreach (var typeInfo in types.Where(x => !x.isType))
                        {
                            var underlyingPropType = MetadataTypeProvider.Resolve(_declaredCompilation, typeInfo.type);

                            var dtoMemberName = $"{prop.Name}{typeInfo.postfix}";
                            var dtoTypeMemberName = prop.Name + types.FirstOrDefault(x => x.isType).postfix;

                            var dtoMember = dtoType.GetMembers(dtoMemberName).OfType<PropertySymbol>().First();
                            var dtoTypeMember = dtoType.GetMembers(dtoTypeMemberName).OfType<PropertySymbol>()
                                .First();

                            var lbl = new NamedLabel("<return>");

                            setValueParam.EmitLoad(il);

                            il.EmitOpCode(ILOpCode.Isinst);
                            il.EmitSymbolToken(m, d, underlyingPropType, null);

                            il.EmitBranch(ILOpCode.Brfalse, lbl);

                            thisPlace.EmitLoad(il);
                            dtoFieldPlace.EmitLoad(il);
                            setValueParam.EmitLoad(il);

                            il.EmitOpCode(ILOpCode.Unbox_any);
                            il.EmitSymbolToken(m, d, underlyingPropType, null);

                            //TODO: We must set the identifier to the dto if this is link
                            //Add the condition for Link type and handle
                            il.EmitCall(m, d, ILOpCode.Call, dtoMember.SetMethod);

                            thisPlace.EmitLoad(il);
                            dtoFieldPlace.EmitLoad(il);
                            il.EmitIntConstant(1);
                            il.EmitCall(m, d, ILOpCode.Call, dtoTypeMember.SetMethod);

                            il.MarkLabel(lbl);
                        }

                        il.EmitCall(m, d, ILOpCode.Newobj, _ct.Exception.Ctor());
                        il.EmitThrow(false);


                        il.EmitRet(true);
                    });
                }
                else
                {
                    var dtoProperty = dtoType.GetMembers(prop.Name).OfType<PropertySymbol>().First();


                    getter.SetMethodBuilder((m, d) =>
                    {
                        return (il) =>
                        {
                            thisPlace.EmitLoad(il);
                            dtoFieldPlace.EmitLoad(il);
                            il.EmitCall(m, d, ILOpCode.Call, dtoProperty.GetMethod);
                            il.EmitRet(false);
                        };
                    });


                    setter
                        .SetMethodBuilder((m, d) =>
                        {
                            return (il) =>
                            {
                                thisPlace.EmitLoad(il);
                                dtoFieldPlace.EmitLoad(il);
                                setValueParam.EmitLoad(il);
                                il.EmitCall(m, d, ILOpCode.Call, dtoProperty.SetMethod);
                                il.EmitRet(true);
                            };
                        });
                }

                objectType.AddMember(getter);
                objectType.AddMember(setter);
                objectType.AddMember(property);
            }

            #endregion

            #region void Save()

            var saveMethod = _ps.SynthesizeMethod(objectType)
                .SetName("save")
                .SetAccess(Accessibility.Public);

            saveMethod
                .SetMethodBuilder((m, d) =>
                {
                    return (il) =>
                    {
                        var managerType =
                            _ps.GetType(QualifiedName.Parse($"{Namespace}.{md.Name}{ManagerPostfix}", true));
                        var mrgSave = managerType.GetMembers("save").OfType<MethodSymbol>().FirstOrDefault() ??
                                      throw new Exception("Method save not found in manager type");

                        var beforeSave = objectType.GetMembers("before_save").OfType<MethodSymbol>()
                            .Where(x => x.ParameterCount == 0).ToArray();

                        if (beforeSave.Length == 1)
                        {
                            thisPlace.EmitLoad(il);
                            il.EmitCall(m, d, ILOpCode.Call, beforeSave[0]);
                        }


                        thisPlace.EmitLoad(il);
                        ctxFieldPlace.EmitLoad(il);

                        thisPlace.EmitLoad(il);
                        dtoFieldPlace.EmitLoad(il);


                        il.EmitCall(m, d, ILOpCode.Call, mrgSave);
                        il.EmitRet(true);
                    };
                });

            #endregion

            objectType.AddMember(dtoField);
            objectType.AddMember(ctxField);

            objectType.AddMember(ctor);
            objectType.AddMember(saveMethod);


            return objectType;
        }

        private NamedTypeSymbol PopulateManagerType(SMEntity md, NamedTypeSymbol dtoType,
            NamedTypeSymbol objectType)
        {
            var managerType =
                _ps.GetSynthesizedType(QualifiedName.Parse($"{Namespace}.{md.Name}{ManagerPostfix}", false));

            MethodSymbol ctor = _ct.QueryAttribute.Ctor();

            //plaint entity save query text
            var updateQueryfield = _ps.SynthesizeField(managerType)
                    .SetIsStatic(true)
                    .SetName($"{md.Name}UpdateQuery")
                    .SetAccess(Accessibility.Public)
                    .SetReadOnly(false)
                    .SetType(_ct.String)
                    .AddAttribute(
                        new SynthesizedAttributeData(_ct.RuntimeInitAttribute.Ctor(_ct.RuntimeInitKind, _ct.Object),
                            new[]
                                {
                                    new TypedConstant(_ct.RuntimeInitKind.Symbol, TypedConstantKind.Enum, 2),
                                    new TypedConstant(_ct.String.Symbol, TypedConstantKind.Primitive, md.FullName)
                                }
                                .ToImmutableArray(), ImmutableArray<KeyValuePair<string, TypedConstant>>.Empty))
                ;

            //plaint entity save query text
            var insertQueryField = _ps.SynthesizeField(managerType)
                    .SetIsStatic(true)
                    .SetName($"{md.Name}InsertQuery")
                    .SetAccess(Accessibility.Public)
                    .SetReadOnly(false)
                    .SetType(_ct.String)
                    .AddAttribute(
                        new SynthesizedAttributeData(_ct.RuntimeInitAttribute.Ctor(_ct.RuntimeInitKind, _ct.Object),
                            new[]
                                {
                                    new TypedConstant(_ct.RuntimeInitKind.Symbol, TypedConstantKind.Enum, 3),
                                    new TypedConstant(_ct.String.Symbol, TypedConstantKind.Primitive, md.FullName)
                                }
                                .ToImmutableArray(), ImmutableArray<KeyValuePair<string, TypedConstant>>.Empty))
                ;


            //plaint entity save query text
            var loadQueryField = _ps.SynthesizeField(managerType)
                    .SetIsStatic(true)
                    .SetName($"{md.Name}LoadQuery")
                    .SetAccess(Accessibility.Public)
                    .SetReadOnly(false)
                    .SetType(_ct.String)
                    .AddAttribute(
                        new SynthesizedAttributeData(_ct.RuntimeInitAttribute.Ctor(_ct.RuntimeInitKind, _ct.Object),
                            new[]
                                {
                                    new TypedConstant(_ct.RuntimeInitKind.Symbol, TypedConstantKind.Enum, 1),
                                    new TypedConstant(_ct.String.Symbol, TypedConstantKind.Primitive, md.FullName)
                                }
                                .ToImmutableArray(), ImmutableArray<KeyValuePair<string, TypedConstant>>.Empty))
                ;

            var updateQueryFieldPlace = new FieldPlace(updateQueryfield);
            var loadQueryFieldPlace = new FieldPlace(loadQueryField);
            var insertQueryFieldPlace = new FieldPlace(insertQueryField);


            var saveApiMethod = _ps.SynthesizeMethod(managerType);

            saveApiMethod.SetName("save_api")
                .SetAccess(Accessibility.Public)
                .SetIsStatic(true)
                .AddAttribute(new SynthesizedAttributeData(
                    _ct.HttpHandlerAttribute.Ctor(_ct.HttpMethodKind, _ct.String),
                    new[]
                    {
                        new TypedConstant(_ct.HttpMethodKind.Symbol, TypedConstantKind.Primitive, 1),
                        new TypedConstant(_ct.String.Symbol, TypedConstantKind.Primitive,
                            $"/{md.Name.ToCamelCase()}/post")
                    }.ToImmutableArray(), ImmutableArray<KeyValuePair<string, TypedConstant>>.Empty));

            {
                var saveDtoPerameter = new SynthesizedParameterSymbol(saveApiMethod, dtoType, 1, RefKind.None);
                var ctxParameter = //new SynthesizedParameterSymbol(saveMethod, _ct.AqContext, 0, RefKind.None);
                    new SpecialParameterSymbol(saveApiMethod, _ct.AqContext, SpecialParameterSymbol.ContextName, 0);
                var sdpp = new ParamPlace(saveDtoPerameter);
                var ctx = new ParamPlace(ctxParameter);

                var objSaveMethod = objectType.GetMembers("save").OfType<MethodSymbol>().First();

                saveApiMethod
                    .SetParameters(ctxParameter, saveDtoPerameter)
                    .SetMethodBuilder((m, d) => il =>
                    {
                        ctx.EmitLoad(il);
                        sdpp.EmitLoad(il);

                        il.EmitCall(m, d, ILOpCode.Newobj, objectType.Constructors.First());
                        il.EmitCall(m, d, ILOpCode.Call, objSaveMethod);

                        il.EmitRet(true);
                    });
            }
            var dbTextProp = _ct.DbCommand.Property("CommandText");

            var saveMethod = _ps.SynthesizeMethod(managerType)
                .SetName("save")
                .SetAccess(Accessibility.Public)
                .SetIsStatic(true);
            {
                var saveDtoPerameter = new SynthesizedParameterSymbol(saveMethod, dtoType, 1, RefKind.None);
                var ctxParameter = //new SynthesizedParameterSymbol(saveMethod, _ct.AqContext, 0, RefKind.None);
                    new SpecialParameterSymbol(saveMethod, _ct.AqContext, SpecialParameterSymbol.ContextName, 0);
                var sdpp = new ParamPlace(saveDtoPerameter);
                var ctx = new ParamPlace(ctxParameter);

                #region Save()

                var paramName = _ct.DbParameter.Property("ParameterName");
                var paramValue = _ct.DbParameter.Property("Value");

                var dbParamsProp = _ct.DbCommand.Property("Parameters");

                var paramsCollectionAdd = dbParamsProp.Symbol.Type.GetMembers("Add").OfType<MethodSymbol>()
                    .FirstOrDefault();

                saveMethod
                    .SetParameters(ctxParameter, saveDtoPerameter)
                    .SetMethodBuilder((m, d) => il =>
                    {
                        var dbLoc = new LocalPlace(il.DefineSynthLocal(saveMethod, "dbCommand", _ct.DbCommand));
                        var paramLoc = new LocalPlace(il.DefineSynthLocal(saveMethod, "dbParameter", _ct.DbParameter));
                        var idClrProp =
                            dtoType.GetMembers(md.IdProperty.Name).OfType<PropertySymbol>().FirstOrDefault() ??
                            throw new Exception("The id property is null");

                        ctx.EmitLoad(il);
                        il.EmitCall(m, d, ILOpCode.Call, _cm.Runtime.CreateCommand);

                        dbLoc.EmitStore(il);

                        //_dto.Id
                        sdpp.EmitLoad(il);
                        il.EmitCall(m, d, ILOpCode.Callvirt, idClrProp.GetMethod);

                        //Default Guid
                        var tmpLoc = new LocalPlace(il.DefineSynthLocal(saveMethod, "", _ct.Guid));
                        tmpLoc.EmitLoadAddress(il);
                        il.EmitOpCode(ILOpCode.Initobj);
                        il.EmitSymbolToken(m, d, _ct.Guid, null);
                        tmpLoc.EmitLoad(il);
                        il.EmitCall(m, d, ILOpCode.Call, _cm.Operators.op_Equality_Guid_Guid);

                        // var tmpLocIfRes = new LocalPlace(il.DefineSynthLocal(saveMethod, "", _ct.Boolean));
                        // tmpLocIfRes.EmitStore(il);

                        var elseLabel = new NamedLabel("<e_o1>");
                        var endLabel = new NamedLabel("<end>");


                        il.EmitBranch(ILOpCode.Brfalse, elseLabel);

                        //set insert query
                        dbLoc.EmitLoad(il);
                        insertQueryFieldPlace.EmitLoad(il);
                        il.EmitCall(m, d, ILOpCode.Callvirt, dbTextProp.Setter);

                        //set to id new value
                        sdpp.EmitLoad(il);
                        il.EmitCall(m, d, ILOpCode.Call, _cm.Operators.NewGuid);
                        il.EmitCall(m, d, ILOpCode.Callvirt, idClrProp.SetMethod);


                        il.EmitBranch(ILOpCode.Br_s, endLabel);


                        il.MarkLabel(elseLabel);

                        dbLoc.EmitLoad(il);
                        updateQueryFieldPlace.EmitLoad(il);
                        il.EmitCall(m, d, ILOpCode.Callvirt, dbTextProp.Setter);

                        il.MarkLabel(endLabel);

                        var paramNumber = 0;

                        foreach (var prop in md.Properties)
                        {
                            var visitRef = false;
                            foreach (var typeInfo in prop.Types.GetOrderedFlattenTypes())
                            {
                                if (visitRef && typeInfo.type.IsReference)
                                    continue;

                                if (typeInfo.isComplex && typeInfo.type.IsReference)
                                    visitRef = true;

                                var clrProp = dtoType.GetMembers($"{prop.Name}{typeInfo.postfix}")
                                    .OfType<PropertySymbol>()
                                    .FirstOrDefault();

                                if (clrProp == null)
                                    throw new NullReferenceException(
                                        $"Clr prop with name {prop.Name}{typeInfo.postfix} not found");

                                dbLoc.EmitLoad(il);
                                il.EmitStringConstant($"p_{paramNumber++}");

                                sdpp.EmitLoad(il);
                                il.EmitCall(m, d, ILOpCode.Call, clrProp.GetMethod);
                                il.EmitOpCode(ILOpCode.Box);
                                il.EmitSymbolToken(m, d, clrProp.Type, null);

                                il.EmitCall(m, d, ILOpCode.Call, _cm.Runtime.CreateParameterHelper);
                            }
                        }

                        dbLoc.EmitLoad(il);
                        il.EmitCall(m, d, ILOpCode.Call, _ct.DbCommand.Method("ExecuteNonQuery"));
                        il.EmitOpCode(ILOpCode.Pop);
                        il.EmitRet(true);
                    });
            }

            #endregion

            #region Create()

            var createMethod = _ps.SynthesizeMethod(managerType);
            {
                var ctxParam =
                    new SpecialParameterSymbol(createMethod, _ct.AqContext, SpecialParameterSymbol.ContextName, 0);
                var ctxPS = new ParamPlace(ctxParam);

                createMethod.SetName("Create")
                    .SetAccess(Accessibility.Public)
                    .SetIsStatic(true)
                    .SetReturn(objectType)
                    .SetParameters(ctxParam)
                    .SetMethodBuilder((m, d) => il =>
                    {
                        ctxPS.EmitLoad(il);
                        il.EmitCall(m, d, ILOpCode.Newobj, dtoType.InstanceConstructors.First());
                        il.EmitCall(m, d, ILOpCode.Newobj, objectType.InstanceConstructors.First());

                        il.EmitRet(true);
                    });
            }

            #endregion

            var loadDtoMethod = _ps.SynthesizeMethod(managerType);
            {
                var ctxParam =
                    new SpecialParameterSymbol(createMethod, _ct.AqContext, SpecialParameterSymbol.ContextName, 0);
                var idParam = new SynthesizedParameterSymbol(createMethod, _ct.Guid, 1, RefKind.None, "id");
                var ctxPS = new ParamPlace(ctxParam);

                loadDtoMethod
                    .SetName("LoadDto")
                    .SetAccess(Accessibility.Public)
                    .SetIsStatic(true)
                    .SetReturn(dtoType)
                    .SetParameters(ctxParam, idParam)
                    .AddAttribute(new SynthesizedAttributeData(
                        _ct.HttpHandlerAttribute.Ctor(_ct.HttpMethodKind, _ct.String),
                        new[]
                        {
                            new TypedConstant(_ct.HttpMethodKind.Symbol, TypedConstantKind.Primitive, 0),
                            new TypedConstant(_ct.String.Symbol, TypedConstantKind.Primitive,
                                $"/{md.Name.ToCamelCase()}/get/{{id}}")
                        }.ToImmutableArray(), ImmutableArray<KeyValuePair<string, TypedConstant>>.Empty))
                    .SetMethodBuilder((m, d) => il =>
                    {
                        var dbLoc = new LocalPlace(il.DefineSynthLocal(loadDtoMethod, "dbCommand", _ct.DbCommand));
                        var paramLoc =
                            new LocalPlace(il.DefineSynthLocal(loadDtoMethod, "dbParameter", _ct.DbParameter));
                        var readerLoc = new LocalPlace(il.DefineSynthLocal(loadDtoMethod, "reader", _ct.DbReader));
                        var idPlace = new ParamPlace(idParam);


                        ctxParam.EmitLoad(il);
                        il.EmitCall(m, d, ILOpCode.Call, _cm.Runtime.CreateCommand);

                        dbLoc.EmitStore(il);

                        dbLoc.EmitLoad(il);
                        loadQueryFieldPlace.EmitLoad(il);
                        il.EmitCall(m, d, ILOpCode.Callvirt, dbTextProp.Setter);


                        dbLoc.EmitLoad(il);
                        il.EmitStringConstant($"@p0");

                        idPlace.EmitLoad(il);
                        il.EmitOpCode(ILOpCode.Box);
                        il.EmitSymbolToken(m, d, idParam.Type, null);

                        il.EmitCall(m, d, ILOpCode.Call, _cm.Runtime.CreateParameterHelper);

                        dbLoc.EmitLoad(il);
                        il.EmitCall(m, d, ILOpCode.Call, _ct.DbCommand.Method("ExecuteReader"));
                        readerLoc.EmitStore(il);

                        var lbl = new NamedLabel("<return>");

                        var dtoLoc = new LocalPlace(il.DefineSynthLocal(loadDtoMethod, "dto", dtoType));

                        il.EmitCall(m, d, ILOpCode.Newobj, dtoType.InstanceConstructors.First());
                        dtoLoc.EmitStore(il);

                        readerLoc.EmitLoad(il);
                        il.EmitCall(m, d, ILOpCode.Callvirt, _ct.DbReader.Method("Read"));

                        // if (Read())
                        il.EmitBranch(ILOpCode.Brfalse, lbl);


                        var getValueMethod = _ct.DbReader.Method("get_Item", _ct.Int32);

                        var index = 0;
                        foreach (var prop in md.Properties)
                        {
                            var visitRef = false;

                            foreach (var typeInfo in prop.Types.GetOrderedFlattenTypes())
                            {
                                if (visitRef && typeInfo.type.IsReference)
                                    continue;

                                if (typeInfo.isComplex && typeInfo.type.IsReference)
                                    visitRef = true;

                                var clrProp = dtoType.GetMembers($"{prop.Name}{typeInfo.postfix}")
                                    .OfType<PropertySymbol>()
                                    .FirstOrDefault();

                                if (clrProp == null)
                                    throw new NullReferenceException(
                                        $"Clr prop with name {prop.Name}{typeInfo.postfix} not found");

                                dtoLoc.EmitLoad(il);
                                readerLoc.EmitLoad(il);
                                il.EmitIntConstant(index++);
                                il.EmitCall(m, d, ILOpCode.Callvirt, getValueMethod);

                                if (clrProp.Type.IsValueType)
                                    il.EmitOpCode(ILOpCode.Unbox_any);
                                else
                                    il.EmitOpCode(ILOpCode.Castclass);
                                il.EmitSymbolToken(m, d, clrProp.Type, null);

                                il.EmitCall(m, d, ILOpCode.Call, clrProp.SetMethod);
                            }
                        }

                        //else
                        il.MarkLabel(lbl);


                        readerLoc.EmitLoad(il);
                        il.EmitCall(m, d, ILOpCode.Callvirt, _ct.DbReader.Method("Close"));

                        dtoLoc.EmitLoad(il);
                        il.EmitRet(false);
                    });
            }
            var typeIdField = _ps.SynthesizeField(managerType)
                .SetName("TypeId")
                .SetAccess(Accessibility.Public)
                .SetIsStatic(true)
                .AddAttribute(
                    new SynthesizedAttributeData(_ct.RuntimeInitAttribute.Ctor(_ct.RuntimeInitKind, _ct.Object),
                        new[]
                            {
                                new TypedConstant(_ct.RuntimeInitKind.Symbol, TypedConstantKind.Enum, 0),
                                new TypedConstant(_ct.String.Symbol, TypedConstantKind.Primitive, md.FullName)
                            }
                            .ToImmutableArray(), ImmutableArray<KeyValuePair<string, TypedConstant>>.Empty))
                .SetType(_ct.Int32);

            managerType.AddMember(saveMethod);
            managerType.AddMember(createMethod);
            managerType.AddMember(loadDtoMethod);
            managerType.AddMember(saveApiMethod);


            managerType.AddMember(typeIdField);
            managerType.AddMember(updateQueryfield);
            managerType.AddMember(loadQueryField);
            managerType.AddMember(insertQueryField);


            return managerType;
        }

        private NamedTypeSymbol PopulateLinkType(SMEntity md)
        {
            var linkType = _ps.GetSynthesizedType(QualifiedName.Parse($"{Namespace}.{md.Name}{LinkPostfix}", false));

            var idField = _ps.SynthesizeField(linkType)
                .SetName("id")
                .SetAccess(Accessibility.Private)
                .SetType(_ct.Guid);


            var thisPlace = new ArgPlace(linkType, 0);


            var ctor = _ps.SynthesizeConstructor(linkType)
                .SetAccess(Accessibility.Public);

            var guidParam = new SynthesizedParameterSymbol(ctor, _ct.Guid, 0, RefKind.None);
            var paramPlace = new ParamPlace(guidParam);

            var fieldPlace = new FieldPlace(idField);

            ctor
                .SetParameters(guidParam)
                .SetMethodBuilder((m, d) => il =>
                {
                    thisPlace.EmitLoad(il);
                    il.EmitCall(m, d, ILOpCode.Call, _ct.Object.Ctor());

                    thisPlace.EmitLoad(il);
                    paramPlace.EmitLoad(il);
                    fieldPlace.EmitStore(il);

                    il.EmitRet(true);
                });

            linkType.AddMember(idField);
            linkType.AddMember(ctor);

            return linkType;
        }
    }
}