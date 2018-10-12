using System;
using System.Collections.Generic;
using System.Text;
using Antlr4.Runtime.Atn;
using ZenPlatform.Configuration.Structure.Data.Types;
using ZenPlatform.Configuration.Structure.Data.Types.Complex;
using ZenPlatform.Configuration.Structure.Data.Types.Primitive;

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
    public class LTSelectExpression : LTField
    {
        public LTSelectExpression(LTQuery query)
        {
            Query = query;
        }

        /// <summary>
        /// Запрос-владелец
        /// </summary>
        public LTQuery Query { get; }

        /// <summary>
        /// Алиас выражения
        /// </summary>
        public string Aliase { get; set; }

        public override IEnumerable<XCTypeBase> GetRexpressionType()
        {
            return Child.GetRexpressionType();
        }
    }

    /// <summary>
    /// Поле
    /// </summary>
    public abstract class LTField : LTExpression
    {
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

        public override IEnumerable<XCTypeBase> GetRexpressionType()
        {
            return Property.Types;
        }
    }

    /// <summary>
    /// Константное значение
    /// </summary>
    public class LTConst : LTExpression
    {
        private readonly XCTypeBase _baseType;

        public LTConst(XCTypeBase baseType)
        {
            _baseType = baseType;
        }

        public override IEnumerable<XCTypeBase> GetRexpressionType()
        {
            yield return _baseType;
        }
    }

    /// <summary>
    /// Кейс
    /// </summary>
    public class LTCase : LTOperationExpression
    {
        protected override int ParamCount => 3;

        public LTExpression When => Arguments[0];
        public LTExpression Then => Arguments[1];
        public LTExpression Else => Arguments[2];

        public override IEnumerable<XCTypeBase> GetRexpressionType()
        {
            foreach (var typeBase in Then.GetRexpressionType())
            {
                yield return typeBase;
            }

            foreach (var typeBase in Else.GetRexpressionType())
            {
                yield return typeBase;
            }
        }
    }

    /// <summary>
    /// Сумма
    /// </summary>
    public class LCSum : LTExpression
    {
        private readonly XCTypeBase _baseType;

        public LCSum(XCTypeBase baseType)
        {
            _baseType = baseType;
        }

        public override IEnumerable<XCTypeBase> GetRexpressionType()
        {
            yield return _baseType;
        }
    }

    /// <summary>
    /// Произвольное выражение
    /// </summary>
    public class LTExpression : LTItem
    {
        /// <summary>
        /// Источник данных узла
        /// </summary>
        public LTExpression Parent { get; set; }

        /// <summary>
        /// Выражение выборки
        /// </summary>
        public LTExpression Child { get; set; }

        public virtual IEnumerable<XCTypeBase> GetRexpressionType()
        {
            yield break;
        }
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

        public override IEnumerable<XCTypeBase> GetRexpressionType()
        {
            yield return new XCBoolean();
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

    public class LTEquals : LTOperationExpression
    {
        protected override int ParamCount => 2;

        public LTExpression Left => Arguments[0];
        public LTExpression Right => Arguments[1];
    }
}