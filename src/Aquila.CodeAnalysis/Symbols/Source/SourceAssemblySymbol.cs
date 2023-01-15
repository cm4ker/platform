using System;
using System.Collections.Immutable;
using System.Diagnostics;
using System.Reflection;
using System.Threading;
using Aquila.CodeAnalysis.Semantics;
using Aquila.Compiler.Utilities;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.PooledObjects;
using Microsoft.CodeAnalysis.Symbols;
using Roslyn.Utilities;

namespace Aquila.CodeAnalysis.Symbols.Source
{
    internal sealed class SourceAssemblySymbol : MetadataOrSourceAssemblySymbol, ISourceAssemblySymbol,
        ISourceAssemblySymbolInternal
    {
        readonly string _simpleName;
        readonly AquilaCompilation _compilation;

        /// <summary>
        /// A list of modules the assembly consists of. 
        /// The first (index=0) module is a SourceModuleSymbol, which is a primary module, the rest are net-modules.
        /// </summary>
        readonly ImmutableArray<ModuleSymbol> _modules;

        AssemblyIdentity _lazyIdentity;

        // Computing the identity requires computing the public key. Computing the public key 
        // can require binding attributes that contain version or strong name information. 
        // Attribute binding will check type visibility which will possibly 
        // check IVT relationships. To correctly determine the IVT relationship requires the public key. 
        // To avoid infinite recursion, this type notes, per thread, the assembly for which the thread 
        // is actively computing the public key (assemblyForWhichCurrentThreadIsComputingKeys). Should a request to determine IVT
        // relationship occur on the thread that is computing the public key, access is optimistically
        // granted provided the simple assembly names match. When such access is granted
        // the assembly to which we have been granted access is noted (optimisticallyGrantedInternalsAccess).
        // After the public key has been computed, the set of optimistic grants is reexamined 
        // to ensure that full identities match. This may produce diagnostics.
        private StrongNameKeys _lazyStrongNameKeys;

        public SourceAssemblySymbol(
            AquilaCompilation compilation,
            string assemblySimpleName,
            string moduleName)
        {
            Debug.Assert(compilation != null);
            Debug.Assert(!String.IsNullOrWhiteSpace(assemblySimpleName));
            Debug.Assert(!String.IsNullOrWhiteSpace(moduleName));

            _compilation = compilation;
            _simpleName = assemblySimpleName;

            var moduleBuilder = new ArrayBuilder<ModuleSymbol>(1);

            moduleBuilder.Add(new SourceModuleSymbol(this, moduleName));

            _modules = moduleBuilder.ToImmutableAndFree();

            if (!compilation.Options.CryptoPublicKey.IsEmpty)
            {
                // Private key is not necessary for assembly identity, only when emitting.  For this reason, the private key can remain null.
                _lazyStrongNameKeys = StrongNameKeys.Create(compilation.Options.CryptoPublicKey, privateKey: null,
                    hasCounterSignature: false, Errors.MessageProvider.Instance);
            }
        }

        public override string Name => _simpleName;

        internal SourceModuleSymbol SourceModule => (SourceModuleSymbol)_modules[0];

        public override ImmutableArray<ModuleSymbol> Modules => _modules;

        public override ImmutableArray<SyntaxReference> DeclaringSyntaxReferences
        {
            get { throw new NotImplementedException(); }
        }

        internal override AquilaCompilation DeclaringCompilation => _compilation;

        public override INamespaceSymbol GlobalNamespace
        {
            get { return SourceModule.GlobalNamespace; }
        }

        internal override bool IsLinked => false;

        public override AssemblyIdentity Identity
        {
            get
            {
                if (_lazyIdentity == null)
                    Interlocked.CompareExchange(ref _lazyIdentity, ComputeIdentity(), null);

                return _lazyIdentity;
            }
        }

        public override Version AssemblyVersionPattern => null; // TODO: Version attribute

        internal override ObsoleteAttributeData ObsoleteAttributeData
        {
            get { throw new NotImplementedException(); }
        }

        internal bool IsDelaySigned
        {
            get
            {
                //commandline setting trumps attribute value. Warning assumed to be given elsewhere
                if (_compilation.Options.DelaySign.HasValue)
                {
                    return _compilation.Options.DelaySign.Value;
                }

                // The public sign argument should also override the attribute
                if (_compilation.Options.PublicSign)
                {
                    return false;
                }

                return false; 
            }
        }

        internal StrongNameKeys StrongNameKeys
        {
            get
            {
                if (_lazyStrongNameKeys == null)
                {
                    Interlocked.CompareExchange(ref _lazyStrongNameKeys, ComputeStrongNameKeys(), null);
                }

                return _lazyStrongNameKeys;
            }
        }

        internal override ImmutableArray<byte> PublicKey
        {
            get { return StrongNameKeys.PublicKey; }
        }

        internal string SignatureKey
        {
            get
            {
                string
                    key = null; 
                return key;
            }
        }

        /// <summary>
        /// This represents what the user claimed in source through the AssemblyFlagsAttribute.
        /// It may be modified as emitted due to presence or absence of the public key.
        /// </summary>
        internal AssemblyNameFlags Flags
        {
            get
            {
                var fieldValue = default(AssemblyNameFlags);

                return fieldValue;
            }
        }

        private StrongNameKeys ComputeStrongNameKeys()
        {
            // when both attributes and command-line options specified, cmd line wins.
            string keyFile = _compilation.Options.CryptoKeyFile;

            // Public sign requires a keyfile
            if (DeclaringCompilation.Options.PublicSign)
            {
                // TODO(https://github.com/dotnet/roslyn/issues/9150):
                // Provide better error message if keys are provided by
                // the attributes. Right now we'll just fall through to the
                // "no key available" error.

                if (!string.IsNullOrEmpty(keyFile) && !PathUtilities.IsAbsolute(keyFile))
                {
                    // If keyFile has a relative path then there should be a diagnostic
                    // about it
                    Debug.Assert(!DeclaringCompilation.Options.Errors.IsEmpty);
                    return StrongNameKeys.None;
                }

                // If we're public signing, we don't need a strong name provider
                return StrongNameKeys.Create(keyFile, Errors.MessageProvider.Instance);
            }

            string keyContainer = _compilation.Options.CryptoKeyContainer;

            bool hasCounterSignature = !string.IsNullOrEmpty(this.SignatureKey);
            return StrongNameKeys.Create(DeclaringCompilation.Options.StrongNameProvider, keyFile, keyContainer,
                hasCounterSignature, Errors.MessageProvider.Instance);
        }

        Version AssemblyVersionAttributeSetting
        {
            get
            {
                var str = FileVersion;
                if (!string.IsNullOrEmpty(str))
                {
                    var sep = str.IndexOfAny(new char[] { '-', ' ' });
                    if (sep >= 0)
                    {
                        str = str.Remove(sep);
                    }

                    if (Version.TryParse(str, out var v))
                    {
                        v = new Version(
                            Math.Max(0, v.Major),
                            Math.Max(0, v.Minor),
                            Math.Max(0, v.Build),
                            Math.Max(0, v.Revision)); // TODO: AssemblyVersionPattern for `-1` fields
                        return v;
                    }
                }

                // no version specified:
                return new Version(1, 0, 0, 0);
            }
        }

        internal string FileVersion => _compilation.Options.VersionString;

        internal string Title => null;

        internal string Description => null;

        internal string Company => null;

        internal string Product => null;

        internal string InformationalVersion => null;

        internal string Copyright => null;

        internal string Trademark => null;

        string AssemblyCultureAttributeSetting => null;

        public AssemblyHashAlgorithm HashAlgorithm => AssemblyHashAlgorithm.Sha1;

        #region ISourceAssemblySymbolInternal

        AssemblyFlags ISourceAssemblySymbolInternal.AssemblyFlags => (AssemblyFlags)Flags;

        string ISourceAssemblySymbolInternal.SignatureKey => SignatureKey;

        AssemblyHashAlgorithm ISourceAssemblySymbolInternal.HashAlgorithm => HashAlgorithm;

        Version IAssemblySymbolInternal.AssemblyVersionPattern => AssemblyVersionPattern;

        bool ISourceAssemblySymbolInternal.InternalsAreVisible => false;

        Compilation ISourceAssemblySymbol.Compilation => _compilation;

        #endregion

        AssemblyIdentity ComputeIdentity()
        {
            return new AssemblyIdentity(
                _simpleName,
                this.AssemblyVersionAttributeSetting,
                this.AssemblyCultureAttributeSetting,
                _compilation.StrongNameKeys.PublicKey,
                hasPublicKey: !_compilation.StrongNameKeys.PublicKey.IsDefault);
        }

        internal override NamedTypeSymbol TryLookupForwardedMetadataTypeWithCycleDetection(
            ref MetadataTypeName emittedName, ConsList<AssemblySymbol> visitedAssemblies)
        {
            int forcedArity = emittedName.ForcedArity;

            if (emittedName.UseCLSCompliantNameArityEncoding)
            {
                if (forcedArity == -1)
                {
                    forcedArity = emittedName.InferredArity;
                }
                else if (forcedArity != emittedName.InferredArity)
                {
                    return null;
                }

                Debug.Assert(forcedArity == emittedName.InferredArity);
            }

            return null;
        }

        public override NamedTypeSymbol GetTypeByMetadataName(string fullyQualifiedMetadataName)
        {
            return SourceModule.SymbolCollection.GetType(
                NameUtils.MakeQualifiedName(fullyQualifiedMetadataName.Replace('.', QualifiedName.Separator), true));
        }

        internal override ImmutableArray<AssemblySymbol> GetLinkedReferencedAssemblies()
        {
            return ImmutableArray<AssemblySymbol>.Empty;
        }

        internal override void SetLinkedReferencedAssemblies(ImmutableArray<AssemblySymbol> assemblies)
        {
            throw ExceptionUtilities.Unreachable;
        }
    }
}