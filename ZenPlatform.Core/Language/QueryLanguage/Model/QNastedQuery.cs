namespace ZenPlatform.Core.Language.QueryLanguage.ZqlModel
{
    /// <summary>
    /// Вложенный запрос
    /// </summary>
    public class QNastedQuery : QItem, IQDataSource
    {
        public QQuery Nasted;
    }
}