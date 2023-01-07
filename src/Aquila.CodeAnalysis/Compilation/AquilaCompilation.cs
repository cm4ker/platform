using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection.Metadata;
using System.Threading;
using System.Threading.Tasks;
using Aquila.CodeAnalysis.Errors;
using Aquila.CodeAnalysis.Syntax;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeGen;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.CodeAnalysis.Emit;
using Microsoft.CodeAnalysis.PooledObjects;
using Microsoft.CodeAnalysis.Symbols;
using Aquila.CodeAnalysis.DocumentationComments;
using Aquila.CodeAnalysis.Emit;
using Aquila.CodeAnalysis.Public;
using Aquila.CodeAnalysis.Semantics;
using Aquila.CodeAnalysis.Symbols;
using Aquila.CodeAnalysis.Symbols.Anonymous;
using Aquila.CodeAnalysis.Symbols.PE;
using Aquila.CodeAnalysis.Symbols.Source;
using Aquila.CodeAnalysis.Utilities;
using Aquila.Metadata;
using Microsoft.Cci;
using Roslyn.Utilities;
using ExceptionUtilities = Aquila.CodeAnalysis.Utilities.ExceptionUtilities;

namespace Aquila.CodeAnalysis
{
    public sealed partial class AquilaCompilation : Compilation
    {
        private readonly SourceSymbolCollection _sourceSymbolCollection;
        private readonly MetadataProvider _metadataProvider;

        MethodSymbol _lazyMainMethod;
        readonly AquilaCompilationOptions _options;

        internal ImmutableArray<IObserver<object>> EventSources => _options.EventSources;

        Task<IEnumerable<Diagnostic>> _lazyAnalysisTask;

        /// <summary>
        /// Manages anonymous types declared in this compilation. Unifies types that are structurally equivalent.
        /// </summary>
        readonly AnonymousTypeManager _anonymousTypeManager;

        private NamespaceSymbol? _lazyGlobalNamespace;

        /// <summary>
        /// Manages types declared by the component
        /// </summary>
        readonly PlatformSymbolCollection _platformSymbolCollection;

        /// <summary>
        /// The <see cref="SourceAssemblySymbol"/> for this compilation. Do not access directly, use Assembly property
        /// instead. This field is lazily initialized by ReferenceManager, ReferenceManager.CacheLockObject must be locked
        /// while ReferenceManager "calculates" the value and assigns it, several threads must not perform duplicate
        /// "calculation" simultaneously.
        /// </summary>
        private SourceAssemblySymbol _lazyAssemblySymbol;

        /// <summary>
        /// Holds onto data related to reference binding.
        /// The manager is shared among multiple compilations that we expect to have the same result of reference binding.
        /// In most cases this can be determined without performing the binding. If the compilation however contains a circular 
        /// metadata reference (a metadata reference that refers back to the compilation) we need to avoid sharing of the binding results.
        /// We do so by creating a new reference manager for such compilation. 
        /// </summary>
        private ReferenceManager _referenceManager;


        /// <summary>
        /// COR library containing base system types.
        /// </summary>
        internal AssemblySymbol CorLibrary => GetBoundReferenceManager().CorLibrary;

        /// <summary>
        /// Aquila COR library containing base platform types.
        /// </summary>
        internal AssemblySymbol AquilaCorLibrary => GetBoundReferenceManager().AquilaCorLibrary;

        /// <summary>
        /// Tables containing all source symbols to be compiled.
        /// Used for enumeration and lookup.
        /// </summary>
        internal SourceSymbolCollection SourceSymbolCollection => _sourceSymbolCollection;

        internal MetadataProvider MetadataProvider => _metadataProvider;

        private IEnumerable<ResourceDescription> SynthesizedResources = null;

        /// <summary>
        /// The AssemblySymbol that represents the assembly being created.
        /// </summary>
        internal SourceAssemblySymbol SourceAssembly
        {
            get
            {
                GetBoundReferenceManager();
                return _lazyAssemblySymbol;
            }
        }

        internal new SourceModuleSymbol SourceModule => (SourceModuleSymbol)this.SourceAssembly.Modules[0];

        /// <summary>
        /// The AssemblySymbol that represents the assembly being created.
        /// </summary>
        internal new AssemblySymbol Assembly => SourceAssembly;

        public new AquilaCompilationOptions Options => _options;

        public IEnumerable<string> ConditionalOptions
        {
            get
            {
                yield return this.Options.OutputKind.ToString();

                if (this.Options.OutputKind == OutputKind.ConsoleApplication)
                {
                    yield return "CLI";
                    yield return "console";
                }

                if (this.Options.OptimizationLevel.IsDebug())
                {
                    yield return "DEBUG";
                }
            }
        }

        private AquilaCompilation(
            string assemblyName,
            AquilaCompilationOptions options,
            ImmutableArray<MetadataReference> references,
            bool isSubmission,
            ReferenceManager referenceManager = null,
            bool reuseReferenceManager = false,
            //SyntaxAndDeclarationManager syntaxAndDeclarations,
            SemanticModelProvider semanticModelProvider = null,
            AsyncQueue<CompilationEvent> eventQueue = null
        )
            : base(assemblyName, references, SyntaxTreeCommonFeatures(ImmutableArray<SyntaxTree>.Empty), isSubmission,
                semanticModelProvider: new CachingSemanticModelProvider(), eventQueue)
        {
            _wellKnownMemberSignatureComparer = new WellKnownMembersSignatureComparer(this);

            _options = options;
            _coreTypes = new CoreTypes(this);
            _coreMethods = new CoreMethods(_coreTypes);
            _anonymousTypeManager = new AnonymousTypeManager(this);


            this.Conversions = new Semantics.Conversions(this);
            this.TypeRefs = new Semantics.PrimitiveBoundTypeRefs(this);

            _referenceManager = (reuseReferenceManager && referenceManager != null)
                ? referenceManager
                : new ReferenceManager(MakeSourceAssemblySimpleName(), options.AssemblyIdentityComparer,
                    referenceManager?.ObservedMetadata, options.SdkDirectory);

            _platformSymbolCollection = new PlatformSymbolCollection(this);
            _sourceSymbolCollection = new SourceSymbolCollection(this);
            _metadataProvider = new MetadataProvider();
        }

