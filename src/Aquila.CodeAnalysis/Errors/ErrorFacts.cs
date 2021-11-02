using System;
using System.Diagnostics;
using System.Globalization;
using System.Reflection;
using System.Resources;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;

namespace Aquila.CodeAnalysis.Errors
{
    /// <summary>
    /// Provides detailed information about compilation errors identified by <see cref="ErrorCode"/>.
    /// </summary>
    internal static class ErrorFacts
    {
        public static DiagnosticSeverity GetSeverity(ErrorCode code)
        {
            var name = code.ToString();
            if (name.Length < 4)
            {
                throw new ArgumentException(nameof(code));
            }

            var prefix = name.Substring(0, 4);
            switch (prefix)
            {
                case "FTL_":
                case "ERR_":
                    return DiagnosticSeverity.Error;
                case "WRN_":
                    return DiagnosticSeverity.Warning;
                case "INF_":
                    return DiagnosticSeverity.Info;
                case "HDN_":
                    return DiagnosticSeverity.Hidden;
                default:
                    throw new ArgumentException(nameof(code));
            }
        }

        /// <remarks>Don't call this during a parse--it loads resources</remarks>
        public static string GetMessage(MessageID code, CultureInfo culture)
        {
            string message = ResourceManager.GetString(code.ToString(), culture);
            Debug.Assert(!string.IsNullOrEmpty(message), code.ToString());
            return message;
        }

        /// <remarks>Don't call this during a parse--it loads resources</remarks>
        public static string GetMessage(ErrorCode code, CultureInfo culture)
        {
            string message = ResourceManager.GetString(code.ToString(), culture);
            Debug.Assert(!string.IsNullOrEmpty(message), code.ToString());
            return message;
        }

        private static System.Resources.ResourceManager s_resourceManager;

        private static System.Resources.ResourceManager ResourceManager
        {
            get
            {
                if (s_resourceManager == null)
                {
                    s_resourceManager = new System.Resources.ResourceManager(typeof(AquilaResources).FullName,
                        typeof(ErrorCode).GetTypeInfo().Assembly);
                }

                return s_resourceManager;
            }
        }

        public static string GetFormatString(ErrorCode code)
        {
            return ErrorStrings.ResourceManager.GetString(code.ToString());
        }

        public static string GetFormatString(ErrorCode code, CultureInfo language)
        {
            return ErrorStrings.ResourceManager.GetString(code.ToString(), language);
        }
    }
}