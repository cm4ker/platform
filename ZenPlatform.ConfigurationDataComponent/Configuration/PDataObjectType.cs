using System;
using System.Collections.Generic;
using System.Text;
using ZenPlatform.Configuration.Data;
using ZenPlatform.Configuration.Data.Types.Complex;

namespace ZenPlatform.DataComponent.Configuration
{

    /// <summary>
    /// Простой контракт для описания типа объекта в компоненте. 
    /// Обязателен к реализации в компоненте.
    /// 
    /// Класс по своей сути не обязывает владельца описывать какие-либо дополнительные функции
    /// Лишь только то, что необходимо для корректной работы компонента
    /// </summary>
    public abstract class PDataObjectType : PObjectType
    {
        protected PDataObjectType(string name, Guid id, PComponent component) : base(name, id, component)
        {
        }
    }

    public abstract class PDataComplexObjectType : PComplexType
    {
        protected PDataComplexObjectType(string name, Guid guid, PObjectType objectType) : base(name, guid, objectType)
        {
        }
    }
}
