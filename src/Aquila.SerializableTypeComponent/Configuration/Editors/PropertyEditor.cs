using System;
using System.Collections.Generic;
using Aquila.Configuration.Common;

namespace Aquila.SerializableTypeComponent.Configuration.Editors
{
    public class PropertyEditor
    {
        private MDSerializableProperty _mp;

        public PropertyEditor(MDSerializableProperty mp)
        {
            _mp = mp;
        }

        public string Name
        {
            get => _mp.Name;
            set => _mp.Name = value;
        }

        public bool IsArray
        {
            get => _mp.IsArray;
            set => _mp.IsArray = value;
        }

        public PropertyEditor SetType(MDType type)
        {
            _mp.Type = type;
            return this;
        }

        public MDType Type => _mp.Type;
    }
}