using System;
using System.Collections.Generic;
using MessagePack;
using ZenPlatform.IdeIntegration.Messages.Models;

namespace ZenPlatform.IdeIntegration.Messages.Messages
{
    /// <summary>
    /// Сообщение - ответ связанное с деревом конфигурации. Когда мы пытаемся открыть плюсиком список, срабатывает именно оно
    /// </summary>
    [MessagePackObject]
    public class XCTreeResponceMessage : PlatformMessage
    {
        public XCTreeResponceMessage()
        {
            Items = new List<XCItem>();
        }

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
        public List<XCItem> Items { get; set; }
    }
}