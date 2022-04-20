using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics;
using System.Linq;
using System.Reflection.Metadata;
using Aquila.CodeAnalysis.CodeGen;
using Aquila.CodeAnalysis.Errors;
using Aquila.CodeAnalysis.Public;
using Aquila.CodeAnalysis.Semantics;
using Aquila.CodeAnalysis.Symbols;
using Aquila.CodeAnalysis.Symbols.Attributes;
using Aquila.CodeAnalysis.Symbols.Synthesized;
using Aquila.Metadata;
using Aquila.Syntax.Metadata;
using Microsoft.CodeAnalysis;

namespace Aquila.CodeAnalysis.Metadata;

/// <summary>
/// Provides symbols witch generated from metadata
/// </summary>
internal partial class MetadataSymbolProvider
{
    private readonly AquilaCompilation _declaredCompilation;
    private CoreTypes _ct;
    private CoreMethods _cm;
    private PlatformSymbolCollection _ps;
    private SynthesizedNamespaceSymbol _entityNamespaceSymbol;
    private readonly DiagnosticBag _diag;

    private const string ObjectPostfix = "Object";
    private const string DtoPostfix = "Dto";
    private const string ManagerPostfix = "Manager";
    private const string LinkPostfix = "Link";

    private const string Namespace = "Entity";

    private const string TableRowPostfix = "Row";
    private const string TableRowObjectPostfix = $"{TableRowPostfix}{ObjectPostfix}";
    private const string TableRowLinkPostfix = $"{TableRowPostfix}{LinkPostfix}";
    private const string TableRowDtoPostfix = $"{TableRowPostfix}{DtoPostfix}";
    private const string ObjectCollectionPostfix = $"{ObjectPostfix}Collection";
    private const string LinkCollectionPostfix = $"{LinkPostfix}Collection";

    public MetadataSymbolProvider(AquilaCompilation declaredCompilation)
    {
        _declaredCompilation = declaredCompilation;

        //force fill the core types
        _declaredCompilation.GetBoundReferenceManager();
        _diag = DiagnosticBag.GetInstance();

        _ct = _declaredCompilation.CoreTypes;
        _cm = _declaredCompilation.CoreMethods;
        _ps = _declaredCompilation.PlatformSymbolCollection;
    }

    public INamespaceSymbol EntityNamespace => _entityNamespaceSymbol;

    public void PopulateNamespaces(IEnumerable<SMEntity> mds)
    {
        _entityNamespaceSymbol =
            _ps.SynthesizeNamespace(_declaredCompilation.SourceModule.GlobalNamespace, Namespace);
    }

    [Flags]
    private enum GeneratedTypeKind
    {
        Dto,
        Manager,
        Object,
        Link,

        Collection,
    }

    private NamedTypeSymbol GetFromMetadata(SMEntityOrTable md, GeneratedTypeKind t)
    {
        if (md is SMEntity se)
            return t switch
            {
                GeneratedTypeKind.Dto => _ps.GetSynthesizedType(
                    QualifiedName.Parse($"{Namespace}.{md.Name}{DtoPostfix}", false)),
                GeneratedTypeKind.Object => _ps.GetSynthesizedType(
                    QualifiedName.Parse($"{Namespace}.{md.Name}{ObjectPostfix}", false)),
                _ => throw new ArgumentOutOfRangeException(nameof(t), t, null)
            };
        if (md is SMTable st)
        {
            return t switch
            {
                GeneratedTypeKind.Dto => _ps.GetSynthesizedType(
                    QualifiedName.Parse($"{Namespace}.{st.Parent.Name}{st.Name}{TableRowDtoPostfix}", false)),
                GeneratedTypeKind.Object => _ps.GetSynthesizedType(
                    QualifiedName.Parse($"{Namespace}.{st.Parent.Name}{st.Name}{TableRowObjectPostfix}",
                        false)),
                GeneratedTypeKind.Link => _ps.GetSynthesizedType(
                    QualifiedName.Parse($"{Namespace}.{st.Parent.Name}{st.Name}{TableRowLinkPostfix}", false)),

                GeneratedTypeKind.Dto | GeneratedTypeKind.Collection => _ps.GetSynthesizedType(
                    QualifiedName.Parse($"{Namespace}.{st.Parent.Name}{st.Name}{TableRowDtoPostfix}", false)),

                GeneratedTypeKind.Object | GeneratedTypeKind.Collection => _ps.GetSynthesizedType(
                    QualifiedName.Parse($"{Namespace}.{st.Parent.Name}{st.Name}{ObjectCollectionPostfix}",
                        false)),
                GeneratedTypeKind.Link | GeneratedTypeKind.Collection => _ps.GetSynthesizedType(
                    QualifiedName.Parse($"{Namespace}.{st.Parent.Name}{st.Name}{LinkCollectionPostfix}",
                        false)),
                _ => throw new ArgumentOutOfRangeException(nameof(t), t, null)
            };
        }

        throw new NotSupportedException();
    }

