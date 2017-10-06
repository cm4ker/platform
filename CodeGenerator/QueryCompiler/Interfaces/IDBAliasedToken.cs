namespace QueryCompiler
{
    public interface IDBAliasedToken : IToken
    {
        void SetAliase(string alias);

        string Alias { get; }
    }
}