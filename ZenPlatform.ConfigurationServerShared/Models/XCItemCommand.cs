using System;
using MessagePack;

namespace ZenPlatform.IdeIntegration.Messages.Models
{
    /// <summary>
    /// Команда элемента конфигурации
    /// </summary>
    [MessagePackObject]
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
}