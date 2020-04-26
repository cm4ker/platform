using System;
using System.Collections.Generic;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using ZenPlatform.Configuration.Common;
using ZenPlatform.Configuration.Contracts.Store;
using ZenPlatform.Configuration.Contracts.TypeSystem;

namespace ZenPlatform.EntityComponent.Configuration
{
    public class PropertyEditor
    {
        private MDProperty _mp;

        public PropertyEditor(MDProperty mp)
        {
            _mp = mp;
        }

        public string Name
        {
            get => _mp.Name;
            set => _mp.Name = value;
        }

        public bool IsComplextType { get; set; }

        public PropertyEditor SetType(MDType type)
        {
            _mp.Types.Add(type);
            return this;
        }

        public IEnumerable<MDType> Types => _mp.Types;

        public void UnsetType(MDType type)
        {
            _mp.Types.Remove(type);
        }

        public void UnsetType(Guid typeId)
        {
            _mp.Types.RemoveAll(x => x.Guid == typeId);
        }


    }
    
    public class TableEditor
    {
        private MDTable _mt;
        private List<PropertyEditor> _props;
        
        public TableEditor(MDTable mt)
        {
            _mt = mt;
            _props = new List<PropertyEditor>();
        }

        public string Name
        {
            get => _mt.Name;
            set => _mt.Name = value;
        }
        
        public PropertyEditor CreateProperty()
        {
            var mp = new MDProperty();
            var a = new PropertyEditor(mp);
            _mt.Properties.Add(mp);
            _props.Add(a);
            return a;
        }
    }
}