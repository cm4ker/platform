namespace ZenPlatform.Shared.ParenChildCollection
{
    public interface IParentItem<TChild, TParent>
        where TParent : class
        where TChild : class, IChildItem<TParent>
    {
        ChildItemCollection<TParent, TChild> Childs { get; }
    }
}