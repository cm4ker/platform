using System;

namespace ZenPlatform.ConfigurationDataComponent
{

    /// <summary>
    /// Базовый класс компонента, от которого необходимо наследоваться, чтобы объявить новый компонент
    /// </summary>
    public abstract class DataComponentBase
    {
        public virtual string Name => "Unknown";
        public virtual string Version => this.GetType().Assembly.GetName().Version.ToString();


    }

    public abstract class EntityManagerBase
    {

    }

    public abstract class EntityBase
    {

    }

    public abstract class EntityGeneratorBase
    {

    }
}
