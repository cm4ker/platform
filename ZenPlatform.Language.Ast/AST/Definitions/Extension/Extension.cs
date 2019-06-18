using ZenPlatform.Compiler.Contracts.Symbols;
using ZenPlatform.Language.Ast.AST.Infrastructure;

namespace ZenPlatform.Language.Ast.AST.Definitions.Extension
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

        public override void Accept(IVisitor visitor)
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