        /// <summary>
        /// Create a duplicate of this compilation with different symbol instances.
        /// </summary>
        public new AquilaCompilation Clone()
        {
            return Update(reuseReferenceManager: true);
        }

        private AquilaCompilation Update(
            string assemblyName = null,
            AquilaCompilationOptions options = null,
            IEnumerable<MetadataReference> references = null,
            ReferenceManager referenceManager = null,
            bool reuseReferenceManager = false,
            SemanticModelProvider semanticModelProvider = null,
            IEnumerable<AquilaSyntaxTree> syntaxTrees = null,
            IEnumerable<EntityMetadata> metadata = null)
        {
            var compilation = new AquilaCompilation(
                assemblyName ?? this.AssemblyName,
                options ?? _options,
                references != null ? references.AsImmutable() : this.ExternalReferences,
                this.IsSubmission,
                referenceManager ?? _referenceManager,
                reuseReferenceManager,
                semanticModelProvider ?? SemanticModelProvider,
                EventQueue);

            compilation.MetadataProvider.AddMetadataRange(metadata ?? MetadataProvider.EntityMetadata);
            compilation.SourceSymbolCollection.AddSyntaxTreeRange(syntaxTrees ?? SyntaxTrees);

            return compilation;
        }

        private AquilaCompilation WithAquilaSyntaxTrees(IEnumerable<AquilaSyntaxTree> syntaxTrees)
        {
            return Update(
                reuseReferenceManager: true,
                syntaxTrees: syntaxTrees);
        }


        /// <summary>
        /// Creates a new compilation with additional metadata.
        /// </summary>
        /// <param name="metadatas">The new metadata.</param>
        /// <returns>A new compilation.</returns>
        public AquilaCompilation AddMetadata(params EntityMetadata[] metadatas)
        {
            return AddMetadata(metadatas.AsEnumerable());
        }

        /// <summary>
        /// Creates a new compilation with additional metadata.
        /// </summary>
        /// <param name="metadatas">The new metadata.</param>
        /// <returns>A new compilation.</returns>
        public AquilaCompilation AddMetadata(IEnumerable<EntityMetadata> metadatas)
        {
            return Update(
                reuseReferenceManager: true,
                metadata: metadatas);
        }

        public AquilaCompilation WithAquilaOptions(AquilaCompilationOptions options)
        {
            return Update(options: options);
        }

        public override ImmutableArray<MetadataReference> DirectiveReferences
        {
            get { throw new NotImplementedException(); }
        }

        /// <summary>
        /// Returns true if this is a case sensitive compilation, false otherwise.  Case sensitivity
        /// affects compilation features such as name lookup as well as choosing what names to emit
        /// when there are multiple different choices (for example between a virtual method and an
        /// override).
        /// </summary>
        public override bool IsCaseSensitive => false;

        public override string Language { get; } = LanguageConstants.LanguageId;

        internal AnonymousTypeManager AnonymousTypeManager => _anonymousTypeManager;

        internal PlatformSymbolCollection PlatformSymbolCollection => _platformSymbolCollection;

        public override IEnumerable<AssemblyIdentity> ReferencedAssemblyNames =>
            Assembly.Modules.SelectMany(module => module.ReferencedAssemblies);

        protected override IAssemblySymbol CommonAssembly => SourceAssembly;

        protected override ITypeSymbol CommonDynamicType
        {
            get { throw new NotImplementedException(); }
        }

        protected override ITypeSymbol CommonScriptGlobalsType => null;


        protected override INamespaceSymbol CommonGlobalNamespace
        {
            get { return this.GlobalNamespace; }
        }

        protected override INamedTypeSymbol CommonObjectType
        {
            get
            {
                Debug.Assert(this.CoreTypes.Object.Symbol != null);
                return (INamedTypeSymbol)this.CoreTypes.Object.Symbol;
            }
        }

        protected override CompilationOptions CommonOptions => _options;

        protected override INamedTypeSymbol CommonScriptClass
        {
            get { throw new NotImplementedException(); }
        }

        /// <summary>
        /// Get a ModuleSymbol that refers to the module being created by compiling all of the code.
        /// By getting the GlobalNamespace property of that module, all of the namespaces and types
        /// defined in source code can be obtained.
        /// </summary>
        protected override IModuleSymbol CommonSourceModule => this.SourceModule;

        internal override CommonAnonymousTypeManager CommonAnonymousTypeManager
        {
            get { throw new NotImplementedException(); }
        }

        internal override ScriptCompilationInfo CommonScriptCompilationInfo
        {
            get
            {
                return null; // throw new NotImplementedException();
            }
        }

        internal override bool IsDelaySigned
        {
            get { return SourceAssembly.IsDelaySigned; }
        }

        internal override StrongNameKeys StrongNameKeys
        {
            get { return SourceAssembly.StrongNameKeys; }
        }

        public static AquilaCompilation Create(
            string assemblyName,
            IEnumerable<AquilaSyntaxTree> syntaxTrees = null,
            IEnumerable<EntityMetadata> metadata = null,
            IEnumerable<MetadataReference> references = null,
            IEnumerable<ResourceDescription> resources = null,
            AquilaCompilationOptions options = null)
        {
            Debug.Assert(options != null);

            var compilation = new AquilaCompilation(
                assemblyName,
                options,
                ValidateReferences<CompilationReference>(references),
                false);

            compilation.SynthesizedResources = resources;

            compilation.CheckAssemblyName(compilation.DeclarationDiagnostics);


            //Add sources
            if (syntaxTrees != null)
                compilation.SourceSymbolCollection.AddSyntaxTreeRange(syntaxTrees);

            if (metadata != null)
                compilation.MetadataProvider.AddMetadataRange(metadata);


            return compilation;
        }

        internal override byte LinkerMajorVersion => 0x0;

        internal override CommonMessageProvider MessageProvider => Aquila.CodeAnalysis.Errors.MessageProvider.Instance;

