using System;
using MessagePack;

namespace ZenPlatform.IdeIntegration.Messages.Messages
{
    /// <summary>
    /// Новый элемент. Относится либо к корню компонента либо к одному из его дочерних элементов
    /// </summary>
    [MessagePackObject]
    public class XCNewItemMessage : PlatformMessage
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
    }
}