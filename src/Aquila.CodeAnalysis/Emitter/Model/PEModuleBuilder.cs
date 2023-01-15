using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics;
using System.Linq;
using Aquila.CodeAnalysis.Emitter;
using Aquila.CodeAnalysis.Symbols;
using Aquila.CodeAnalysis.Symbols.Attributes;
using Aquila.CodeAnalysis.Symbols.PE;
using Aquila.CodeAnalysis.Symbols.Synthesized;
using Microsoft.CodeAnalysis.CodeGen;
using Microsoft.CodeAnalysis.Emit;
using Roslyn.Utilities;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.PooledObjects;
using Cci = Microsoft.Cci;
using Microsoft.CodeAnalysis.Emit.NoPia;
using Aquila.CodeAnalysis.Utilities;
using Microsoft.CodeAnalysis.Symbols;
using ExceptionUtilities = Aquila.CodeAnalysis.Utilities.ExceptionUtilities;
using ReferenceEqualityComparer = Roslyn.Utilities.ReferenceEqualityComparer;


namespace Aquila.CodeAnalysis.Emit
{
    internal abstract partial class PEModuleBuilder : CommonPEModuleBuilder, ITokenDeferral
    {
        private readonly SourceModuleSymbol _sourceModule;
        private readonly AquilaCompilation _compilation;

        private readonly EmitOptions _emitOptions;

        /// <summary>
        /// Gets script type containing entry point and additional assembly level symbols.
        /// </summary>
        internal SynthesizedEntryPointTypeSymbol EntryPointType { get; }

        /// <summary>
        /// Manages synthesized methods and fields.
        /// </summary>
        public SynthesizedManager SynthesizedManager { get; }

        Cci.ICustomAttribute _debuggableAttribute,
            _targetFrameworkAttribute,
            _aquilaTargetPlatformAttribute,
            _assemblyinformationalversionAttribute;

        protected readonly ConcurrentDictionary<Aquila.CodeAnalysis.Symbols.Symbol, Cci.IModuleReference>
            AssemblyOrModuleSymbolToModuleRefMap =
                new ConcurrentDictionary<Aquila.CodeAnalysis.Symbols.Symbol, Cci.IModuleReference>();

        readonly ConcurrentDictionary<Aquila.CodeAnalysis.Symbols.Symbol, object> _genericInstanceMap =
            new ConcurrentDictionary<Aquila.CodeAnalysis.Symbols.Symbol, object>();

        readonly AquilaRootModuleType _rootModuleType = new AquilaRootModuleType();
        PrivateImplementationDetails _privateImplementationDetails;

        HashSet<string>
            _namesOfTopLevelTypes; // initialized with set of type names within first call to GetTopLevelTypes()

        internal readonly CommonModuleCompilationState CompilationState;

        /// <summary>
        /// Builders for synthesized static constructors.
        /// </summary>
        readonly ConcurrentDictionary<Cci.ITypeDefinition, ILBuilder> _cctorBuilders =
            new ConcurrentDictionary<Cci.ITypeDefinition, ILBuilder>(ReferenceEqualityComparer.Instance);

        protected PEModuleBuilder(
            AquilaCompilation compilation,
            SourceModuleSymbol sourceModule,
            Cci.ModulePropertiesForSerialization serializationProperties,
            IEnumerable<ResourceDescription> manifestResources,
            OutputKind outputKind,
            EmitOptions emitOptions)
            : base(manifestResources, emitOptions, outputKind, serializationProperties, compilation)
        {
            Debug.Assert(sourceModule != null);
            Debug.Assert(serializationProperties != null);

            _compilation = compilation;
            _sourceModule = sourceModule;
            _emitOptions = emitOptions;
            this.CompilationState = new CommonModuleCompilationState();
            this.SynthesizedManager = new SynthesizedManager(this);
            this.EntryPointType = new SynthesizedEntryPointTypeSymbol(_compilation);

            //
            AssemblyOrModuleSymbolToModuleRefMap.Add(sourceModule, this);
        }


        #region PEModuleBuilder

