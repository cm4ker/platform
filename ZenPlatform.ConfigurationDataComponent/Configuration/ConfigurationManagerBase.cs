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
        private readonly XCComponent _component;


        protected ConfigurationManagerBase(XCComponent component)
        {
            _component = component;
        }

        /// <summary>
        /// Компонент
        /// </summary>
        protected XCComponent Component => _component;

        /// <inheritdoc />
        public virtual XCObjectTypeBase Create(XCObjectTypeBase parentType = null)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc />
        public virtual void Delete(XCObjectTypeBase type)
        {
            throw new NotImplementedException();
        }
    }
}