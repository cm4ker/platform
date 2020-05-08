using System;

namespace Aquila.Configuration.Structure
{
    /// <summary>
    /// Декорирует свойство для сериализации / десериализации даже если оно приватное
    /// </summary>
    public class XCDecoratedForSerializationAttribute : Attribute
    {
    }
}