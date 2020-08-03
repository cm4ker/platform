using System;

namespace Aquila.CodeAnalysis
{
    internal static class Constants
    {
        public const string AquilaLanguageName = "AQLang";
        public const string ScriptFileExtension = ".aq";

        /// <summary>
        /// PHP language Guid used by debugger.
        /// </summary>
        /// <remarks>Needs support in editor to show correct language name in call stack.</remarks>
        public static readonly Guid CorSymLanguageTypePeachpie = new Guid("{3D70E75B-578F-4DC3-9902-695135E893A5}");
    }
}
