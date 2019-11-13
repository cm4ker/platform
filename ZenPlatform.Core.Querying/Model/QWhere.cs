namespace ZenPlatform.Core.Querying.Model
{
    public partial class QWhere : QItem
    {
        public QExpression Expression { get; }

        public QWhere(QExpression expression)
        {
            Expression = expression;
            Expression.Parent = this;
        }
    }
}