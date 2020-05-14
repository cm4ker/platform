using System;
using MessagePack;
using Aquila.Configuration;

namespace Aquila.IdeIntegration.Shared.Messages
{
    /// <summary>
    /// Запрос на дочерние элементы элемента конфигурации
    /// </summary>
    [MessagePackObject]
    public class XCTreeRequestMessage : PlatformMessage
    {
        public XCTreeRequestMessage(Guid itemId, Guid requestId, XCNodeKind itemType)
        {
            ItemId = itemId;
            RequestId = requestId;
            ItemType = itemType;
        }


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
        [Key(2)]
        public XCNodeKind ItemType { get; set; }
    }

    /// <summary>
    /// Запрос команд конкретного элемента, происходит при нажатии правой кнопки мыши на элементе
    /// </summary>
    [MessagePackObject]
    public class XCItemCommandRequest : PlatformMessage
    {
        /// <summary>
        /// Идентификатор элемента для которого будут вычисленны команды
        /// </summary>
        public Guid ItemId { get; set; }

        /// <summary>
        /// Идентификатор запроса для ассинхронной модели
        /// </summary>
        public Guid RequestId { get; set; }

        /// <summary>
        /// Тип элемента
        /// </summary>
        [Key(2)]
        public XCNodeKind ItemType { get; set; }
    }
}