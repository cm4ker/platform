using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Aquila.Core.Utilities
{
    /// <summary>
    /// Extension context methods.
    /// </summary>
    public static class ContextExtensions
    {
        /// <summary>
        /// Gets runtime informational version including suffix if provided.
        /// </summary>
        public static string GetRuntimeInformationalVersion()
        {
            return typeof(AqContext).Assembly.GetCustomAttribute<AssemblyInformationalVersionAttribute>()
                .InformationalVersion;
        }

        /// <summary>
        /// Gets value indicating whether the runtime was built as debug.
        /// </summary>
        public static bool IsDebugRuntime() =>
#if DEBUG
            true
#else
            false
#endif
        ;
    }
}