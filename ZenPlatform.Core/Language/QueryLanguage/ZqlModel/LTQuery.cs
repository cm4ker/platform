using System.Collections.Generic;

namespace ZenPlatform.Core.Language.QueryLanguage.ZqlModel
{
    /// <summary>
    /// Заппрос
    /// </summary>
    public class LTQuery : LTItem
    {
        public LTQuery()
        {
            Select = new List<LTSelectExpression>();
            From = new List<ILTDataSource>();
        }

        /// <summary>
        /// Список выбранных полей
        /// </summary>
        public List<LTSelectExpression> Select { get; set; }


        /// <summary>
        /// Список выбранных таблиц
        /// </summary>
        public List<ILTDataSource> From { get; set; }

        /// <summary>
        /// Список наложенной фильтрации
        /// </summary>
        public LTExpression Where { get; set; }

        /// <summary>
        /// Список сгруппировнных данных
        /// </summary>
        public List<LTExpression> GroupBy { get; set; }

        /// <summary>
        /// Список наложенной фильтрации на группы
        /// </summary>
        public LTExpression Having { get; set; }

        /// <summary>
        /// Список полей сортировки
        /// </summary>
        public List<LTExpression> OrderBy { get; set; }
    }
}