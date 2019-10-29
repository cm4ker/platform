using System.Collections.Generic;
using ZenPlatform.Configuration.Structure.Data.Types;
using ZenPlatform.Configuration.Structure.Data.Types.Complex;
using ZenPlatform.Core.Language.QueryLanguage.ZqlModel;

namespace ZenPlatform.Core.Language.QueryLanguage.Model
{
    /// <summary>
    /// Выражение в предложении  SELECT 
    /// </summary>
    public class QSelectExpression : QField
    {
        public QSelectExpression(QExpression expression) : base(expression)
        {
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
        public QSourceFieldExpression(QObjectTable parent, XCObjectPropertyBase property) : base(parent)
        {
            Property = property;
        }

        public XCObjectPropertyBase Property { get; }

        public override string GetName()
        {
            return Property.Name;
        }

        public override IEnumerable<XCTypeBase> GetRexpressionType()
        {
            return Property.Types;
        }
    }
}