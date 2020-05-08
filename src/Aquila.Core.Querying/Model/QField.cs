using System;

namespace Aquila.Core.Querying.Model
{
    /// <summary>
    /// Поле
    /// </summary>
    public abstract partial class QField : QExpression
    {
        public virtual string GetName()
        {
            return "Unknown";
        }
    }
}