using System;
using ZenPlatform.Shared.ParenChildCollection;

namespace ZenPlatform.Configuration.Contracts
{
    public interface IXCDataRuleContent : IChildItem<IXCRole>
    {
        /// <summary>
        /// Идентификатор объекта конфигурации базы данных
        /// </summary>
        Guid ObjectId { get; set; }

        /// <summary>
        /// Содержание xml-представления роли для объекта
        /// Так как роли у компонентов разные и они не имеют ничего общего по структуре - они
        /// просто реализуются внутри компоннета, унаследуя класс XCDataRuleBase
        /// </summary>
        string Content { get; set; }

        string RealContent { get; }

        /// <summary>
        /// Привязанные правила к контенту
        /// </summary>
        IXCDataRule Rule { get; }

        /// <summary>
        /// Объект к которому принадлежит правило
        /// </summary>
        IXCObjectType Object { get; }

        IXCRole Role { get; }
        void Load();
    }
}