﻿using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using Aquila.CodeAnalysis.Utilities;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Emit;
using Microsoft.CodeAnalysis.PooledObjects;
using Microsoft.CodeAnalysis.Text;
using Roslyn.Utilities;

namespace Aquila.CodeAnalysis.CommandLine
{
    public sealed class AquilaCommandLineParser : CommandLineParser
    {
        public static AquilaCommandLineParser Default { get; } = new AquilaCommandLineParser();

        protected override string RegularFileExtension { get; } = LanguageConstants.ScriptFileExtension;
        protected override string ScriptFileExtension { get; } = LanguageConstants.ScriptFileExtension;

        internal AquilaCommandLineParser()
            : base(Errors.MessageProvider.Instance, false)
        {
        }

        /// <summary>
        /// Extends support for encoding names in additon to codepages.
        /// </summary>
        new static Encoding TryParseEncodingName(string arg)
        {
            try
            {
                var encoding = Encoding.GetEncoding(arg);
                if (encoding != null)
                {
                    return encoding;
                }
            }
            catch
            {
                // ignore
            }

            // default behavior:
            return CommandLineParser.TryParseEncodingName(arg);
        }

        internal override CommandLineArguments CommonParse(IEnumerable<string> args, string baseDirectory,
            string sdkDirectoryOpt, string additionalReferenceDirectories)
        {
            List<Diagnostic> diagnostics = new List<Diagnostic>();
            var flattenedArgs = ArrayBuilder<string>.GetInstance();
            List<string> scriptArgs = IsScriptCommandLineParser ? new List<string>() : null;
            FlattenArgs(args, diagnostics, flattenedArgs, scriptArgs, baseDirectory);

            var sourceFiles = new List<CommandLineSourceFile>();
            var metadataFiles = new List<string>();
            var viewFiles = new List<CommandLineSourceFile>();
            var metadataReferences = new List<CommandLineReference>();
            var analyzers = new List<CommandLineAnalyzerReference>();
            var analyzerConfigPaths = new List<string>();
            var additionalFiles = new List<CommandLineSourceFile>();
            var embeddedFiles = new List<CommandLineSourceFile>();
            var managedResources = new List<ResourceDescription>();
            var defines = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
            string outputDirectory = baseDirectory;
            string subDirectory = null;
            string targetFramework = null;
            string outputFileName = null;
            string documentationPath = null;
            string moduleName = null;
            string runtimeMetadataVersion = null; // will be read from cor library if not specified in cmd
            string compilationName = null;
            string versionString = null;
            Encoding codepage = null;
            bool embedAllSourceFiles = false;
            AquilaOptimizationLevel optimization = AquilaOptimizationLevel.Debug;
            bool concurrentBuild = true;
            var diagnosticOptions = new Dictionary<string, ReportDiagnostic>();
            OutputKind outputKind = OutputKind.ConsoleApplication;
            bool optionsEnded = false;
            bool displayHelp = false, displayLogo = true;
            bool emitPdb = true, debugPlus = false;
            string sourceLink = null;
            string mainTypeName = null, pdbPath = null;
            Version languageVersion = null;
            bool? delaySignSetting = null;
            string keyFileSetting = null;
            string keyContainerSetting = null;
            bool publicSign = false;
            bool shortOpenTags = false;
            bool printFullPaths = false;
            bool resourcesOrModulesSpecified = false;
            DebugInformationFormat debugInformationFormat = DebugInformationFormat.Pdb;
            List<string> referencePaths = new List<string>();
            List<string> keyFileSearchPaths = new List<string>();
            var autoload_classmapfiles = new HashSet<string>(StringComparer.InvariantCultureIgnoreCase);
            var autoload_files = new HashSet<string>(StringComparer.InvariantCultureIgnoreCase);
            var autoload_psr4 = new List<(string prefix, string path)>();

            if (sdkDirectoryOpt != null) referencePaths.Add(sdkDirectoryOpt);
            if (!string.IsNullOrEmpty(additionalReferenceDirectories))
                referencePaths.AddRange(additionalReferenceDirectories.Split(new char[] { ';' },
                    StringSplitOptions.RemoveEmptyEntries));

            foreach (string arg in flattenedArgs)
            {
                ArrayBuilder<string> filePathBuilder;
                Debug.Assert(optionsEnded || !arg.StartsWith("@", StringComparison.Ordinal));

                ReadOnlyMemory<char> nameMemory;
                ReadOnlyMemory<char>? valueMemory;
                if (optionsEnded || !TryParseOption(arg, out nameMemory, out valueMemory))
                {
                    filePathBuilder = ArrayBuilder<string>.GetInstance();
                    ParseFileArgument(arg.AsMemory(), baseDirectory, filePathBuilder, diagnostics);
                    foreach (var path in filePathBuilder)
                    {
                        sourceFiles.Add(ToCommandLineSourceFile(path));
                    }

                    filePathBuilder.Free();


                    continue;
                }

                string value = valueMemory is { } m ? m.Span.ToString() : null ?? "";
                string name = nameMemory.Span.ToString().ToLowerInvariant();

                switch (name)
                {
                    case "?":
                    case "help":
                        displayHelp = true;
                        continue;

                    case "d":
                    case "define":
                        ParseDefine(value, defines);
                        continue;

                    case "codepage":
                        value = RemoveQuotesAndSlashes(value);
                        if (value == null)
                        {
                            diagnostics.Add(
                                Errors.MessageProvider.Instance.CreateDiagnostic(Errors.ErrorCode.ERR_SwitchNeedsValue,
                                    Location.None, name));
                            continue;
                        }

                        var encoding = TryParseEncodingName(value);
                        if (encoding == null)
                        {
                            diagnostics.Add(
                                Errors.MessageProvider.Instance.CreateDiagnostic(Errors.ErrorCode.FTL_BadCodepage,
                                    Location.None, value));
                            continue;
                        }

                        codepage = encoding;
                        continue;

                    case "r":
                    case "reference":
                        metadataReferences.AddRange(ParseAssemblyReferences(arg, value, diagnostics,
                            embedInteropTypes: false));
                        continue;

                    case "er":
                        //cache directory with dirs
                        continue;

                    case "sourcelink":
                        value = RemoveQuotesAndSlashes(value);
                        if (!string.IsNullOrEmpty(value))
                        {
                            sourceLink = ParseGenericPathToFile(value, diagnostics, baseDirectory);
                        }

                        continue;
                    //metadata
                    case "md":
                        value = RemoveQuotesAndSlashes(value);
                        if (string.IsNullOrEmpty(value))
                        {
                            diagnostics.Add(
                                Errors.MessageProvider.Instance.CreateDiagnostic(Errors.ErrorCode.ERR_FileNotFound,
                                    Location.None, ""));
                        }
                        else
                        {
                            metadataFiles.Add(value);
                        }

                        continue;
                    //views
                    case "view":
                        value = RemoveQuotesAndSlashes(value);
                        if (string.IsNullOrEmpty(value))
                        {
                            diagnostics.Add(
                                Errors.MessageProvider.Instance.CreateDiagnostic(Errors.ErrorCode.ERR_FileNotFound,
                                    Location.None, ""));
                        }
                        else
                        {
                            viewFiles.Add(new CommandLineSourceFile(value, false));
                        }

                        continue;
                    case "debug":
                    case "debug-type":
                        emitPdb = true;

                        // unused, parsed for backward compat only
                        if (!string.IsNullOrEmpty(value))
                        {
                            switch (value.ToLower())
                            {
                                case "full":
                                case "pdbonly":
                                    debugInformationFormat = DebugInformationFormat.Pdb;
                                    break;
                                case "portable":
                                    debugInformationFormat = DebugInformationFormat.PortablePdb;
                                    break;
                                case "embedded":
                                    debugInformationFormat = DebugInformationFormat.Embedded;
                                    break;
                            }
                        }

                        continue;

                    case "debug+":
                        //guard against "debug+:xx"
                        if (value != null)
                            break;

                        emitPdb = true;
                        debugPlus = true;
                        continue;

                    case "debug-":
                        if (value != null)
                            break;

                        emitPdb = false;
                        debugPlus = false;
                        continue;

                    case "o":
                    case "optimize":
                    case "o+":
                    case "optimize+":
                        if (value == null)
                        {
                            optimization = AquilaOptimizationLevel.Release;
                        }
                        else if (bool.TryParse(value, out var optimizationBool))
                        {
                            optimization = optimizationBool
                                ? AquilaOptimizationLevel.Release
                                : AquilaOptimizationLevel.Debug;
                        }
                        else if (int.TryParse(value, out var optimizationNumber) &&
                                 Enum.IsDefined(typeof(AquilaOptimizationLevel), optimizationNumber))
                        {
                            optimization = (AquilaOptimizationLevel)optimizationNumber;
                        }
                        else if (Enum.TryParse(value, true, out optimization))
                        {
                            //
                        }
                        else
                        {
                            diagnostics.Add(Errors.MessageProvider.Instance.CreateDiagnostic(
                                Errors.ErrorCode.ERR_BadCompilationOptionValue, Location.None, name, value));
                        }

                        continue;

                    case "o-":
                    case "optimize-":
                        if (value != null)
                            break;

                        optimization = AquilaOptimizationLevel.Debug;
                        continue;

                    case "p":
                    case "parallel":
                    case "p+":
                    case "parallel+":
                        if (value != null)
                            break;

                        concurrentBuild = true;
                        continue;

                    case "p-":
                    case "parallel-":
                        if (value != null)
                            break;

                        concurrentBuild = false;
                        continue;

                    case "nowarn":
                        if (string.IsNullOrEmpty(value))
                        {
                            diagnostics.Add(
                                Errors.MessageProvider.Instance.CreateDiagnostic(
                                    Errors.ErrorCode.ERR_SwitchNeedsValue,
                                    Location.None, name));
                        }
                        else
                        {
                            foreach (var warn in value.Split(new char[] { ',', ';', ' ' },
                                         StringSplitOptions.RemoveEmptyEntries))
                            {
                                diagnosticOptions[warn] = ReportDiagnostic.Suppress;
                            }
                        }

                        continue;

                    case "langversion":
                        value = RemoveQuotesAndSlashes(value);
                        if (string.IsNullOrEmpty(value))
                        {
                            diagnostics.Add(
                                Errors.MessageProvider.Instance.CreateDiagnostic(
                                    Errors.ErrorCode.ERR_SwitchNeedsValue,
                                    Location.None, name));
                        }
                        else if (string.Equals(value, "default", StringComparison.OrdinalIgnoreCase) ||
                                 string.Equals(value, "latest", StringComparison.OrdinalIgnoreCase))
                        {
                            languageVersion = null; // default
                        }
                        else if (!Version.TryParse(value, out languageVersion))
                        {
                            throw new ArgumentException("langversion");
                        }

                        continue;

                    case "delaysign":
                    case "delaysign+":
                        if (value != null)
                        {
                            break;
                        }

                        delaySignSetting = true;
                        continue;

                    case "delaysign-":
                        if (value != null)
                        {
                            break;
                        }

                        delaySignSetting = false;
                        continue;

                    case "publicsign":
                    case "publicsign+":
                        if (value != null)
                        {
                            break;
                        }

                        publicSign = true;
                        continue;

                    case "publicsign-":
                        if (value != null)
                        {
                            break;
                        }

                        publicSign = false;
                        continue;

                    case "keyfile":
                        value = RemoveQuotesAndSlashes(value);
                        if (string.IsNullOrEmpty(value))
                        {
                            diagnostics.Add(
                                Errors.MessageProvider.Instance.CreateDiagnostic(
                                    Errors.ErrorCode.ERR_SwitchNeedsValue,
                                    Location.None, name));
                        }
                        else
                        {
                            keyFileSetting = value;
                        }

                        // NOTE: Dev11/VB also clears "keycontainer", see also:
                        //
                        // MSDN: In case both /keyfile and /keycontainer are specified (either by command line option or by 
                        // MSDN: custom attribute) in the same compilation, the compiler will first try the key container. 
                        // MSDN: If that succeeds, then the assembly is signed with the information in the key container. 
                        // MSDN: If the compiler does not find the key container, it will try the file specified with /keyfile. 
                        // MSDN: If that succeeds, the assembly is signed with the information in the key file and the key 
                        // MSDN: information will be installed in the key container (similar to sn -i) so that on the next 
                        // MSDN: compilation, the key container will be valid.
                        continue;

                    case "keycontainer":
                        if (string.IsNullOrEmpty(value))
                        {
                            diagnostics.Add(
                                Errors.MessageProvider.Instance.CreateDiagnostic(
                                    Errors.ErrorCode.ERR_SwitchNeedsValue,
                                    Location.None, name));
                        }
                        else
                        {
                            keyContainerSetting = value;
                        }

                        // NOTE: Dev11/VB also clears "keyfile", see also:
                        //
                        // MSDN: In case both /keyfile and /keycontainer are specified (either by command line option or by 
                        // MSDN: custom attribute) in the same compilation, the compiler will first try the key container. 
                        // MSDN: If that succeeds, then the assembly is signed with the information in the key container. 
                        // MSDN: If the compiler does not find the key container, it will try the file specified with /keyfile. 
                        // MSDN: If that succeeds, the assembly is signed with the information in the key file and the key 
                        // MSDN: information will be installed in the key container (similar to sn -i) so that on the next 
                        // MSDN: compilation, the key container will be valid.
                        continue;

                    case "shortopentag":
                        if (string.IsNullOrEmpty(value))
                        {
                            shortOpenTags = true;
                        }
                        else if (!bool.TryParse(RemoveQuotesAndSlashes(value), out shortOpenTags))
                        {
                            diagnostics.Add(Errors.MessageProvider.Instance.CreateDiagnostic(
                                Errors.ErrorCode.ERR_BadCompilationOptionValue, Location.None,
                                name, value));
                        }

                        continue;

                    case "shortopentag+":
                        shortOpenTags = true;
                        continue;

                    case "shortopentag-":
                        shortOpenTags = false;
                        continue;

                    case "nologo":
                        displayLogo = false;
                        continue;

                    case "m":
                    case "main":
                        // Remove any quotes for consistent behavior as MSBuild can return quoted or 
                        // unquoted main.    
                        var unquoted = RemoveQuotesAndSlashes(value);
                        if (string.IsNullOrEmpty(unquoted))
                        {
                            diagnostics.Add(
                                Errors.MessageProvider.Instance.CreateDiagnostic(
                                    Errors.ErrorCode.ERR_SwitchNeedsValue,
                                    Location.None, name));
                        }
                        else
                        {
                            mainTypeName = unquoted;
                        }

                        continue;

                    case "fullpaths":
                        if (value != null)
                        {
                            if (!bool.TryParse(RemoveQuotesAndSlashes(value), out printFullPaths))
                            {
                                diagnostics.Add(Errors.MessageProvider.Instance.CreateDiagnostic(
                                    Errors.ErrorCode.ERR_BadCompilationOptionValue, Location.None,
                                    name, value));
                            }
                        }
                        else
                        {
                            printFullPaths = true;
                        }

                        continue;

                    case "pdb":
                        if (string.IsNullOrEmpty(value))
                        {
                            diagnostics.Add(
                                Errors.MessageProvider.Instance.CreateDiagnostic(
                                    Errors.ErrorCode.ERR_SwitchNeedsValue,
                                    Location.None, name));
                        }
                        else
                        {
                            pdbPath = ParsePdbPath(value, diagnostics, baseDirectory);
                        }

                        continue;

                    case "out":
                        if (string.IsNullOrWhiteSpace(value))
                        {
                            diagnostics.Add(
                                Errors.MessageProvider.Instance.CreateDiagnostic(
                                    Errors.ErrorCode.ERR_SwitchNeedsValue,
                                    Location.None, name));
                        }
                        else
                        {
                            ParseOutputFile(value, diagnostics, baseDirectory, out outputFileName, out outputDirectory);
                        }

                        continue;

                    case "t":
                    case "target":
                        if (value == null)
                        {
                            break; // force 'unrecognized option'
                        }

                        if (string.IsNullOrEmpty(value))
                        {
                            diagnostics.Add(
                                Errors.MessageProvider.Instance.CreateDiagnostic(
                                    Errors.ErrorCode.ERR_SwitchNeedsValue,
                                    Location.None, name));
                        }
                        else
                        {
                            outputKind = ParseTarget(value, diagnostics);
                        }

                        continue;

                    case "target-framework":
                        if (string.IsNullOrEmpty(value))
                        {
                            diagnostics.Add(
                                Errors.MessageProvider.Instance.CreateDiagnostic(
                                    Errors.ErrorCode.ERR_SwitchNeedsValue,
                                    Location.None, name));
                        }
                        else
                        {
                            targetFramework = value;
                        }

                        continue;

                    case "xmldoc":
                    case "doc":
                        documentationPath = value ?? string.Empty;
                        break;

                    case "modulename":
                        var unquotedModuleName = RemoveQuotesAndSlashes(value);
                        if (string.IsNullOrEmpty(unquotedModuleName))
                        {
                            diagnostics.Add(
                                Errors.MessageProvider.Instance.CreateDiagnostic(
                                    Errors.ErrorCode.ERR_SwitchNeedsValue,
                                    Location.None, name));
                        }
                        else
                        {
                            moduleName = unquotedModuleName;
                        }

                        continue;

                    case "version":
                    case "v":
                        versionString = RemoveQuotesAndSlashes(value);
                        continue;

                    case "runtimemetadataversion":
                        unquoted = RemoveQuotesAndSlashes(value);
                        if (string.IsNullOrEmpty(unquoted))
                        {
                            diagnostics.Add(
                                Errors.MessageProvider.Instance.CreateDiagnostic(
                                    Errors.ErrorCode.ERR_SwitchNeedsValue,
                                    Location.None, name));
                        }
                        else
                        {
                            runtimeMetadataVersion = unquoted;
                        }

                        continue;

                    case "res":
                    case "resource":
                        if (value == null)
                        {
                            break; // Dev11 reports unrecognized option
                        }

                        var embeddedResource =
                            ParseResourceDescription(arg, valueMemory.Value, baseDirectory, diagnostics,
                                embedded: true);
                        if (embeddedResource != null)
                        {
                            managedResources.Add(embeddedResource);
                            resourcesOrModulesSpecified = true;
                        }

                        continue;

                    case "linkres":
                    case "linkresource":
                        if (value == null)
                        {
                            break; // Dev11 reports unrecognized option
                        }

                        var linkedResource =
                            ParseResourceDescription(arg, valueMemory.Value, baseDirectory, diagnostics,
                                embedded: false);
                        if (linkedResource != null)
                        {
                            managedResources.Add(linkedResource);
                            resourcesOrModulesSpecified = true;
                        }

                        continue;

                    case "subdir":
                        if (!string.IsNullOrEmpty(value))
                        {
                            // TODO: check value
                            subDirectory =
                                Utilities.AquilaFileUtilities.NormalizeSlashes(RemoveQuotesAndSlashes(value));
                        }

                        continue;

                    case "embed":
                        if (string.IsNullOrEmpty(value))
                        {
                            embedAllSourceFiles = true;
                            continue;
                        }

                        foreach (var path in ParseSeparatedFileArgument(value, baseDirectory, diagnostics))
                        {
                            embeddedFiles.Add(ToCommandLineSourceFile(path));
                        }

                        continue;

                    case "autoload":
                        if (string.IsNullOrWhiteSpace(value))
                        {
                            diagnostics.Add(
                                Errors.MessageProvider.Instance.CreateDiagnostic(
                                    Errors.ErrorCode.ERR_SwitchNeedsValue,
                                    Location.None, name));
                            break;
                        }

                        const string classmapprefix = "classmap,";
                        const string psr4prefix = "psr-4,";
                        const string filesprefix = "files,";

                        if (value.StartsWith(classmapprefix, StringComparison.OrdinalIgnoreCase))
                        {
                            // "classmap,<fullfilename>"
                            autoload_classmapfiles.Add(value.Substring(classmapprefix.Length));
                            break;
                        }
                        else if (value.StartsWith(psr4prefix, StringComparison.OrdinalIgnoreCase))
                        {
                            // "psr-4,<prefix>,<path>"
                            var prefix_dir = value.Substring(psr4prefix.Length);
                            var comma = prefix_dir.IndexOf(',');
                            if (comma >= 0)
                            {
                                autoload_psr4.Add((prefix_dir.Remove(comma), prefix_dir.Substring(comma + 1)));
                            }

                            break;
                        }
                        else if (value.StartsWith(filesprefix, StringComparison.OrdinalIgnoreCase))
                        {
                            // "files,<fullfilename>"
                            autoload_files.Add(value.Substring(filesprefix.Length));
                            break;
                        }

                        // not handled
                        diagnostics.Add(Errors.MessageProvider.Instance.CreateDiagnostic(
                            Errors.ErrorCode.ERR_BadCompilationOptionValue, Location.None, name,
                            value));
                        break;

                    default:
                        break;
                }
            }

            GetCompilationAndModuleNames(diagnostics, outputKind, sourceFiles,
                sourceFiles.Count != 0, null, ref outputFileName, ref moduleName,
                out compilationName);

            //
            if (sourceFiles.Count == 0 && !IsScriptCommandLineParser &&
                (outputKind.IsNetModule() || !resourcesOrModulesSpecified))
            {
                // warning: no source files specified
                diagnostics.Add(
                    Errors.MessageProvider.Instance.CreateDiagnostic(Errors.ErrorCode.WRN_NoSourceFiles,
                        Location.None));
            }

            // XML Documentation path
            if (documentationPath != null)
            {
                if (documentationPath.Length == 0)
                {
                    // default xmldoc file name
                    documentationPath =
                        PathUtilities.CombinePossiblyRelativeAndRelativePaths(outputDirectory,
                            compilationName + ".xml");
                }
                else
                {
                    // resolve path
                    documentationPath =
                        PathUtilities.CombinePossiblyRelativeAndRelativePaths(baseDirectory, documentationPath);
                }
            }

            // sanitize autoload paths, prefix them with subdir
            for (int i = 0; i < autoload_psr4.Count; i++)
            {
                var value = autoload_psr4[i];
                var path = subDirectory == null
                    ? value.path
                    : PathUtilities.CombinePathsUnchecked(subDirectory, value.path);

                autoload_psr4[i] = (value.prefix, AquilaFileUtilities.NormalizeSlashes(path));
            }

            autoload_classmapfiles = new HashSet<string>(
                autoload_classmapfiles.Select(path =>
                    AquilaFileUtilities.NormalizeSlashes(subDirectory == null
                        ? path
                        : PathUtilities.CombinePathsUnchecked(subDirectory, path))),
                autoload_classmapfiles.Comparer);

            autoload_files = new HashSet<string>(
                autoload_files.Select(path =>
                    AquilaFileUtilities.NormalizeSlashes(subDirectory == null
                        ? path
                        : PathUtilities.CombinePathsUnchecked(subDirectory, path))),
                autoload_files.Comparer);

            // event source // TODO: change to EventSource
            var evetsources = new[]
                    { CreateObserver("Aquila.Compiler.Diagnostics.Observer,Aquila.Compiler.Diagnostics", moduleName) }
                .WhereNotNull();

#if TRACE
            evetsources = evetsources.Concat(new CompilationTrackerExtension.TraceObserver());
#endif

            // Dev11 searches for the key file in the current directory and assembly output directory.
            // We always look to base directory and then examine the search paths.
            keyFileSearchPaths.Add(baseDirectory);
            if (baseDirectory != outputDirectory)
            {
                keyFileSearchPaths.Add(outputDirectory);
            }

            // Public sign doesn't use the legacy search path settings
            if (publicSign && !string.IsNullOrWhiteSpace(keyFileSetting))
            {
                keyFileSetting = ParseGenericPathToFile(keyFileSetting, diagnostics, baseDirectory);
            }

            if (embedAllSourceFiles)
            {
                embeddedFiles.AddRange(sourceFiles);
            }

            if (embeddedFiles.Count > 0 && !emitPdb)
            {
                diagnostics.Add(
                    Errors.MessageProvider.Instance.CreateDiagnostic(
                        Errors.ErrorCode.ERR_CannotEmbedWithoutPdb,
                        Location.None));
            }

            var parseOptions = new AquilaParseOptions
            (
                documentationMode: DocumentationMode.Diagnose, // always diagnose
                kind: SourceCodeKind.Regular,
                languageVersion: languageVersion,
                shortOpenTags: shortOpenTags,
                features: ImmutableDictionary<string, string>.Empty // features: parsedFeatures
            );

            var scriptParseOptions = parseOptions.WithKind(SourceCodeKind.Script);

            var options = new AquilaCompilationOptions
            (
                outputKind: outputKind,
                baseDirectory: baseDirectory,
                sdkDirectory: sdkDirectoryOpt,
                //targetFramework: targetFramework,
                moduleName: moduleName,
                mainTypeName: mainTypeName,
                scriptClassName: WellKnownMemberNames.DefaultScriptClassName,
                versionString: versionString,
                parseOptions: parseOptions,
                defines: defines.ToImmutableDictionary(),
                diagnostics: diagnostics.AsImmutable(),
                specificDiagnosticOptions: diagnosticOptions,
                //usings: usings,
                optimizationLevel: optimization,
                checkOverflow: false, // checkOverflow,
                //deterministic: deterministic,
                concurrentBuild: concurrentBuild,
                cryptoKeyContainer: keyContainerSetting,
                cryptoKeyFile: keyFileSetting,
                delaySign: delaySignSetting,
                platform: Platform.AnyCpu,
                publicSign: publicSign
            )
            {
                EventSources = evetsources.AsImmutableOrEmpty(),
                Autoload_PSR4 = autoload_psr4,
                Autoload_ClassMapFiles = autoload_classmapfiles,
                Autoload_Files = autoload_files,
            };

            if (debugPlus)
            {
                options = options.WithDebugPlusMode(debugPlus);
            }

            var emitOptions = new EmitOptions
            (
                metadataOnly: false,
                debugInformationFormat: debugInformationFormat,
                pdbFilePath: null, // to be determined later
                pdbChecksumAlgorithm: System.Security.Cryptography.HashAlgorithmName.SHA1,
                outputNameOverride: null, // to be determined later
                //baseAddress: baseAddress,
                //highEntropyVirtualAddressSpace: highEntropyVA,
                //fileAlignment: fileAlignment,
                //subsystemVersion: subsystemVersion,
                runtimeMetadataVersion: runtimeMetadataVersion
            );

            return new AquilaCommandLineArguments()
            {
                // TODO: parsed arguments
                IsScriptRunner = IsScriptCommandLineParser,
                //InteractiveMode = interactiveMode || IsScriptRunner && sourceFiles.Count == 0,
                BaseDirectory = baseDirectory,
                //PathMap = pathMap,
                Errors = ImmutableArray<Diagnostic>.Empty,
                Utf8Output = true,
                CompilationName = compilationName,
                OutputFileName = outputFileName,
                PdbPath = pdbPath,
                EmitPdb = emitPdb,
                SourceLink = sourceLink,
                OutputDirectory = outputDirectory,
                DocumentationPath = documentationPath,
                //ErrorLogPath = errorLogPath,
                //AppConfigPath = appConfigPath,
                SourceFiles = sourceFiles.AsImmutable(),
                MetadataFiles = metadataFiles.ToImmutableArray(),
                ViewFiles = viewFiles.ToImmutableArray(),
                Encoding = codepage, // Encoding.UTF8,
                ChecksumAlgorithm = SourceHashAlgorithm.Sha1, // checksumAlgorithm,
                MetadataReferences = metadataReferences.AsImmutable(),
                AnalyzerConfigPaths = analyzerConfigPaths.AsImmutable(),
                AnalyzerReferences = analyzers.AsImmutable(),
                AdditionalFiles = additionalFiles.AsImmutable(),
                ReferencePaths = referencePaths.AsImmutable(),
                SourcePaths = ImmutableArray<string>.Empty, //sourcePaths.AsImmutable(),
                KeyFileSearchPaths = keyFileSearchPaths.AsImmutable(),
                //Win32ResourceFile = win32ResourceFile,
                //Win32Icon = win32IconFile,
                //Win32Manifest = win32ManifestFile,
                //NoWin32Manifest = noWin32Manifest,
                DisplayLogo = displayLogo,
                DisplayHelp = displayHelp,
                ManifestResources = managedResources.AsImmutable(),
                CompilationOptions = options,
                ParseOptions = IsScriptCommandLineParser ? scriptParseOptions : parseOptions,
                EmitOptions = emitOptions,
                //ScriptArguments = scriptArgs.AsImmutableOrEmpty(),
                //TouchedFilesPath = touchedFilesPath,
                PrintFullPaths = printFullPaths,
                //ShouldIncludeErrorEndLocation = errorEndLocation,
                //PreferredUILang = preferredUILang,
                //SqmSessionGuid = sqmSessionGuid,
                //ReportAnalyzer = reportAnalyzer
                EmbeddedFiles = embeddedFiles.AsImmutable(),
            };
        }

