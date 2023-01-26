﻿using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using Aquila.CodeAnalysis;
using Aquila.CodeAnalysis.Symbols;
using Aquila.Core;
using Aquila.Metadata;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Emit;
using Microsoft.CodeAnalysis.Text;

namespace Aquila.Library.Scripting
{
    /// <summary>
    /// Script representing a compiled submission.
    /// </summary>
    [DebuggerDisplay("Script ({AssemblyName.Name})")]
    sealed class Script : AqContext.IScript
    {
        private const string MainModuleName = "main";
        private const string EntryPointMethodName = "main";

        #region Fields & Properties

        /// <summary>
        /// Set of dependency submissions.
        /// Can be empty.
        /// These scripts are expected to be evaluated when running this script.
        /// Cannot be <c>null</c>.
        /// </summary>
        readonly Script[] _previousSubmissions;

        /// <summary>
        /// The entry method of the submissions global code.
        /// </summary>
        readonly Func<AqContext, object> _entryPoint;

        /// <summary>
        /// Submission assembly image.
        /// </summary>
        readonly ImmutableArray<byte> _image;

        /// <summary>
        /// Submission assembly name.
        /// </summary>
        readonly AssemblyName _assemblyName;

        /// <summary>
        /// In case of valid submission, <c>&lt;Script&gt;</c> type representing the submissions global code.
        /// </summary>
        readonly Type _script;

        /// <summary>
        /// References to scripts that precedes this one.
        /// Current script requires these to be evaluated first.
        /// </summary>
        public IReadOnlyList<Script> DependingSubmissions => _previousSubmissions;

        /// <summary>
        /// Gets the assembly content.
        /// </summary>
        public ImmutableArray<byte> Image => _image;

        /// <summary>
        /// Gets the script assembly name.
        /// </summary>
        public AssemblyName AssemblyName => _assemblyName;

        #endregion

        #region Initialization

        private Script(AssemblyName assemblyName, MemoryStream peStream, MemoryStream pdbStream,
            AquilaCompilationFactory builder, IEnumerable<Script> previousSubmissions)
        {
            _assemblyName = assemblyName;

            //
            peStream.Position = 0;
            if (pdbStream != null)
            {
                pdbStream.Position = 0;
            }

            var ass = builder.LoadFromStream(assemblyName, peStream, pdbStream);
            if (ass == null)
            {
                throw new ArgumentException();
            }

            _image = peStream.ToArray().ToImmutableArray();

            var t = ass.GetType(WellKnownAquilaNames.MainModuleName);

            _entryPoint = ctx =>
            {
                ctx.Instance.UpdateAssembly(ass);
                return ((MethodInfo)t.GetMember(WellKnownAquilaNames.MainMethodName).FirstOrDefault()).Invoke(null,
                    new object[] { ctx });
            };

            if (_entryPoint == null)
            {
                throw new InvalidOperationException();
            }

            // // we have to "declare" the script, so its referenced symbols and compiled files are loaded into the context
            // // this comes once after loading the assembly
            // Context.AddScriptReference(ass);

            // find out highest dependent submissions, if any
            _previousSubmissions = CollectDependencies(ass, builder, previousSubmissions);
        }

        private Script(Func<AqContext, object> entryPoint)
        {
            _entryPoint = entryPoint;
            _assemblyName = new AssemblyName();
            _image = ImmutableArray<byte>.Empty;
            _previousSubmissions = Array.Empty<Script>();
        }

        /// <summary>
        /// Collects scripts which declarations were used directly in the compiled assembly.
        /// These scripts are dependencies to the assembly so they must be evaluated first in order to re-use <paramref name="assembly"/> in future.
        /// </summary>
        /// <param name="assembly">Compiled assembly.</param>
        /// <param name="builder">Assembly factory.</param>
        /// <param name="previousSubmissions">All scripts referenced by the assembly compilation.</param>
        /// <returns></returns>
        private static Script[] CollectDependencies(Assembly assembly, AquilaCompilationFactory builder,
            IEnumerable<Script> previousSubmissions)
        {
            // collect dependency scripts from referenced assemblies
            var dependencies = new HashSet<Script>();
            foreach (var refname in
                     assembly
                         .GetReferencedAssemblies()) // TODO: only assemblies really used within the {assembly} -> optimizes caching
            {
                var refass = builder.TryGetSubmissionAssembly(refname);
                if (refass != null)
                {
                    var refscript = previousSubmissions.First(s => s.AssemblyName.Name == refass.GetName().Name);
                    Debug.Assert(refscript != null);
                    if (refscript != null)
                    {
                        dependencies.Add(refscript);
                    }
                }
            }

            // remove dependencies of dependencies -> not needed for checking
            var toremove = new HashSet<Script>();
            foreach (var d in dependencies)
            {
                toremove.UnionWith(d.DependingSubmissions);
            }

            dependencies.ExceptWith(toremove);

            //
            return (dependencies.Count != 0)
                ? dependencies.ToArray()
                : Array.Empty<Script>();
        }

        static string BuildSubmissionFileName(string referrer, string evalname)
        {
            // build a non-existing file path for the submission code (i.e. eval) being invoked from within a source code (referrer)

            // embedded code file and file paths in general do not like some special characters,
            // get rid of them:
            return $"{referrer}.{evalname}.aq"
                    .Replace("<", "")
                    .Replace(">", "")
                    .Replace("`", "")
                    .Replace("~", "")
                ;
        }

