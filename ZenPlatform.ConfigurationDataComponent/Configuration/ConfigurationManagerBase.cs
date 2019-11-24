using System;
using System.Collections.Generic;
using System.Text;
using ZenPlatform.Configuration.Contracts;
using ZenPlatform.Configuration.Structure.Data;
using ZenPlatform.Configuration.Structure.Data.Types.Complex;

namespace ZenPlatform.DataComponent.Configuration
{
    /// <summary>
    /// Менеджер конфигурации. Обязательный элемент для имплементации к дочернему компоненту.
    /// </summary>
    public abstract class ConfigurationManagerBase : IXComponentManager
    {
        private readonly IXCComponent _component;

        protected ConfigurationManagerBase(IXCComponent component)
        {
            _component = component;
        }

        /// <summary>
        /// Компонент
        /// </summary>
        protected IXCComponent Component => _component;

        /// <inheritdoc />
        public virtual IXCObjectType Create(IXCObjectType parentType = null)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc />
        public virtual void Delete(IXCObjectType type)
        {
            throw new NotImplementedException();
        }
    }
}