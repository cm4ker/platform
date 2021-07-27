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
using Aquila.Metadata;
using Aquila.Syntax.Syntax;
using Microsoft.CodeAnalysis;

namespace Aquila.Syntax.Metadata
{
    internal class MetadataSymbolProvider
    {
        private readonly AquilaCompilation _declaredCompilation;
        private CoreTypes _ct;
        private PlatformSymbolCollection _ps;

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
            _ps = _declaredCompilation.PlatformSymbolCollection;
        }

        public void PopulateTypes(IEnumerable<SMEntity> mds)
        {
            foreach (var md in mds)
            {
                //Dto
                _ps.SynthesizeType()
                    .SetName($"{md.Name}{DtoPostfix}")
                    .SetAccess(Accessibility.Public)
                    .SetNamespace(Namespace);

                //Object
                _ps.SynthesizeType()
                    .SetName($"{md.Name}{ObjectPostfix}")
                    .SetAccess(Accessibility.Public)
                    .SetNamespace(Namespace)
                    .AddAttribute(new SynthesizedAttributeData(_ct.EntityAttribute.Ctor(),
                        ImmutableArray<TypedConstant>.Empty,
                        ImmutableArray<KeyValuePair<string, TypedConstant>>.Empty));

                //Manager
                _ps.SynthesizeType()
                    .SetName($"{md.Name}{ManagerPostfix}")
                    .SetNamespace(Namespace)
                    .SetIsStatic(true);

                //Link
                _ps.SynthesizeType()
                    .SetName($"{md.Name}{LinkPostfix}")
                    .SetAccess(Accessibility.Public)
                    .SetNamespace(Namespace)
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

        private NamedTypeSymbol PopulateDtoType(SMEntity md)
        {
            var dtoType = _ps.GetSynthesizedType(QualifiedName.Parse($"{Namespace}.{md.Name}{DtoPostfix}", false));

            var ctor = _ps.SynthesizeConstructor(dtoType);
            ctor
                .SetMethodBuilder((m, d) => (il) => { il.EmitRet(true); });

            foreach (var prop in md.Properties)
            {
                var isComplexType = prop.Types.Count() > 1;

                TypeSymbol propType;

                if (isComplexType)
                {
                    _ps.CreatePropertyWithBackingField(dtoType, _ct.Int32, $"{prop.Name}_T");

                    var hasLinkPropery = false;

                    foreach (var type in prop.Types)
                    {
                        propType = MetadataTypeProvider.Resolve(_declaredCompilation, type);

                        var postfix = GetPropertyPostfix(type);

                        //if we already have link property then we don't need it again
                        if (propType.IsLink() && hasLinkPropery)
                            continue;

                        if (propType.IsLink())
                        {
                            hasLinkPropery = true;
                            _ps.CreatePropertyWithBackingField(dtoType, _ct.Guid, $"{prop.Name}_{postfix}");
                        }
                        else
                            _ps.CreatePropertyWithBackingField(dtoType, propType, $"{prop.Name}_{postfix}");
                    }
                }
                else
                {
                    propType = MetadataTypeProvider.Resolve(_declaredCompilation, prop.Types.First());

                    if (propType is MissingMetadataTypeSymbol)
                        throw new Exception($"Error during build metadata unknown type: {propType}");

                    if (propType.IsLink())
                        _ps.CreatePropertyWithBackingField(dtoType, _ct.Guid, prop.Name);
                    else
                        _ps.CreatePropertyWithBackingField(dtoType, propType, prop.Name);
                }
            }

            dtoType.AddMember(ctor);
            return dtoType;
        }

        private NamedTypeSymbol PopulateObjectType(SMEntity md, NamedTypeSymbol dtoType)
        {
            var objectType =
                _ps.GetSynthesizedType(QualifiedName.Parse($"{Namespace}.{md.Name}{ObjectPostfix}", false));

            //Internal field
            var dtoField = _ps.SynthesizeField(objectType);
            dtoField
                .SetName("_dto")
                .SetAccess(Accessibility.Private)
                .SetType(dtoType);


            var ctor = _ps.SynthesizeConstructor(objectType);
            var dtoParam = new SynthesizedParameterSymbol(ctor, dtoType, 0, RefKind.None, "dto");

            var thisPlace = new ArgPlace(objectType, 0);
            var paramPlace = new ParamPlace(dtoParam);
            var dtoFieldPlace = new FieldPlace(dtoField);

            ctor
                .SetParameters(dtoParam)
                .SetMethodBuilder((m, d) => (il) =>
                {
                    thisPlace.EmitLoad(il);
                    paramPlace.EmitLoad(il);
                    dtoFieldPlace.EmitStore(il);

                    il.EmitRet(true);
                });


            foreach (var prop in md.Properties)
            {
                var isComplexType = prop.Types.Count() > 1;

                var getter = _ps.SynthesizeMethod(objectType);
                var setter = _ps.SynthesizeMethod(objectType);

                var propType = (isComplexType)
                    ? _ct.Object
                    : MetadataTypeProvider.Resolve(_declaredCompilation, prop.Types.First());

                getter.SetAccess(Accessibility.Public)
                    .SetName($"get_{prop.Name}")
                    .SetReturn(propType);

                setter.SetAccess(Accessibility.Public)
                    .SetName($"set_{prop.Name}");

                var param = new SynthesizedParameterSymbol(setter, propType, 0, RefKind.None);
                setter.SetParameters(param);

                var setValueParam = new ParamPlace(param);

                if (isComplexType)
                {
                    getter.SetMethodBuilder((m, d) =>
                    {
                        return (il) =>
                        {
                            foreach (var type in prop.Types)
                            {
                                var underlyingPropType = MetadataTypeProvider.Resolve(_declaredCompilation, type);
                                var postfix = GetPropertyPostfix(type);

                                var dtoMemberName = $"{prop.Name}_{postfix}";
                                var dtoTypeMemberName = $"{prop.Name}_T";

                                var dtoMember = dtoType.GetMembers(dtoMemberName).OfType<PropertySymbol>().First();
                                var dtoTypeMember = dtoType.GetMembers(dtoTypeMemberName).OfType<PropertySymbol>()
                                    .First();

                                thisPlace.EmitLoad(il);
                                dtoFieldPlace.EmitLoad(il);
                                il.EmitCall(m, d, ILOpCode.Call, dtoTypeMember.GetMethod);

                                //TODO: here load value form manager or for builtin types from the constants
                                il.EmitIntConstant(1);

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
                        foreach (var type in prop.Types)
                        {
                            var underlyingPropType = MetadataTypeProvider.Resolve(_declaredCompilation, type);
                            var postfix = GetPropertyPostfix(type);

                            var dtoMemberName = $"{prop.Name}_{postfix}";
                            var dtoTypeMemberName = $"{prop.Name}_T";

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
            }

            var saveMethod = _ps.SynthesizeMethod(objectType)
                .SetName("Save")
                .SetAccess(Accessibility.Public);

            saveMethod
                .SetMethodBuilder((m, d) =>
                {
                    return (il) =>
                    {
                        var managerType =
                            _ps.GetType(QualifiedName.Parse($"{Namespace}.{md.Name}{ManagerPostfix}", true));
                        var mrgSave = managerType.GetMembers("Save").OfType<MethodSymbol>().FirstOrDefault() ??
                                      throw new Exception("Method save not found in manager type");

                        thisPlace.EmitLoad(il);
                        dtoFieldPlace.EmitLoad(il);
                        il.EmitCall(m, d, ILOpCode.Call, mrgSave);
                        il.EmitRet(true);
                    };
                });

            objectType.AddMember(dtoField);
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
            var saveQueryField = _ps.SynthesizeField(managerType)
                    .SetIsStatic(true)
                    .SetName($"{md.Name}SaveQuery")
                    .SetAccess(Accessibility.Public)
                    .SetReadOnly(false)
                    .SetType(_ct.String)
                    .AddAttribute(new SynthesizedAttributeData(_ct.QueryAttribute.Ctor(),
                        ImmutableArray<TypedConstant>.Empty, ImmutableArray<KeyValuePair<string, TypedConstant>>.Empty))
                ;

            //plaint entity save query text
            var loadQueryField = _ps.SynthesizeField(managerType)
                    .SetIsStatic(true)
                    .SetName($"{md.Name}LoadQuery")
                    .SetAccess(Accessibility.Public)
                    .SetReadOnly(false)
                    .SetType(_ct.String)
                    .AddAttribute(new SynthesizedAttributeData(_ct.QueryAttribute.Ctor(),
                        ImmutableArray<TypedConstant>.Empty, ImmutableArray<KeyValuePair<string, TypedConstant>>.Empty))
                ;


            var saveMethod = _ps.SynthesizeMethod(managerType)
                .SetName("Save")
                .SetAccess(Accessibility.Public)
                .SetIsStatic(true);

            var saveDtoPerameter = new SynthesizedParameterSymbol(saveMethod, dtoType, 0, RefKind.None);
            var sdpp = new ParamPlace(saveDtoPerameter);

            saveMethod
                .SetParameters(saveDtoPerameter)
                .SetMethodBuilder((m, d) => il =>
                {
                    //TODO: handle saving
                    il.EmitRet(true);
                });

            var createMethod = _ps.SynthesizeMethod(managerType)
                .SetName("Create")
                .SetAccess(Accessibility.Public)
                .SetIsStatic(true)
                .SetReturn(objectType)
                .SetMethodBuilder((m, d) => il =>
                {
                    il.EmitCall(m, d, ILOpCode.Newobj, dtoType.InstanceConstructors.First());
                    il.EmitCall(m, d, ILOpCode.Newobj, objectType.InstanceConstructors.First());

                    il.EmitRet(true);
                });

            var loadMethod = _ps.SynthesizeMethod(managerType)
                .SetName("Load")
                .SetAccess(Accessibility.Public)
                .SetIsStatic(true)
                .SetReturn(objectType)
                .SetMethodBuilder((m, d) => il =>
                {
                    il.EmitNullConstant();
                    il.EmitRet(true);
                });

            var typeIdField = _ps.SynthesizeField(managerType)
                .SetName("TypeId")
                .SetAccess(Accessibility.Public)
                .SetIsStatic(true)
                .SetType(_ct.Int32);

            managerType.AddMember(saveMethod);
            managerType.AddMember(createMethod);
            managerType.AddMember(loadMethod);

            managerType.AddMember(typeIdField);
            managerType.AddMember(saveQueryField);
            managerType.AddMember(loadQueryField);

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
                    paramPlace.EmitLoad(il);
                    fieldPlace.EmitStore(il);

                    il.EmitRet(true);
                });

            linkType.AddMember(idField);
            linkType.AddMember(ctor);

            return linkType;
        }


        private static string GetPropertyPostfix(SMType type)
        {
            var postfix = type.Kind switch
            {
                SMTypeKind.Int => "N",
                SMTypeKind.DateTime => "D",
                SMTypeKind.Binary => "B",
                SMTypeKind.String => "S",
                SMTypeKind.Guid => "G",
                SMTypeKind.Reference => "R",
                _ => throw new NotImplementedException()
            };
            return postfix;
        }
    }
}