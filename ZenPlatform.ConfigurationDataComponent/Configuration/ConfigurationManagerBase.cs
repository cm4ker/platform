using System;
using System.Collections.Generic;
using System.Text;
using ZenPlatform.Configuration.Contracts;
using ZenPlatform.Configuration.Contracts.Editors;
using ZenPlatform.Configuration.Contracts.TypeSystem;
using ZenPlatform.Configuration.Structure.Data;
using ZenPlatform.Configuration.Structure.Data.Types.Complex;

namespace ZenPlatform.DataComponent.Configuration
{
    /// <summary>
    /// Менеджер конфигурации. Обязательный элемент для имплементации к дочернему компоненту.
    /// </summary>
    public abstract class ConfigurationManagerBase : IXComponentManager
    {
        private readonly IComponent _component;

        protected ConfigurationManagerBase(IComponent component)
        {
            _component = component;
        }

        /// <summary>
        /// Компонент
        /// </summary>
        protected IComponent Component => _component;

        /// <inheritdoc />
        public virtual ITypeEditor Create()
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