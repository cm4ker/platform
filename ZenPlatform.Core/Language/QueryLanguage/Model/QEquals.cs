using ZenPlatform.Core.Language.QueryLanguage.ZqlModel;

namespace ZenPlatform.Core.Language.QueryLanguage.Model
{
    public class QEquals : QOperationExpression
    {
        protected override int ParamCount => 2;

        public QExpression Left => Arguments[0];
        public QExpression Right => Arguments[1];
    }
}