        internal MetadataConstant CreateConstant(
            TypeSymbol type,
            object value,
            SyntaxNode syntaxNodeOpt,
            DiagnosticBag diagnostics)
        {
            return new MetadataConstant(Translate(type, syntaxNodeOpt, diagnostics), value);
        }

        #endregion

        #region Synthesized

        /// <summary>
        /// Gets enumeration of synthesized fields for <paramref name="container"/>.
        /// </summary>
        /// <param name="container">Containing type symbol.</param>
        /// <returns>Enumeration of synthesized fields.</returns>
        public IEnumerable<FieldSymbol> GetSynthesizedFields(Cci.ITypeDefinition container) =>
            SynthesizedManager.GetMembers<FieldSymbol>(container);

        /// <summary>
        /// Gets enumeration of synthesized properties for <paramref name="container"/>.
        /// </summary>
        /// <param name="container">Containing type symbol.</param>
        /// <returns>Enumeration of synthesized properties.</returns>
        public IEnumerable<PropertySymbol> GetSynthesizedProperties(Cci.ITypeDefinition container) =>
            SynthesizedManager.GetMembers<PropertySymbol>(container);

        /// <summary>
        /// Gets enumeration of synthesized methods for <paramref name="container"/>.
        /// </summary>
        /// <param name="container">Containing type symbol.</param>
        /// <returns>Enumeration of synthesized methods.</returns>
        public IEnumerable<MethodSymbol> GetSynthesizedMethods(Cci.ITypeDefinition container) =>
            SynthesizedManager.GetMembers<MethodSymbol>(container);

        /// <summary>
        /// Gets enumeration of synthesized nested types for <paramref name="container"/>.
        /// </summary>
        /// <param name="container">Containing type symbol.</param>
        /// <returns>Enumeration of synthesized nested types.</returns>
        public IEnumerable<TypeSymbol> GetSynthesizedTypes(Cci.ITypeDefinition container) =>
            SynthesizedManager.GetMembers<TypeSymbol>(container);

        internal override ImmutableDictionary<ISymbolInternal, ImmutableArray<ISymbolInternal>>
            GetAllSynthesizedMembers()
        {
            throw new NotImplementedException();
        }

        #endregion

        internal SourceModuleSymbol SourceModule => _sourceModule;

        public ArrayMethods ArrayMethods
        {
            get { throw new NotImplementedException(); }
        }

