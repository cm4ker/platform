// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Aquila.CodeAnalysis.Errors;
using Microsoft.CodeAnalysis;
using Roslyn.Utilities;

namespace Aquila.CodeAnalysis
{
    /// <summary>
    /// Specifies the language version.
    /// </summary>
    public enum LanguageVersion
    {
        /// <summary>
        /// Aquila language version 1
        /// </summary>
        CSharp1 = 1,

        /// <summary>
        /// The latest major supported version.
        /// </summary>
        LatestMajor = int.MaxValue - 2,

        /// <summary>
        /// Preview of the next language version.
        /// </summary>
        Preview = int.MaxValue - 1,

        /// <summary>
        /// The latest supported version of the language.
        /// </summary>
        Latest = int.MaxValue,

        /// <summary>
        /// The default language version, which is the latest supported version.
        /// </summary>
        Default = 0,
    }

    internal static class LanguageVersionExtensionsInternal
    {
        internal static bool IsValid(this LanguageVersion value)
        {
            switch (value)
            {
                case LanguageVersion.CSharp1:
                case LanguageVersion.Preview:
                    return true;
            }

            return false;
        }

        internal static ErrorCode GetErrorCode(this LanguageVersion version)
        {
            switch (version)
            {
                default:
                    throw ExceptionUtilities.UnexpectedValue(version);
            }
        }
    }

    internal class CSharpRequiredLanguageVersion : RequiredLanguageVersion
    {
        internal LanguageVersion Version { get; }

        internal CSharpRequiredLanguageVersion(LanguageVersion version)
        {
            Version = version;
        }

        public override string ToString() => Version.ToDisplayString();
    }

    public static class LanguageVersionFacts
    {
        /// <summary>
        /// Usages of TestOptions.RegularNext and LanguageVersionFacts.CSharpNext
        /// will be replaced with TestOptions.RegularN and LanguageVersion.CSharpN when language version N is introduced.
        /// </summary>
        internal const LanguageVersion CSharpNext = LanguageVersion.Preview;

        /// <summary>
        /// Displays the version number in the format expected on the command-line (/langver flag).
        /// For instance, "6", "7.0", "7.1", "latest".
        /// </summary>
        public static string ToDisplayString(this LanguageVersion version)
        {
            switch (version)
            {
                case LanguageVersion.CSharp1:
                    return "1";
                case LanguageVersion.Default:
                    return "default";
                case LanguageVersion.Latest:
                    return "latest";
                case LanguageVersion.LatestMajor:
                    return "latestmajor";
                case LanguageVersion.Preview:
                    return "preview";
                default:
                    throw ExceptionUtilities.UnexpectedValue(version);
            }
        }

        /// <summary>
        /// Try parse a <see cref="LanguageVersion"/> from a string input, returning default if input was null.
        /// </summary>
        public static bool TryParse(string? version, out LanguageVersion result)
        {
            if (version == null)
            {
                result = LanguageVersion.Default;
                return true;
            }

            switch (CaseInsensitiveComparison.ToLower(version))
            {
                case "default":
                    result = LanguageVersion.Default;
                    return true;

                case "latest":
                    result = LanguageVersion.Latest;
                    return true;

                case "latestmajor":
                    result = LanguageVersion.LatestMajor;
                    return true;

                case "preview":
                    result = LanguageVersion.Preview;
                    return true;

                case "1":
                case "1.0":
                case "iso-1":
                    result = LanguageVersion.CSharp1;
                    return true;

                default:
                    result = LanguageVersion.Default;
                    return false;
            }
        }

        /// <summary>
        /// Map a language version (such as Default, Latest, or CSharpN) to a specific version (CSharpM).
        /// </summary>
        public static LanguageVersion MapSpecifiedToEffectiveVersion(this LanguageVersion version)
        {
            switch (version)
            {
                case LanguageVersion.Latest:
                case LanguageVersion.Default:
                case LanguageVersion.LatestMajor:
                    return LanguageVersion.CSharp1;
                default:
                    return version;
            }
        }

        internal static LanguageVersion CurrentVersion => LanguageVersion.CSharp1;

        /// <summary>Inference of tuple element names was added in C# 7.1</summary>
        internal static bool DisallowInferredTupleElementNames(this LanguageVersion self)
        {
            return self < MessageID.IDS_FeatureInferredTupleNames.RequiredVersion();
        }

        internal static bool AllowNonTrailingNamedArguments(this LanguageVersion self)
        {
            return self >= MessageID.IDS_FeatureNonTrailingNamedArguments.RequiredVersion();
        }

        internal static bool AllowAttributesOnBackingFields(this LanguageVersion self)
        {
            return self >= MessageID.IDS_FeatureAttributesOnBackingFields.RequiredVersion();
        }

        internal static bool AllowImprovedOverloadCandidates(this LanguageVersion self)
        {
            return self >= MessageID.IDS_FeatureImprovedOverloadCandidates.RequiredVersion();
        }
    }
}