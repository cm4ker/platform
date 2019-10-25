namespace ZenPlatform.Core.Language.QueryLanguage.Model
{
    /// <summary>
    /// Интерфейс поддержки источника данных
    /// </summary>
    public interface IQDataSource
    {
    }


    public interface IQAliased
    {
        string Alias { get; set; }
    }
}