        public sealed override IEnumerable<Cci.ICustomAttribute> GetSourceAssemblyAttributes(bool isRefAssembly)
        {
            // [Debuggable(DebuggableAttribute.DebuggingModes.Default | DebuggableAttribute.DebuggingModes.DisableOptimizations)]
            if (_compilation.Options.DebugPlusMode)
            {
                if (_debuggableAttribute == null)
                {
                    var debuggableAttrCtor =
                        (MethodSymbol)this.Compilation.GetWellKnownTypeMember(WellKnownMember
                            .System_Diagnostics_DebuggableAttribute__ctorDebuggingModes);
                    _debuggableAttribute = new SynthesizedAttributeData(debuggableAttrCtor,
                        ImmutableArray.Create(new TypedConstant(Compilation.CoreTypes.Int32.Symbol,
                            TypedConstantKind.Primitive,
                            DebuggableAttribute.DebuggingModes.Default |
                            DebuggableAttribute.DebuggingModes.DisableOptimizations)),
                        ImmutableArray<KeyValuePair<string, TypedConstant>>.Empty);
                }

                yield return _debuggableAttribute;
            }

            // [assembly: TargetFramework(".NETCoreApp,Version=v3.1", FrameworkDisplayName = "")]
            if (_targetFrameworkAttribute == null)
            {
                var targetFrameworkType =
                    (NamedTypeSymbol)this.Compilation.GetTypeByMetadataName(
                        "System.Runtime.Versioning.TargetFrameworkAttribute");
                string targetFramework = _compilation.Options.TargetFramework;

                _targetFrameworkAttribute = new SynthesizedAttributeData(targetFrameworkType.Constructors[0],
                    ImmutableArray.Create(new TypedConstant(Compilation.CoreTypes.String.Symbol,
                        TypedConstantKind.Primitive, targetFramework)),
                    ImmutableArray.Create(new KeyValuePair<string, TypedConstant>("FrameworkDisplayName",
                        new TypedConstant(Compilation.CoreTypes.String.Symbol, TypedConstantKind.Primitive, ""))));
            }

            yield return _targetFrameworkAttribute;

            //[assembly: TargetPlatform("1.0.0.0")]
            if (_aquilaTargetPlatformAttribute == null)
            {
                var targetAttributeCtor = this.Compilation.AquilaCorLibrary
                    .GetTypeByMetadataName(CoreTypes.AquilaTargetPlatformAttributeFullName).InstanceConstructors
                    .First();
                _aquilaTargetPlatformAttribute = new SynthesizedAttributeData(targetAttributeCtor,
                    ImmutableArray.Create(new TypedConstant(Compilation.CoreTypes.String.Symbol,
                        TypedConstantKind.Primitive,
                        targetAttributeCtor.ContainingAssembly.Identity.Version.ToString())), 
                    ImmutableArray<KeyValuePair<string, TypedConstant>>.Empty);
            }

            yield return _aquilaTargetPlatformAttribute;


            // [assembly: AssemblyInformationalVersion( FileVersion )]
            if (Compilation.SourceAssembly.FileVersion != null)
            {
                if (_assemblyinformationalversionAttribute == null)
                {
                    var attr = (NamedTypeSymbol)this.Compilation.GetTypeByMetadataName(
                        "System.Reflection.AssemblyInformationalVersionAttribute");
                    if (attr != null)
                    {
                        _assemblyinformationalversionAttribute = new SynthesizedAttributeData(
                            attr.InstanceConstructors[0],
                            ImmutableArray.Create(
                                Compilation.CreateTypedConstant(Compilation.SourceAssembly.FileVersion)),
                            ImmutableArray<KeyValuePair<string, TypedConstant>>.Empty);
                    }
                }

                if (_assemblyinformationalversionAttribute != null)
                {
                    yield return _assemblyinformationalversionAttribute;
                }
            }

            //
            yield break;
        }

        public sealed override IEnumerable<Cci.SecurityAttribute> GetSourceAssemblySecurityAttributes()
        {
            yield break;
        }

        public sealed override IEnumerable<Cci.ICustomAttribute> GetSourceModuleAttributes()
        {
            return SourceModule.GetCustomAttributesToEmit(CompilationState).Cast<Cci.ICustomAttribute>();
        }

        internal sealed override Cci.ICustomAttribute SynthesizeAttribute(WellKnownMember attributeConstructor)
        {
            throw new NotImplementedException();
        }

        public override string DefaultNamespace
        {
            get
            {
                // used for PDB writer,
                // C# returns null
                return null;
            }
        }

        public override bool GenerateVisualBasicStylePdb => false;

        public override IEnumerable<string> LinkedAssembliesDebugInfo
        {
            get { throw new NotImplementedException(); }
        }

        internal override string ModuleName => Name;

        public IEnumerable<Cci.IModuleReference> ModuleReferences
        {
            get
            {
                // Let's not add any module references explicitly,
                // PeWriter will implicitly add those needed.
                return SpecializedCollections.EmptyEnumerable<Cci.IModuleReference>();
            }
        }

        public override string Name => _sourceModule.Name;

        internal override Compilation CommonCompilation => _compilation;

        internal AquilaCompilation Compilation => _compilation;

        internal override CommonEmbeddedTypesManager CommonEmbeddedTypesManagerOpt
        {
            get { throw new NotImplementedException(); }
        }

        internal override CommonModuleCompilationState CommonModuleCompilationState
        {
            get { throw new NotImplementedException(); }
        }

        internal override IAssemblySymbolInternal CommonCorLibrary => _compilation.CorLibrary;

        internal EmitOptions EmitOptions => _emitOptions;

        public Cci.IDefinition AsDefinition(EmitContext context)
        {
            throw new NotImplementedException();
        }

