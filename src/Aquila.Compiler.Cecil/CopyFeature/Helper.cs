using Microsoft.Extensions.Logging;
using Mono.Cecil;

namespace Aquila.Compiler.Cecil.CopyFeature
{
    public static class Helper
    {
        public static void Info(this ILogger l, string msg)
        {
        }

        public static void Error(this ILogger l, string msg)
        {
        }

        public static void Log(this ILogger l, string msg)
        {
        }

        public static void Warn(this ILogger l, string msg)
        {
        }

        public static void Verbose(this ILogger l, string msg)
        {
        }

        public static void DuplicateIgnored(this ILogger l, string msg, PropertyDefinition pd)
        {
        }
    }
}