using System.Collections.Generic;

namespace ZenPlatform.Compiler.AST.Definitions.Extension
{
    /// <summary>
    /// Отвечает за расширение языка через знак $
    /// </summary>
    public class Extension : Infrastructure.Expression
    {
        public string ExtensionName;

        public string Path;

        public InstructionsBody InstructionsBody { get; set; }

        public ExtensionKind Kind;

        public Extension(string extensionName, ExtensionKind kind = ExtensionKind.CallOrAssign)
        {
            ExtensionName = extensionName;
            Kind = kind;
        }
    }


    public enum ExtensionKind
    {
        CallOrAssign,
        Instructions
    }
}