        protected override Cci.IAssemblyReference GetCorLibraryReferenceToEmit(EmitContext context)
        {
            Debug.Assert(_compilation.CorLibrary != null);

            return Translate(_compilation.CorLibrary, context.Diagnostics);
        }

        protected override IEnumerable<Cci.IAssemblyReference> GetAssemblyReferencesFromAddedModules(
            DiagnosticBag diagnostics)
        {
            // Cannot be retrieved from GetCoreLibraryReferenceToEmit, because it can return only one reference
            Debug.Assert(_compilation.AquilaCorLibrary != null);
            yield return Translate(_compilation.AquilaCorLibrary, diagnostics);

            ImmutableArray<ModuleSymbol> modules = SourceModule.ContainingAssembly.Modules;

            for (int i = 1; i < modules.Length; i++)
            {
                foreach (AssemblySymbol aRef in modules[i].ReferencedAssemblySymbols)
                {
                    yield return Translate(aRef, diagnostics);
                }
            }
        }

        public IEnumerable<Cci.ICustomAttribute> GetAttributes(EmitContext context)
        {
            throw new NotImplementedException();
        }

        public override ImmutableArray<Cci.ExportedType> GetExportedTypes(DiagnosticBag diagnostics)
        {
            return ImmutableArray<Cci.ExportedType>.Empty;
        }

        public Cci.IFieldReference GetFieldForData(ImmutableArray<byte> data, SyntaxNode syntaxNode,
            DiagnosticBag diagnostics)
        {
            throw new NotImplementedException();
        }

        public override ImmutableArray<Cci.UsedNamespaceOrType> GetImports()
        {
            return ImmutableArray<Cci.UsedNamespaceOrType>.Empty;
        }

        public Cci.IMethodReference GetInitArrayHelper()
        {
            throw new NotImplementedException();
        }

        public override Cci.ITypeReference GetPlatformType(Cci.PlatformType t, EmitContext context)
        {
            throw new NotImplementedException();
        }

        protected override void AddEmbeddedResourcesFromAddedModules(ArrayBuilder<Cci.ManagedResource> builder,
            DiagnosticBag diagnostics)
        {
            throw new NotSupportedException(); // override
        }

        public override MultiDictionary<Cci.DebugSourceDocument, Cci.DefinitionWithLocation> GetSymbolToLocationMap()
        {
            var result = new MultiDictionary<Cci.DebugSourceDocument, Cci.DefinitionWithLocation>();
            return result;
        }

        public override IEnumerable<Cci.INamespaceTypeDefinition> GetTopLevelTypeDefinitions(EmitContext context)
        {
            Cci.TypeReferenceIndexer typeReferenceIndexer = null;

            // First time through, we need to collect emitted names of all top level types.
            HashSet<string> names = (_namesOfTopLevelTypes == null) ? new HashSet<string>() : null;

            AddTopLevelType(names, _rootModuleType);
            VisitTopLevelType(typeReferenceIndexer, _rootModuleType);
            yield return _rootModuleType;

            foreach (var type in this.GetAnonymousTypeDefinitions(context))
            {
                AddTopLevelType(names, type);
                VisitTopLevelType(typeReferenceIndexer, type);
                yield return type;
            }

            foreach (var type in this.GetTopLevelSourceTypeDefinitions(context))
            {
                AddTopLevelType(names, type);
                VisitTopLevelType(typeReferenceIndexer, type);
                yield return type;
            }

            var privateImpl = this.PrivateImplClass;
            if (privateImpl != null)
            {
                AddTopLevelType(names, privateImpl);
                VisitTopLevelType(typeReferenceIndexer, privateImpl);
                yield return privateImpl;
            }

            if (names != null)
            {
                Debug.Assert(_namesOfTopLevelTypes == null);
                _namesOfTopLevelTypes = names;
            }
        }

