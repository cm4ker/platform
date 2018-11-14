using System;
using MessagePack;

namespace ZenPlatform.IdeIntegration.Messages.Messages
{
    /// <summary>
    /// Запрос на дочерние элементы элемента конфигурации
    /// </summary>
    [MessagePackObject]
    public class XCTreeRequestMessage : PlatformMessage
    {
        /// <summary>
        /// Идентификатор элемента, для которого запрашиваем поддерево
        /// </summary>
        [Key(0)]
        public Guid ItemId { get; set; }

        /// <summary>
        /// Идентификатор запроса, необходим для ассинхронной модели, чтобы знать на что мы отвечаем
        /// </summary>
        [Key(1)]
        public Guid RequestId { get; set; }

        /// <summary>
        /// Тип элемента
        /// </summary>
        public XCTreeItemType ItemType { get; set; }
    }

    /// <summary>
    /// Запросить корневой элемент
    /// </summary>
    public enum XCTreeItemType
    {
        /// <summary>
        /// Корневой элемент
        /// </summary>
        Root,

        /// <summary>
        /// Данные
        /// </summary>
        Data,
    }
}