using System;
using System.Collections.Generic;
using ZenPlatform.Configuration.Common;

namespace ZenPlatform.EntityComponent.Configuration
{
    public class PropertyEditor
    {
        private MDProperty _mp;

        public PropertyEditor()
        {
            _mp = new MDProperty();
        }

        public string Name
        {
            get => _mp.Name;
            set => _mp.Name = value;
        }

        public bool IsComplextType { get; set; }

        public void SetType(MDType type)
        {
            _mp.Types.Add(type);
        }

        public void SetType(Guid typeId)
        {
            _mp.Types.Add(new TypeRef(typeId));
        }

        public void UnsetType(MDType type)
        {
            _mp.Types.Remove(type);
        }

        public void UnsetType(Guid typeId)
        {
            _mp.Types.RemoveAll(x => x.Guid == typeId);
        }

        public void Apply()
        {
            //TODO: Register propertys
        }
    }
}