        private void GetCompilationAndModuleNames(
            List<Diagnostic> diagnostics,
            OutputKind outputKind,
            List<CommandLineSourceFile> sourceFiles,
            bool sourceFilesSpecified,
            string moduleAssemblyName,
            ref string outputFileName,
            ref string moduleName,
            out string compilationName)
        {
            // simple name
            string simpleName = null;
            if (outputFileName != null)
            {
                simpleName = PathUtilities.GetFileName(outputFileName, false);
            }
            else if (sourceFiles.Count != 0)
            {
                simpleName = PathUtilities.GetFileName(sourceFiles[0].Path, false);
            }

            // assembly name
            compilationName = simpleName;

            if (moduleName == null)
            {
                moduleName = simpleName;
            }

            // file name
            if (outputFileName == null)
            {
                outputFileName = simpleName + outputKind.GetDefaultExtension();
            }
        }

        private static OutputKind ParseTarget(string value, IList<Diagnostic> diagnostics)
        {
            switch (value.ToLowerInvariant())
            {
                case "exe":
                    return OutputKind.ConsoleApplication;

                case "winexe":
                    return OutputKind.WindowsApplication;

                case "library":
                    return OutputKind.DynamicallyLinkedLibrary;

                case "module":
                    return OutputKind.NetModule;

                case "appcontainerexe":
                    return OutputKind.WindowsRuntimeApplication;

                case "winmdobj":
                    return OutputKind.WindowsRuntimeMetadata;

                default:
                    throw new ArgumentException("value");
            }
        }

