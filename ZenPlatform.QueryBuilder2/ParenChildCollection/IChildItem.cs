namespace ZenPlatform.QueryBuilder2.ParenChildCollection
{
    /// <summary>
    /// Defines the contract for an object that has a parent object
    /// </summary>
    /// <typeparam name="TParent">Type of the parent object</typeparam>
    public interface IChildItem<TParent> where TParent : class
    {
        TParent Parent { get; set; }
    }

    public interface IParentItem<TChild, TParent>
        where TParent : class
        where TChild : class, IChildItem<TParent>
    {
        ChildItemCollection<TParent, TChild> Childs { get; }
    }
}