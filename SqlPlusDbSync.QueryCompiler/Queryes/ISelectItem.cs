namespace SqlPlusDbSync.QueryCompiler.Queryes
{
    public interface ISelectItem : ISingleResultObject
    {
        string Alias { get; set; }
        ITable Owner { get; }
    }
}