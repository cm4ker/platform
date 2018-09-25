using System;
using System.Collections.Generic;
using System.Text;
using ZenPlatform.Configuration.Structure.Data.Types.Complex;

namespace ZenPlatform.Core.Language.QueryLanguage.ZqlModel
{
    public class Query
    {

    }

    public class NastedQuery
    {
        public Query Nasted;
    }

    public class ObjectTable
    {
        public XCObjectTypeBase ObjectType;
    }

    public abstract class Field
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


    public class Expression
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
