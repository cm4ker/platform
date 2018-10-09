using System;
using System.Collections.Generic;
using System.Text;
using Antlr4.Runtime.Atn;
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

    /// <summary>
    /// Выражение в предложении  SELECT 
    /// </summary>
    public class LTSelectExpression : LTItem
    {
        /// <summary>
        /// Источник данных.
        /// </summary>
        public LTExpression SourceParent { get; set; }

        /// <summary>
        /// Выражение выборки
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
    public class LTObjectField : LTExpression
    {
        public LTObjectField(XCObjectPropertyBase property)
        {
            Property = property;
        }

        public XCObjectPropertyBase Property { get; set; }
    }

    /// <summary>
    /// Константное значение
    /// </summary>
    public class LTConst : LTExpression
    {
    }

    /// <summary>
    /// Произвольное выражение
    /// </summary>
    public class LTExpression : LTItem
    {
    }

    public class LTOperationExpression : LTExpression
    {
        public LTOperationExpression()
        {
            Arguments = new List<LTExpression>();
        }

        protected List<LTExpression> Arguments { get; }

        protected virtual int ParamCount => throw new NotImplementedException();

        public void PushArgument(LTExpression argument)
        {
            if (ParamCount != 0)
                if (Arguments.Count == ParamCount)
                    throw new Exception("Enough params today");

            Arguments.Add(argument);
        }
    }

    public class LTAnd : LTOperationExpression
    {
        protected override int ParamCount => 0;
    }

    public class LTOr : LTOperationExpression
    {
        protected override int ParamCount => 0;
    }

    public class LTCase : LTOperationExpression
    {
        protected override int ParamCount => 3;
        public LTExpression When;
        public LTExpression Then;
        public LTExpression Else;
    }
}