    public void PopulateTypes(IEnumerable<SMEntity> mds)
    {
        foreach (var md in mds)
        {
            if (!md.IsValid)
            {
                _diag.Add(MessageProvider.Instance
                    .CreateDiagnostic(ErrorCode.ERR_InvalidMetadataConsistance, null));

                continue;
            }

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


            //Tables
            foreach (var table in md.Tables)
            {
                _ps.SynthesizeType(_entityNamespaceSymbol, $"{md.Name}{table.Name}{TableRowObjectPostfix}")
                    .SetAccess(Accessibility.Public);

                _ps.SynthesizeType(_entityNamespaceSymbol, $"{md.Name}{table.Name}{TableRowLinkPostfix}")
                    .SetAccess(Accessibility.Public);

                _ps.SynthesizeType(_entityNamespaceSymbol, $"{md.Name}{table.Name}{TableRowDtoPostfix}")
                    .SetAccess(Accessibility.Public);

                _ps.SynthesizeType(_entityNamespaceSymbol, $"{md.Name}{table.Name}{ObjectCollectionPostfix}")
                    .SetAccess(Accessibility.Public);

                _ps.SynthesizeType(_entityNamespaceSymbol, $"{md.Name}{table.Name}{LinkCollectionPostfix}")
                    .SetAccess(Accessibility.Public);
            }
        }
    }

    public void PopulateMembers(IEnumerable<SMEntity> mds)
    {
        foreach (var md in mds)
        {
            if (!md.IsValid)
            {
                _diag.Add(MessageProvider.Instance
                    .CreateDiagnostic(ErrorCode.ERR_InvalidMetadataConsistance, null));

                continue;
            }

            var dtoType = PopulateDtoType(md);
            var objectType = PopulateObjectType(md, dtoType);
            var linkType = PopulateLinkType(md);
            var managerType = PopulateManagerType(md, dtoType, objectType, linkType);

            foreach (var table in md.Tables)
            {
                PopulateTableDtoType(md, table);
                PopulateTableObjectType(md, table);
                PopulateTableLinkType(md, table);
                PopulateTableCollection(md, table);
                PopulateTableLinkCollection(md, table);
            }
        }
    }

