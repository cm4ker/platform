using System.Collections.Generic;

namespace ZenPlatform.Core.Quering.Model
{
    /// <summary>
    /// Интерфейс поддержки источника данных
    /// </summary>
    public interface IQDataSource
    {
        IEnumerable<QField> GetFields();
    }


    public interface IQAliased
    {
        string Alias { get; set; }
    }
}