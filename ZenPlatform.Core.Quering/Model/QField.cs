namespace ZenPlatform.Core.Quering.Model
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