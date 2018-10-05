using System;
using System.Collections.Generic;
using System.Text;
using ZenPlatform.Configuration.Structure.Data.Types.Complex;

namespace ZenPlatform.Core.Language.QueryLanguage.ZqlModel
{
    /// <summary>
    /// Элемент логических связей в запросе. LT - Logical tree
    /// </summary>
    public abstract class LTItem
    {
        public string Token;
    }

    /// <summary>
    /// Интерфейс поддержки источника данных
    /// </summary>
    public interface ILTDataSource
    {
    }

    /// <summary>
    /// Заппрос
    /// </summary>
    public class LTQuery : LTItem
    {
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
        public LTExpression GroupBy { get; set; }

        /// <summary>
        /// Список наложенной фильтрации на группы
        /// </summary>
        public LTExpression Having { get; set; }

        /// <summary>
        /// Список отсортированных 
        /// </summary>
        public List<LTExpression> OrderBy { get; set; }
    }


    /// <summary>
    /// Вложенный запрос
    /// </summary>
    public class LTNastedQuery : LTItem, ILTDataSource
    {
        public LTQuery Nasted;
    }

    /// <summary>
    /// Таблица объекта
    /// </summary>
    public class LTObjectTable : LTItem, ILTDataSource
    {
        public XCObjectTypeBase ObjectType;
    }

    /// <summary>
    /// Выражение в предложении  SELECT 
    /// </summary>
    public class LTSelectExpression : LTExpression
    {
        /// <summary>
        /// Источник данных.
        /// </summary>
        public LTExpression SourceParent { get; set; }

        /// <summary>
        /// Выражение
        /// </summary>
        public LTExpression Expression { get; set; }

        /// <summary>
        /// Алиас выражения
        /// </summary>
        public string Aliase { get; set; }
    }

    /// <summary>
    /// Поле
    /// </summary>
    public abstract class LTField : LTExpression
    {
        /// <summary>
        /// Источник данных узла
        /// </summary>
        public LTExpression SourceParent { get; set; }
    }


    /// <summary>
    /// Поле объекта имеет конкретную привязку к конкретному объекту
    /// </summary>
    public class LTObjectField : LTField
    {
        public XCObjectPropertyBase Property { get; set; }
    }

    public class ExpressionField : LTField
    {
    }

    public class ConstField : LTField
    {
    }

    public class LTAliase
    {
    }

    public class LTExpression : LTItem
    {
    }

    public class LogicalExpression : LTExpression
    {
        public LTExpression FirstOperand;
        public LTExpression SecondOperand;
    }

    public class And : LogicalExpression
    {
    }

    public class Or : LogicalExpression
    {
    }

    public class CaseExpression : LTExpression
    {
        public LTExpression When;
        public LTExpression Then;
        public LTExpression Else;
    }
}