using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Reflection;
using Aquila.Metadata;

namespace Aquila.Core
{
    public partial class AqContext
    {
        /// <summary>
        /// Script options.
        /// </summary>
        public sealed class ScriptOptions
        {
            /// <summary>
            /// Script context.
            /// </summary>
            public AqContext Context { get; set; }

            /// <summary>
            /// The path and location within the script source if it originated from a file, empty otherwise.
            /// </summary>
            public Location Location;

            /// <summary>
            /// Specifies whether debugging symbols should be emitted.
            /// </summary>
            public bool EmitDebugInformation { get; set; }

            /// <summary>
            /// Value indicating the script is a submission
            /// </summary>
            public bool IsSubmission { get; set; }

            /// <summary>
            /// Optional. Collection of additional metadata references.
            /// </summary>
            public string[] AdditionalReferences { get; set; }

            /// <summary>
            /// Optional.
            /// Sets the language version.
            /// By default, it uses the version of currently running scripts, or the latest version available.
            /// </summary>
            public Version LanguageVersion { get; set; }
        }

        /// <summary>
        /// Provides dynamic scripts compilation in current context.
        /// </summary>
        public interface IScriptingProvider
        {
            /// <summary>
            /// Gets compiled code.
            /// </summary>
            /// <param name="options">Compilation options.</param>
            /// <param name="code">Script source code.</param>
            /// <returns>Compiled script instance.</returns>
            IScript CreateScript(ScriptOptions options, string code, MetadataProvider metadata);
        }

        /// <summary>
        /// Encapsulates a compiled script that can be evaluated.
        /// </summary>
        public interface IScript
        {
            /// <summary>
            /// Evaluates the script.
            /// </summary>
            /// <param name="ctx">Current runtime context.</param>
            /// <returns>Return value of the script.</returns>
            object Evaluate(AqContext ctx);

            /// <summary>
            /// Resolves global function handle(s).
            /// </summary>
            IEnumerable<MethodInfo> GetGlobalRoutineHandle(string name);

            ImmutableArray<byte> Image { get; }
        }
    }
}