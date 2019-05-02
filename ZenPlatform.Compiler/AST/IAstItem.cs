namespace ZenPlatform.Compiler.AST
{
    public interface ILineInfo
    {
        int Line { get; set; }
        int Position { get; set; }
    }

    public class AstNode : ILineInfo
    {
        public int Line { get; set; }
        public int Position { get; set; }
    }
}