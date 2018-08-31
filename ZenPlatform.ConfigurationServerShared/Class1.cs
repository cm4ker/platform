using System;
using System.Collections.Generic;
using MessagePack;

namespace ZenPlatform.ConfigurationServerMessages
{
    [MessagePackObject]
    public class XCTreeRequestMessage
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
    }

    /// <summary>
    /// Сообщение - ответ связанное с деревом конфигурации. Когда мы пытаемся открыть плюсиком список, срабатывает именно оно
    /// </summary>
    [MessagePackObject]
    public class XCTreeResponceMessage
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
        public List<XCItem> Items { get; set; }
    }

    [MessagePackObject]
    public class XCItemCommandsRequestMessage
    {
        /// <summary>
        /// Идентификатор запрашиваемого элемента
        /// </summary>
        [Key(0)]
        public Guid ItemId { get; set; }

        /// <summary>
        /// Идентификатор языка
        /// </summary>
        [Key(1)]
        public Guid LanguageId { get; set; }
    }

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

    /// <summary>
    /// Команда элемента конфигурации
    /// </summary>
    [MessagePackObject()]
    public class XCItemCommand
    {
        /// <summary>
        /// Идентификатор комманды
        /// </summary>
        public Guid CommandId { get; set; }

        /// <summary>
        /// Идентификатор родительской комманды, елси вдруг понадобиться, чтобы список был вложенным
        /// </summary>
        public Guid ParentCommandId { get; set; }

        /// <summary>
        /// Наименование комманды.
        /// </summary>
        public string CommandName { get; set; }

        /// <summary>
        /// Картинка команды
        /// </summary>
        [Key(2)]
        public byte[] CommandImage { get; set; }
    }


    /// <summary>
    /// Данные элемента конфигурации
    /// </summary>
    [MessagePackObject]
    public class XCItem
    {
        /// <summary>
        /// Идентификатор элемента
        /// </summary>
        [Key(0)]
        public Guid ItemId { get; set; }

        /// <summary>
        /// Имя элемента
        /// </summary>
        [Key(1)]
        public string ItemName { get; set; }

        /// <summary>
        /// Картинка элемента
        /// </summary>
        [Key(2)]
        public byte[] ItemImage { get; set; }
    }
}
