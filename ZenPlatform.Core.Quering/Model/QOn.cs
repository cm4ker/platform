namespace ZenPlatform.Core.Language.QueryLanguage.Model
{
    /// <summary>
    /// Предложение ON в части FROM при соединении таблиц
    /// </summary>
    public class QOn : QOperationExpression
    {
        public QExpression Expression { get; }

        public QOn(QExpression expression)
        {
            Expression = expression;
        }
    }
}