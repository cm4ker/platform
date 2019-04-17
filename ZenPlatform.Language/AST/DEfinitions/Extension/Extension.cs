using System.Collections.Generic;

namespace ZenPlatform.Language.AST.Definitions.Extension
{
    /// <summary>
    /// Отвечает за расширение языка через знак $
    /// </summary>
    public class Extension : Infrastructure.Expression
    {
        public string ExtensionName;

        public string Path;

        public InstructionsBody InstructionsBody { get; set; }
    }


    public enum ExtensionType
    {
        CallOrAssign,
        Instructions
    }
}