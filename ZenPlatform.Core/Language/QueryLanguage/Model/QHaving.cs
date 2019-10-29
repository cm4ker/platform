namespace ZenPlatform.Core.Language.QueryLanguage.Model
{
    public class QHaving
    {
        public QExpression Expression { get; }

        public QHaving(QExpression expression)
        {
            Expression = expression;
        }
    }
}