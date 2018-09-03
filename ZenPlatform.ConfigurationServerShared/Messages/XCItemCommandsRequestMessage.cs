using System;
using MessagePack;

namespace ZenPlatform.ConfigurationServerMessages.Messages
{
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
}