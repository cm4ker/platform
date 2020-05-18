using System;
using Aquila.Configuration.Common;

namespace Aquila.SerializableTypeComponent.Configuration
{
    public class MDSerializableProperty
    {
        public MDSerializableProperty()
        {
            Id = Guid.NewGuid();
        }

        public Guid Id { get; set; }

        public string Name { get; set; }

        public bool IsArray { get; set; }

        public MDType Type { get; set; }
    }
}