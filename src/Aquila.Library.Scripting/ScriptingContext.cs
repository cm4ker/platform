using System.Collections.Generic;
using Aquila.Core;

namespace Aquila.Library.Scripting
{
    /// <summary>
    /// Data associated with <see cref="Context"/>.
    /// </summary>
    sealed class ScriptingContext
    {
        /// <summary>
        /// Gets data associated with given context.
        /// </summary>
        public static ScriptingContext EnsureContext(AqContext ctx) => new ScriptingContext();

        /// <summary>
        /// Set of submissions already evaluated within the context.
        /// </summary>
        public HashSet<Script> Submissions { get; } = new HashSet<Script>();

        /// <summary>
        /// Index of function created with
        /// </summary>
        public int LastLambdaIndex { get; set; } = 0;
    }
}