        internal override IDictionary<ValueTuple<string, string>, MetadataReference> ReferenceDirectiveMap
        {
            get { throw new NotImplementedException(); }
        }

        internal override IEnumerable<ReferenceDirective> ReferenceDirectives
        {
            get
            {
                return ImmutableArray<ReferenceDirective>.Empty; // throw new NotImplementedException();
            }
        }

        public override bool ContainsSymbolsWithName(Func<string, bool> predicate,
            SymbolFilter filter = SymbolFilter.TypeAndMember,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            if (predicate == null)
            {
                throw CodeAnalysis.Utilities.ExceptionUtilities.ArgumentNull(nameof(predicate));
            }

            if (filter == SymbolFilter.None)
            {
                //throw new ArgumentException(CSharpResources.NoNoneSearchCriteria, nameof(filter));
            }

            //return this.Declarations.ContainsName(predicate, filter, cancellationToken);

            throw new NotImplementedException();
        }

        public override bool ContainsSymbolsWithName(string name, SymbolFilter filter = SymbolFilter.TypeAndMember,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            throw new NotImplementedException();
        }

        protected override INamedTypeSymbol CommonCreateErrorTypeSymbol(INamespaceOrTypeSymbol container, string name,
            int arity)
        {
            return new MissingMetadataTypeSymbol(name, arity, false);
        }

        protected override INamespaceSymbol CommonCreateErrorNamespaceSymbol(INamespaceSymbol container, string name)
        {
            return new MissingNamespaceSymbol((NamespaceSymbol)container, name);
        }

        protected override INamedTypeSymbol CommonCreateTupleTypeSymbol(ImmutableArray<ITypeSymbol> elementTypes,
            ImmutableArray<string> elementNames, ImmutableArray<Location> elementLocations,
            ImmutableArray<NullableAnnotation> elementNullableAnnotations)
        {
            throw new NotImplementedException();
        }

        protected override INamedTypeSymbol CommonCreateTupleTypeSymbol(INamedTypeSymbol underlyingType,
            ImmutableArray<string> elementNames, ImmutableArray<Location> elementLocations,
            ImmutableArray<NullableAnnotation> elementNullableAnnotations)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// The bag in which semantic analysis should deposit its diagnostics.
        /// </summary>
        internal DiagnosticBag DeclarationDiagnostics
        {
            get
            {
                if (_lazyDeclarationDiagnostics == null)
                {
                    var diagnostics = new DiagnosticBag();
                    Interlocked.CompareExchange(ref _lazyDeclarationDiagnostics, diagnostics, null);
                }

                return _lazyDeclarationDiagnostics;
            }
        }

        private DiagnosticBag _lazyDeclarationDiagnostics;


        private protected override bool IsSymbolAccessibleWithinCore(ISymbol symbol, ISymbol within,
            ITypeSymbol throughType)
        {
            throw new NotImplementedException();
        }

        public override ImmutableArray<Diagnostic> GetParseDiagnostics(
            CancellationToken cancellationToken = default(CancellationToken))
        {
            return GetDiagnostics(CompilationStage.Parse, false, cancellationToken);
        }

        public override ImmutableArray<Diagnostic> GetDeclarationDiagnostics(
            CancellationToken cancellationToken = default(CancellationToken))
        {
            return GetDiagnostics(CompilationStage.Declare, false, cancellationToken);
        }

        public override ImmutableArray<Diagnostic> GetMethodBodyDiagnostics(
            CancellationToken cancellationToken = default(CancellationToken))
        {
            return GetDiagnostics(CompilationStage.Compile, false, cancellationToken);
        }

        public override ImmutableArray<Diagnostic> GetDiagnostics(
            CancellationToken cancellationToken = default(CancellationToken))
        {
            return GetDiagnostics(CompilationStage.Compile, true, cancellationToken);
        }

        internal ImmutableArray<Diagnostic> GetDiagnostics(CompilationStage stage, bool includeEarlierStages,
            CancellationToken cancellationToken)
        {
            var diagnostics = DiagnosticBag.GetInstance();
            GetDiagnostics(stage, includeEarlierStages, diagnostics, cancellationToken);
            return diagnostics.ToReadOnlyAndFree();
        }

        internal override void GetDiagnostics(CompilationStage stage, bool includeEarlierStages,
            DiagnosticBag diagnostics, CancellationToken cancellationToken)
        {
            var builder = DiagnosticBag.GetInstance();

            // Parse
            if (stage == CompilationStage.Parse || (stage > CompilationStage.Parse && includeEarlierStages))
            {
                var syntaxTrees = this.SyntaxTrees;
                if (this.Options.ConcurrentBuild)
                {
                    Parallel.ForEach(syntaxTrees,
                        UICultureUtilities.WithCurrentUICulture<AquilaSyntaxTree>(syntaxTree =>
                        {
                            builder.AddRange(syntaxTree.GetDiagnostics(cancellationToken));
                        }));
                }
                else
                {
                    foreach (var syntaxTree in syntaxTrees)
                    {
                        cancellationToken.ThrowIfCancellationRequested();
                        builder.AddRange(syntaxTree.GetDiagnostics(cancellationToken));
                    }
                }
            }

            // Declare
            if (stage == CompilationStage.Declare || stage > CompilationStage.Declare && includeEarlierStages)
            {
                // CheckAssemblyName(builder);
                builder.AddRange(Options.Errors);
                builder.AddRange(Options.Diagnostics);

                cancellationToken.ThrowIfCancellationRequested();

                // the set of diagnostics related to establishing references.
                builder.AddRange(GetBoundReferenceManager().Diagnostics);

                //cancellationToken.ThrowIfCancellationRequested();

                try
                {
                    builder.AddRange(this.BindAndAnalyseTask(cancellationToken).Result.AsImmutable());
                }
                catch (AggregateException e) when (e.InnerException != null)
                {
                    // unwrap the aggregate exception, keep original stacktrace
                    System.Runtime.ExceptionServices.ExceptionDispatchInfo.Capture(e.InnerException).Throw();
                }

                cancellationToken.ThrowIfCancellationRequested();

                // resolve entry point
                this.GetEntryPoint(cancellationToken);

                //
                builder.AddRange(_lazyDeclarationDiagnostics?.AsEnumerable() ?? Enumerable.Empty<Diagnostic>());
            }

            cancellationToken.ThrowIfCancellationRequested();

            // Compile
            if (stage == CompilationStage.Compile || stage > CompilationStage.Compile && includeEarlierStages)
            {
                var methodBodyDiagnostics = DiagnosticBag.GetInstance();
                // TODO: perform compilation and report diagnostics
                // GetDiagnosticsForAllMethodBodies(methodBodyDiagnostics, cancellationToken); 
                builder.AddRangeAndFree(methodBodyDiagnostics);
            }

            // Before returning diagnostics, we filter warnings
            // to honor the compiler options (e.g., /nowarn, /warnaserror and /warn) and the pragmas.
            FilterAndAppendAndFreeDiagnostics(diagnostics, ref builder, cancellationToken);
        }

