using ZenPlatform.Compiler.AST.Infrastructure;

namespace ZenPlatform.Compiler.AST.Definitions.Extension
{
    /// <summary>
    /// Отвечает за расширение языка через знак $
    /// </summary>
    public class Extension : Expression
    {
        public string ExtensionName;

        public string Path;

        public InstructionsBodyNode InstructionsBody { get; set; }

        public ExtensionKind Kind;

        public Extension(ILineInfo lineInfo, string extensionName, ExtensionKind kind = ExtensionKind.CallOrAssign) :
            base(lineInfo)
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