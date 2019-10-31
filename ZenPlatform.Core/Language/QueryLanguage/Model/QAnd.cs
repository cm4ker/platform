namespace ZenPlatform.Core.Language.QueryLanguage.Model
{
    public class QAnd : QOperationExpression
    {
        public QAnd(QExpression left, QExpression right)
        {
            Left = left;
            Right = right;
        }

        public QExpression Left { get; }
        public QExpression Right { get; }
    }
    
    public class QAdd : QOperationExpression
    {
        public QAdd(QExpression left, QExpression right)
        {
            Left = left;
            Right = right;
        }

        public QExpression Left { get; }
        public QExpression Right { get; }
    }
}