        public override ImmutableArray<MetadataReference> GetUsedAssemblyReferences(
            CancellationToken cancellationToken = new CancellationToken())
        {
            throw new NotImplementedException();
        }

        public override IEnumerable<ISymbol> GetSymbolsWithName(Func<string, bool> predicate,
            SymbolFilter filter = SymbolFilter.TypeAndMember,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            if (predicate == null)
            {
                throw CodeAnalysis.Utilities.ExceptionUtilities.ArgumentNull(nameof(predicate));
            }

            if (filter == SymbolFilter.None)
            {
                //throw new ArgumentException(CSharpResources.NoNoneSearchCriteria, nameof(filter));
            }

            //return new SymbolSearcher(this).GetSymbolsWithName(predicate, filter, cancellationToken);

            throw new NotImplementedException();
        }

        public override IEnumerable<ISymbol> GetSymbolsWithName(string name,
            SymbolFilter filter = SymbolFilter.TypeAndMember,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            throw new NotImplementedException();
        }

        public override CompilationReference ToMetadataReference(
            ImmutableArray<string> aliases = default(ImmutableArray<string>), bool embedInteropTypes = false)
        {
            throw new NotImplementedException();
        }


        /// <summary>
        /// Get all modules in this compilation, including the source module, added modules, and all
        /// modules of referenced assemblies that do not come from an assembly with an extern alias.
        /// Metadata imported from aliased assemblies is not visible at the source level except through
        /// the use of an extern alias directive. So exclude them from this list which is used to construct
        /// the global namespace.
        /// </summary>
        private void GetAllUnaliasedModules(ArrayBuilder<ModuleSymbol> modules)
        {
            // NOTE: This includes referenced modules - they count as modules of the compilation assembly.
            modules.AddRange(Assembly.Modules);

            var referenceManager = GetBoundReferenceManager();

            for (int i = 0; i < referenceManager.ReferencedAssemblies.Length; i++)
            {
                modules.AddRange(referenceManager.ReferencedAssemblies[i].Modules);
            }
        }


        /// <summary>
        /// Return a list of assembly symbols than can be accessed without using an alias.
        /// For example:
        ///   1) /r:A.dll /r:B.dll -> A, B
        ///   2) /r:Foo=A.dll /r:B.dll -> B
        ///   3) /r:Foo=A.dll /r:A.dll -> A
        /// </summary>
        internal void GetUnaliasedReferencedAssemblies(ArrayBuilder<AssemblySymbol> assemblies)
        {
            var referenceManager = GetBoundReferenceManager();
            var referencedAssemblies = referenceManager.GetReferencedAssemblies().ToArray();

            for (int i = 0; i < referencedAssemblies.Length; i++)
            {
                //if (referenceManager.DeclarationsAccessibleWithoutAlias(i))
                {
                    assemblies.Add((AssemblySymbol)referencedAssemblies[i].Value);
                }
            }
        }

        protected override void AppendDefaultVersionResource(Stream resourceStream)
        {
            var sourceAssembly = SourceAssembly;
            string fileVersion = sourceAssembly.FileVersion ?? sourceAssembly.Identity.Version.ToString();

            Win32ResourceConversions.AppendVersionToResourceStream(resourceStream,
                !this.Options.OutputKind.IsApplication(),
                fileVersion: fileVersion,
                originalFileName: this.SourceModule.Name,
                internalName: this.SourceModule.Name,
                productVersion: sourceAssembly.InformationalVersion ?? fileVersion,
                fileDescription: sourceAssembly.Title ?? " ", //alink would give this a blank if nothing was supplied.
                legalCopyright: sourceAssembly.Copyright ??
                                " ", //alink would give this a blank if nothing was supplied.
                legalTrademarks: sourceAssembly.Trademark,
                productName: sourceAssembly.Product,
                comments: sourceAssembly.Description,
                companyName: sourceAssembly.Company,
                assemblyVersion: sourceAssembly.Identity.Version
            );
        }

        protected override Compilation CommonClone()
        {
            return Clone();
        }

        protected override IArrayTypeSymbol CommonCreateArrayTypeSymbol(ITypeSymbol elementType, int rank,
            NullableAnnotation elementNullableAnnotation)
        {
            return ArrayTypeSymbol.CreateCSharpArray(SourceAssembly, (TypeSymbol)elementType, rank: rank);
        }

        protected override IPointerTypeSymbol CommonCreatePointerTypeSymbol(ITypeSymbol elementType)
        {
            return new PointerTypeSymbol((TypeSymbol)elementType);
        }

        protected override IFunctionPointerTypeSymbol CommonCreateFunctionPointerTypeSymbol(ITypeSymbol returnType,
            RefKind returnRefKind,
            ImmutableArray<ITypeSymbol> parameterTypes, ImmutableArray<RefKind> parameterRefKinds,
            SignatureCallingConvention callingConvention,
            ImmutableArray<INamedTypeSymbol> callingConventionTypes)
        {
            throw new NotImplementedException();
        }

        protected override INamedTypeSymbol CommonCreateNativeIntegerTypeSymbol(bool signed)
        {
            throw new NotImplementedException();
        }