        public override IEnumerable<Cci.INamespaceTypeDefinition> GetTopLevelSourceTypeDefinitions(EmitContext context)
        {
            // <script> type containing assembly level symbols
            yield return this.EntryPointType;

            foreach (var t in this.Compilation.AnonymousTypeManager.GetAllCreatedTemplates())
                yield return t;

            foreach (var t in this.Compilation.PlatformSymbolCollection.GetAllCreatedTypes())
                yield return t;

            foreach (var t in _sourceModule.SymbolCollection.GetModuleTypes())
                yield return t;

            foreach (var t in _sourceModule.SymbolCollection.GetViewTypes())
            {
                yield return t;
            }

            var namespacesToProcess = new Stack<INamespaceSymbol>(this.SynthesizedManager.Namespaces);

            while (namespacesToProcess.Count != 0)
            {
                var ns = namespacesToProcess.Pop();
                foreach (var member in ns.GetMembers())
                {
                    var memberNamespace = member as INamespaceSymbol;
                    if (memberNamespace != null)
                    {
                        namespacesToProcess.Push(memberNamespace);
                    }
                    else
                    {
                        var type = (NamedTypeSymbol)member;
                        yield return type;
                    }
                }
            }
        }

        public override IEnumerable<Cci.INamespaceTypeDefinition> GetEmbeddedTypeDefinitions(EmitContext context) =>
            ImmutableArray<Cci.INamespaceTypeDefinition>.Empty;

        public override IEnumerable<Cci.INamespaceTypeDefinition> GetAdditionalTopLevelTypeDefinitions(
            EmitContext context) =>
            ImmutableArray<Cci.INamespaceTypeDefinition>.Empty;

        public static Cci.TypeMemberVisibility MemberVisibility(Aquila.CodeAnalysis.Symbols.Symbol symbol)
        {
            switch (symbol.DeclaredAccessibility)
            {
                case Accessibility.Public:
                    return Cci.TypeMemberVisibility.Public;

                case Accessibility.Private:
                    if (symbol.ContainingType != null && symbol.ContainingType.TypeKind == TypeKind.Submission)
                    {
                        // top-level private member:
                        return Cci.TypeMemberVisibility.Public;
                    }
                    else
                    {
                        return Cci.TypeMemberVisibility.Private;
                    }

                case Accessibility.Internal:
                    if (symbol.ContainingAssembly.IsInteractive)
                    {
                        // top-level or nested internal member:
                        return Cci.TypeMemberVisibility.Public;
                    }
                    else
                    {
                        return Cci.TypeMemberVisibility.Assembly;
                    }

                case Accessibility.Protected:
                    if (symbol.ContainingType.TypeKind == TypeKind.Submission)
                    {
                        // top-level protected member:
                        return Cci.TypeMemberVisibility.Public;
                    }
                    else
                    {
                        return Cci.TypeMemberVisibility.Family;
                    }

                case Accessibility.ProtectedAndInternal
                    : // Not supported by language, but we should be able to import it.
                    Debug.Assert(symbol.ContainingType.TypeKind != TypeKind.Submission);
                    return Cci.TypeMemberVisibility.FamilyAndAssembly;

                case Accessibility.ProtectedOrInternal:
                    if (symbol.ContainingAssembly.IsInteractive)
                    {
                        // top-level or nested protected internal member:
                        return Cci.TypeMemberVisibility.Public;
                    }
                    else
                    {
                        return Cci.TypeMemberVisibility.FamilyOrAssembly;
                    }
                case Accessibility.NotApplicable:
                    return Cci.TypeMemberVisibility.Public;

                default:
                    throw ExceptionUtilities.UnexpectedValue(symbol.DeclaredAccessibility);
            }
        }

        #region Private Implementation Details Type

        internal PrivateImplementationDetails PrivateImplClass
        {
            get { return _privateImplementationDetails; }
        }

        internal override bool SupportsPrivateImplClass
        {
            get { return false; } // TODO: true when GetSpecialType() will be implemented
        }

        internal override IModuleSymbolInternal CommonSourceModule => SourceModule;

        #endregion

        static void AddTopLevelType(HashSet<string> names, Cci.INamespaceTypeDefinition type)
        {
            names?.Add(MetadataHelpers.BuildQualifiedName(type.NamespaceName,
                Cci.MetadataWriter.GetMangledName(type, 0)));
        }

