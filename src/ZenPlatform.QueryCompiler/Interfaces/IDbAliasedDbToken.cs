namespace ZenPlatform.QueryBuilder.Interfaces
{
    public interface IDbAliasedDbToken : IDBToken
    {
        void SetAliase(string alias);

        string Alias { get; }
    }
}