using System;
using MessagePack;
using ZenPlatform.Configuration;

namespace ZenPlatform.IdeIntegration.Messages.Models
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
        /// Тип элемента
        /// </summary>
        [Key(3)]
        public XCNodeKind ItemType { get; set; }

        /// <summary>
        /// Имя элемента
        /// </summary>
        [Key(1)]
        public string ItemName { get; set; }

        /// <summary>
        /// Идентификатор родителя
        /// </summary>
        [Key(4)]
        public Guid ParentId { get; set; }


        /// <summary>
        /// Картинка элемента
        /// </summary>
        [Key(2)]
        public byte[] ItemImage { get; set; }
    }


    /// <summary>
    /// Команда 
    /// </summary>
    [MessagePackObject]
    public class XCCommand
    {
        /// <summary>
        /// Какому элементу принадлежит команда
        /// </summary>
        [Key(0)]
        public Guid ParentId { get; set; }

        /// <summary>
        /// Имя команды
        /// </summary>
        [Key(1)]
        public string CommandName { get; set; }

        /// <summary>
        /// Описание команды
        /// </summary>
        [Key(3)]
        public string CommandDescription { get; set; }

        /// <summary>
        /// Уникальный идентификатор команды
        /// </summary>
        [Key(2)]
        public string CommandUID { get; set; }
    }
}