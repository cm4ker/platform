using ZenPlatform.Core.Language.QueryLanguage.ZqlModel;

namespace ZenPlatform.Core.Language.QueryLanguage.Model
{
    public class QEquals : QOperationExpression
    {
        public QEquals(QExpression left, QExpression right)
        {
            Left = left;
            Right = right;
        }

        public QExpression Left { get; }
        public QExpression Right { get; }
    }
    
    
    public class QNotEquals : QOperationExpression
    {
        public QNotEquals(QExpression left, QExpression right)
        {
            Left = left;
            Right = right;
        }

        public QExpression Left { get; }
        public QExpression Right { get; }
    }
}