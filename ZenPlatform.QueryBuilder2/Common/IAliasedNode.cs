namespace ZenPlatform.QueryBuilder2.Common
{
    public interface IAliasedNode
    {
        string Alias { get; }

        bool IsAliased { get; }
    }
}