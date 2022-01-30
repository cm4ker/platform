using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.PooledObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.Immutable;
using System.Threading;
using Aquila.CodeAnalysis;
using Roslyn.Utilities;

namespace Aquila.CodeAnalysis
{
    /// <summary>
    /// Represents various options that affect compilation, such as 
    /// whether to emit an executable or a library, whether to optimize
    /// generated code, and so on.
    /// </summary>
    public sealed class AquilaCompilationOptions : CompilationOptions, IEquatable<AquilaCompilationOptions>
    {
        /// <summary>
        /// Compilation root directory.
        /// All script paths will be emitted relatively to this path.
        /// </summary>
        public string BaseDirectory { get; private set; }

        /// <summary>
        /// Compilation root directory.
        /// All script paths will be emitted relatively to this path.
        /// </summary>
        public string SdkDirectory { get; private set; }

        /// <summary>
        /// What framework is the compiled assembly supposed to run on,
        /// e.g. <c>.NETCoreApp,Version=v3.1</c>.
        /// </summary>
        public string TargetFramework { get; private set; } = ".NETCoreApp,Version=v6.0";

        /// <summary>
        /// Whether to generate an embedded resource containing additional information about the source symbols.
        /// Used by runtime for reflection.
        /// Default is <c>true</c>.
        /// </summary>
        public bool EmbedSourceMetadata { get; private set; }

        /// <summary>
        /// Source language options.
        /// </summary>
        public AquilaParseOptions ParseOptions { get; private set; }


        /// <summary>
        /// The compilation language version.
        /// Gets <see cref="AquilaParseOptions.LanguageVersion"/> or default language version if not specified.
        /// </summary>
        public Version LanguageVersion => ParseOptions?.LanguageVersion ?? new Version(1, 0);

        /// <summary>
        /// Options diagnostics.
        /// </summary>
        public ImmutableArray<Diagnostic> Diagnostics { get; private set; }

        /// <summary>
        /// The file version string.
        /// </summary>
        public string VersionString { get; private set; }

        /// <summary>
        /// List of observer instances.
        /// </summary>
        public ImmutableArray<IObserver<object>> EventSources { get; internal set; }

        /// <summary>
        /// The compilation optimization level.
        /// </summary>
        public new AquilaOptimizationLevel OptimizationLevel { get; internal set; }

        /// <summary>
        /// Set of compile-time defined constants.
        /// </summary>
        public ImmutableDictionary<string, string> Defines { get; internal set; }

        /// <summary>
        /// Set of relative file names from which class map will be generated.
        /// Contained types will be marked as autoloaded.
        /// </summary>
        public ISet<string> Autoload_ClassMapFiles { get; internal set; }

        /// <summary>
        /// Set of relative file names to be marked as autoloaded.
        /// </summary>
        public ISet<string> Autoload_Files { get; internal set; }

        /// <summary>
        /// Collection of PSR-4 autoload rules.
        /// Matching types (classes, traits and interfaces) will be marked as autoloaded.
        /// </summary>
        public IReadOnlyCollection<(string prefix, string path)> Autoload_PSR4 { get; internal set; }

        /// <summary>
        /// Global Nullable context options.
        /// </summary>
        public override NullableContextOptions NullableContextOptions { get; protected set; }

        ///// <summary>
        ///// Flags applied to the top-level binder created for each syntax tree in the compilation 
        ///// as well as for the binder of global imports.
        ///// </summary>
        //internal BinderFlags TopLevelBinderFlags { get; private set; }

