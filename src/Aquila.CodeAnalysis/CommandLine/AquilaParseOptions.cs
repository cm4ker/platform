using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.PooledObjects;
using Roslyn.Utilities;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Aquila.CodeAnalysis;
using Aquila.CodeAnalysis;

namespace Aquila.CodeAnalysis
{
    /// <summary>
    /// This class stores several source parsing related options and offers access to their values.
    /// </summary>
    public sealed class AquilaParseOptions : ParseOptions, IEquatable<AquilaParseOptions>
    {
        /// <summary>
        /// The default parse options.
        /// </summary>
        public static AquilaParseOptions Default { get; } = new AquilaParseOptions();

        ImmutableDictionary<string, string> _features;

        /// <summary>
        /// Gets required language version.
        /// <c>null</c> value respects the parser's default which is always the latest version.
        /// </summary>
        public Version LanguageVersion => _languageVersion;

        readonly Version _languageVersion;

        /// <summary>
        /// Whether to allow the deprecated short open tag syntax.
        /// </summary>
        public bool AllowShortOpenTags => _allowShortOpenTags;

        readonly bool _allowShortOpenTags;

        public AquilaParseOptions(
            DocumentationMode documentationMode = DocumentationMode.Parse,
            SourceCodeKind kind = SourceCodeKind.Regular,
            Version languageVersion = null,
            bool shortOpenTags = false)
            : base(kind, documentationMode)
        {
            if (!kind.IsValid())
            {
                throw new ArgumentOutOfRangeException(nameof(kind));
            }

            _languageVersion = languageVersion;
            _allowShortOpenTags = shortOpenTags;
        }

        internal AquilaParseOptions(
            DocumentationMode documentationMode,
            SourceCodeKind kind,
            Version languageVersion,
            bool shortOpenTags,
            ImmutableDictionary<string, string> features)
            : this(documentationMode, kind, languageVersion, shortOpenTags)
        {
            _features = features ?? throw new ArgumentNullException(nameof(features));
        }

        private AquilaParseOptions(AquilaParseOptions other)
            : this(
                documentationMode: other.DocumentationMode,
                kind: other.Kind,
                languageVersion: other.LanguageVersion,
                shortOpenTags: other.AllowShortOpenTags)
        {
        }

        public new AquilaParseOptions WithKind(SourceCodeKind kind)
        {
            if (kind == this.Kind)
            {
                return this;
            }

            if (!kind.IsValid())
            {
                throw new ArgumentOutOfRangeException(nameof(kind));
            }

            return new AquilaParseOptions(this) { Kind = kind };
        }

        public new AquilaParseOptions WithDocumentationMode(DocumentationMode documentationMode)
        {
            if (documentationMode == this.DocumentationMode)
            {
                return this;
            }

            if (!documentationMode.IsValid())
            {
                throw new ArgumentOutOfRangeException(nameof(documentationMode));
            }

            return new AquilaParseOptions(this) { DocumentationMode = documentationMode };
        }

        public override ParseOptions CommonWithKind(SourceCodeKind kind)
        {
            return WithKind(kind);
        }

        protected override ParseOptions CommonWithDocumentationMode(DocumentationMode documentationMode)
        {
            return WithDocumentationMode(documentationMode);
        }

        protected override ParseOptions CommonWithFeatures(IEnumerable<KeyValuePair<string, string>> features)
        {
            return WithFeatures(features);
        }

        /// <summary>
        /// Enable some experimental language features for testing.
        /// </summary>
        public new AquilaParseOptions WithFeatures(IEnumerable<KeyValuePair<string, string>> features)
        {
            if (features == null)
            {
                throw new ArgumentNullException(nameof(features));
            }

            return new AquilaParseOptions(this)
                { _features = features.ToImmutableDictionary(StringComparer.OrdinalIgnoreCase) };
        }

        public override IReadOnlyDictionary<string, string> Features
        {
            get { return _features; }
        }

        public override IEnumerable<string> PreprocessorSymbolNames
        {
            get { return ImmutableArray<string>.Empty; }
        }

        public override string Language => LanguageConstants.LanguageId;

        internal bool IsFeatureEnabled(MessageID feature)
        {
            return true;
        }

        public override bool Equals(object obj)
        {
            return this.Equals(obj as AquilaParseOptions);
        }

        public bool Equals(AquilaParseOptions other)
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

        public override int GetHashCode()
        {
            return
                Hash.Combine(base.GetHashCodeHelper(), 0);
        }

        internal override void ValidateOptions(ArrayBuilder<Diagnostic> builder)
        {
            // The options are already validated in the setters, throwing exceptions if incorrect
        }
    }
}