namespace ZenPlatform.Core.Language.QueryLanguage.ZqlModel
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