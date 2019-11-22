using System.Collections.Generic;

namespace ZenPlatform.Configuration.Contracts.Entity
{
    /// <summary>
    /// Контракт контекста модели запроса
    /// </summary>
    public interface IQueryModelContext
    {
        /// <summary>
        /// Параметры, которые идут с контекстом.
        /// Компонент, которому будет передано управление, сможет также их анализировать
        /// </summary>
        Dictionary<string, object> Parameters { get; set; }
    }
}