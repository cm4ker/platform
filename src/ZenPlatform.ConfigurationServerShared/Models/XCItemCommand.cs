using System;
using MessagePack;

namespace ZenPlatform.IdeIntegration.Shared.Models
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
        [Key(0)]
        public Guid CommandId { get; set; }

        /// <summary>
        /// Идентификатор родительской комманды, елси вдруг понадобиться, чтобы список был вложенным
        /// </summary>
        [Key(1)]
        public Guid ParentCommandId { get; set; }

        /// <summary>
        /// Наименование комманды.
        /// </summary>
        [Key(3)]
        public string CommandName { get; set; }

        /// <summary>
        /// Картинка команды
        /// </summary>
        [Key(2)]
        public byte[] CommandImage { get; set; }
    }
}