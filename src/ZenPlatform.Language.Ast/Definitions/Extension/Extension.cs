using ZenPlatform.Compiler.Contracts.Symbols;

namespace ZenPlatform.Language.Ast.Definitions.Extension
{
    /// <summary>
    /// Отвечает за расширение языка через знак $
    /// </summary>
    public partial class Extension : Expression
    {
        public string ExtensionName;

        public string Path;

        public Block Block { get; set; }

        public ExtensionKind Kind;

        public Extension(ILineInfo lineInfo, string extensionName, ExtensionKind kind = ExtensionKind.CallOrAssign) :
            base(lineInfo)
        {
            ExtensionName = extensionName;
            Kind = kind;
        }

        public override T Accept<T>(AstVisitorBase<T> visitor)
        {
            throw new System.NotImplementedException();
        }
    }


    public enum ExtensionKind
    {
        CallOrAssign,
        Instructions
    }
}