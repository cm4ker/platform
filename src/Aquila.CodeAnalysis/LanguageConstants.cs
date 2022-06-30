using System;

namespace Aquila.CodeAnalysis
{
    public static class LanguageConstants
    {
        public const string LanguageId = "aqlang";
        public const string ScriptFileExtension = ".aq";
        public const string ViewFileExtension = ".aqview";
        public const string MetadataFileExtension = ".aqmd";

        /// <summary>
        /// Aquila language Guid used by debugger.
        /// </summary>
        /// <remarks>Needs support in editor to show correct language name in call stack.</remarks>
        public static readonly Guid CorSymLanguageTypeAquila = new Guid("F8281D41-1E74-489A-B603-AB9D85FAE9BA");
    }
}