        protected override INamedTypeSymbol CommonCreateAnonymousTypeSymbol(ImmutableArray<ITypeSymbol> memberTypes,
            ImmutableArray<string> memberNames, ImmutableArray<Location> memberLocations,
            ImmutableArray<bool> memberIsReadOnly, ImmutableArray<NullableAnnotation> memberNullableAnnotations)
        {
            throw new NotImplementedException();
        }

        protected override ISymbol CommonGetAssemblyOrModuleSymbol(MetadataReference reference)
        {
            if (reference == null)
            {
                throw new ArgumentNullException(nameof(reference));
            }

            if (reference.Properties.Kind == MetadataImageKind.Assembly)
            {
                return GetBoundReferenceManager().GetReferencedAssemblySymbol(reference);
            }
            else
            {
                Debug.Assert(reference.Properties.Kind == MetadataImageKind.Module);
                throw new NotImplementedException();

                //int index = GetBoundReferenceManager().GetReferencedModuleIndex(reference);
                //return index < 0 ? null : this.Assembly.Modules[index];
            }
        }

        private protected override MetadataReference CommonGetMetadataReference(IAssemblySymbol assemblySymbol) =>
            GetBoundReferenceManager().GetMetadataReference((IAssemblySymbolInternal)assemblySymbol);


        /// <summary>
        /// Gets the root namespace that contains all namespaces and types defined in source code or in
        /// referenced metadata, merged into a single namespace hierarchy.
        /// </summary>
        internal new NamespaceSymbol GlobalNamespace
        {
            get
            {
                if (_lazyGlobalNamespace is null)
                {
                    // Get the root namespace from each module, and merge them all together
                    // Get all modules in this compilation, ones referenced directly by the compilation
                    // as well as those referenced by all referenced assemblies.

                    var modules = ArrayBuilder<ModuleSymbol>.GetInstance();
                    GetAllUnaliasedModules(modules);

                    var result = MergedNamespaceSymbol.Create(
                        new NamespaceExtent(this),
                        null,
                        modules.SelectDistinct(m => m.GlobalNamespace));

                    modules.Free();

                    Interlocked.CompareExchange(ref _lazyGlobalNamespace, result, null);
                }

                return _lazyGlobalNamespace;
            }
        }

        protected override INamespaceSymbol CommonGetCompilationNamespace(INamespaceSymbol namespaceSymbol)
        {
            throw new NotImplementedException();
        }

        protected override IMethodSymbol CommonGetEntryPoint(CancellationToken cancellationToken)
        {
            if (_lazyMainMethod == null && _options.OutputKind.IsApplication())
            {
                var method = FindEntryPoint(cancellationToken);
                if (method == null)
                {
                    DeclarationDiagnostics.Add(Location.None, ErrorCode.ERR_StartupObjectNotFound,
                        Options.MainTypeName);
                    method = new MissingMethodSymbol(); // dummy symbol
                }

                _lazyMainMethod = method;
            }

            return _lazyMainMethod;
        }

        MethodSymbol FindEntryPoint(CancellationToken cancellationToken)
        {
            return SourceSymbolCollection.GetModuleType("main")
                ?.GetMembers("main").OfType<MethodSymbol>().FirstOrDefault();
        }

        protected override ImmutableArray<SyntaxTree> CommonSyntaxTrees =>
            SyntaxTrees.Cast<SyntaxTree>().ToImmutableArray();

        public new ImmutableArray<AquilaSyntaxTree> SyntaxTrees => this.SourceSymbolCollection.SyntaxTrees;

        internal override Guid DebugSourceDocumentLanguageId => LanguageConstants.CorSymLanguageTypeAquila;

        protected override bool CommonContainsSyntaxTree(SyntaxTree syntaxTree)
        {
            return this.SyntaxTrees.Contains(syntaxTree);
        }

        protected override SemanticModel CommonGetSemanticModel(SyntaxTree syntaxTree, bool ignoreAccessibility)
        {
            if (syntaxTree == null)
            {
                throw new ArgumentNullException(nameof(syntaxTree));
            }

            SemanticModel? model = null;
            if (SemanticModelProvider != null)
            {
                model = SemanticModelProvider.GetSemanticModel(syntaxTree, this, ignoreAccessibility);
                Debug.Assert(model != null);
            }

            return model ?? CreateSemanticModel(syntaxTree, ignoreAccessibility);
        }

        internal override SemanticModel CreateSemanticModel(SyntaxTree syntaxTree, bool ignoreAccessibility)
            => new AquilaSemanticModel(this, syntaxTree);

        protected override Compilation CommonAddSyntaxTrees(IEnumerable<SyntaxTree> trees)
        {
            return WithAquilaSyntaxTrees(this.SyntaxTrees.Concat(trees.Cast<AquilaSyntaxTree>()));
        }

        protected override Compilation CommonRemoveAllSyntaxTrees()
        {
            return WithAquilaSyntaxTrees(ImmutableArray<AquilaSyntaxTree>.Empty);
        }

        protected override Compilation CommonRemoveSyntaxTrees(IEnumerable<SyntaxTree> trees)
        {
            return WithAquilaSyntaxTrees(this.SyntaxTrees.Except(trees.OfType<AquilaSyntaxTree>()));
        }

        protected override Compilation CommonReplaceSyntaxTree(SyntaxTree oldTree, SyntaxTree newTree)
        {
            if (oldTree == null)
            {
                throw ExceptionUtilities.ArgumentNull(nameof(oldTree));
            }

            if (newTree == null)
            {
                return this.RemoveSyntaxTrees(oldTree);
            }

            if (newTree == oldTree)
            {
                return this;
            }

            if (!ContainsSyntaxTree(oldTree))
            {
                throw new KeyNotFoundException();
            }

            return WithAquilaSyntaxTrees(SyntaxTrees.Select(t => (t == oldTree) ? (AquilaSyntaxTree)newTree : t));
        }

        internal override int GetSyntaxTreeOrdinal(SyntaxTree tree)
        {
            Debug.Assert(this.ContainsSyntaxTree(tree));
            return SourceSymbolCollection.OrdinalMap[tree];
        }

        protected override Compilation CommonWithAssemblyName(string outputName)
        {
            if (this.AssemblyName == outputName)
            {
                return this;
            }

            return Update(assemblyName: outputName);
        }

