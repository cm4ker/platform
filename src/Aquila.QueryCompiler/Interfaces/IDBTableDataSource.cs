namespace Aquila.QueryBuilder.Interfaces
{
    public interface IDBTableDataSource : IDBDataSource
    {
        string Name { get; }
    }

    public interface IDBDataSource : IDBAliasedFieldContainer
    {

    }
}