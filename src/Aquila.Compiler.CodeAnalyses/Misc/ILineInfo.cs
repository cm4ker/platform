namespace Aquila.Language.Ast.Misc
{
    public interface ILineInfo
    {
        int Line { get; set; }
        int Position { get; set; }
    }
}