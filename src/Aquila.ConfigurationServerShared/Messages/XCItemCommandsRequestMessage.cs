using System;
using MessagePack;

namespace Aquila.IdeIntegration.Shared.Messages
{
    
    /// <summary>
    /// Запрос на доступные команды у элемента конфигурации
    /// </summary>
    [MessagePackObject]
    public class XCItemCommandsRequestMessage : PlatformMessage
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