        // Defaults correspond to the compiler's defaults or indicate that the user did not specify when that is significant.
        // That's significant when one option depends on another's setting. SubsystemVersion depends on Platform and Target.
        public AquilaCompilationOptions(
            OutputKind outputKind,
            string baseDirectory,
            string sdkDirectory,
            bool reportSuppressedDiagnostics = false,
            string moduleName = null,
            string mainTypeName = null,
            string scriptClassName = null,
            string versionString = null,
            AquilaOptimizationLevel optimizationLevel = AquilaOptimizationLevel.Debug,
            bool checkOverflow = false,
            string cryptoKeyContainer = null,
            string cryptoKeyFile = null,
            ImmutableArray<byte> cryptoPublicKey = default,
            bool? delaySign = null,
            Platform platform = Platform.AnyCpu,
            ReportDiagnostic generalDiagnosticOption = ReportDiagnostic.Default,
            int warningLevel = 4,
            IEnumerable<KeyValuePair<string, ReportDiagnostic>> specificDiagnosticOptions = null,
            bool concurrentBuild = true,
            bool deterministic = false,
            DateTime currentLocalTime = default(DateTime),
            XmlReferenceResolver xmlReferenceResolver = null,
            SourceReferenceResolver sourceReferenceResolver = null,
            MetadataReferenceResolver metadataReferenceResolver = null,
            AssemblyIdentityComparer assemblyIdentityComparer = null,
            StrongNameProvider strongNameProvider = null,
            bool publicSign = false,
            bool embedSourceMetadata = true,
            ImmutableArray<Diagnostic> diagnostics = default,
            AquilaParseOptions parseOptions = null,
            ImmutableDictionary<string, string> defines = default,
            bool referencesSupersedeLowerVersions = false)
            : this(outputKind, baseDirectory, sdkDirectory,
                reportSuppressedDiagnostics, moduleName, mainTypeName, scriptClassName,
                versionString,
                optimizationLevel, checkOverflow,
                cryptoKeyContainer, cryptoKeyFile, cryptoPublicKey, delaySign, platform,
                generalDiagnosticOption, warningLevel,
                specificDiagnosticOptions, concurrentBuild, deterministic,
                debugPlusMode: false,
                currentLocalTime: currentLocalTime,
                xmlReferenceResolver: xmlReferenceResolver,
                sourceReferenceResolver: sourceReferenceResolver,
                metadataReferenceResolver: metadataReferenceResolver,
                assemblyIdentityComparer: assemblyIdentityComparer,
                strongNameProvider: strongNameProvider,
                metadataImportOptions: MetadataImportOptions.Public,
                publicSign: publicSign,
                embedSourceMetadata: embedSourceMetadata,
                diagnostics: diagnostics,
                defines: defines,
                parseOptions: parseOptions,
                referencesSupersedeLowerVersions: referencesSupersedeLowerVersions)
        {
        }

        // Expects correct arguments.
        internal AquilaCompilationOptions(
            OutputKind outputKind,
            string baseDirectory,
            string sdkDirectory,
            bool reportSuppressedDiagnostics,
            string moduleName,
            string mainTypeName,
            string scriptClassName,
            string versionString,
            AquilaOptimizationLevel optimizationLevel,
            bool checkOverflow,
            string cryptoKeyContainer,
            string cryptoKeyFile,
            ImmutableArray<byte> cryptoPublicKey,
            bool? delaySign,
            Platform platform,
            ReportDiagnostic generalDiagnosticOption,
            int warningLevel,
            IEnumerable<KeyValuePair<string, ReportDiagnostic>> specificDiagnosticOptions,
            bool concurrentBuild,
            bool deterministic,
            DateTime currentLocalTime,
            bool debugPlusMode,
            XmlReferenceResolver xmlReferenceResolver,
            SourceReferenceResolver sourceReferenceResolver,
            MetadataReferenceResolver metadataReferenceResolver,
            AssemblyIdentityComparer assemblyIdentityComparer,
            StrongNameProvider strongNameProvider,
            MetadataImportOptions metadataImportOptions,
            bool publicSign,
            bool embedSourceMetadata,
            ImmutableArray<Diagnostic> diagnostics,
            AquilaParseOptions parseOptions,
            ImmutableDictionary<string, string> defines,
            bool referencesSupersedeLowerVersions)
            : base(outputKind, reportSuppressedDiagnostics, moduleName, mainTypeName, scriptClassName,
                cryptoKeyContainer, cryptoKeyFile, cryptoPublicKey, delaySign, publicSign,
                optimizationLevel.AsOptimizationLevel(), checkOverflow,
                platform, generalDiagnosticOption, warningLevel,
                specificDiagnosticOptions.ToImmutableDictionaryOrEmpty(),
                concurrentBuild, deterministic, currentLocalTime, debugPlusMode, xmlReferenceResolver,
                sourceReferenceResolver, syntaxTreeOptionsProvider: null, metadataReferenceResolver,
                assemblyIdentityComparer,
                strongNameProvider, metadataImportOptions, referencesSupersedeLowerVersions)
        {
            this.BaseDirectory = baseDirectory;
            this.SdkDirectory = sdkDirectory;
            this.EmbedSourceMetadata = embedSourceMetadata;
            this.ParseOptions = parseOptions;
            this.Diagnostics = diagnostics;
            this.VersionString = versionString;
            this.OptimizationLevel = optimizationLevel;
            this.Defines = defines;
        }