    private IEnumerable<(string propName, TypeSymbol type)> GetDtoPropertySchema(SMProperty prop)
    {
        var isComplexType = prop.Types.Count() > 1;

        TypeSymbol propType;
        var hasLinkProperty = false;

        foreach (var info in prop.GetOrderedFlattenTypes())
        {
            if (info.type.IsUnknown)
            {
                _diag.Add(MessageProvider.Instance
                    .CreateDiagnostic(ErrorCode.ERR_InvalidMetadataConsistance, Location.None));

                continue;
            }

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

        Debug.Assert(dtoType != null);

        var thisPlace = new ThisArgPlace(dtoType);
        var ctor = _ps.SynthesizeConstructor(dtoType);
        ctor
            .SetMethodBuilder((m, d) => (il) =>
            {
                thisPlace.EmitLoad(il);
                il.EmitCall(m, d, ILOpCode.Call, _ct.Object.Ctor());

                foreach (var mdTable in md.Tables)
                {
                    var prop = dtoType.GetMembers(mdTable.Name).OfType<PropertySymbol>().Single();
                    var propPlace = new PropertyPlace(null, prop);
                    
                    var rowDtoType = GetFromMetadata(mdTable, GeneratedTypeKind.Dto | GeneratedTypeKind.Collection);
                    var listType = _ct.List_arg1.Construct(rowDtoType);
                    
                    thisPlace.EmitLoad(il);
                    il.EmitCall(m, d, ILOpCode.Newobj, listType.Ctor());
                    propPlace.EmitStore(il);


                }
                
                il.EmitRet(true);
            });


        foreach (var prop in md.Properties)
        {
            if (!prop.IsValid)
            {
                _diag.Add(MessageProvider.Instance
                    .CreateDiagnostic(ErrorCode.ERR_InvalidMetadataConsistance, null));

                continue;
            }

            foreach (var schema in GetDtoPropertySchema(prop))
            {
                _ps.CreatePropertyWithBackingField(dtoType, schema.type, schema.propName);
            }
        }

        foreach (var mdTable in md.Tables)
        {
            var rowDtoType = GetFromMetadata(mdTable, GeneratedTypeKind.Dto | GeneratedTypeKind.Collection);
            var listType = _ct.List_arg1.Construct(rowDtoType);
            _ps.CreatePropertyWithBackingField(dtoType, listType, mdTable.Name);
        }

        dtoType.AddMember(ctor);
        return dtoType;
    }

    private NamedTypeSymbol PopulateObjectType(SMEntity md, NamedTypeSymbol dtoType)
    {
        var objectType =
            _ps.GetSynthesizedType(QualifiedName.Parse($"{Namespace}.{md.Name}{ObjectPostfix}", false));
        var linkType = _ps.GetSynthesizedType(QualifiedName.Parse($"{Namespace}.{md.Name}{LinkPostfix}", false));


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

        var thisPlace = new ArgPlace(objectType, 0);

        var dtoFieldPlace = new FieldPlace(dtoField);
        var ctxFieldPlace = new FieldPlace(ctxField);


        #region Constructor

        var ctor = _ps.SynthesizeConstructor(objectType);
        var ctxParam = new SpecialParameterSymbol(ctor, _ct.AqContext, SpecialParameterSymbol.ContextName, 0);
        var dtoParam = new SynthesizedParameterSymbol(ctor, dtoType, 1, RefKind.None, "dto");


        #region Tables

        List<(FieldSymbol fl, FieldPlace flPlace, NamedTypeSymbol collectionType, PropertySymbol dtoProp)>
            tablesSynth = new();

        var index = 2;
        foreach (var mdTable in md.Tables)
        {
            var collectionType =
                _ps.GetSynthesizedType(
                    QualifiedName.Parse($"{Namespace}.{md.Name}{mdTable.Name}{ObjectCollectionPostfix}", true));
            var objectTableRowType = _ps.GetSynthesizedType(
                QualifiedName.Parse($"{Namespace}.{md.Name}{mdTable.Name}{TableRowDtoPostfix}", true));

            //var a = ((NamedTypeSymbol)_ct.ImmutableArray_arg1.Symbol).Construct();
            // var tableParam =
            //     new SynthesizedParameterSymbol(ctor, collectionType, index++, RefKind.None, $"{mdTable.Name}");
            // var tableParamPlace = new ParamPlace(tableParam);

            var tableField = _ps.SynthesizeField(objectType);
            tableField
                .SetName(mdTable.Name + "_table")
                .SetAccess(Accessibility.Private)
                .SetType(collectionType);

            var dtoProp = dtoType.GetMembers(mdTable.Name).OfType<PropertySymbol>().Single();

            var tableFieldPlace = new FieldPlace(tableField);

            tablesSynth.Add((tableField, tableFieldPlace, collectionType, dtoProp));

            objectType.AddMember(tableField);


            var tableGetMethod = _ps.SynthesizeMethod(objectType)
                .SetName($"get_{mdTable.Name}")
                .SetReturn(collectionType)
                .SetMethodBuilder((m, d) => il =>
                {
                    thisPlace.EmitLoad(il);
                    tableFieldPlace.EmitLoad(il);
                    il.EmitRet(false);
                });

            var tableProperty = _ps.SynthesizeProperty(objectType);
            tableProperty
                .SetName(mdTable.Name)
                .SetType(collectionType)
                .SetGetMethod(tableGetMethod);

            objectType.AddMember(tableGetMethod);
            objectType.AddMember(tableProperty);
        }

        #endregion

        var constructorParameters = new ParameterSymbol[] { ctxParam, dtoParam };


        var dtoPS = new ParamPlace(dtoParam);
        var ctxPS = new ParamPlace(ctxParam);

        ctor
            .SetParameters(constructorParameters)
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

                //save each table
                foreach (var t in tablesSynth)
                {
                    var index = 0;
                    var collType = t.collectionType;

                    var propPlace = new PropertyPlace(null, t.dtoProp);

                    thisPlace.EmitLoad(il);
                    ctxPS.EmitLoad(il);

                    //load table property
                    dtoPS.EmitLoad(il);
                    propPlace.EmitLoad(il);

                    dtoPS.EmitLoad(il);
                    il.EmitCall(m, d, ILOpCode.Newobj, collType.Constructors.First());
                    t.flPlace.EmitStore(il);
                }

                il.EmitRet(true);
            });