        protected override Compilation CommonWithOptions(CompilationOptions options)
        {
            if (!(options is AquilaCompilationOptions))
            {
                throw ExceptionUtilities.UnexpectedValue(options);
            }

            return WithAquilaOptions((AquilaCompilationOptions)options);
        }

        protected override Compilation CommonWithReferences(IEnumerable<MetadataReference> newReferences)
        {
            return Update(references: newReferences);
        }

        protected override Compilation CommonWithScriptCompilationInfo(ScriptCompilationInfo info)
        {
            throw new NotImplementedException();
        }

        internal override AnalyzerDriver CreateAnalyzerDriver(ImmutableArray<DiagnosticAnalyzer> analyzers,
            AnalyzerManager analyzerManager, SeverityFilter severityFilter)
        {
            static int GetKind(SyntaxNode node) => node.RawKind;
            static bool IsComment(SyntaxTrivia trivia) => false;
            return new AnalyzerDriver<int>(analyzers, GetKind, analyzerManager, severityFilter, IsComment);
        }

        internal override void SerializePdbEmbeddedCompilationOptions(BlobBuilder builder)
        {
            WriteValue(CompilationOptionNames.LanguageVersion, Options.LanguageVersion.ToString());

            if (Options.CheckOverflow)
            {
                WriteValue(CompilationOptionNames.Checked, Options.CheckOverflow.ToString());
            }

            if (Options.NullableContextOptions != NullableContextOptions.Disable)
            {
                WriteValue(CompilationOptionNames.Nullable, Options.NullableContextOptions.ToString());
            }

            //if (Options.AllowUnsafe)
            //{
            //    WriteValue(CompilationOptionNames.Unsafe, Options.AllowUnsafe.ToString());
            //}

            //var preprocessorSymbols = GetPreprocessorSymbols();
            //if (preprocessorSymbols.Any())
            //{
            //    WriteValue(CompilationOptionNames.Define, string.Join(",", preprocessorSymbols));
            //}

            void WriteValue(string key, string value)
            {
                builder.WriteUTF8(key);
                builder.WriteByte(0);
                builder.WriteUTF8(value);
                builder.WriteByte(0);
            }
        }

        internal override CommonReferenceManager CommonGetBoundReferenceManager()
        {
            return GetBoundReferenceManager();
        }

        internal new ReferenceManager GetBoundReferenceManager()
        {
            if (_lazyAssemblySymbol == null)
            {
                lock (_referenceManager)
                {
                    _lazyAssemblySymbol = _referenceManager.CreateSourceAssemblyForCompilation(this);
                }

                Debug.Assert(_lazyAssemblySymbol != null);
            }

            // referenceManager can only be accessed after we initialized the lazyAssemblySymbol.
            // In fact, initialization of the assembly symbol might change the reference manager.
            return _referenceManager;
        }

        internal override ISymbolInternal CommonGetWellKnownTypeMember(WellKnownMember member)
        {
            return GetWellKnownTypeMember(member);
        }

        internal override ITypeSymbolInternal CommonGetWellKnownType(WellKnownType wellknownType)
        {
            return GetWellKnownType(wellknownType);
        }


        internal override int CompareSourceLocations(Location loc1, Location loc2)
        {
            Debug.Assert(loc1.IsInSource);
            Debug.Assert(loc2.IsInSource);

            var comparison = CompareSyntaxTreeOrdering(loc1.SourceTree, loc2.SourceTree);
            if (comparison != 0)
            {
                return comparison;
            }

            return loc1.SourceSpan.Start - loc2.SourceSpan.Start;
        }

        internal override int CompareSourceLocations(SyntaxReference loc1, SyntaxReference loc2)
        {
            var comparison = CompareSyntaxTreeOrdering(loc1.SyntaxTree, loc2.SyntaxTree);
            if (comparison != 0)
            {
                return comparison;
            }

            return loc1.Span.Start - loc2.Span.Start;
        }

        internal override int CompareSourceLocations(SyntaxNode loc1, SyntaxNode loc2)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Ensures semantic binding and flow analysis.
        /// </summary>
        /// <returns>The result of the task contains enumeration of diagnostics.</returns>
        public async Task<IEnumerable<Diagnostic>> BindAndAnalyseTask(CancellationToken cancellationToken)
        {
            if (_lazyAnalysisTask == null)
            {
                _lazyAnalysisTask = Task.Run(() => SourceCompiler.BindAndAnalyze(this, cancellationToken));
            }

            return await _lazyAnalysisTask.ConfigureAwait(false);
        }

        internal override bool CompileMethods(CommonPEModuleBuilder moduleBuilder, bool emittingPdb,
            bool emitMetadataOnly, bool emitTestCoverageData, DiagnosticBag diagnostics,
            Predicate<ISymbolInternal> filterOpt, CancellationToken cancellationToken)
        {
            // The diagnostics should include syntax and declaration errors. We insert these before calling Emitter.Emit, so that the emitter
            // does not attempt to emit if there are declaration errors (but we do insert all errors from method body binding...)
            bool
                hasDeclarationErrors =
                    false; // !FilterAndAppendDiagnostics(diagnostics, GetDiagnostics(CompilationStage.Declare, true, cancellationToken));

            var moduleBeingBuilt = (PEModuleBuilder)moduleBuilder;

            if (emitMetadataOnly)
            {
                throw new NotImplementedException();
            }

            if (emittingPdb)
            {
                if (!CreateDebugDocuments(
                        moduleBeingBuilt.DebugDocumentsBuilder,
                        moduleBeingBuilt.EmbeddedTexts.Concat(CollectAdditionalEmbeddedTexts()),
                        diagnostics))
                {
                    return false;
                }
            }

            // Use a temporary bag so we don't have to refilter pre-existing diagnostics.
            DiagnosticBag methodBodyDiagnosticBag = DiagnosticBag.GetInstance();

            // Perform initial bind of method bodies in spite of earlier errors. This is the same
            // behavior as when calling GetDiagnostics()

            try
            {
                SourceCompiler.CompileSources(
                    this,
                    moduleBeingBuilt,
                    emittingPdb,
                    hasDeclarationErrors,
                    methodBodyDiagnosticBag,
                    cancellationToken);

                bool hasMethodBodyErrorOrWarningAsError =
                    !FilterAndAppendAndFreeDiagnostics(diagnostics, ref methodBodyDiagnosticBag, cancellationToken);

                if (hasDeclarationErrors || hasMethodBodyErrorOrWarningAsError)
                {
                    return false;
                }

                return true;
            }
            catch (Exception ex)
            {
                this.TrackException(ex);
                throw;
            }
        }