        private AquilaCompilationOptions(AquilaCompilationOptions other) : this(
            outputKind: other.OutputKind,
            baseDirectory: other.BaseDirectory,
            sdkDirectory: other.SdkDirectory,
            moduleName: other.ModuleName,
            mainTypeName: other.MainTypeName,
            scriptClassName: other.ScriptClassName,
            versionString: other.VersionString,
            optimizationLevel: other.OptimizationLevel,
            checkOverflow: other.CheckOverflow,
            cryptoKeyContainer: other.CryptoKeyContainer,
            cryptoKeyFile: other.CryptoKeyFile,
            cryptoPublicKey: other.CryptoPublicKey,
            delaySign: other.DelaySign,
            platform: other.Platform,
            generalDiagnosticOption: other.GeneralDiagnosticOption,
            warningLevel: other.WarningLevel,
            specificDiagnosticOptions: other.SpecificDiagnosticOptions,
            concurrentBuild: other.ConcurrentBuild,
            deterministic: other.Deterministic,
            currentLocalTime: other.CurrentLocalTime,
            debugPlusMode: other.DebugPlusMode,
            xmlReferenceResolver: other.XmlReferenceResolver,
            sourceReferenceResolver: other.SourceReferenceResolver,
            metadataReferenceResolver: other.MetadataReferenceResolver,
            assemblyIdentityComparer: other.AssemblyIdentityComparer,
            strongNameProvider: other.StrongNameProvider,
            metadataImportOptions: other.MetadataImportOptions,
            reportSuppressedDiagnostics: other.ReportSuppressedDiagnostics,
            publicSign: other.PublicSign,
            embedSourceMetadata: other.EmbedSourceMetadata,
            diagnostics: other.Diagnostics,
            parseOptions: other.ParseOptions,
            defines: other.Defines,
            referencesSupersedeLowerVersions: other.ReferencesSupersedeLowerVersions)
        {
            EventSources = other.EventSources;
            Autoload_ClassMapFiles = other.Autoload_ClassMapFiles;
            Autoload_Files = other.Autoload_Files;
            Autoload_PSR4 = other.Autoload_PSR4;
        }

        public override string Language => Constants.AquilaLanguageName;

        internal override ImmutableArray<string> GetImports() => ImmutableArray<string>.Empty; // Usings;

        public new AquilaCompilationOptions WithOutputKind(OutputKind kind)
        {
            if (kind == this.OutputKind)
            {
                return this;
            }

            return new AquilaCompilationOptions(this) { OutputKind = kind };
        }

        public new AquilaCompilationOptions WithModuleName(string moduleName)
        {
            if (moduleName == this.ModuleName)
            {
                return this;
            }

            return new AquilaCompilationOptions(this) { ModuleName = moduleName };
        }

        public new AquilaCompilationOptions WithScriptClassName(string name)
        {
            if (name == this.ScriptClassName)
            {
                return this;
            }

            return new AquilaCompilationOptions(this) { ScriptClassName = name };
        }

        public new AquilaCompilationOptions WithMainTypeName(string name)
        {
            if (name == this.MainTypeName)
            {
                return this;
            }

            return new AquilaCompilationOptions(this) { MainTypeName = name };
        }

