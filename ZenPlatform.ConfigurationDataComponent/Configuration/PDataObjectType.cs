using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Text;
using Microsoft.CodeAnalysis.VisualBasic.Syntax;
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

    /// <summary>
    /// Контракт для реализации свойства объекта, обязателен для реализции в компоненте
    /// </summary>
    public abstract class PDataProperty : PProperty
    {
        protected PDataProperty(PObjectType owner) : base(owner)
        {
        }
    }

    public sealed class SupportedTypeManager
    {
        private List<Type> _supportedObjects;

        private IEnumerable<Type> SupportedTypes
        {
            get { return _supportedObjects; }
        }

        public void RegisterType(Type type)
        {
            if (!type.IsSubclassOf(typeof(PDataObjectType))) throw new Exception("Регистрация неправильного типа");

            _supportedObjects.Add(type);
        }
    }
}