        /// <summary>
        /// Compiles <paramref name="code"/> and creates script.
        /// </summary>
        /// <param name="options">Compilation options.</param>
        /// <param name="code">Code to be compiled.</param>
        /// <param name="builder">Assembly builder.</param>
        /// <param name="previousSubmissions">Enumeration of scripts that were evaluated within current context. New submission may reference them.</param>
        /// <returns>New script representing the compiled code.</returns>
        public static Script Create(AqContext.ScriptOptions options, string code, AquilaCompilationFactory builder,
            IEnumerable<Script> previousSubmissions, MetadataProvider metadata)
        {
            // use the language version of the requesting context
            var languageVersion = options.LanguageVersion;
            bool shortOpenTags = false;

            // unique in-memory assembly name
            var name = builder.GetNewSubmissionName();

            var kind = options.IsSubmission ? SourceCodeKind.Script : SourceCodeKind.Regular;

            if (kind == SourceCodeKind.Script && options.EmitDebugInformation)
            {
            }

            // parse the source code
            var tree = AquilaSyntaxTree.ParseText(
                SourceText.From(code, Encoding.UTF8, SourceHashAlgorithm.Sha256),
                new AquilaParseOptions(
                    kind: kind,
                    languageVersion: languageVersion,
                    shortOpenTags: shortOpenTags),
                options.IsSubmission ? BuildSubmissionFileName(options.Location.Path, name.Name) : options.Location.Path
            );
            var viewTree = (AquilaSyntaxTree)AquilaSyntaxTree.ParseText(
                "<div href=\"123\">Hello world\r\n\r\n</div> @code{fn test_view_func(){}}",
                new AquilaParseOptions(kind: SourceCodeKind.View));

            var diagnostics = tree.GetDiagnostics().ToImmutableArray();
            if (!HasErrors(diagnostics))
            {
                // TODO: collect required types from {tree}, remember as a script dependencies
                // TODO: perform class autoload (now before compilation, and then always before invocation)

                // list of scripts that were eval'ed in the context already,
                // our compilation may depend on them
                var dependingSubmissions = previousSubmissions.Where(s => !s.Image.IsDefaultOrEmpty);
                IEnumerable<MetadataReference> metadatareferences
                    = dependingSubmissions.Select(s => MetadataReference.CreateFromImage(s.Image));

                if (options.AdditionalReferences != null)
                {
                    // add additional script references
                    metadatareferences
                        = metadatareferences.Concat(
                            options.AdditionalReferences.Select(r => MetadataReference.CreateFromFile(r)));
                }

                // create the compilation object
                // TODO: add conditionally declared types into the compilation tables
                var compilation = (AquilaCompilation)builder.CoreCompilation
                    .WithAssemblyName(name.Name)
                    .AddMetadata(metadata.EntityMetadata)
                    .AddSyntaxTrees(tree, viewTree)
                    .AddReferences(metadatareferences);

                var emitOptions = new EmitOptions();
                var embeddedTexts = default(IEnumerable<EmbeddedText>);

                if (options.EmitDebugInformation)
                {
                    compilation = compilation.WithAquilaOptions(compilation.Options
                        .WithOptimizationLevel(OptimizationLevel.Debug)
                        .WithDebugPlusMode(true)
                        .WithConcurrentBuild(false)
                    );

                    if (options.IsSubmission)
                    {
                        emitOptions = emitOptions.WithDebugInformationFormat(DebugInformationFormat.PortablePdb);
                        embeddedTexts = new[] { EmbeddedText.FromSource(tree.FilePath, tree.GetText()) };
                    }
                }
                else
                {
                    compilation =
                        compilation.WithAquilaOptions(
                            compilation.Options.WithOptimizationLevel(OptimizationLevel.Release));
                }

                diagnostics = compilation.GetDeclarationDiagnostics();
                if (!HasErrors(diagnostics))
                {
                    var peStream = new MemoryStream();
                    var pdbStream = options.EmitDebugInformation ? new MemoryStream() : null;

                    var result = compilation.Emit(peStream,
                        pdbStream: pdbStream,
                        options: emitOptions,
                        embeddedTexts: embeddedTexts
                    );

                    if (result.Success)
                    {
                        // DEBUG DUMP
                        var fname = @"C:\test\" +
                                    Path.GetFileNameWithoutExtension(tree.FilePath);
                        File.WriteAllBytes(fname + ".dll", peStream.ToArray());
                        File.WriteAllBytes(fname + ".pdb", pdbStream.ToArray());

                        return new Script(name, peStream, pdbStream, builder, previousSubmissions);
                    }
                    else
                    {
                        diagnostics = result.Diagnostics;
                    }
                }
            }

            //
            return CreateInvalid(diagnostics);
        }

        /// <summary>
        /// Initializes an invalid script that throws diagnostics upon invoking.
        /// </summary>
        private static Script CreateInvalid(ImmutableArray<Diagnostic> diagnostics)
        {
            var errors = string.Join(Environment.NewLine,
                diagnostics.Select(d => $"{d.Severity} {d.Id}: {d.GetMessage()}"));

            return new Script(ctx => throw new InvalidOperationException(errors));
        }

        #endregion

        /// <summary>
        /// Checks if given collection contains fatal errors.
        /// </summary>
        private static bool HasErrors(IEnumerable<Diagnostic> diagnostics)
        {
            return diagnostics.Any(d => d.Severity == DiagnosticSeverity.Error);
        }

        #region Context.IScript

        [DebuggerNonUserCode, DebuggerStepThrough]
        public object Evaluate(AqContext ctx)
        {
            return _entryPoint(ctx);
        }


        /// <summary>
        /// Resolves global function handle(s).
        /// </summary>
        public IEnumerable<MethodInfo> GetGlobalRoutineHandle(string name)
        {
            if (_script == null)
            {
                return Enumerable.Empty<MethodInfo>();
            }
            else
            {
                return _script.GetTypeInfo().DeclaredMethods.Where(m => m.IsStatic && m.Name == name);
            }
        }

        #endregion
    }
}