using System.Collections.Generic;

namespace ZenPlatform.Core.Language.QueryLanguage.Model
{
    /// <summary>
    /// Вложенный запрос
    /// </summary>
    public class QNastedQuery : QItem, IQDataSource
    {
        public QQuery Nasted;

        public IEnumerable<QField> GetFields()
        {
            foreach (var prop in Nasted.Select)
            {
                yield return prop;
            }
        }
    }
}