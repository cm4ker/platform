namespace ZenPlatform.Shared.ParenChildCollection
{
    public interface IParentItem<TChild, TParent>
        where TParent : class
        where TChild : class, IChildItem<TParent>
    {
        IReadOnlyChildItemCollection<TParent, TChild> Children { get; }
    }
}