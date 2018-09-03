using System;
using System.Collections.Generic;
using MessagePack;

namespace ZenPlatform.ConfigurationServerMessages.Messages
{
    [MessagePackObject]
    public class XCItemCommandsResponceMessage
    {
        /// <summary>
        /// Идентификатор запроса, необходим для ассинхронной модели, чтобы знать на что мы отвечаем
        /// </summary>
        [Key(0)]
        public Guid RequestId { get; set; }

        /// <summary>
        /// Идентификатор родителя. Если мы будем удалённо, без действия пользователя добавлять элементы, или изменять их
        /// Мы прсото определяем какого родителя мы хотим обновить и отправляем сообщение
        /// </summary>
        [Key(1)]
        public Guid ParentId { get; set; }

        /// <summary>
        /// Элементы
        /// </summary>
        [Key(2)]
        public List<XCItemCommand> Items { get; set; }
    }
}