        #endregion

        #region Props

        foreach (var prop in md.Properties)
        {
            if (!prop.IsValid)
            {
                _diag.Add(MessageProvider.Instance
                    .CreateDiagnostic(ErrorCode.ERR_InvalidMetadataConsistance, null));

                continue;
            }

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

                            if (type.type.IsReference)
                            {
                                var linkType = _ps.GetType(QualifiedName.Parse(
                                    $"{Namespace}.{type.type.GetSemantic().Name}{LinkPostfix}", true));

                                var linkCtor = linkType.Ctor(_ct.AqContext, _ct.Guid);

                                thisPlace.EmitLoad(il);
                                ctxFieldPlace.EmitLoad(il);

                                thisPlace.EmitLoad(il);
                                dtoFieldPlace.EmitLoad(il);

                                //TODO: Here we need create link (or get it from the cache)
                                il.EmitCall(m, d, ILOpCode.Call, dtoMember.GetMethod);

                                il.EmitCall(m, d, ILOpCode.Newobj, linkCtor);
                                il.EmitOpCode(ILOpCode.Box);
                                il.EmitSymbolToken(m, d, linkType, null);
                            }
                            else
                            {
                                thisPlace.EmitLoad(il);
                                dtoFieldPlace.EmitLoad(il);

                                //TODO: Here we need create link (or get it from the cache)
                                il.EmitCall(m, d, ILOpCode.Call, dtoMember.GetMethod);

                                il.EmitOpCode(ILOpCode.Box);
                                il.EmitSymbolToken(m, d, dtoMember.GetMethod.ReturnType, null);
                            }

                            il.EmitRet(false);
                            il.MarkLabel(lbl);
                        }

