using System;
using MessagePack;

namespace ZenPlatform.ConfigurationServerMessages.Messages
{
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