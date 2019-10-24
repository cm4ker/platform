using ZenPlatform.Configuration.Structure.Data.Types.Complex;

namespace ZenPlatform.Core.Language.QueryLanguage.ZqlModel
{
    /// <summary>
    /// Таблица объекта
    /// </summary>
    public class LTObjectTable : LTItem, ILTDataSource
    {
        public LTObjectTable(XCObjectTypeBase type, string alias)
        {
            ObjectType = type;
            Alias = alias;
        }

        /// <summary>
        /// Ссылка на тип объекта
        /// </summary>
        public XCObjectTypeBase ObjectType { get; }

        /// <summary>
        /// Алиас
        /// </summary>
        public string Alias { get; }
    }
}