        IEnumerable<EmbeddedText> CollectAdditionalEmbeddedTexts()
        {
            // TODO: if (EmbedPharContentIntoPdb):

            yield break;

            // foreach (var f in this.SourceSymbolCollection.GetFiles())
            // {
            //     var tree = f.SyntaxTree;
            //     if (tree.IsPharEntry || tree.IsPharStub)
            //     {
            //         yield return EmbeddedText.FromSource(tree.GetDebugSourceDocumentPath(), tree.GetText());
            //     }
            // }
        }

        IEnumerable<ResourceDescription> CollectAdditionalManifestResources()
        {
            yield break;
        }


        internal override bool GenerateResourcesAndDocumentationComments(CommonPEModuleBuilder moduleBuilder,
            Stream? xmlDocumentationStream,
            Stream? win32ResourcesStream, bool useRawWin32Resources, string? outputNameOverride,
            DiagnosticBag diagnostics,
            CancellationToken cancellationToken)
        {
            // Use a temporary bag so we don't have to refilter pre-existing diagnostics.
            DiagnosticBag methodBodyDiagnosticBag = DiagnosticBag.GetInstance();

            var moduleBeingBuilt = (PEModuleBuilder)moduleBuilder;

            try
            {
                SetupWin32Resources(moduleBeingBuilt, win32ResourcesStream, methodBodyDiagnosticBag);

                ReportManifestResourceDuplicates(
                    moduleBeingBuilt.ManifestResources,
                    SourceAssembly.Modules.Skip(1).Select((m) => m.Name), //all modules except the first one
                    AddedModulesResourceNames(methodBodyDiagnosticBag),
                    methodBodyDiagnosticBag);

                if (!FilterAndAppendAndFreeDiagnostics(diagnostics, ref methodBodyDiagnosticBag, cancellationToken))
                {
                    return false;
                }
            }
            catch (Exception ex)
            {
                this.TrackException(ex);
                throw;
            }

            cancellationToken.ThrowIfCancellationRequested();

            // Use a temporary bag so we don't have to refilter pre-existing diagnostics.
            DiagnosticBag xmlDiagnostics = DiagnosticBag.GetInstance();

            string assemblyName =
                FileNameUtilities.ChangeExtension(moduleBeingBuilt.EmitOptions.OutputNameOverride, extension: null);
            DocumentationCommentCompiler.WriteDocumentationCommentXml(this, assemblyName, xmlDocumentationStream,
                xmlDiagnostics,
                cancellationToken);

            if (!FilterAndAppendAndFreeDiagnostics(diagnostics, ref xmlDiagnostics, cancellationToken))
            {
                return false;
            }

            return true;
        }

        private IEnumerable<string> AddedModulesResourceNames(DiagnosticBag diagnostics)
        {
            //ImmutableArray<ModuleSymbol> modules = SourceAssembly.Modules;

            //for (int i = 1; i < modules.Length; i++)
            //{
            //    var m = (Symbols.Metadata.PE.PEModuleSymbol)modules[i];
            //    ImmutableArray<EmbeddedResource> resources;

            //    try
            //    {
            //        resources = m.Module.GetEmbeddedResourcesOrThrow();
            //    }
            //    catch (BadImageFormatException)
            //    {
            //        diagnostics.Add(new CSDiagnosticInfo(ErrorCode.ERR_BindToBogus, m), NoLocation.Singleton);
            //        continue;
            //    }

            //    foreach (var resource in resources)
            //    {
            //        yield return resource.Name;
            //    }
            //}

            yield break;
        }

        internal string GetRuntimeMetadataVersion(EmitOptions emitOptions, DiagnosticBag diagnostics)
        {
            string runtimeMDVersion = GetRuntimeMetadataVersion(emitOptions);
            if (runtimeMDVersion != null)
            {
                return runtimeMDVersion;
            }

            //DiagnosticBag runtimeMDVersionDiagnostics = DiagnosticBag.GetInstance();
            //runtimeMDVersionDiagnostics.Add(ErrorCode.WRN_NoRuntimeMetadataVersion, NoLocation.Singleton);
            //if (!FilterAndAppendAndFreeDiagnostics(diagnostics, ref runtimeMDVersionDiagnostics))
            //{
            //    return null;
            //}

            return string.Empty; //prevent emitter from crashing.
        }

        private string GetRuntimeMetadataVersion(EmitOptions emitOptions)
        {
            var corAssembly = SourceAssembly.CorLibrary as PEAssemblySymbol;

            if ((object)corAssembly != null)
            {
                return corAssembly.Assembly.ManifestModule.MetadataVersion;
            }

            return emitOptions.RuntimeMetadataVersion;
        }

        private void SetupWin32Resources(PEModuleBuilder moduleBeingBuilt, Stream win32Resources,
            DiagnosticBag diagnostics)
        {
            if (win32Resources == null)
                return;

            switch (DetectWin32ResourceForm(win32Resources))
            {
                case Win32ResourceForm.COFF:
                    //moduleBeingBuilt.Win32ResourceSection = MakeWin32ResourcesFromCOFF(win32Resources, diagnostics);
                    break;
                case Win32ResourceForm.RES:
                    //moduleBeingBuilt.Win32Resources = MakeWin32ResourceList(win32Resources, diagnostics);
                    break;
                default:
                    //diagnostics.Add(ErrorCode.ERR_BadWin32Res, NoLocation.Singleton, "Unrecognized file format.");
                    break;
            }
        }

