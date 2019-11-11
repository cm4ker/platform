using System.Collections.Generic;
using System.Linq;
using ZenPlatform.Configuration.Structure.Data.Types;
using ZenPlatform.Configuration.Structure.Data.Types.Complex;

using ZenPlatform.Language.Ast.Definitions;

namespace ZenPlatform.Core.Language.QueryLanguage.Model
{
    /// <summary>
    /// Выражение в предложении  SELECT 
    /// </summary>
    public class QSelectExpression : QField
    {
        public QSelectExpression(QExpression expression) : base(expression)
        {
            expression.Parent = this;
        }

        public override IEnumerable<XCTypeBase> GetRexpressionType()
        {
            return ((QExpression) Child).GetRexpressionType();
        }

        public override string GetName()
        {
            if (Child is QField ase) return ase.GetName();
            return base.GetName();
        }
    }

    public class QAliasedSelectExpression : QSelectExpression
    {
        public QAliasedSelectExpression(QExpression child, string alias) : base(child)
        {
            Alias = alias;
            child.Parent = this;
        }

        public string Alias { get; }

        public override string GetName()
        {
            return Alias;
        }
    }

    /// <summary>
    /// Выражение, основанное на конечном поле данных
    /// </summary>
    public class QSourceFieldExpression : QField
    {
        public QSourceFieldExpression(QObjectTable child, XCObjectPropertyBase property) : base(child)
        {
            Property = property;
        }

        public XCObjectPropertyBase Property { get; }

        public QObjectTable Object => Child as QObjectTable;

        public override string GetName()
        {
            return Property.Name;
        }

        public override IEnumerable<XCTypeBase> GetRexpressionType()
        {
            return Property.Types;
        }
    }

    public class QLookupField : QField
    {
        public QLookupField(string propName, QExpression child) : base(child)
        {
            BaseExpression = child;
            PropName = propName;
        }

        public QExpression BaseExpression { get; }

        public string PropName { get; }

        public IEnumerable<XCObjectPropertyBase> GetProperties()
        {
            return BaseExpression.GetRexpressionType().Where(x =>
                    x is XCObjectTypeBase ot && ot.GetProperties().Any(p => p.Name == PropName))
                .Select(x => ((XCObjectTypeBase) x).GetPropertyByName(PropName));
        }

        public override IEnumerable<XCTypeBase> GetRexpressionType()
        {
            return BaseExpression.GetRexpressionType().Where(x =>
                    x is XCObjectTypeBase ot && ot.GetProperties().Any(p => p.Name == PropName))
                .SelectMany(x => ((XCObjectTypeBase) x).GetPropertyByName(PropName).Types);
        }

        public override string GetName()
        {
            return PropName;
        }
    }
}