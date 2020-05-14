using System;
using System.Collections.Generic;

namespace Aquila.SerializableTypeComponent.Configuration
{
    /// <summary>
    /// Тип данных, который может сериализоваться (отсутствуют циклические зависимости)
    /// </summary>
    public class MDSerializableType
    {
        public MDSerializableType()
        {
            Properties = new List<MDSerializableProperty>();
            RefId  = Guid.NewGuid();
        }

        public Guid RefId { get; set; }

        public string Name { get; set; }

        public List<MDSerializableProperty> Properties { get; set; }
    }
}