        static void VisitTopLevelType(Cci.TypeReferenceIndexer noPiaIndexer, Cci.INamespaceTypeDefinition type)
        {
            noPiaIndexer?.Visit((Cci.ITypeDefinition)type);
        }

        public override bool IsPlatformType(Cci.ITypeReference typeRef, Cci.PlatformType platformType)
        {
            var namedType = typeRef as PENamedTypeSymbol;
            if (namedType != null)
            {
                if (platformType == Cci.PlatformType.SystemType)
                {
                    return (object)namedType == (object)Compilation.GetWellKnownType(WellKnownType.System_Type);
                }

                return namedType.SpecialType == (SpecialType)platformType;
            }

            return false;
        }

        internal override void CompilationFinished()
        {
            this.CompilationState.Freeze();
            this.Compilation.TrackOnCompleted();
        }

        internal override Cci.ITypeReference EncTranslateType(ITypeSymbolInternal type, DiagnosticBag diagnostics)
        {
            throw new NotImplementedException();
        }

        public override IEnumerable<Cci.INamespaceTypeDefinition> GetAnonymousTypeDefinitions(EmitContext context)
        {
            return ImmutableArray<Cci.INamespaceTypeDefinition>.Empty;
        }


        internal Cci.INamedTypeReference GetSpecialType(SpecialType specialType, SyntaxNode syntaxNodeOpt,
            DiagnosticBag diagnostics)
        {
            Debug.Assert(diagnostics != null);

            var typeSymbol = SourceModule.ContainingAssembly.GetSpecialType(specialType);
            return (Cci.INamedTypeReference)Translate(typeSymbol, syntaxNodeOpt, diagnostics, needDeclaration: true);
        }

        internal override Cci.IAssemblyReference Translate(IAssemblySymbolInternal iassembly, DiagnosticBag diagnostics)
        {
            var assembly = (AssemblySymbol)iassembly;

            if (ReferenceEquals(SourceModule.ContainingAssembly, assembly))
            {
                return (Cci.IAssemblyReference)this;
            }

            Cci.IModuleReference reference;

            if (AssemblyOrModuleSymbolToModuleRefMap.TryGetValue(assembly, out reference))
            {
                return (Cci.IAssemblyReference)reference;
            }

            AssemblyReference asmRef = new AssemblyReference(assembly);

            AssemblyReference cachedAsmRef =
                (AssemblyReference)AssemblyOrModuleSymbolToModuleRefMap.GetOrAdd(assembly, asmRef);

            // TryAdd because whatever is associated with assembly should be associated with Modules[0]
            AssemblyOrModuleSymbolToModuleRefMap.TryAdd(assembly.Modules[0], cachedAsmRef);

            return cachedAsmRef;
        }

        internal override Cci.IMethodReference Translate(IMethodSymbolInternal symbol, DiagnosticBag diagnostics,
            bool needDeclaration)
        {
            return Translate((MethodSymbol)symbol, null, diagnostics, /*null,*/ needDeclaration);
        }

        internal Cci.IMethodReference Translate(
            MethodSymbol methodSymbol,
            SyntaxNode syntaxNodeOpt,
            DiagnosticBag diagnostics,
            bool needDeclaration)
        {
            object reference;
            Cci.IMethodReference methodRef;
            NamedTypeSymbol container = methodSymbol.ContainingType;

            Debug.Assert(methodSymbol.IsDefinitionOrDistinct());

            if (!methodSymbol.IsDefinition)
            {
                Debug.Assert(!needDeclaration);

                return methodSymbol;
            }
            else if (!needDeclaration)
            {
                bool methodIsGeneric = methodSymbol.IsGenericMethod;
                bool typeIsGeneric = IsGenericType(container);

                if (methodIsGeneric || typeIsGeneric)
                {
                    if (_genericInstanceMap.TryGetValue(methodSymbol, out reference))
                    {
                        return (Cci.IMethodReference)reference;
                    }

                    if (methodIsGeneric)
                    {
                        if (typeIsGeneric)
                        {
                            // Specialized and generic instance at the same time.
                            methodRef = new SpecializedGenericMethodInstanceReference(methodSymbol);
                        }
                        else
                        {
                            methodRef = new GenericMethodInstanceReference(methodSymbol);
                        }
                    }
                    else
                    {
                        Debug.Assert(typeIsGeneric);
                        methodRef = new SpecializedMethodReference(methodSymbol);
                    }

                    methodRef = (Cci.IMethodReference)_genericInstanceMap.GetOrAdd(methodSymbol, methodRef);

                    return methodRef;
                }
            }

            return methodSymbol;
        }

