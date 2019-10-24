namespace ZenPlatform.Core.Language.QueryLanguage.ZqlModel
{
    public class LTEquals : LTOperationExpression
    {
        protected override int ParamCount => 2;

        public LTExpression Left => Arguments[0];
        public LTExpression Right => Arguments[1];
    }
}