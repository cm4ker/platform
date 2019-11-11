using System.Collections.Generic;

namespace ZenPlatform.Core.Language.QueryLanguage.Model
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