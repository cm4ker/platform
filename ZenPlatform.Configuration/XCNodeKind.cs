using System;
using System.Collections.Generic;
using System.Text;

namespace ZenPlatform.Configuration
{
    /// <summary>
    /// Вид ноды конфигурации, нужен для того, чтобы можно было определять тип по свойсву
    /// </summary>
    public enum XCNodeKind
    {
        /// <summary>
        /// Компонент
        /// </summary>
        Component,

        /// <summary>
        /// Свойство
        /// </summary>
        Property,

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
        

    }
}
