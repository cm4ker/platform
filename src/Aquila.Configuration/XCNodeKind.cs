using System;
using System.Collections.Generic;
using System.Text;

namespace Aquila.Configuration
{
    /// <summary>
    /// Вид ноды конфигурации, нужен для того, чтобы можно было определять тип по свойсву
    /// </summary>
    public enum XCNodeKind
    {
        /// <summary>
        /// Начало конфигурации
        /// </summary>
        Nil,
    
        /// <summary>
        /// Компонент
        /// </summary>
        Component,

        /// <summary>
        /// Свойство
        /// </summary>
        Property,

        /// <summary>
        /// Корневой элемент свойств
        /// </summary>
        PropertyRoot,
        
        
        /// <summary>
        /// Тип
        /// </summary>
        Type,

        /// <summary>
        /// Корень конфигурации
        /// </summary>
        Root,

        /// <summary>
        /// Раздел данных 
        /// </summary>
        Data,
        
        /// <summary>
        /// Раздел ролей
        /// </summary>
        Roles,
        
        /// <summary>
        /// Раздел модулей
        /// </summary>
        Modules,
        
        /// <summary>
        /// Раздел интерфейсов (UI)
        /// </summary>
        Interface,
        
        /// <summary>
        /// Раздел языков
        /// </summary>
        Languages,
        
        /// <summary>
        /// Присоединённый компонент
        /// </summary>
        AttachedComponent
    }
}