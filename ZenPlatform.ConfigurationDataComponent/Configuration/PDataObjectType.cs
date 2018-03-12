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
    /// </summary>
    public abstract class PDataObjectType : PObjectType
    {
        protected PDataObjectType(string name, Guid id, PComponent component) : base(name, id, component)
        {
        }
    }
}