                        il.EmitCall(m, d, ILOpCode.Newobj, _ct.Exception.Ctor());
                        il.EmitThrow(false);
                    };
                });

                setter.SetMethodBuilder((m, d) => (il) =>
                {
                    var doneLbl = new NamedLabel("<done>");

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

                        {
                            if (typeInfo.type.IsReference)
                            {
                                var referenceIdField =
                                    underlyingPropType.GetMembers("id").OfType<FieldSymbol>().FirstOrDefault() ??
                                    throw new Exception("the Id property not found");

                                var referenceIdFieldPlace = new FieldPlace(referenceIdField);

                                referenceIdFieldPlace.EmitLoad(il);
                            }
                        }

                        //TODO: We must set the identifier to the dto if this is link
                        //Add the condition for Link type and handle
                        il.EmitCall(m, d, ILOpCode.Call, dtoMember.SetMethod);

                        thisPlace.EmitLoad(il);
                        dtoFieldPlace.EmitLoad(il);


                        if (typeInfo.type.IsPrimitive)
                        {
                            il.EmitIntConstant((int)typeInfo.type.Kind);
                        }
                        else
                        {
                            var managerType =
                                _ps.GetType(QualifiedName.Parse(
                                    $"{Namespace}.{typeInfo.type.GetSemantic().Name}{ManagerPostfix}",
                                    true));
                            var typeIDField = managerType.GetMembers("TypeId").OfType<FieldSymbol>()
                                .FirstOrDefault();
                            var fieldPlace = new FieldPlace(typeIDField, m);
                            fieldPlace.EmitLoad(il);
                        }


                        il.EmitCall(m, d, ILOpCode.Call, dtoTypeMember.SetMethod);
                        il.EmitBranch(ILOpCode.Br, doneLbl);
                        il.MarkLabel(lbl);
                    }

                    il.EmitCall(m, d, ILOpCode.Newobj, _ct.Exception.Ctor());
                    il.EmitThrow(false);

                    il.MarkLabel(doneLbl);

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

                            if (prop.Types.First().IsReference)
                            {
                                var referenceIdField =
                                    setValueParam.Type.GetMembers("id").OfType<FieldSymbol>()
                                        .FirstOrDefault() ??
                                    throw new Exception("the Id property not found");

                                var referenceIdFieldPlace = new FieldPlace(referenceIdField);

                                referenceIdFieldPlace.EmitLoad(il);
                            }

                            il.EmitCall(m, d, ILOpCode.Call, dtoProperty.SetMethod);
                            il.EmitRet(true);
                        };
                    });
            }

            objectType.AddMember(getter);
            objectType.AddMember(setter);
            objectType.AddMember(property);
        }

        var linkGetMethod = _ps.SynthesizeMethod(objectType)
            .SetName($"get_link")
            .SetReturn(linkType)
            .SetMethodBuilder((m, d) => il =>
            {
                var dtoIdProp = dtoType.GetMembers("Id").OfType<PropertySymbol>().First();
                var dtoPropPlace = new PropertyPlace(dtoFieldPlace, dtoIdProp);


                thisPlace.EmitLoad(il);
                ctxFieldPlace.EmitLoad(il);

                thisPlace.EmitLoad(il);
                //dtoFieldPlace.EmitLoad(il);

                dtoPropPlace.EmitLoad(il);

                il.EmitCall(m, d, ILOpCode.Newobj, linkType.Ctor(_ct.AqContext, _ct.Guid));

                il.EmitRet(false);
            });

        var linkProperty = _ps.SynthesizeProperty(objectType);
        linkProperty
            .SetName("link")
            .SetType(linkType)
            .SetGetMethod(linkGetMethod);

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
                    var mrgSave = managerType.GetMembers("save_dto").OfType<MethodSymbol>().FirstOrDefault() ??
                                  throw new Exception("Method save_dto not found in manager type");

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


                    foreach (var table in md.Tables)
                    {
                        var tableColType =
                            GetFromMetadata(table, GeneratedTypeKind.Object | GeneratedTypeKind.Collection);
                        var colSaveMethod = tableColType.GetMembers("save").OfType<MethodSymbol>().Single();

                        var tableClrProp = objectType.GetMembers(table.Name).OfType<PropertySymbol>().Single();
                        thisPlace.EmitLoad(il);
                        il.EmitCall(m, d, ILOpCode.Call, tableClrProp.GetMethod);
                        il.EmitCall(m, d, ILOpCode.Call, colSaveMethod);
                    }

                    il.EmitRet(true);
                };
            });

        #endregion

        #region void Delete()

        var deleteMethod = _ps.SynthesizeMethod(objectType)
            .SetName("delete")
            .SetAccess(Accessibility.Public);

        deleteMethod
            .SetMethodBuilder((m, d) =>
            {
                return (il) =>
                {
                    var managerType =
                        _ps.GetType(QualifiedName.Parse($"{Namespace}.{md.Name}{ManagerPostfix}", true));
                    var mrgDelete = managerType.GetMembers("delete").OfType<MethodSymbol>().FirstOrDefault() ??
                                    throw new Exception("Method save not found in manager type");

                    thisPlace.EmitLoad(il);
                    ctxFieldPlace.EmitLoad(il);

                    thisPlace.EmitLoad(il);
                    dtoFieldPlace.EmitLoad(il);


                    il.EmitCall(m, d, ILOpCode.Call, mrgDelete);
                    il.EmitRet(true);
                };
            });

        #endregion

        objectType.AddMember(dtoField);
        objectType.AddMember(ctxField);

        objectType.AddMember(ctor);
        objectType.AddMember(saveMethod);
        objectType.AddMember(deleteMethod);

        objectType.AddMember(linkGetMethod);
        objectType.AddMember(linkProperty);


        return objectType;
    }

    private NamedTypeSymbol PopulateLinkType(SMEntity md)
    {
        var linkType = _ps.GetSynthesizedType(QualifiedName.Parse($"{Namespace}.{md.Name}{LinkPostfix}", false));
        var managerType = _ps.GetType(QualifiedName.Parse($"{Namespace}.{md.Name}{ManagerPostfix}", true));
        var dtoType = _ps.GetType(QualifiedName.Parse($"{Namespace}.{md.Name}{DtoPostfix}", true));


        var idField = _ps.SynthesizeField(linkType)
            .SetName("id")
            .SetAccess(Accessibility.Public)
            .SetType(_ct.Guid);

        var dtoField = _ps.SynthesizeField(linkType)
            .SetName("_dto")
            .SetAccess(Accessibility.Private)
            .SetType(dtoType);

        var ctxField = _ps.SynthesizeField(linkType);
        ctxField
            .SetName(SpecialParameterSymbol.ContextName)
            .SetAccess(Accessibility.Private)
            .SetType(_ct.AqContext);

        var thisPlace = new ArgPlace(linkType, 0);


        var ctor = _ps.SynthesizeConstructor(linkType)
            .SetAccess(Accessibility.Public);

        var ctxParam = new SynthesizedParameterSymbol(ctor, _ct.AqContext, 0, RefKind.None);
        var guidParam = new SynthesizedParameterSymbol(ctor, _ct.Guid, 1, RefKind.None);

        var paramPlace = new ParamPlace(guidParam);
        var ctxPlace = new ParamPlace(ctxParam);


        var idPlace = new FieldPlace(idField);
        var ctxFieldPlace = new FieldPlace(ctxField);
        var dtoPlace = new FieldPlace(dtoField);

        ctor
            .SetParameters(ctxParam, guidParam)
            .SetMethodBuilder((m, d) => il =>
            {
                thisPlace.EmitLoad(il);
                il.EmitCall(m, d, ILOpCode.Call, _ct.Object.Ctor());

                thisPlace.EmitLoad(il);
                paramPlace.EmitLoad(il);
                idPlace.EmitStore(il);

                thisPlace.EmitLoad(il);
                ctxPlace.EmitLoad(il);
                ctxFieldPlace.EmitStore(il);

                il.EmitRet(true);
            });


        #region Reload

        var reload = _ps.SynthesizeMethod(linkType)
            .SetName("reload")
            .SetAccess(Accessibility.Public)
            .SetMethodBuilder((m, d) => il =>
            {
                var loadMethod = managerType.GetMembers("load_dto").OfType<MethodSymbol>().FirstOrDefault();

                thisPlace.EmitLoad(il);

                thisPlace.EmitLoad(il);
                ctxFieldPlace.EmitLoad(il);

                thisPlace.EmitLoad(il);
                idPlace.EmitLoad(il);

                il.EmitCall(m, d, ILOpCode.Call, loadMethod)
                    .Expect(dtoType);
                dtoPlace.EmitStore(il);

                il.EmitRet(true);
            });

        #endregion

        #region get_object

        var objectType = GetFromMetadata(md, GeneratedTypeKind.Object);
        var get_object = _ps.SynthesizeMethod(linkType)
            .SetName("get_object")
            .SetReturn(objectType)
            .SetAccess(Accessibility.Public)
            .SetMethodBuilder((m, d) => il =>
            {
                var loadMethod = managerType.GetMembers("load_object").OfType<MethodSymbol>().FirstOrDefault();

                thisPlace.EmitLoad(il);

                thisPlace.EmitLoad(il);
                ctxFieldPlace.EmitLoad(il);

                thisPlace.EmitLoad(il);
                idPlace.EmitLoad(il);

                il.EmitCall(m, d, ILOpCode.Call, loadMethod)
                    .Expect(objectType);
                
                il.EmitRet(false);
            });

        #endregion
        
        #region Props

        foreach (var prop in md.Properties)
        {
            if (!prop.IsValid)
            {
                _diag.Add(MessageProvider.Instance
                    .CreateDiagnostic(ErrorCode.ERR_InvalidMetadataConsistance, null));

                continue;
            }

            var isComplexType = prop.Types.Count() > 1;

            var getter = _ps.SynthesizeMethod(linkType);
            var property = _ps.SynthesizeProperty(linkType);

            var propType = (isComplexType)
                ? _ct.Object
                : MetadataTypeProvider.Resolve(_declaredCompilation, prop.Types.First());

            getter.SetAccess(Accessibility.Public)
                .SetName($"get_{prop.Name}")
                .SetReturn(propType);


            property.SetType(propType)
                .SetGetMethod(getter)
                .SetName(prop.Name);


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

                            var reloadLabel = new NamedLabel("<reload>");

                            thisPlace.EmitLoad(il);
                            dtoPlace.EmitLoad(il);
                            il.EmitBranch(ILOpCode.Brtrue_s, reloadLabel);
                            thisPlace.EmitLoad(il);
                            il.EmitCall(m, d, ILOpCode.Call, reload);

                            il.MarkLabel(reloadLabel);

                            thisPlace.EmitLoad(il);
                            dtoPlace.EmitLoad(il);
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

                            if (type.type.IsReference)
                            {
                                var linkType = _ps.GetType(QualifiedName.Parse(
                                    $"{Namespace}.{type.type.GetSemantic().Name}{LinkPostfix}", true));

                                var linkCtor = linkType.Ctor(_ct.AqContext, _ct.Guid);

                                thisPlace.EmitLoad(il);
                                ctxFieldPlace.EmitLoad(il);

                                thisPlace.EmitLoad(il);
                                dtoPlace.EmitLoad(il);

                                //TODO: Here we need create link (or get it from the cache)
                                il.EmitCall(m, d, ILOpCode.Call, dtoMember.GetMethod);

                                il.EmitCall(m, d, ILOpCode.Newobj, linkCtor);
                                il.EmitOpCode(ILOpCode.Box);
                                il.EmitSymbolToken(m, d, linkType, null);
                            }
                            else
                            {
                                thisPlace.EmitLoad(il);
                                dtoPlace.EmitLoad(il);

                                //TODO: Here we need create link (or get it from the cache)
                                il.EmitCall(m, d, ILOpCode.Call, dtoMember.GetMethod);

                                il.EmitOpCode(ILOpCode.Box);
                                il.EmitSymbolToken(m, d, dtoMember.GetMethod.ReturnType, null);
                            }

                            il.EmitRet(false);
                            il.MarkLabel(lbl);
                        }

                        il.EmitCall(m, d, ILOpCode.Newobj, _ct.Exception.Ctor());
                        il.EmitThrow(false);
                    };
                });
            }
            else
            {
                var dtoProperty = dtoType.GetMembers(prop.Name).OfType<PropertySymbol>().First();


                getter.SetMethodBuilder((m, d) =>
                {
                    return (il) =>
                    {
                        var reloadLabel = new NamedLabel("<reload>");

                        thisPlace.EmitLoad(il);
                        dtoPlace.EmitLoad(il);
                        il.EmitBranch(ILOpCode.Brtrue_s, reloadLabel);
                        thisPlace.EmitLoad(il);
                        il.EmitCall(m, d, ILOpCode.Call, reload);

                        il.MarkLabel(reloadLabel);

                        thisPlace.EmitLoad(il);
                        dtoPlace.EmitLoad(il);
                        il.EmitCall(m, d, ILOpCode.Call, dtoProperty.GetMethod);
                        il.EmitRet(false);
                    };
                });
            }

            linkType.AddMember(getter);
            linkType.AddMember(property);
        }

        // var linkGetMethod = _ps.SynthesizeMethod(linkType)
        //     .SetName($"get_link")
        //     .SetReturn(linkType)
        //     .SetMethodBuilder((m, d) => il =>
        //     {
        //         var dtoIdProp = dtoType.GetMembers("Id").OfType<PropertySymbol>().First();
        //         var dtoPropPlace = new PropertyPlace(dtoFieldPlace, dtoIdProp);
        //
        //
        //         thisPlace.EmitLoad(il);
        //         ctxFieldPlace.EmitLoad(il);
        //
        //         thisPlace.EmitLoad(il);
        //         //dtoFieldPlace.EmitLoad(il);
        //
        //         dtoPropPlace.EmitLoad(il);
        //
        //         il.EmitCall(m, d, ILOpCode.Newobj, linkType.Ctor(_ct.AqContext, _ct.Guid));
        //
        //         il.EmitRet(false);
        //     });
        //
        // var linkProperty = _ps.SynthesizeProperty(objectType);
        // linkProperty
        //     .SetName("link")
        //     .SetType(linkType)
        //     .SetGetMethod(linkGetMethod);

        #endregion


        linkType.AddMember(idField);
        linkType.AddMember(ctxField);
        linkType.AddMember(dtoField);

        linkType.AddMember(reload);
        linkType.AddMember(get_object);
        linkType.AddMember(ctor);

        return linkType;
    }
}