        private static void ParseDefine(string value, Dictionary<string, string> defines)
        {
            foreach (var pair in value.Split(new char[] { ',', ';' }, StringSplitOptions.RemoveEmptyEntries))
            {
                // NAME=VALUE
                var eq = pair.IndexOf('=');
                if (eq < 0)
                {
                    defines[pair] = null;
                }
                else
                {
                    defines[pair.Remove(eq)] = pair.Substring(eq + 1);
                }
            }
        }

        internal override void GenerateErrorForNoFilesFoundInRecurse(string path, IList<Diagnostic> errors)
        {
            // nothing
        }
        
        private IEnumerable<CommandLineReference> ParseAssemblyReferences(string arg, string value,
            IList<Diagnostic> diagnostics, bool embedInteropTypes)
        {
            if (string.IsNullOrWhiteSpace(value))
                throw new ArgumentException("value"); // TODO: ErrorCode

            int eqlOrQuote = value.IndexOfAny(new[] { '"', '=' });

            string alias;
            if (eqlOrQuote >= 0 && value[eqlOrQuote] == '=')
            {
                alias = value.Substring(0, eqlOrQuote);
                value = value.Substring(eqlOrQuote + 1);
            }
            else
            {
                alias = null;
            }

            List<string> paths = ParseSeparatedPaths(value).Where((path) => !string.IsNullOrWhiteSpace(path)).ToList();
            if (alias != null)
            {
                throw new NotSupportedException(); // TODO: ErrorCode
            }

            foreach (string path in paths)
            {
                var aliases = (alias != null) ? ImmutableArray.Create(alias) : ImmutableArray<string>.Empty;

                var properties =
                    new MetadataReferenceProperties(MetadataImageKind.Assembly, aliases, embedInteropTypes);
                yield return new CommandLineReference(path, properties);
            }
        }

