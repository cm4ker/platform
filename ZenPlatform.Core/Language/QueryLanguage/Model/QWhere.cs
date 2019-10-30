namespace ZenPlatform.Core.Language.QueryLanguage.Model
{
    public class QWhere : QItem
    {
        public QExpression Expression { get; }

        public QWhere(QExpression expression)
        {
            Expression = expression;
            Expression.Parent = this;
        }
    }
}