using ZenPlatform.Configuration.Structure.Data.Types.Complex;


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