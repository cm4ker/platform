using System;
using MessagePack;

namespace ZenPlatform.ConfigurationServerMessages.Messages
{
    
    /// <summary>
    /// Запрос на доступные команды у элемента конфигурации
    /// </summary>
    [MessagePackObject]
    public class XCItemCommandsRequestMessage : XCMessage
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
}