        public new AquilaCompilationOptions WithCryptoKeyContainer(string name)
        {
            if (name == this.CryptoKeyContainer)
            {
                return this;
            }

            return new AquilaCompilationOptions(this) { CryptoKeyContainer = name };
        }

        public new AquilaCompilationOptions WithCryptoKeyFile(string path)
        {
            if (path == this.CryptoKeyFile)
            {
                return this;
            }

            return new AquilaCompilationOptions(this) { CryptoKeyFile = path };
        }

        public new AquilaCompilationOptions WithCryptoPublicKey(ImmutableArray<byte> value)
        {
            if (value.IsDefault)
            {
                value = ImmutableArray<byte>.Empty;
            }

            if (value == this.CryptoPublicKey)
            {
                return this;
            }

            return new AquilaCompilationOptions(this) { CryptoPublicKey = value };
        }

        public new AquilaCompilationOptions WithDelaySign(bool? value)
        {
            if (value == this.DelaySign)
            {
                return this;
            }

            return new AquilaCompilationOptions(this) { DelaySign = value };
        }

        public new AquilaCompilationOptions WithOptimizationLevel(OptimizationLevel value)
        {
            if (value == base.OptimizationLevel)
            {
                return this;
            }

            return new AquilaCompilationOptions(this) { OptimizationLevel = value.AsAquilaOptimizationLevel() };
        }

        public new AquilaCompilationOptions WithOverflowChecks(bool enabled)
        {
            if (enabled == this.CheckOverflow)
            {
                return this;
            }

            return new AquilaCompilationOptions(this) { CheckOverflow = enabled };
        }

        public new AquilaCompilationOptions WithPlatform(Platform platform)
        {
            if (this.Platform == platform)
            {
                return this;
            }

            return new AquilaCompilationOptions(this) { Platform = platform };
        }

        public new AquilaCompilationOptions WithPublicSign(bool publicSign)
        {
            if (this.PublicSign == publicSign)
            {
                return this;
            }

            return new AquilaCompilationOptions(this) { PublicSign = publicSign };
        }

        protected override CompilationOptions CommonWithGeneralDiagnosticOption(ReportDiagnostic value) =>
            WithGeneralDiagnosticOption(value);

        protected override CompilationOptions CommonWithSpecificDiagnosticOptions(
            ImmutableDictionary<string, ReportDiagnostic> specificDiagnosticOptions) =>
            WithSpecificDiagnosticOptions(specificDiagnosticOptions);

        protected override CompilationOptions CommonWithSpecificDiagnosticOptions(
            IEnumerable<KeyValuePair<string, ReportDiagnostic>> specificDiagnosticOptions) =>
            WithSpecificDiagnosticOptions(specificDiagnosticOptions);

        protected override CompilationOptions CommonWithReportSuppressedDiagnostics(bool reportSuppressedDiagnostics) =>
            WithReportSuppressedDiagnostics(reportSuppressedDiagnostics);

        public new AquilaCompilationOptions WithGeneralDiagnosticOption(ReportDiagnostic value)
        {
            if (this.GeneralDiagnosticOption == value)
            {
                return this;
            }

            return new AquilaCompilationOptions(this) { GeneralDiagnosticOption = value };
        }

        public new AquilaCompilationOptions WithSpecificDiagnosticOptions(
            ImmutableDictionary<string, ReportDiagnostic> values)
        {
            if (values == null)
            {
                values = ImmutableDictionary<string, ReportDiagnostic>.Empty;
            }

            if (this.SpecificDiagnosticOptions == values)
            {
                return this;
            }

            return new AquilaCompilationOptions(this) { SpecificDiagnosticOptions = values };
        }

        public new AquilaCompilationOptions WithSpecificDiagnosticOptions(
            IEnumerable<KeyValuePair<string, ReportDiagnostic>> values) =>
            new AquilaCompilationOptions(this) { SpecificDiagnosticOptions = values.ToImmutableDictionaryOrEmpty() };

