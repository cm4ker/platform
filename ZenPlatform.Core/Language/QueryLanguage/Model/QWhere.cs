namespace ZenPlatform.Core.Language.QueryLanguage.Model
{
    public class QWhere
    {
        public QExpression Expression { get; }

        public QWhere(QExpression expression)
        {
            Expression = expression;
        }
    }
}