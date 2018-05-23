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
        public virtual bool IsAbstract { get; set; }

        /// <summary>
        /// Это тип от которого нельзя наследоваться
        /// </summary>
        public virtual bool IsSealed { get; set; }

        /// <summary>
        /// Ссылка на родителя, нельзя установить, в случае, если IsSealed = true
        /// В этом лучае Parent будет всегда null
        /// </summary>
        public PObjectType Parent { get; set; }

        /// <inheritdoc />
        public override Guid Id { get; }


        /// <summary>
        /// Список свойств объекта
        /// </summary>
        public virtual List<PProperty> Properties { get; }

        /// <inheritdoc />
        public override string Name { get; }

    }
}