namespace ZenPlatform.Core.Language.QueryLanguage.ZqlModel
{
    public class QEquals : QOperationExpression
    {
        protected override int ParamCount => 2;

        public QExpression Left => Arguments[0];
        public QExpression Right => Arguments[1];
    }
}