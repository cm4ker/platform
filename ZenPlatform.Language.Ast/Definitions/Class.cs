using ZenPlatform.Compiler.Contracts.Symbols;

namespace ZenPlatform.Language.Ast.Definitions
{
    /// <summary>
    /// Олицетворяет класс в платформе
    /// </summary>
    public partial class Class : TypeEntity
    {
        public bool ImplementsReference { get; set; }
    }
}