        internal static ResourceDescription ParseResourceDescription(
            string arg,
            ReadOnlyMemory<char> resourceDescriptor,
            string baseDirectory,
            IList<Diagnostic> diagnostics,
            bool embedded)
        {
            ParseResourceDescription(
                resourceDescriptor,
                baseDirectory,
                false,
                out string filePath,
                out string fullPath,
                out string fileName,
                out string resourceName,
                out string accessibility);

            bool isPublic;
            if (accessibility == null)
            {
                // If no accessibility is given, we default to "public".
                // NOTE: Dev10 distinguishes between null and empty/whitespace-only.
                isPublic = true;
            }
            else if (string.Equals(accessibility, "public", StringComparison.OrdinalIgnoreCase))
            {
                isPublic = true;
            }
            else if (string.Equals(accessibility, "private", StringComparison.OrdinalIgnoreCase))
            {
                isPublic = false;
            }
            else
            {
                return null;
            }

            if (string.IsNullOrEmpty(filePath))
            {
                return null;
            }

            if (fullPath == null || string.IsNullOrWhiteSpace(fileName) ||
                fileName.IndexOfAny(Path.GetInvalidFileNameChars()) >= 0)
            {
                return null;
            }

            Func<Stream> dataProvider = () =>
            {
                // Use FileShare.ReadWrite because the file could be opened by the current process.
                // For example, it is an XML doc file produced by the build.
                return new FileStream(fullPath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
            };
            return new ResourceDescription(resourceName, fileName, dataProvider, isPublic, embedded, checkArgs: false);
        }

        static IObserver<object> CreateObserver(string name, string moduleName)
        {
            var ci = name.IndexOf(',');
            if (ci > 0)
            {
                var tname = name.Remove(ci).Trim();
                var assname = name.Substring(ci + 1).Trim();

                try
                {
                    var ass = System.Reflection.Assembly.Load(assname);
                    if (ass != null)
                    {
                        var t = ass.GetType(tname, throwOnError: false);
                        if (t != null)
                        {
                            return Activator.CreateInstance(t, AquilaCompiler.GetVersion(), moduleName) as
                                IObserver<object>;
                        }
                    }
                }
                catch
                {
                }
            }

            return null;
        }
    }
}