namespace ZenPlatform.Core.Language.QueryLanguage.ZqlModel
{
    /// <summary>
    /// Вложенный запрос
    /// </summary>
    public class LTNastedQuery : LTItem, ILTDataSource
    {
        public LTQuery Nasted;
    }
}