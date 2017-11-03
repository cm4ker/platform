using System.Collections.Generic;

namespace ZenPlatform.Configuration.Data
{

    /// <summary>
    /// Поле (свойство) объекта конфигурации.
    /// Содержит в себе информацию, приближённую к базе данных. 
    /// 1) Наименование колонки связанных данных с таблицей
    /// 2) Наименование поля в классе DTO (псевдоним/aliase)
    /// </summary>
    public class PProperty
    {
        private readonly PObjectType _owner;

        public PProperty(PObjectType owner)
        {
            _owner = owner;
            // _owner.Propertyes.Add(this);
            Types = new List<PTypeBase>();
        }

        /// <summary>
        /// Наименование столбца в базе данных
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Столбец уникальности
        /// Показывает, можно ли по этому столбцу обнаружить значение
        /// </summary>
        public bool Unique { get; set; }

        /// <summary>
        /// Указывает на то, что используется композитный ключ.
        /// Если не устанавливать это свойство, в таком случае при добавлении объекта 
        /// </summary>
        public bool CompositeUnique { get; set; }

        /// <summary>
        /// Псевдоним, если установить это свойство, то оно перезапишет Name
        /// </summary>
        public string Alias { get; set; }

        /// <summary>
        /// Тип связанных данных
        /// Тип может быть комплексным, поэтому тут присутствует список типов
        /// </summary>
        public List<PTypeBase> Types { get; set; }


        /// <summary>
        /// Владелец свойства.
        /// За время жизни владельца нельзя менять. Если свойство принадлежит какому-то объекту, то оно живёт с ним до момента его уничтожения
        /// </summary>
        public PObjectType Owner => _owner;
    }
}