        public new AquilaCompilationOptions WithReportSuppressedDiagnostics(bool reportSuppressedDiagnostics)
        {
            if (reportSuppressedDiagnostics == this.ReportSuppressedDiagnostics)
            {
                return this;
            }

            return new AquilaCompilationOptions(this) { ReportSuppressedDiagnostics = reportSuppressedDiagnostics };
        }

        public AquilaCompilationOptions WithWarningLevel(int warningLevel)
        {
            if (warningLevel == this.WarningLevel)
            {
                return this;
            }

            return new AquilaCompilationOptions(this) { WarningLevel = warningLevel };
        }

        public new AquilaCompilationOptions WithConcurrentBuild(bool concurrentBuild)
        {
            if (concurrentBuild == this.ConcurrentBuild)
            {
                return this;
            }

            return new AquilaCompilationOptions(this) { ConcurrentBuild = concurrentBuild };
        }

        public new AquilaCompilationOptions WithDeterministic(bool deterministic)
        {
            if (deterministic == this.Deterministic)
            {
                return this;
            }

            return new AquilaCompilationOptions(this) { Deterministic = deterministic };
        }

        public AquilaCompilationOptions WithDebugPlusMode(bool debugPlusMode)
        {
            if (debugPlusMode == this.DebugPlusMode)
            {
                return this;
            }

            return new AquilaCompilationOptions(this) { DebugPlusMode = debugPlusMode };
        }

        public AquilaCompilationOptions WithDefines(ImmutableDictionary<string, string> defines)
        {
            if (this.Defines == defines)
            {
                return this;
            }

            return new AquilaCompilationOptions(this) { Defines = defines };
        }

        public new AquilaCompilationOptions WithMetadataImportOptions(MetadataImportOptions value)
        {
            if (value == this.MetadataImportOptions)
            {
                return this;
            }

            return new AquilaCompilationOptions(this) { MetadataImportOptions = value };
        }

        public new AquilaCompilationOptions WithXmlReferenceResolver(XmlReferenceResolver resolver)
        {
            if (ReferenceEquals(resolver, this.XmlReferenceResolver))
            {
                return this;
            }

            return new AquilaCompilationOptions(this) { XmlReferenceResolver = resolver };
        }

        public new AquilaCompilationOptions WithSourceReferenceResolver(SourceReferenceResolver resolver)
        {
            if (ReferenceEquals(resolver, this.SourceReferenceResolver))
            {
                return this;
            }

            return new AquilaCompilationOptions(this) { SourceReferenceResolver = resolver };
        }

        public new AquilaCompilationOptions WithMetadataReferenceResolver(MetadataReferenceResolver resolver)
        {
            if (ReferenceEquals(resolver, this.MetadataReferenceResolver))
            {
                return this;
            }

            return new AquilaCompilationOptions(this) { MetadataReferenceResolver = resolver };
        }

        public new AquilaCompilationOptions WithAssemblyIdentityComparer(AssemblyIdentityComparer comparer)
        {
            comparer = comparer ?? AssemblyIdentityComparer.Default;

            if (ReferenceEquals(comparer, this.AssemblyIdentityComparer))
            {
                return this;
            }

            return new AquilaCompilationOptions(this) { AssemblyIdentityComparer = comparer };
        }

        public new AquilaCompilationOptions WithStrongNameProvider(StrongNameProvider provider)
        {
            if (ReferenceEquals(provider, this.StrongNameProvider))
            {
                return this;
            }

            return new AquilaCompilationOptions(this) { StrongNameProvider = provider };
        }

        protected override CompilationOptions CommonWithDeterministic(bool deterministic) =>
            WithDeterministic(deterministic);

        protected override CompilationOptions CommonWithOutputKind(OutputKind kind) => WithOutputKind(kind);

        protected override CompilationOptions CommonWithPlatform(Platform platform) => WithPlatform(platform);

        protected override CompilationOptions CommonWithPublicSign(bool publicSign) => WithPublicSign(publicSign);

