namespace ZenPlatform.Core.Quering.Model
{
    public class QHaving : QItem
    {
        public QExpression Expression { get; }

        public QHaving(QExpression expression)
        {
            Expression = expression;
            Expression.Parent = this;
        }
    }
}