        /// <summary>
        /// Gets resource of additional metadata for each of declared symbol.
        /// </summary>
        ResourceDescription SourceMetadataResource()
        {
            return new ResourceDescription(".source.metadata.resources", () =>
            {
                var table = this.SourceSymbolCollection;
                var resources = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

                var stream = new MemoryStream();
                var writer = new System.Resources.ResourceWriter(stream);

                foreach (var pair in resources)
                {
                    writer.AddResource(pair.Key, pair.Value);
                }

                //
                writer.Generate();
                stream.Position = 0;
                return stream;
            }, isPublic: true);
        }

        internal override CommonPEModuleBuilder CreateModuleBuilder(EmitOptions emitOptions,
            IMethodSymbol debugEntryPoint, Stream sourceLinkStream, IEnumerable<EmbeddedText> embeddedTexts,
            IEnumerable<ResourceDescription> manifestResources, CompilationTestData testData, DiagnosticBag diagnostics,
            CancellationToken cancellationToken)
        {
            Debug.Assert(!IsSubmission || HasCodeToEmit());

            var runtimeMDVersion = GetRuntimeMetadataVersion(emitOptions, diagnostics);
            if (runtimeMDVersion == null)
            {
                Debug.Assert(runtimeMDVersion != null, "Set AquilaCommandLineArguments.EmitOptions");
                return null;
            }

            var moduleProps = ConstructModuleSerializationProperties(emitOptions, runtimeMDVersion);

            if (manifestResources == null)
            {
                manifestResources = SpecializedCollections.EmptyEnumerable<ResourceDescription>();
            }

            if (SynthesizedResources != null)
            {
                manifestResources = manifestResources.Concat(SynthesizedResources);
            }

            if (Options.EmbedSourceMetadata)
            {
                manifestResources = manifestResources.Concat(new[] { SourceMetadataResource() });
            }

            PEModuleBuilder moduleBeingBuilt;
            if (_options.OutputKind.IsNetModule())
            {
                moduleBeingBuilt = new PENetModuleBuilder(
                    this,
                    SourceModule,
                    emitOptions,
                    moduleProps,
                    manifestResources);
            }
            else
            {
                var kind = _options.OutputKind.IsValid() ? _options.OutputKind : OutputKind.DynamicallyLinkedLibrary;
                moduleBeingBuilt = new PEAssemblyBuilder(
                    SourceAssembly,
                    moduleProps,
                    manifestResources,
                    kind,
                    emitOptions);
            }

            if (debugEntryPoint != null)
            {
                moduleBeingBuilt.SetDebugEntryPoint((IMethodSymbolInternal)debugEntryPoint, diagnostics);
            }

            moduleBeingBuilt.SourceLinkStreamOpt = sourceLinkStream;

            if (embeddedTexts != null)
            {
                moduleBeingBuilt.EmbeddedTexts = embeddedTexts;
            }

            // testData is only passed when running tests.
            if (testData != null)
            {
                //moduleBeingBuilt.SetMethodTestData(testData.Methods);
                //testData.Module = moduleBeingBuilt;
                throw new NotImplementedException();
            }

            return moduleBeingBuilt;
        }


        internal override EmitDifferenceResult EmitDifference(EmitBaseline baseline, IEnumerable<SemanticEdit> edits,
            Func<ISymbol, bool> isAddedSymbol, Stream metadataStream,
            Stream ilStream, Stream pdbStream, CompilationTestData? testData, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Filter out warnings based on the compiler options (/nowarn, /warn and /warnaserror) and the pragma warning directives.
        /// </summary>
        /// <returns>True when there is no error.</returns>
        private bool FilterAndAppendDiagnostics(DiagnosticBag accumulator, IEnumerable<Diagnostic> incoming,
            CancellationToken cancellationToken)
        {
            bool hasError = false;
            bool reportSuppressedDiagnostics = Options.ReportSuppressedDiagnostics;

            foreach (Diagnostic d in incoming)
            {
                var filtered = _options.FilterDiagnostic(d, cancellationToken);
                if (filtered == null ||
                    (!reportSuppressedDiagnostics && filtered.IsSuppressed))
                {
                    continue;
                }
                else if (filtered.Severity == DiagnosticSeverity.Error)
                {
                    hasError = true;
                }

                accumulator.Add(filtered);
            }

            return !hasError;
        }

        internal override bool HasCodeToEmit()
        {
            return false; //SourceSymbolCollection.GetFiles().Any();
        }

        internal override bool HasSubmissionResult()
        {
            throw new NotImplementedException();
        }

        internal override void ValidateDebugEntryPoint(IMethodSymbol debugEntryPoint, DiagnosticBag diagnostics)
        {
            Debug.Assert(debugEntryPoint != null);

            // Debug entry point has to be a method definition from this compilation.
            var methodSymbol = debugEntryPoint as MethodSymbol;
            if (methodSymbol?.DeclaringCompilation != this || !methodSymbol.IsDefinition)
            {
                //diagnostics.Add(ErrorCode.ERR_DebugEntryPointNotSourceMethodDefinition, Location.None);
            }
        }

        internal override Compilation WithEventQueue(AsyncQueue<CompilationEvent> eventQueue)
        {
            throw new NotImplementedException();
        }

        internal override Compilation WithSemanticModelProvider(SemanticModelProvider semanticModelProvider)
        {
            if (this.SemanticModelProvider == semanticModelProvider)
            {
                return this;
            }

            return new AquilaCompilation(
                this.AssemblyName,
                _options,
                this.ExternalReferences,
                this.IsSubmission,
                _referenceManager,
                reuseReferenceManager: true,
                semanticModelProvider);
        }

        internal override void AddDebugSourceDocumentsForChecksumDirectives(DebugDocumentsBuilder documentsBuilder,
            SyntaxTree tree, DiagnosticBag diagnostics)
        {
        }

        internal override void ReportUnusedImports(DiagnosticBag diagnostics, CancellationToken cancellationToken)
        {
            //throw new NotImplementedException();
        }

        internal override void CompleteTrees(SyntaxTree filterTree)
        {
            //throw new NotImplementedException();
        }

        internal override bool IsUnreferencedAssemblyIdentityDiagnosticCode(int code)
        {
            // TODO: Compare with the appropriate error code when it's supported
            return false;
        }
    }
}