        protected override CompilationOptions CommonWithOptimizationLevel(OptimizationLevel value) =>
            WithOptimizationLevel(value);

        protected override CompilationOptions CommonWithAssemblyIdentityComparer(AssemblyIdentityComparer comparer) =>
            WithAssemblyIdentityComparer(comparer);

        protected override CompilationOptions CommonWithXmlReferenceResolver(XmlReferenceResolver resolver) =>
            WithXmlReferenceResolver(resolver);

        protected override CompilationOptions CommonWithSourceReferenceResolver(SourceReferenceResolver resolver) =>
            WithSourceReferenceResolver(resolver);

        protected override CompilationOptions CommonWithSyntaxTreeOptionsProvider(SyntaxTreeOptionsProvider? resolver)
        {
            throw new NotImplementedException();
        }

        protected override CompilationOptions CommonWithMetadataReferenceResolver(MetadataReferenceResolver resolver) =>
            WithMetadataReferenceResolver(resolver);

        protected override CompilationOptions CommonWithStrongNameProvider(StrongNameProvider provider) =>
            WithStrongNameProvider(provider);

        protected override CompilationOptions CommonWithConcurrentBuild(bool concurrent) =>
            WithConcurrentBuild(concurrent);

        protected override CompilationOptions CommonWithModuleName(string moduleName) =>
            WithModuleName(moduleName);

        protected override CompilationOptions CommonWithMainTypeName(string mainTypeName) =>
            WithMainTypeName(mainTypeName);

        protected override CompilationOptions CommonWithScriptClassName(string scriptClassName) =>
            WithScriptClassName(scriptClassName);

        protected override CompilationOptions CommonWithCryptoKeyContainer(string cryptoKeyContainer) =>
            WithCryptoKeyContainer(cryptoKeyContainer);

        protected override CompilationOptions CommonWithCryptoKeyFile(string cryptoKeyFile) =>
            WithCryptoKeyFile(cryptoKeyFile);

        protected override CompilationOptions CommonWithCryptoPublicKey(ImmutableArray<byte> cryptoPublicKey) =>
            WithCryptoPublicKey(cryptoPublicKey);

        protected override CompilationOptions CommonWithDelaySign(bool? delaySign) =>
            WithDelaySign(delaySign);

        protected override CompilationOptions CommonWithCheckOverflow(bool checkOverflow) =>
            WithOverflowChecks(checkOverflow);

        protected override CompilationOptions CommonWithMetadataImportOptions(MetadataImportOptions value) =>
            WithMetadataImportOptions(value);

        [Obsolete]
        protected override CompilationOptions CommonWithFeatures(ImmutableArray<string> features)
        {
            throw new NotImplementedException();
        }

