using System.Collections.Generic;
using ZenPlatform.Configuration.Structure.Data.Types.Complex;
using ZenPlatform.Core.Language.QueryLanguage.ZqlModel;

namespace ZenPlatform.Core.Language.QueryLanguage.Model
{
    /// <summary>
    /// Таблица объекта
    /// </summary>
    public class QObjectTable : QItem, IQDataSource, IQAliased
    {
        public QObjectTable(XCObjectTypeBase type)
        {
            ObjectType = type;
        }

        /// <summary>
        /// Ссылка на тип объекта
        /// </summary>
        public XCObjectTypeBase ObjectType { get; }

        /// <summary>
        /// Алиас
        /// </summary>
        public string Alias { get; set; }

        public IEnumerable<QField> GetFields()
        {
            foreach (var prop in ObjectType.GetProperties())
            {
                yield return new QSourceFieldExpression(prop);
            }
        }
    }
}