using System.Collections.Generic;
using System.Linq;
using ZenPlatform.Configuration.Contracts;


namespace ZenPlatform.Core.Querying.Model
{
    /// <summary>
    /// Выражение в предложении  SELECT 
    /// </summary>
    public partial class QSelectExpression : QField
    {
        public override IEnumerable<IXCType> GetExpressionType()
        {
            return ((QExpression) Childs.First()).GetExpressionType();
        }

        public override string GetName()
        {
            if (Childs.First() is QField ase) return ase.GetName();
            return base.GetName();
        }

        public override string ToString()
        {
            return "Expr";
        }
    }

    /// <summary>
    /// Выражение, основанное на конечном поле данных
    /// </summary>
    public partial class QSourceFieldExpression : QField
    {
        public QObjectTable Object => Childs.First() as QObjectTable;

        public override string GetName()
        {
            return Property.Name;
        }

        public override IEnumerable<IXCType> GetExpressionType()
        {
            return Property.Types;
        }

        public override string ToString()
        {
            return "Field: " + Property.Name;
        }
    }

    public partial class QLookupField : QField
    {
        public IEnumerable<IXProperty> GetProperties()
        {
            return BaseExpression.GetExpressionType().Where(x =>
                    x is IXCObjectType ot && ot.GetProperties().Any(p => p.Name == PropName))
                .Select(x => ((IXCObjectType) x).GetPropertyByName(PropName));
        }

        public override IEnumerable<IXCType> GetExpressionType()
        {
            return BaseExpression.GetExpressionType().Where(x =>
                    x is IXCObjectType ot && ot.GetProperties().Any(p => p.Name == PropName))
                .SelectMany(x => ((IXCObjectType) x).GetPropertyByName(PropName).Types);
        }

        public override string GetName()
        {
            return PropName;
        }
    }
}