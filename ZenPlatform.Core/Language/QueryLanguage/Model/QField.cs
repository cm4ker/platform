using ZenPlatform.Core.Language.QueryLanguage.ZqlModel;

namespace ZenPlatform.Core.Language.QueryLanguage.Model
{
    /// <summary>
    /// Поле
    /// </summary>
    public abstract class QField : QExpression
    {
        protected QField(QItem child)
        {
            Child = child;
        }

        public virtual string GetName()
        {
            return "Unknown";
        }
    }
}