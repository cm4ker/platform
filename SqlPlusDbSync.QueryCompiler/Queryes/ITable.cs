using System.Collections.Generic;

namespace SqlPlusDbSync.QueryCompiler.Queryes
{
    public interface ITable : IQueryObject
    {
        List<ISelectItem> GetFileds();
        string Alias { get; set; }
    }
}