        internal Cci.IMethodReference TranslateOverriddenMethodReference(
            MethodSymbol methodSymbol,
            SyntaxNode syntaxNodeOpt,
            DiagnosticBag diagnostics)
        {
            Cci.IMethodReference methodRef;
            NamedTypeSymbol container = methodSymbol.ContainingType;

            if (IsGenericType(container))
            {
                if (methodSymbol.IsDefinition)
                {
                    object reference;

                    if (_genericInstanceMap.TryGetValue(methodSymbol, out reference))
                    {
                        methodRef = (Cci.IMethodReference)reference;
                    }
                    else
                    {
                        methodRef = new SpecializedMethodReference(methodSymbol);
                        methodRef = (Cci.IMethodReference)_genericInstanceMap.GetOrAdd(methodSymbol, methodRef);
                    }
                }
                else
                {
                    methodRef = new SpecializedMethodReference(methodSymbol);
                }
            }
            else
            {
                Debug.Assert(methodSymbol.IsDefinition);
                methodRef = methodSymbol;
            }

            return methodRef;
        }

        internal static Cci.IGenericParameterReference Translate(TypeParameterSymbol param)
        {
            if (!param.IsDefinition)
                throw new InvalidOperationException(
                    /*string.Format(CSharpResources.GenericParameterDefinition, param.Name)*/);

            return param;
        }

        internal sealed override Cci.ITypeReference Translate(ITypeSymbolInternal typeSymbol, SyntaxNode syntaxNodeOpt,
            DiagnosticBag diagnostics)
        {
            Debug.Assert(diagnostics != null);

            switch (typeSymbol.Kind)
            {
                case SymbolKind.ArrayType:
                    return Translate((ArrayTypeSymbol)typeSymbol);

                case SymbolKind.ErrorType:
                case SymbolKind.NamedType:
                    return Translate((NamedTypeSymbol)typeSymbol, syntaxNodeOpt, diagnostics);

                case SymbolKind.PointerType:
                    return Translate((PointerTypeSymbol)typeSymbol);

                case SymbolKind.TypeParameter:
                    return Translate((TypeParameterSymbol)typeSymbol);
            }

            throw ExceptionUtilities.UnexpectedValue(typeSymbol.Kind);
        }

        internal Cci.IFieldReference Translate(
            FieldSymbol fieldSymbol,
            SyntaxNode syntaxNodeOpt,
            DiagnosticBag diagnostics,
            bool needDeclaration = false)
        {
            Debug.Assert(fieldSymbol.IsDefinitionOrDistinct());
            
            if (!fieldSymbol.IsDefinition)
            {
                Debug.Assert(!needDeclaration);

                return fieldSymbol;
            }
            else if (!needDeclaration && IsGenericType(fieldSymbol.ContainingType))
            {
                object reference;
                Cci.IFieldReference fieldRef;

                if (_genericInstanceMap.TryGetValue(fieldSymbol, out reference))
                {
                    return (Cci.IFieldReference)reference;
                }

                fieldRef = new SpecializedFieldReference(fieldSymbol);
                fieldRef = (Cci.IFieldReference)_genericInstanceMap.GetOrAdd(fieldSymbol, fieldRef);

                return fieldRef;
            }

            return fieldSymbol;
        }

