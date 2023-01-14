using Microsoft.CodeAnalysis;

namespace Aquila.CodeAnalysis.Symbols
{
    public static class WellKnownAquilaNames
    {
        /// <summary>
        /// Name of function representing a script global code.
        /// </summary>
        public const string
            GlobalMethodName =
                "<" + WellKnownMemberNames.EntryPointMethodName + ">";

        /// <summary>
        /// Main module name, root of the modules
        /// </summary>
        public const string MainModuleName = "main";

        /// <summary>
        /// Entry point method name
        /// </summary>
        public const string MainMethodName = "main";
        
        /// <summary>
        /// Name of special script type.
        /// </summary>
        public const string DefaultEntryPointClassName = "<EntryPoint>";

        /// <summary>
        /// Namespace containing all script types.
        /// </summary>
        public const string ScriptsRootNamespace = "<Root>";

        /// <summary>
        /// Name of special nested class containing context bound static fields and constants.
        /// </summary>
        public const string StaticsHolderClassName = "_statics";

        /// <summary>
        /// Format string for a generator state machine method name.
        /// </summary>
        public const string GeneratorStateMachineNameFormatString = "<>sm_{0}";

        /// <summary>
        /// Field with flag whether the class's Dispose() was called already.
        /// </summary>
        public static string SynthesizedDisposedFieldName => "<>b_disposed";
    }
}