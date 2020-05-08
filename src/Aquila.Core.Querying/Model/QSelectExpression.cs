using System.Collections.Generic;
using System.Linq;
using Aquila.Configuration.Contracts;
using Aquila.Configuration.Contracts.TypeSystem;


namespace Aquila.Core.Querying.Model
{
    /// <summary>
    /// Выражение в предложении  SELECT 
    /// </summary>
    public partial class QSelectExpression : QField
    {
        public override IEnumerable<IPType> GetExpressionType()
        {
            return ((QExpression) Children.First()).GetExpressionType();
        }

        public override string GetName()
        {
            if (Children.First() is QField ase) return ase.GetName();
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
        public override string GetName()
        {
            return Property.Name;
        }

        public override IEnumerable<IPType> GetExpressionType()
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
        public IEnumerable<IPProperty> GetProperties()
        {
            return BaseExpression.GetExpressionType().Where(x =>
                    x.IsObject && x.Properties.Any(p => p.Name == PropName))
                .Select(x => x.FindPropertyByName(PropName));
        }

        public override IEnumerable<IPType> GetExpressionType()
        {
            return GetProperties().SelectMany(x => x.Types);
        }

        public override string GetName()
        {
            return PropName;
        }
    }
}