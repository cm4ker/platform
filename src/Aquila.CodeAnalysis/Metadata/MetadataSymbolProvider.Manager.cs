using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Reflection.Metadata;
using Aquila.CodeAnalysis.CodeGen;
using Aquila.CodeAnalysis.Emit;
using Aquila.CodeAnalysis.Errors;
using Aquila.CodeAnalysis.Semantics;
using Aquila.CodeAnalysis.Symbols;
using Aquila.CodeAnalysis.Symbols.Attributes;
using Aquila.CodeAnalysis.Symbols.Synthesized;
using Aquila.Metadata;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeGen;
using Roslyn.Utilities;

namespace Aquila.CodeAnalysis.Metadata;

internal partial class MetadataSymbolProvider
{
    private NamedTypeSymbol PopulateManagerType(SMEntity md, NamedTypeSymbol dtoType,
        NamedTypeSymbol objectType, NamedTypeSymbol linkType)
    {
        var managerType =
            _ps.GetSynthesizedType(QualifiedName.Parse($"{Namespace}.{md.Name}{ManagerPostfix}", false));

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


        #region save_api()

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
                }.ToImmutableArray(), ImmutableArray<KeyValuePair<string, TypedConstant>>.Empty))
            .AddAttribute(new SynthesizedAttributeData(
                _ct.CrudHandlerAttribute.Ctor(_ct.HttpMethodKind, _ct.String),
                new[]
                {
                    new TypedConstant(_ct.HttpMethodKind.Symbol, TypedConstantKind.Primitive, 1),
                    new TypedConstant(_ct.String.Symbol, TypedConstantKind.Primitive, md.Name.ToCamelCase())
                }.ToImmutableArray(), ImmutableArray<KeyValuePair<string, TypedConstant>>.Empty));


        {
            var saveDtoPerameter = new SynthesizedParameterSymbol(saveApiMethod, dtoType, 1, RefKind.None, "dto");
            var ctxParameter =
                new SpecialParameterSymbol(saveApiMethod, _ct.AqContext, SpecialParameterSymbol.ContextName, 0);
            var sdpp = new ParamPlace(saveDtoPerameter);
            var ctx = new ParamPlace(ctxParameter);

            var objSaveMethod = objectType.GetMembers("save").OfType<MethodSymbol>().First();

            saveApiMethod
                .SetParameters(ctxParameter, saveDtoPerameter)
                .SetMethodBuilder((m, d) => il =>
                {
                    //TODO: need rework and introduce the SerializedObject (with embedded tables)
                    // ctx.EmitLoad(il);
                    // sdpp.EmitLoad(il);
                    //
                    // il.EmitCall(m, d, ILOpCode.Newobj, objectType.Constructors.First());
                    // il.EmitCall(m, d, ILOpCode.Call, objSaveMethod);

                    il.EmitRet(true);
                });
        }

        #endregion

        #region Save()

        var saveMethod = _ps.SynthesizeMethod(managerType)
            .SetName("save_dto")
            .SetAccess(Accessibility.Public)
            .SetIsStatic(true);
        {
            var saveDtoPerameter = new SynthesizedParameterSymbol(saveMethod, dtoType, 1, RefKind.None);
            var ctxParameter = //new SynthesizedParameterSymbol(saveMethod, _ct.AqContext, 0, RefKind.None);
                new SpecialParameterSymbol(saveMethod, _ct.AqContext, SpecialParameterSymbol.ContextName, 0);
            var sdpp = new ParamPlace(saveDtoPerameter);
            var ctx = new ParamPlace(ctxParameter);


            var paramName = _ct.DbParameter.Property("ParameterName");
            var paramValue = _ct.DbParameter.Property("Value");

            var dbParamsProp = _ct.DbCommand.Property("Parameters");

            var paramsCollectionAdd = dbParamsProp.Symbol.Type.GetMembers("Add").OfType<MethodSymbol>()
                .FirstOrDefault();

            saveMethod
                .SetParameters(ctxParameter, saveDtoPerameter)
                .SetMethodBuilder((m, d) => il =>
                {
                    var dbLoc = new LocalPlace(
                        il.DefineSynthLocal(saveMethod, "dbCommand", _ct.DbCommand));
                    var paramLoc =
                        new LocalPlace(
                            il.DefineSynthLocal(saveMethod, "dbParameter", _ct.DbParameter));
                    var idClrProp =
                        dtoType.GetMembers(md.IdProperty.Name).OfType<PropertySymbol>().FirstOrDefault() ??
                        throw new Exception("The id property is null");


                    var sym = _ct.AqParamValue.AsSZArray().Symbol;
                    var arrLoc = new LocalPlace(il.DefineSynthLocal(saveMethod, "", sym));


                    //_dto.Id
                    sdpp.EmitLoad(il);
                    il.EmitCall(m, d, ILOpCode.Callvirt, idClrProp.GetMethod);

                    //Default Guid
                    var tmpLoc = new LocalPlace(il.DefineSynthLocal(saveMethod, "", _ct.Guid));
                    tmpLoc.EmitLoadAddress(il);
                    il.EmitOpCode(ILOpCode.Initobj);
                    ((ILBuilder)il).EmitSymbolToken((PEModuleBuilder)m, (DiagnosticBag)d,
                        (TypeSymbol)_ct.Guid, null);
                    tmpLoc.EmitLoad(il);
                    il.EmitCall(m, d, ILOpCode.Call, _cm.Operators.op_Equality_Guid_Guid);

                    // var tmpLocIfRes = new LocalPlace(il.DefineSynthLocal(saveMethod, "", _ct.Boolean));
                    // tmpLocIfRes.EmitStore(il);

                    var elseLabel = new NamedLabel("<e_o1>");
                    var endLabel = new NamedLabel("<end>");

                    il.EmitBranch(ILOpCode.Brfalse, elseLabel);

                    //set to id new value
                    sdpp.EmitLoad(il);
                    il.EmitCall(m, d, ILOpCode.Call, _cm.Operators.NewGuid);
                    il.EmitCall(m, d, ILOpCode.Callvirt, idClrProp.SetMethod);


                    #region Load prop values into array

                    void EmitLocalValuesArray()
                    {
                        var properties = md.Properties.Where(x => x.IsValid).ToImmutableArray();

                        il.EmitIntConstant(properties
                            .Select(x => x.GetOrderedFlattenTypes().DistinctBy(a => a.postfix).Count())
                            .Sum());
                        il.EmitOpCode(ILOpCode.Newarr);
                        il.EmitSymbolToken(m, d, (TypeSymbol)_ct.AqParamValue, null);
                        arrLoc.EmitStore(il);

                        var elemIndex = 0;
                        foreach (var prop in properties)
                        {
                            var visitRef = false;
                            var flattenTypes = prop.Types.GetOrderedFlattenTypes().ToImmutableArray();

                            for (var index = 0; index < flattenTypes.Length; index++)
                            {
                                var typeInfo = flattenTypes[index];
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

                                //Load values array
                                arrLoc.EmitLoad(il);
                                il.EmitIntConstant(elemIndex++);

                                //emit object
                                //dbLoc.EmitLoad(il);
                                il.EmitStringConstant(clrProp.Name);

                                sdpp.EmitLoad(il);
                                il.EmitCall(m, d, ILOpCode.Call, clrProp.GetMethod);
                                il.EmitOpCode(ILOpCode.Box);
                                il.EmitSymbolToken(m, d, clrProp.Type, null);

                                il.EmitCall(m, d, ILOpCode.Newobj, _ct.AqParamValue.Ctor(_ct.String, _ct.Object));

                                il.EmitOpCode(ILOpCode.Stelem_ref);
                            }
                        }
                    }

                    #endregion

                    EmitLocalValuesArray();

                    //set insert query
                    ctx.EmitLoad(il);
                    il.EmitStringConstant(md.FullName);
                    arrLoc.EmitLoad(il);

                    il.EmitCall(m, d, ILOpCode.Call, _cm.Runtime.InvokeInsert);

                    il.EmitBranch(ILOpCode.Br, endLabel);
                    il.MarkLabel(elseLabel);

                    //fill array by local values
                    EmitLocalValuesArray();
                    ctx.EmitLoad(il);
                    il.EmitStringConstant(md.FullName);
                    arrLoc.EmitLoad(il);

                    il.EmitCall(m, d, ILOpCode.Call, _cm.Runtime.InvokeUpdate);

                    il.MarkLabel(endLabel);

                    il.EmitRet(true);
                });
        }

        #endregion

        #region Delete()

        var deleteMethod = _ps.SynthesizeMethod(managerType)
            .SetName("delete")
            .SetAccess(Accessibility.Public)
            .SetIsStatic(true);
        {
            var deleteDtoPerameter = new SynthesizedParameterSymbol(saveMethod, dtoType, 1, RefKind.None);
            var ctxParameter =
                new SpecialParameterSymbol(saveMethod, _ct.AqContext, SpecialParameterSymbol.ContextName, 0);
            var ddpp = new ParamPlace(deleteDtoPerameter);
            var ctx = new ParamPlace(ctxParameter);

            deleteMethod
                .SetParameters(ctxParameter, deleteDtoPerameter)
                .SetMethodBuilder((m, d) => il =>
                {
                    var dbLoc = new LocalPlace(il.DefineSynthLocal(saveMethod, "dbCommand", _ct.DbCommand));
                    var paramLoc = new LocalPlace(il.DefineSynthLocal(saveMethod, "dbParameter", _ct.DbParameter));
                    var idClrProp =
                        dtoType.GetMembers(md.IdProperty.Name).OfType<PropertySymbol>().FirstOrDefault() ??
                        throw new Exception("The id property is null");


                    var sym = _ct.AqParamValue.AsSZArray().Symbol;
                    var arrLoc = new LocalPlace(il.DefineSynthLocal(saveMethod, "", sym));

                    il.EmitIntConstant(1);
                    il.EmitOpCode(ILOpCode.Newarr);
                    il.EmitSymbolToken(m, d, _ct.AqParamValue, null);
                    arrLoc.EmitStore(il);


                    //Load values array
                    arrLoc.EmitLoad(il);
                    il.EmitIntConstant(0);
                    il.EmitStringConstant(idClrProp.Name);

                    ddpp.EmitLoad(il);
                    il.EmitCall(m, d, ILOpCode.Call, idClrProp.GetMethod);
                    il.EmitOpCode(ILOpCode.Box);
                    il.EmitSymbolToken(m, d, idClrProp.Type, null);
                    il.EmitCall(m, d, ILOpCode.Newobj, _ct.AqParamValue.Ctor(_ct.String, _ct.Object));
                    il.EmitOpCode(ILOpCode.Stelem_ref);

                    ctx.EmitLoad(il);
                    il.EmitStringConstant(md.FullName);
                    arrLoc.EmitLoad(il);
                    il.EmitCall(m, d, ILOpCode.Call, _cm.Runtime.InvokeDelete);

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

            createMethod.SetName("create")
                .SetAccess(Accessibility.Public)
                .SetIsStatic(true)
                .SetReturn(objectType)
                .SetParameters(ctxParam)
                .SetMethodBuilder((m, d) => il =>
                {
                    var dtoLoc = new LocalPlace(il.DefineSynthLocal(createMethod, "dtoLocal", dtoType));

                    il.EmitCall(m, d, ILOpCode.Newobj, dtoType.InstanceConstructors.First());
                    dtoLoc.EmitStore(il);

                    ctxPS.EmitLoad(il);
                    dtoLoc.EmitLoad(il);

                    foreach (var table in md.Tables)
                    {
                        var objectCollectionType =
                            GetFromMetadata(table, GeneratedTypeKind.Collection | GeneratedTypeKind.Object);
                        var rowDtoType = GetFromMetadata(table, GeneratedTypeKind.Dto);

                        var colCtor = objectCollectionType.Ctor(_ct.AqContext,
                            _ct.IEnumerable_arg1.Construct(rowDtoType),
                            dtoType);
                        ctxPS.EmitLoad(il);

                        il.EmitIntConstant(0);
                        il.EmitOpCode(ILOpCode.Newarr);
                        il.EmitSymbolToken(m, d, rowDtoType, null);


                        dtoLoc.EmitLoad(il);

                        il.EmitCall(m, d, ILOpCode.Newobj, colCtor);
                    }

                    il.EmitCall(m, d, ILOpCode.Newobj, objectType.InstanceConstructors.First());

                    il.EmitRet(true);
                });
        }

        #endregion

        #region GetLink

        var getLink = _ps.SynthesizeMethod(managerType);

        getLink.SetName("get_link")
            .SetAccess(Accessibility.Public)
            .SetIsStatic(true)
            ;

        {
            var idParameter = new SynthesizedParameterSymbol(getLink, _ct.Guid, 1, RefKind.None, "id");
            var ctxParameter =
                new SpecialParameterSymbol(getLink, _ct.AqContext, SpecialParameterSymbol.ContextName, 0);
            var idP = new ParamPlace(idParameter);
            var ctx = new ParamPlace(ctxParameter);

            getLink
                .SetParameters(ctxParameter, idParameter)
                .SetReturn(linkType)
                .SetMethodBuilder((m, d) => il =>
                {
                    ctx.EmitLoad(il);
                    idP.EmitLoad(il);

                    il.EmitCall(m, d, ILOpCode.Newobj, linkType.Ctor(_ct.AqContext, _ct.Guid));

                    il.EmitRet(false);
                });
        }

        #endregion

        #region Readers

        var readers = new Dictionary<string, SynthesizedMethodSymbol>();
        /*
             Readers naming convection: reader_Postfix_delegate
             
             example: reader_invoice_dto_delegate
                      reader_invoice_nomenclatures_dto_delegate
             
             
             */

        List<SMEntityOrTable> gen = new List<SMEntityOrTable> { md };
        gen.AddRange(md.Tables);

        foreach (var g in gen)
        {
            var ddDtoType = GetFromMetadata(g, MetadataSymbolProvider.GeneratedTypeKind.Dto);

            var readerVoid = _ps.SynthesizeMethod(managerType);
            var dbParam = new SynthesizedParameterSymbol(readerVoid, _ct.DbReader, 0, RefKind.None);
            var dbp = new ParamPlace(dbParam);

            readerVoid
                .SetParameters(dbParam)
                .SetName($"reader_{g.FullName}")
                .SetIsStatic(true)
                .SetReturn(ddDtoType)
                .SetMethodBuilder((m, d) => il =>
                {
                    var dtoLoc = new LocalPlace(il.DefineSynthLocal(readerVoid, "dto", ddDtoType));

                    il.EmitCall(m, d, ILOpCode.Newobj, ddDtoType.InstanceConstructors.First());
                    dtoLoc.EmitStore(il);

                    var getValueMethod = _ct.DbReader.Method("get_Item", _ct.Int32);

                    var index = 0;
                    foreach (var prop in g.Properties)
                    {
                        if (!prop.IsValid)
                        {
                            _diag.Add(MessageProvider.Instance
                                .CreateDiagnostic(ErrorCode.ERR_InvalidMetadataConsistance, null));

                            continue;
                        }

                        var visitRef = false;

                        foreach (var typeInfo in prop.Types.GetOrderedFlattenTypes())
                        {
                            if (visitRef && typeInfo.type.IsReference)
                                continue;

                            if (typeInfo.isComplex && typeInfo.type.IsReference)
                                visitRef = true;

                            var clrProp = ddDtoType
                                .GetMembers($"{prop.Name}{typeInfo.postfix}")
                                .OfType<PropertySymbol>().FirstOrDefault<PropertySymbol>();

                            if (clrProp == null)
                                throw new NullReferenceException(
                                    $"Clr prop with name {prop.Name}{typeInfo.postfix} not found");

                            dtoLoc.EmitLoad(il);
                            dbp.EmitLoad(il);
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

                    dtoLoc.EmitLoad(il);
                    // il.EmitOpCode(ILOpCode.Box);
                    // il.EmitSymbolToken(m, d, dtoType, null);
                    il.EmitRet(false);
                });

            readers.Add(readerVoid.Name, readerVoid);
        }

        #endregion

        #region Load dto

        var loadDtoMethod = _ps.SynthesizeMethod(managerType);
        {
            var ctxParam =
                new SpecialParameterSymbol(createMethod, _ct.AqContext, SpecialParameterSymbol.ContextName, 0);
            var idParam = new SynthesizedParameterSymbol(createMethod, _ct.Guid, 1, RefKind.None, "id");
            var ctxPS = new ParamPlace(ctxParam);
            var idPl = new ParamPlace(idParam);

            loadDtoMethod
                .SetName("load_dto")
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
                .AddAttribute(new SynthesizedAttributeData(
                    _ct.CrudHandlerAttribute.Ctor(_ct.HttpMethodKind, _ct.String),
                    new[]
                    {
                        new TypedConstant(_ct.HttpMethodKind.Symbol, TypedConstantKind.Primitive, 0),
                        new TypedConstant(_ct.String.Symbol, TypedConstantKind.Primitive, md.Name.ToCamelCase())
                    }.ToImmutableArray(), ImmutableArray<KeyValuePair<string, TypedConstant>>.Empty))
                .SetMethodBuilder((m, d) => il =>
                {
                    var idClrProp =
                        dtoType.GetMembers(md.IdProperty.Name).OfType<PropertySymbol>().FirstOrDefault() ??
                        throw new Exception("The id property is null");

                    var sym = _ct.AqParamValue.AsSZArray().Symbol;
                    var resultArrSym = ((NamedTypeSymbol)_ct.ImmutableArray_arg1.Symbol).Construct(dtoType);
                    var arrLoc = new LocalPlace(il.DefineSynthLocal(loadDtoMethod, "", sym));
                    var resultArrLoc = new LocalPlace(il.DefineSynthLocal(loadDtoMethod, "", resultArrSym));

                    il.EmitIntConstant(1);
                    il.EmitOpCode(ILOpCode.Newarr);
                    il.EmitSymbolToken(m, d, (TypeSymbol)_ct.AqParamValue, null);
                    arrLoc.EmitStore(il);

                    //Load values array
                    arrLoc.EmitLoad(il);
                    il.EmitIntConstant(0);
                    il.EmitStringConstant(md.IdProperty.Name);

                    idPl.EmitLoad(il);
                    il.EmitOpCode(ILOpCode.Box);
                    il.EmitSymbolToken(m, d, idPl.Type, null);
                    il.EmitCall(m, d, ILOpCode.Newobj, _ct.AqParamValue.Ctor(_ct.String, _ct.Object));
                    il.EmitOpCode(ILOpCode.Stelem_ref);

                    ctxPS.EmitLoad(il);
                    il.EmitStringConstant(md.FullName);
                    arrLoc.EmitLoad(il);
                    //load return_void function

                    var func = _ct.AqReadDelegate.Construct(dtoType);

                    // Func<,>(object @object, IntPtr method)
                    var func_ctor = func.InstanceConstructors.Single(m =>
                        m.ParameterCount == 2 &&
                        m.Parameters[0].Type.SpecialType == SpecialType.System_Object &&
                        m.Parameters[1].Type.SpecialType == SpecialType.System_IntPtr
                    );

                    il.EmitNullConstant();
                    il.EmitOpCode(ILOpCode.Ldftn);
                    il.EmitSymbolToken(m, d, readers[$"reader_{md.FullName}"], null);
                    il.EmitCall(m, d, ILOpCode.Newobj, func_ctor);

                    var invokeSelect = _cm.Runtime.InvokeSelect.Symbol.Construct(dtoType);
                    var get_item_sym = invokeSelect.ReturnType.GetMembers("get_Item").OfType<MethodSymbol>()
                        .First<MethodSymbol>();

                    il.EmitCall(m, d, ILOpCode.Call, invokeSelect);
                    resultArrLoc.EmitStore(il);

                    resultArrLoc.EmitLoadAddress(il);
                    il.EmitIntConstant(0);
                    il.EmitCall(m, d, ILOpCode.Call, get_item_sym);

                    il.EmitRet(false);
                });
        }

        #endregion

        #region load_dto_collection

        MethodSymbol generateSymFor(SMTable table)
        {
            var loadDtoCollectionMethod = _ps.SynthesizeMethod(managerType);
            {
                var ctxParam =
                    new SpecialParameterSymbol(createMethod, _ct.AqContext, SpecialParameterSymbol.ContextName, 0);
                var idParam = new SynthesizedParameterSymbol(createMethod, _ct.Guid, 1, RefKind.None, "id");
                var ctxPS = new ParamPlace(ctxParam);
                var idPl = new ParamPlace(idParam);
                var rowDtoType = GetFromMetadata(table, GeneratedTypeKind.Dto | GeneratedTypeKind.Collection);


                var resultType = _ct.IEnumerable_arg1.Construct(rowDtoType);

                loadDtoCollectionMethod
                    .SetName($"load_{table.FullName}")
                    .SetAccess(Accessibility.Public)
                    .SetIsStatic(true)
                    .SetReturn(resultType)
                    .SetParameters(ctxParam, idParam)
                    .SetMethodBuilder((m, d) => il =>
                    {
                        var idClrProp =
                            dtoType.GetMembers(md.IdProperty.Name).OfType<PropertySymbol>().FirstOrDefault() ??
                            throw new Exception("The id property is null");

                        var sym = _ct.AqParamValue.AsSZArray().Symbol;
                        var resultArrSym = ((NamedTypeSymbol)_ct.ImmutableArray_arg1.Symbol).Construct(dtoType);
                        var arrLoc = new LocalPlace(il.DefineSynthLocal(loadDtoCollectionMethod, "", sym));
                        var resultArrLoc =
                            new LocalPlace(il.DefineSynthLocal(loadDtoCollectionMethod, "", resultArrSym));

                        il.EmitIntConstant(1);
                        il.EmitOpCode(ILOpCode.Newarr);
                        il.EmitSymbolToken(m, d, (TypeSymbol)_ct.AqParamValue, null);
                        arrLoc.EmitStore(il);

                        //Load values array
                        arrLoc.EmitLoad(il);
                        il.EmitIntConstant(0);
                        il.EmitStringConstant(table.ParentProperty.Name);

                        idPl.EmitLoad(il);
                        il.EmitOpCode(ILOpCode.Box);
                        il.EmitSymbolToken(m, d, idPl.Type, null);
                        il.EmitCall(m, d, ILOpCode.Newobj, _ct.AqParamValue.Ctor(_ct.String, _ct.Object));
                        il.EmitOpCode(ILOpCode.Stelem_ref);

                        ctxPS.EmitLoad(il);
                        il.EmitStringConstant(table.FullName);
                        arrLoc.EmitLoad(il);
                        //load return_void function

                        var func = _ct.AqReadDelegate.Construct(rowDtoType);

                        // Func<,>(object @object, IntPtr method)
                        var func_ctor = func.InstanceConstructors.Single(m =>
                            m.ParameterCount == 2 &&
                            m.Parameters[0].Type.SpecialType == SpecialType.System_Object &&
                            m.Parameters[1].Type.SpecialType == SpecialType.System_IntPtr
                        );

                        il.EmitNullConstant();
                        il.EmitOpCode(ILOpCode.Ldftn);
                        il.EmitSymbolToken(m, d, readers[$"reader_{table.FullName}"], null);
                        il.EmitCall(m, d, ILOpCode.Newobj, func_ctor);

                        var invokeSelect = _cm.Runtime.InvokeSelect.Symbol.Construct(rowDtoType);
                        var get_item_sym = invokeSelect.ReturnType.GetMembers("get_Item").OfType<MethodSymbol>()
                            .First<MethodSymbol>();

                        il.EmitCall(m, d, ILOpCode.Call, invokeSelect);
                        il.EmitRet(false);
                    });
            }

            return loadDtoCollectionMethod;
        }

        foreach (var table in md.Tables)
        {
            managerType.AddMember(generateSymFor(table));
        }

        #endregion

        #region load_object

        var loadObjectMethod = _ps.SynthesizeMethod(managerType);
        {
            var ctxParam =
                new SpecialParameterSymbol(loadObjectMethod, _ct.AqContext, SpecialParameterSymbol.ContextName, 0);
            var idParam = new SynthesizedParameterSymbol(loadObjectMethod, _ct.Guid, 1, RefKind.None, "id");
            var ctxPS = new ParamPlace(ctxParam);
            var idPl = new ParamPlace(idParam);

            loadObjectMethod
                .SetName("load_object")
                .SetAccess(Accessibility.Public)
                .SetIsStatic(true)
                .SetReturn(objectType)
                .SetParameters(ctxParam, idParam)
                .SetMethodBuilder((m, d) => il =>
                {
                    var dtoPlace = new LocalPlace(il.DefineSynthLocal(loadObjectMethod, "dto", dtoType));

                    ctxPS.EmitLoad(il);
                    idPl.EmitLoad(il);
                    il.EmitCall(m, d, ILOpCode.Call, loadDtoMethod);
                    dtoPlace.EmitStore(il);


                    List<IPlace> colPlaces = new List<IPlace>();


                    foreach (var table in md.Tables)
                    {
                        var index = 0;
                        var loadColMethod = managerType.GetMembers($"load_{table.FullName}").OfType<MethodSymbol>()
                            .Single();
                        var collType = GetFromMetadata(table, GeneratedTypeKind.Collection | GeneratedTypeKind.Object);

                        var colPlace =
                            new LocalPlace(il.DefineSynthLocal(loadObjectMethod, $"col_{index++}", collType));

                        ctxPS.EmitLoad(il);
                        ctxPS.EmitLoad(il);
                        idPl.EmitLoad(il);
                        il.EmitCall(m, d, ILOpCode.Call, loadColMethod);
                        //colPlace.EmitStore(il);
                        dtoPlace.EmitLoad(il);
                        il.EmitCall(m, d, ILOpCode.Newobj, collType.Constructors.First());
                        colPlace.EmitStore(il);
                        colPlaces.Add(colPlace);
                    }

                    ctxPS.EmitLoad(il);
                    dtoPlace.EmitLoad(il);

                    foreach (var place in colPlaces)
                    {
                        place.EmitLoad(il);
                    }

                    il.EmitCall(m, d, ILOpCode.Newobj, objectType.Constructors.First());


                    il.EmitRet(false);
                });
        }

        #endregion

        managerType.AddMember(saveMethod);
        managerType.AddMember(deleteMethod);
        managerType.AddMember(loadObjectMethod);
        //managerType.AddMember(readerVoid);

        foreach (var r in readers)
        {
            managerType.AddMember(r.Value);
        }

        managerType.AddMember(saveApiMethod);
        managerType.AddMember(createMethod);
        managerType.AddMember(loadDtoMethod);
        managerType.AddMember(getLink);


        managerType.AddMember(typeIdField);

        return managerType;
    }
}