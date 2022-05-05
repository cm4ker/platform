using System;
using System.Collections.Immutable;
using System.Diagnostics;
using System.Linq;
using System.Reflection.Metadata;
using Aquila.CodeAnalysis.CodeGen;
using Aquila.CodeAnalysis.Emit;
using Aquila.CodeAnalysis.Errors;
using Aquila.CodeAnalysis.Public;
using Aquila.CodeAnalysis.Semantics;
using Aquila.CodeAnalysis.Semantics.Graph;
using Aquila.CodeAnalysis.Symbols;
using Aquila.CodeAnalysis.Symbols.Synthesized;
using Aquila.Metadata;
using Aquila.Querying;
using Aquila.Syntax.Metadata;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeGen;

namespace Aquila.CodeAnalysis.Metadata;

partial class MetadataSymbolProvider
{
    #region Table symbols

    private void PopulateTableCollection(SMEntity md, SMTable table)
    {
        var collectionType = _ps.GetSynthesizedType(QualifiedName.Parse(
            $"{Namespace}.{md.Name}{table.Name}{ObjectCollectionPostfix}",
            false));
        var rowObjectType = GetFromMetadata(table, GeneratedTypeKind.Object);
        var dtoType = GetFromMetadata(md, GeneratedTypeKind.Dto);
        var rowDtoType = GetFromMetadata(table, GeneratedTypeKind.Dto);

        var objectType = _ps.GetSynthesizedType(QualifiedName.Parse(
            $"{Namespace}.{md.Name}{ObjectPostfix}",
            false));

        var listDtoType = _ct.IEnumerable_arg1.Construct(rowDtoType);
        var thisPlace = new ArgPlace(collectionType, 0);

        #region Fields

        //Internal field
        var dtoField = _ps.SynthesizeField(collectionType);
        dtoField
            .SetName("_dto")
            .SetAccess(Accessibility.Private)
            .SetType(dtoType);

        var ctxField = _ps.SynthesizeField(collectionType);
        ctxField
            .SetName(SpecialParameterSymbol.ContextName)
            .SetAccess(Accessibility.Private)
            .SetType(_ct.AqContext);

        var innerListField = _ps.SynthesizeField(collectionType)
            .SetName("_innerList")
            .SetAccess(Accessibility.Private)
            .SetType(_ct.List_arg1.Construct(rowObjectType));

        var f_dtoPlace = new FieldPlace(dtoField);
        var f_ctxPlace = new FieldPlace(ctxField);
        var f_innerListPlace = new FieldPlace(innerListField);

        #endregion

        #region row object populate

        var rowFunc = _ps.SynthesizeMethod(collectionType);
        var p_x = new SpecialParameterSymbol(rowFunc, rowDtoType, "x", 0);
        var p_xPlace = new ParamPlace(p_x);

        rowFunc
            .SetParameters(p_x)
            .SetIsStatic(false)
            .SetName("<row_object_populate>")
            .SetReturn(rowObjectType)
            .SetMethodBuilder((m, d) => (il) =>
            {
                thisPlace.EmitLoad(il);
                f_ctxPlace.EmitLoad(il);

                p_xPlace.EmitLoad(il);

                il.EmitCall(m, d, ILOpCode.Newobj, rowObjectType.Ctor(_ct.AqContext, rowDtoType));
                il.EmitRet(false);
            });

        #endregion

        #region Constructor

        var ctor = _ps.SynthesizeConstructor(collectionType);

        var ctxParam = new SpecialParameterSymbol(ctor, _ct.AqContext, SpecialParameterSymbol.ContextName, 0);
        var dtoListParam = new SynthesizedParameterSymbol(ctor, listDtoType, 1, RefKind.None, "dtoList");
        var dtoParam = new SynthesizedParameterSymbol(ctor, dtoType, 2, RefKind.None, "dto");

        var p_ctxPlace = new ParamPlace(ctxParam);
        var p_dtoListPlace = new ParamPlace(dtoListParam);
        var p_dtoPlace = new ParamPlace(dtoParam);


        ctor
            .SetParameters(ctxParam, dtoListParam, dtoParam)
            .SetMethodBuilder((m, d) => (il) =>
            {
                thisPlace.EmitLoad(il);
                il.EmitCall(m, d, ILOpCode.Call, _ct.Object.Ctor());

                thisPlace.EmitLoad(il);
                p_ctxPlace.EmitLoad(il);
                f_ctxPlace.EmitStore(il);

                thisPlace.EmitLoad(il);
                p_dtoPlace.EmitLoad(il);
                f_dtoPlace.EmitStore(il);


                var funcCtor = _ct.Func_arg2.Construct(rowDtoType, rowObjectType).Ctor(_ct.Object, _ct.IntPtr);

                thisPlace.EmitLoad(il);
                p_dtoListPlace.EmitLoad(il);
                thisPlace.EmitLoad(il);
                il.EmitOpCode(ILOpCode.Ldftn);
                il.EmitSymbolToken(m, d, rowFunc, null);
                il.EmitCall(m, d, ILOpCode.Newobj, funcCtor);

                il.EmitCall(m, d, ILOpCode.Call, _cm.Operators.Select.Symbol.Construct(rowDtoType, rowObjectType));
                il.EmitCall(m, d, ILOpCode.Call, _cm.Operators.ToList.Symbol.Construct(rowObjectType));

                f_innerListPlace.EmitStore(il);

                il.EmitRet(true);
            });

        #endregion


        #region create()

        var createMethod = _ps.SynthesizeMethod(collectionType);

        createMethod
            .SetName("create")
            .SetIsStatic(false)
            .SetReturn(rowObjectType)
            .SetMethodBuilder(((m, d) => (il) =>
            {
                thisPlace.EmitLoad(il);
                f_ctxPlace.EmitLoad(il);

                il.EmitCall(m, d, ILOpCode.Newobj, rowDtoType.Ctor());
                il.EmitOpCode(ILOpCode.Dup);

                var parentPropety = rowDtoType.GetMembers(table.ParentProperty.Name).OfType<PropertySymbol>().Single();
                var idProperty = dtoType.GetMembers(md.IdProperty.Name).OfType<PropertySymbol>().Single();

                var resultStore = new LocalPlace(il.DefineSynthLocal(createMethod, "result", rowObjectType));

                //load Id
                thisPlace.EmitLoad(il);
                f_dtoPlace.EmitLoad(il);
                il.EmitCall(m, d, ILOpCode.Call, idProperty.GetMethod);

                //set it to the parent dto parent property
                il.EmitCall(m, d, ILOpCode.Call, parentPropety.SetMethod);

                il.EmitCall(m, d, ILOpCode.Newobj, rowObjectType.Ctor(_ct.AqContext, rowDtoType));
                resultStore.EmitStore(il);

                var addMethod = innerListField.Type.GetMembers("Add").OfType<MethodSymbol>()
                    .Where(x => x.ParameterCount == 1 && x.Parameters[0].Type == rowObjectType).Single();

                thisPlace.EmitLoad(il);
                f_innerListPlace.EmitLoad(il);
                resultStore.EmitLoad(il);
                il.EmitCall(m, d, ILOpCode.Call, addMethod);

                resultStore.EmitLoad(il);
                il.EmitRet(false);
            }));

        #endregion


        #region remove()

        var removeMethod = _ps.SynthesizeMethod(collectionType);
        {
            var rowParameter = new SynthesizedParameterSymbol(removeMethod, rowObjectType, 0, RefKind.None, "row");
            var row_place = new ParamPlace(rowParameter);
            removeMethod
                .SetName("remove")
                .SetIsStatic(false)
                .SetParameters(rowParameter)
                .SetMethodBuilder(((m, d) => (il) =>
                {
                    var removeMethod = innerListField.Type.GetMembers("Remove").OfType<MethodSymbol>()
                        .First(x => x.GetParameterCount() == 1);
                    thisPlace.EmitLoad(il);
                    f_innerListPlace.EmitLoad(il);
                    rowParameter.EmitLoad(il);
                    il.EmitCall(m, d, ILOpCode.Call, removeMethod);
                    il.EmitRet(true);
                }));
        }

        #endregion

        var clearMethod = _ps.SynthesizeMethod(collectionType);

        #region clear()

        {
            clearMethod
                .SetName("clear")
                .SetIsStatic(false)
                .SetMethodBuilder(((m, d) => (il) =>
                {
                    var clearMethod = innerListField.Type.GetMembers("Clear").OfType<MethodSymbol>()
                        .First(x => x.GetParameterCount() == 0);
                    thisPlace.EmitLoad(il);
                    f_innerListPlace.EmitLoad(il);
                    il.EmitCall(m, d, ILOpCode.Call, clearMethod);
                    il.EmitRet(true);
                }));
        }

        #endregion

        #region Save()

        var saveMethod = _ps.SynthesizeMethod(collectionType)
            .SetName("save")
            .SetAccess(Accessibility.Public)
            .SetIsStatic(false);
        {
            var paramName = _ct.DbParameter.Property("ParameterName");
            var paramValue = _ct.DbParameter.Property("Value");

            var dbParamsProp = _ct.DbCommand.Property("Parameters");

            var paramsCollectionAdd = dbParamsProp.Symbol.Type.GetMembers("Add").OfType<MethodSymbol>()
                .FirstOrDefault();

            saveMethod
                .SetMethodBuilder((m, d) => il =>
                {
                    //remove all first

                    #region delete

                    var sym = _ct.AqParamValue.AsSZArray().Symbol;
                    var arrLoc2 = new LocalPlace(il.DefineSynthLocal(saveMethod, "", sym));

                    il.EmitIntConstant(1);
                    il.EmitOpCode(ILOpCode.Newarr);
                    il.EmitSymbolToken(m, d, _ct.AqParamValue, null);
                    arrLoc2.EmitStore(il);

                    var parentClrProp = dtoType.GetMembers($"{md.IdProperty.Name}")
                        .OfType<PropertySymbol>()
                        .FirstOrDefault();

                    //Load values array
                    arrLoc2.EmitLoad(il);
                    il.EmitIntConstant(0);
                    il.EmitStringConstant(md.IdProperty.Name);

                    thisPlace.EmitLoad(il);
                    f_dtoPlace.EmitLoad(il);
                    il.EmitCall(m, d, ILOpCode.Call, parentClrProp.GetMethod);
                    il.EmitOpCode(ILOpCode.Box);
                    il.EmitSymbolToken(m, d, parentClrProp.Type, null);
                    il.EmitCall(m, d, ILOpCode.Newobj, _ct.AqParamValue.Ctor(_ct.String, _ct.Object));
                    il.EmitOpCode(ILOpCode.Stelem_ref);

                    thisPlace.EmitLoad(il);
                    f_ctxPlace.EmitLoad(il);
                    il.EmitStringConstant(table.FullName);
                    arrLoc2.EmitLoad(il);

                    il.EmitCall(m, d, ILOpCode.Call, _cm.Runtime.InvokeDelete);

                    #endregion


                    thisPlace.EmitLoad(il);
                    f_innerListPlace.EmitLoad(il);
                    var enumeratorMember = f_innerListPlace.Type.GetMembers("GetEnumerator").OfType<MethodSymbol>()
                        .FirstOrDefault() ?? throw new Exception("Can't find enumerator method");

                    var enumeratorType = enumeratorMember.ReturnType;
                    var getCurrentMethod =
                        enumeratorType.GetMembers("get_Current").OfType<MethodSymbol>().FirstOrDefault();
                    var moveNextMethod = enumeratorType.GetMembers("MoveNext").OfType<MethodSymbol>().FirstOrDefault();
                    var disposeMethod = enumeratorType.GetMembers("Dispose").OfType<MethodSymbol>()
                        .Single(x => x.GetParameterCount() == 0);

                    var enumeratorLocal = new LocalPlace(il.DefineSynthLocal(saveMethod, "enumerator", enumeratorType));
                    var currentLocal = new LocalPlace(il.DefineSynthLocal(saveMethod, "currentItem", rowObjectType));
                    var currentDtoLocal = new LocalPlace(il.DefineSynthLocal(saveMethod, "currentDtoItem", rowDtoType));

                    var f_rowDtoPlace = new FieldPlace(rowObjectType.GetMembers("_dto").OfType<FieldSymbol>().First());

                    il.EmitCall(m, d, ILOpCode.Call, enumeratorMember);
                    enumeratorLocal.EmitStore(il);

                    il.OpenLocalScope(ScopeType.TryCatchFinally);
                    il.OpenLocalScope(ScopeType.Try);


                    var conditionLabel = new NamedLabel("<cond>");
                    var startCycleLabel = new NamedLabel("<startCycle>");

                    il.EmitBranch(ILOpCode.Br, conditionLabel);
                    il.MarkLabel(startCycleLabel);

                    //cycle start
                    enumeratorLocal.EmitLoadAddress(il);
                    il.EmitCall(m, d, ILOpCode.Call, getCurrentMethod);
                    currentLocal.EmitStore(il);

                    currentLocal.EmitLoad(il);
                    f_rowDtoPlace.EmitLoad(il);
                    currentDtoLocal.EmitStore(il);


                    var arrLoc = new LocalPlace(il.DefineSynthLocal(saveMethod, "", sym));


                    #region Load prop values into array

                    void EmitLocalValuesArray()
                    {
                        var properties = table.Properties.Where(x => x.IsValid).ToImmutableArray();

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
                                IPlace dtoPlace;
                                PropertySymbol clrProp;

                                var typeInfo = flattenTypes[index];
                                if (visitRef && typeInfo.type.IsReference)
                                    continue;

                                if (typeInfo.isComplex && typeInfo.type.IsReference)
                                    visitRef = true;

                                string propName = $"{prop.Name}{typeInfo.postfix}";

                                if (prop == table.ParentProperty)
                                {
                                    dtoPlace = f_dtoPlace;
                                    clrProp = dtoType.GetMembers($"{md.IdProperty.Name}")
                                        .OfType<PropertySymbol>()
                                        .FirstOrDefault();
                                }
                                else
                                {
                                    dtoPlace = currentDtoLocal;
                                    clrProp = rowDtoType.GetMembers(propName)
                                        .OfType<PropertySymbol>()
                                        .FirstOrDefault();
                                }


                                if (clrProp == null)
                                    throw new NullReferenceException(
                                        $"Clr prop with name {propName} not found");

                                //Load values array
                                arrLoc.EmitLoad(il);
                                il.EmitIntConstant(elemIndex++);

                                //emit object
                                //dbLoc.EmitLoad(il);
                                il.EmitStringConstant(propName);

                                if (prop == table.ParentProperty)
                                    thisPlace.EmitLoad(il);

                                dtoPlace.EmitLoad(il);
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
                    thisPlace.EmitLoad(il);
                    f_ctxPlace.EmitLoad(il);
                    il.EmitStringConstant(table.FullName);
                    arrLoc.EmitLoad(il);

                    il.EmitCall(m, d, ILOpCode.Call, _cm.Runtime.InvokeInsert);

                    //cycle end

                    il.MarkLabel(conditionLabel);
                    enumeratorLocal.EmitLoadAddress(il);
                    il.EmitCall(m, d, ILOpCode.Call, moveNextMethod);
                    il.EmitBranch(ILOpCode.Brtrue, startCycleLabel);


                    //close try block
                    il.CloseLocalScope();

                    il.OpenLocalScope(ScopeType.Finally);
                    enumeratorLocal.EmitLoadAddress(il);
                    il.EmitCall(m, d, ILOpCode.Call, disposeMethod);

                    //close finaly
                    il.CloseLocalScope();

                    //close all try catch
                    il.CloseLocalScope();


                    il.EmitRet(true);
                });
        }

        #endregion

        #region Enumerator

        var getEnumMethod = innerListField.Type.GetMembers("GetEnumerator").OfType<MethodSymbol>().FirstOrDefault();
        var getEnumerotorMethod = _ps.SynthesizeMethod(collectionType)
            .SetReturn(getEnumMethod.ReturnType)
            .SetName("GetEnumerator")
            .SetAccess(Accessibility.Public)
            .SetMethodBuilder((m, d) => il =>
            {
                thisPlace.EmitLoad(il);
                f_innerListPlace.EmitLoad(il);
                il.EmitCall(m, d, ILOpCode.Call, getEnumMethod);
                il.EmitRet(false);
            });

        #endregion

        collectionType.AddMember(getEnumerotorMethod);
        collectionType.AddMember(rowFunc);
        collectionType.AddMember(ctor);
        collectionType.AddMember(createMethod);
        collectionType.AddMember(saveMethod);
        collectionType.AddMember(ctxField);
        collectionType.AddMember(dtoField);
        collectionType.AddMember(innerListField);
        collectionType.AddMember(removeMethod);
        collectionType.AddMember(clearMethod);
    }

    private void PopulateTableLinkCollection(SMEntity md, SMTable table)
    {
        var collectionType = _ps.GetSynthesizedType(QualifiedName.Parse(
            $"{Namespace}.{md.Name}{table.Name}{LinkCollectionPostfix}",
            false));
        var rowLinkType = GetFromMetadata(table, GeneratedTypeKind.Link);
        var dtoType = GetFromMetadata(md, GeneratedTypeKind.Dto);
        var rowDtoType = GetFromMetadata(table, GeneratedTypeKind.Dto);

        var listDtoType = _ct.List_arg1.Construct(rowDtoType);
        var thisPlace = new ArgPlace(collectionType, 0);

        #region Fields

        //Internal field
        var dtoField = _ps.SynthesizeField(collectionType);
        dtoField
            .SetName("_dto")
            .SetAccess(Accessibility.Private)
            .SetType(dtoType);

        var ctxField = _ps.SynthesizeField(collectionType);
        ctxField
            .SetName(SpecialParameterSymbol.ContextName)
            .SetAccess(Accessibility.Private)
            .SetType(_ct.AqContext);

        var innerListField = _ps.SynthesizeField(collectionType)
            .SetName("_innerList")
            .SetAccess(Accessibility.Private)
            .SetType(_ct.List_arg1.Construct(rowLinkType));

        var f_dtoPlace = new FieldPlace(dtoField);
        var f_ctxPlace = new FieldPlace(ctxField);
        var f_innerListPlace = new FieldPlace(innerListField);

        #endregion

        #region row object populate

        var rowFunc = _ps.SynthesizeMethod(collectionType);
        var p_x = new SpecialParameterSymbol(rowFunc, rowDtoType, "x", 0);
        var p_xPlace = new ParamPlace(p_x);

        rowFunc
            .SetParameters(p_x)
            .SetIsStatic(false)
            .SetAccess(Accessibility.Private)
            .SetName("<row_object_populate>")
            .SetReturn(rowLinkType)
            .SetMethodBuilder((m, d) => (il) =>
            {
                thisPlace.EmitLoad(il);
                f_ctxPlace.EmitLoad(il);

                p_xPlace.EmitLoad(il);

                il.EmitCall(m, d, ILOpCode.Newobj, rowLinkType.Ctor(_ct.AqContext, rowDtoType));
                il.EmitRet(false);
            });

        #endregion

        #region Constructor

        var ctor = _ps.SynthesizeConstructor(collectionType);

        var ctxParam = new SpecialParameterSymbol(ctor, _ct.AqContext, SpecialParameterSymbol.ContextName, 0);
        var dtoListParam = new SynthesizedParameterSymbol(ctor, listDtoType, 1, RefKind.None, "dtoList");
        var dtoParam = new SynthesizedParameterSymbol(ctor, dtoType, 2, RefKind.None, "dto");

        var p_ctxPlace = new ParamPlace(ctxParam);
        var p_dtoListPlace = new ParamPlace(dtoListParam);
        var p_dtoPlace = new ParamPlace(dtoParam);


        ctor
            .SetParameters(ctxParam, dtoListParam, dtoParam)
            .SetMethodBuilder((m, d) => (il) =>
            {
                thisPlace.EmitLoad(il);
                il.EmitCall(m, d, ILOpCode.Call, _ct.Object.Ctor());

                thisPlace.EmitLoad(il);
                p_ctxPlace.EmitLoad(il);
                f_ctxPlace.EmitStore(il);

                thisPlace.EmitLoad(il);
                p_dtoPlace.EmitLoad(il);
                f_dtoPlace.EmitStore(il);


                var funcCtor = _ct.Func_arg2.Construct(rowDtoType, rowLinkType).Ctor(_ct.Object, _ct.IntPtr);

                thisPlace.EmitLoad(il);
                p_dtoListPlace.EmitLoad(il);
                thisPlace.EmitLoad(il);
                il.EmitOpCode(ILOpCode.Ldftn);
                il.EmitSymbolToken(m, d, rowFunc, null);
                il.EmitCall(m, d, ILOpCode.Newobj, funcCtor);

                il.EmitCall(m, d, ILOpCode.Call, _cm.Operators.Select.Symbol.Construct(rowDtoType, rowLinkType));
                il.EmitCall(m, d, ILOpCode.Call, _cm.Operators.ToList.Symbol.Construct(rowLinkType));

                f_innerListPlace.EmitStore(il);

                il.EmitRet(true);
            });

        #endregion

        collectionType.AddMember(rowFunc);
        collectionType.AddMember(ctor);
        collectionType.AddMember(ctxField);
        collectionType.AddMember(dtoField);
        collectionType.AddMember(innerListField);
    }

    private void PopulateTableObjectType(SMEntity md, SMTable table)
    {
        var rowObjectType =
            _ps.GetSynthesizedType(QualifiedName.Parse($"{Namespace}.{md.Name}{table.Name}{TableRowObjectPostfix}",
                false));
        var rowDtoType =
            _ps.GetSynthesizedType(QualifiedName.Parse($"{Namespace}.{md.Name}{table.Name}{TableRowDtoPostfix}",
                false));
        //var linkType = _ps.GetSynthesizedType(QualifiedName.Parse($"{Namespace}.{md.Name}{LinkPostfix}", false));

        #region Fields

        //Internal field
        var dtoField = _ps.SynthesizeField(rowObjectType);
        dtoField
            .SetName("_dto")
            .SetAccess(Accessibility.Public)
            .SetType(rowDtoType);

        var ctxField = _ps.SynthesizeField(rowObjectType);
        ctxField
            .SetName(SpecialParameterSymbol.ContextName)
            .SetAccess(Accessibility.Private)
            .SetType(_ct.AqContext);

        #endregion

        #region Constructor

        var ctor = _ps.SynthesizeConstructor(rowObjectType);
        var ctxParam = new SpecialParameterSymbol(ctor, _ct.AqContext, SpecialParameterSymbol.ContextName, 0);
        var dtoParam = new SynthesizedParameterSymbol(ctor, rowDtoType, 1, RefKind.None, "dto");


        var thisPlace = new ArgPlace(rowObjectType, 0);
        var dtoPS = new ParamPlace(dtoParam);
        var ctxPS = new ParamPlace(ctxParam);

        var dtoFieldPlace = new FieldPlace(dtoField);
        var ctxFieldPlace = new FieldPlace(ctxField);


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

        foreach (var prop in table.Properties)
        {
            //reserved property
            if (prop.IsParentProperty)
            {
                continue;
            }

            if (!prop.IsValid)
            {
                _diag.Add(MessageProvider.Instance
                    .CreateDiagnostic(ErrorCode.ERR_InvalidMetadataConsistance, null));

                continue;
            }

            var isComplexType = prop.Types.Count() > 1;

            var getter = _ps.SynthesizeMethod(rowObjectType);
            var setter = _ps.SynthesizeMethod(rowObjectType);
            var property = _ps.SynthesizeProperty(rowObjectType);

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

                            var dtoMember = rowDtoType.GetMembers(dtoMemberName).OfType<PropertySymbol>().First();
                            var dtoTypeMember = rowDtoType.GetMembers(dtoTypeMemberName).OfType<PropertySymbol>()
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

                        var dtoMember = rowDtoType.GetMembers(dtoMemberName).OfType<PropertySymbol>().First();
                        var dtoTypeMember = rowDtoType.GetMembers(dtoTypeMemberName).OfType<PropertySymbol>()
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
                var dtoProperty = rowDtoType.GetMembers(prop.Name).OfType<PropertySymbol>().First();


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

            rowObjectType.AddMember(getter);
            rowObjectType.AddMember(setter);
            rowObjectType.AddMember(property);
        }

        #endregion

        rowObjectType.AddMember(dtoField);
        rowObjectType.AddMember(ctxField);

        rowObjectType.AddMember(ctor);
    }

    private void PopulateTableDtoType(SMEntity md, SMTable table)
    {
        var dtoType =
            _ps.GetSynthesizedType(QualifiedName.Parse($"{Namespace}.{md.Name}{table.Name}{TableRowDtoPostfix}",
                false));

        Debug.Assert(dtoType != null);

        var thisPlace = new ThisArgPlace(dtoType);
        var ctor = _ps.SynthesizeConstructor(dtoType);
        ctor
            .SetMethodBuilder((m, d) => (il) =>
            {
                thisPlace.EmitLoad(il);
                il.EmitCall(m, d, ILOpCode.Call, _ct.Object.Ctor());

                il.EmitRet(true);
            });


        foreach (var prop in table.Properties)
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

        dtoType.AddMember(ctor);
    }

    private void PopulateTableLinkType(SMEntity md, SMTable table)
    {
        var rowLinkType =
            _ps.GetSynthesizedType(QualifiedName.Parse($"{Namespace}.{md.Name}{table.Name}{TableRowLinkPostfix}",
                false));
        var rowDtoType =
            _ps.GetType(QualifiedName.Parse($"{Namespace}.{md.Name}{table.Name}{TableRowDtoPostfix}", true));


        var idField = _ps.SynthesizeField(rowLinkType)
            .SetName("id")
            .SetAccess(Accessibility.Private)
            .SetType(_ct.Guid);

        var dtoField = _ps.SynthesizeField(rowLinkType)
            .SetName("_dto")
            .SetAccess(Accessibility.Private)
            .SetType(rowDtoType);

        var ctxField = _ps.SynthesizeField(rowLinkType);
        ctxField
            .SetName(SpecialParameterSymbol.ContextName)
            .SetAccess(Accessibility.Private)
            .SetType(_ct.AqContext);

        var thisPlace = new ArgPlace(rowLinkType, 0);
        var dtoPlace = new FieldPlace(dtoField);

        #region Constructor

        var ctor = _ps.SynthesizeConstructor(rowLinkType);
        var ctxParam = new SpecialParameterSymbol(ctor, _ct.AqContext, SpecialParameterSymbol.ContextName, 0);
        var dtoParam = new SynthesizedParameterSymbol(ctor, rowDtoType, 1, RefKind.None, "dto");

        var dtoPS = new ParamPlace(dtoParam);
        var ctxPS = new ParamPlace(ctxParam);

        var dtoFieldPlace = new FieldPlace(dtoField);
        var ctxFieldPlace = new FieldPlace(ctxField);


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

        foreach (var prop in table.Properties)
        {
            if (!prop.IsValid)
            {
                _diag.Add(MessageProvider.Instance
                    .CreateDiagnostic(ErrorCode.ERR_InvalidMetadataConsistance, null));

                continue;
            }

            var isComplexType = prop.Types.Count() > 1;

            var getter = _ps.SynthesizeMethod(rowLinkType);
            var property = _ps.SynthesizeProperty(rowLinkType);

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

                            var dtoMember = rowDtoType.GetMembers(dtoMemberName).OfType<PropertySymbol>().First();
                            var dtoTypeMember = rowDtoType.GetMembers(dtoTypeMemberName).OfType<PropertySymbol>()
                                .First();

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
                var dtoProperty = rowDtoType.GetMembers(prop.Name).OfType<PropertySymbol>().First();


                getter.SetMethodBuilder((m, d) =>
                {
                    return (il) =>
                    {
                        thisPlace.EmitLoad(il);
                        dtoPlace.EmitLoad(il);
                        il.EmitCall(m, d, ILOpCode.Call, dtoProperty.GetMethod);
                        il.EmitRet(false);
                    };
                });
            }

            rowLinkType.AddMember(getter);
            rowLinkType.AddMember(property);
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


        rowLinkType.AddMember(idField);
        rowLinkType.AddMember(ctxField);
        rowLinkType.AddMember(dtoField);

        rowLinkType.AddMember(ctor);
    }

    #endregion
}