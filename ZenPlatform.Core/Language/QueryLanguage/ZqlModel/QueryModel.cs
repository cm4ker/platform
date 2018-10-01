using System;
using System.Collections.Generic;
using System.Text;
using ZenPlatform.Configuration.Structure.Data.Types.Complex;

namespace ZenPlatform.Core.Language.QueryLanguage.ZqlModel
{

    public abstract class LogicalTreeQueryItem
    {
        public string Token;
    }

    public class QueryLTree : LogicalTreeQueryItem
    {
        public List<Expression> Select { get; set; }
        public List<IDataSource> From { get; set; }
        public Expression Where { get; set; }
        public Expression GroupBy { get; set; }
        public Expression Having { get; set; }
        public List<Expression> OrderBy { get; set; }
    }

    public interface IDataSource { }

    public class NastedQuery : LogicalTreeQueryItem, IDataSource
    {
        public QueryLTree Nasted;
    }

    public class ObjectTable : LogicalTreeQueryItem, IDataSource
    {
        public XCObjectTypeBase ObjectType;
    }

    public abstract class Field : Expression
    {

    }

    public class ObjectField : Field
    {
        public XCObjectPropertyBase Property;
    }

    public class ExpressionField : Field
    {

    }

    public class ConstField : Field
    {

    }

    public class Aliase
    {

    }

    public class Expression : LogicalTreeQueryItem
    {

    }

    public class LogicalExpression : Expression
    {
        public Expression FirstOperand;
        public Expression SecondOperand;
    }

    public class And : LogicalExpression
    { }

    public class Or : LogicalExpression
    { }

    public class CaseExpression : Expression
    {
        public Expression When;
        public Expression Then;
        public Expression Else;
    }
}
