namespace ZenPlatform.Core.Language.QueryLanguage.Model
{
    /// <summary>
    /// Вложенный запрос
    /// </summary>
    public class QNastedQuery : QItem, IQDataSource
    {
        public QQuery Nasted;
    }
}