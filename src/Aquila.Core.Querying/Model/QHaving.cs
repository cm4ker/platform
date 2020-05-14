namespace Aquila.Core.Querying.Model
{
    public partial class QHaving : QItem
    {
        public QExpression Expression { get; }

        public QHaving(QExpression expression)
        {
            Expression = expression;
        }
    }
}