        internal override void ValidateOptions(ArrayBuilder<Diagnostic> builder)
        {
            ////  /main & /target:{library|netmodule|winmdobj}
            //if (this.MainTypeName != null)
            //{
            //    if (this.OutputKind.IsValid() && !this.OutputKind.IsApplication())
            //    {
            //        builder.Add(Diagnostic.Create(MessageProvider.Instance, (int)ErrorCode.ERR_NoMainOnDLL));
            //    }

            //    if (!MainTypeName.IsValidClrTypeName())
            //    {
            //        builder.Add(Diagnostic.Create(MessageProvider.Instance, (int)ErrorCode.ERR_BadCompilationOptionValue, nameof(MainTypeName), MainTypeName));
            //    }
            //}

            //if (!Platform.IsValid())
            //{
            //    builder.Add(Diagnostic.Create(MessageProvider.Instance, (int)ErrorCode.ERR_BadPlatformType, Platform.ToString()));
            //}

            //if (ModuleName != null)
            //{
            //    Exception e = MetadataHelpers.CheckAssemblyOrModuleName(ModuleName, nameof(ModuleName));
            //    if (e != null)
            //    {
            //        builder.Add(Diagnostic.Create(MessageProvider.Instance, (int)ErrorCode.ERR_BadCompilationOption, e.Message));
            //    }
            //}

            //if (!OutputKind.IsValid())
            //{
            //    builder.Add(Diagnostic.Create(MessageProvider.Instance, (int)ErrorCode.ERR_BadCompilationOptionValue, nameof(OutputKind), OutputKind.ToString()));
            //}

            //if (!OptimizationLevel.IsValid())
            //{
            //    builder.Add(Diagnostic.Create(MessageProvider.Instance, (int)ErrorCode.ERR_BadCompilationOptionValue, nameof(OptimizationLevel), OptimizationLevel.ToString()));
            //}

            //if (ScriptClassName == null || !ScriptClassName.IsValidClrTypeName())
            //{
            //    builder.Add(Diagnostic.Create(MessageProvider.Instance, (int)ErrorCode.ERR_BadCompilationOptionValue, nameof(ScriptClassName), ScriptClassName ?? "null"));
            //}

            //if (WarningLevel < 0 || WarningLevel > 4)
            //{
            //    builder.Add(Diagnostic.Create(MessageProvider.Instance, (int)ErrorCode.ERR_BadCompilationOptionValue, nameof(WarningLevel), WarningLevel));
            //}

            //if (Usings != null && Usings.Any(u => !u.IsValidClrNamespaceName()))
            //{
            //    builder.Add(Diagnostic.Create(MessageProvider.Instance, (int)ErrorCode.ERR_BadCompilationOptionValue, nameof(Usings), Usings.Where(u => !u.IsValidClrNamespaceName()).First() ?? "null"));
            //}

            //if (Platform == Platform.AnyCpu32BitPreferred && OutputKind.IsValid() && !(OutputKind == OutputKind.ConsoleApplication || OutputKind == OutputKind.WindowsApplication || OutputKind == OutputKind.WindowsRuntimeApplication))
            //{
            //    builder.Add(Diagnostic.Create(MessageProvider.Instance, (int)ErrorCode.ERR_BadPrefer32OnLib));
            //}

            //// TODO: add check for 
            ////          (kind == 'arm' || kind == 'appcontainer' || kind == 'winmdobj') &&
            ////          (version >= "6.2")

            //if (!CryptoPublicKey.IsEmpty)
            //{
            //    if (CryptoKeyFile != null)
            //    {
            //        builder.Add(Diagnostic.Create(MessageProvider.Instance, (int)ErrorCode.ERR_MutuallyExclusiveOptions, nameof(CryptoPublicKey), nameof(CryptoKeyFile)));
            //    }

            //    if (CryptoKeyContainer != null)
            //    {
            //        builder.Add(Diagnostic.Create(MessageProvider.Instance, (int)ErrorCode.ERR_MutuallyExclusiveOptions, nameof(CryptoPublicKey), nameof(CryptoKeyContainer)));
            //    }
            //}

            //if (PublicSign && DelaySign == true)
            //{
            //    builder.Add(Diagnostic.Create(MessageProvider.Instance, (int)ErrorCode.ERR_MutuallyExclusiveOptions, nameof(PublicSign), nameof(DelaySign)));
            //}
        }

        public bool Equals(AquilaCompilationOptions other)
        {
            if (object.ReferenceEquals(this, other))
            {
                return true;
            }

            if (!base.EqualsHelper(other))
            {
                return false;
            }

            return true;
        }

        public override bool Equals(object obj)
        {
            return this.Equals(obj as AquilaCompilationOptions);
        }

        public override int GetHashCode()
        {
            return base.GetHashCodeHelper();
        }

        internal override Diagnostic FilterDiagnostic(Diagnostic diagnostic, CancellationToken cancellationToken)
        {
            if (diagnostic == null)
            {
                return null;
            }

            ReportDiagnostic reportAction;

            if (SpecificDiagnosticOptions != null &&
                SpecificDiagnosticOptions.TryGetValue(diagnostic.Id, out ReportDiagnostic d))
            {
                reportAction = d;
            }
            else
            {
                reportAction = ReportDiagnostic.Default;
            }

            return diagnostic.WithReportDiagnostic(reportAction);
        }
    }
}