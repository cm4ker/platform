using System.Collections.Generic;
using ZenPlatform.Core.Language.QueryLanguage.ZqlModel;

namespace ZenPlatform.Core.Language.QueryLanguage.Model
{
    /// <summary>
    /// Заппрос
    /// </summary>
    public class QQuery : LTItem
    {
        public QQuery()
        {
            Select = new List<QSelectExpression>();
            From = new List<IQDataSource>();
        }

        /// <summary>
        /// Список выбранных полей
        /// </summary>
        public List<QSelectExpression> Select { get; set; }


        /// <summary>
        /// Список выбранных таблиц
        /// </summary>
        public List<IQDataSource> From { get; set; }

        /// <summary>
        /// Список наложенной фильтрации
        /// </summary>
        public QExpression Where { get; set; }

        /// <summary>
        /// Список сгруппировнных данных
        /// </summary>
        public List<QExpression> GroupBy { get; set; }

        /// <summary>
        /// Список наложенной фильтрации на группы
        /// </summary>
        public QExpression Having { get; set; }

        /// <summary>
        /// Список полей сортировки
        /// </summary>
        public List<QExpression> OrderBy { get; set; }
    }
}