        internal Cci.ITypeReference Translate(
            NamedTypeSymbol namedTypeSymbol, SyntaxNode syntaxOpt, DiagnosticBag diagnostics,
            bool fromImplements = false,
            bool needDeclaration = false)
        {
            Debug.Assert(namedTypeSymbol.IsDefinitionOrDistinct());
            Debug.Assert(diagnostics != null);

            // Substitute error types with a special singleton object.
            // Unreported bad types can come through NoPia embedding, for example.
            if (namedTypeSymbol.OriginalDefinition.Kind == SymbolKind.ErrorType)
            {
                throw new NotImplementedException($"Translate(ErrorType {namedTypeSymbol.Name})");
            }

            if (!namedTypeSymbol.IsDefinition)
            {
                // generic instantiation for sure
                Debug.Assert(!needDeclaration);

                if (namedTypeSymbol.IsUnboundGenericType)
                {
                    namedTypeSymbol = namedTypeSymbol.OriginalDefinition;
                }
                else
                {
                    return namedTypeSymbol;
                }
            }
            else if (!needDeclaration)
            {
                object reference;
                Cci.INamedTypeReference typeRef;

                NamedTypeSymbol container = namedTypeSymbol.ContainingType;

                if (namedTypeSymbol.Arity > 0)
                {
                    if (_genericInstanceMap.TryGetValue(namedTypeSymbol, out reference))
                    {
                        return (Cci.INamedTypeReference)reference;
                    }

                    if ((object)container != null)
                    {
                        if (IsGenericType(container))
                        {
                            // Container is a generic instance too.
                            typeRef = new SpecializedGenericNestedTypeInstanceReference(namedTypeSymbol);
                        }
                        else
                        {
                            typeRef = new GenericNestedTypeInstanceReference(namedTypeSymbol);
                        }
                    }
                    else
                    {
                        typeRef = new GenericNamespaceTypeInstanceReference(namedTypeSymbol);
                    }

                    typeRef = (Cci.INamedTypeReference)_genericInstanceMap.GetOrAdd(namedTypeSymbol, typeRef);

                    return typeRef;
                }
                else if (IsGenericType(container))
                {
                    Debug.Assert((object)container != null);

                    if (_genericInstanceMap.TryGetValue(namedTypeSymbol, out reference))
                    {
                        return (Cci.INamedTypeReference)reference;
                    }

                    typeRef = new SpecializedNestedTypeReference(namedTypeSymbol);
                    typeRef = (Cci.INamedTypeReference)_genericInstanceMap.GetOrAdd(namedTypeSymbol, typeRef);

                    return typeRef;
                }
            }

            // NoPia: See if this is a type, which definition we should copy into our assembly.
            Debug.Assert(namedTypeSymbol.IsDefinition);

            return namedTypeSymbol;
        }

        internal static Cci.IArrayTypeReference Translate(ArrayTypeSymbol symbol)
        {
            return symbol;
        }

        internal static Cci.IPointerTypeReference Translate(PointerTypeSymbol symbol)
        {
            return symbol;
        }

        internal ImmutableArray<Cci.IParameterTypeInformation> Translate(ImmutableArray<ParameterSymbol> @params)
        {
            return @params.Cast<Cci.IParameterTypeInformation>().ToImmutableArray();
        }

        private bool IsSourceDefinition(IMethodSymbol method)
        {
            return (object)method.ContainingModule == _sourceModule && method.IsDefinition;
        }

        public static bool IsGenericType(INamedTypeSymbol toCheck)
        {
            while ((object)toCheck != null)
            {
                if (toCheck.Arity > 0)
                {
                    return true;
                }

                toCheck = toCheck.ContainingType;
            }

            return false;
        }

        public override IEnumerable<(Cci.ITypeDefinition, ImmutableArray<Cci.DebugSourceDocument>)>
            GetTypeToDebugDocumentMap(EmitContext context)
        {
            return new List<(Cci.ITypeDefinition, ImmutableArray<Cci.DebugSourceDocument>)>();
        }

        public override SymbolChanges? EncSymbolChanges { get; }
        public override EmitBaseline? PreviousGeneration { get; }
    }
}