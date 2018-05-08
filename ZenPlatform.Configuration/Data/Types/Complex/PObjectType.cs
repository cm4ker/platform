using System;
using System.Collections.Generic;

namespace ZenPlatform.Configuration.Data.Types.Complex
{
    public abstract class PObjectType : PTypeBase
    {
        protected PObjectType(string name, Guid id, PComponent component)
        {
            Name = name;
            Id = (id == Guid.Empty) ? Guid.NewGuid() : id;
            Properties = new List<PProperty>();

            OwnerComponent = component;
        }

        /// <summary>
        /// Компонент-владелец
        /// </summary>
        public PComponent OwnerComponent { get; }

        //TODO: Сделать ссылку на используемый компонент для данного объекта. Ссылка будет присваиваться из менеджера компонентов

        /// <summary>
        /// Это абстрактный тип.
        /// Если да, в таком случае от этого класса можно только наследоваться. Экзепляр этого класса создавать нельзя
        /// 
        /// Для того, чтобы наследоваться от класса используйте абстракцию <see cref="PComplexType"/>
        /// </summary>
        public virtual bool IsAbstractType { get; set; }

        /// <inheritdoc />
        public override Guid Id { get; }

        public List<PProperty> Properties { get; }

        /// <inheritdoc />
        public override string Name { get; }

    }
}