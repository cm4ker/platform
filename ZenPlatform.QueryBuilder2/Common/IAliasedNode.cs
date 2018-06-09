namespace ZenPlatform.QueryBuilder2
{
    public interface IAliasedNode
    {
        string Alias { get; }

        bool IsAliased { get; }
    }
}