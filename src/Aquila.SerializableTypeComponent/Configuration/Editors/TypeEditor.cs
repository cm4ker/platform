using System;
using System.Collections.Generic;
using System.Linq;
using Aquila.Configuration.Common;
using Aquila.Configuration.Common.TypeSystem;
using Aquila.Core.Contracts.Configuration.Store;
using Aquila.Core.Contracts.TypeSystem;

namespace Aquila.SerializableTypeComponent.Configuration.Editors
{
    public class TypeEditor
    {
        private readonly IInfrastructure _inf;
        private readonly ITypeManager _tm;
        private IComponent _com;
        private MDSerializableType _md;
        private readonly List<PropertyEditor> _props;

        public IInfrastructure Infrastructure => _inf;

        public TypeEditor(IInfrastructure inf)
        {
            _inf = inf;
            _tm = inf.TypeManager;

            _md = new MDSerializableType();
            _props = new List<PropertyEditor>();
        }

        public TypeEditor(IInfrastructure inf, MDSerializableType md) : this(inf)
        {
            _md = md;
            _props.AddRange(md.Properties.Select(m => new PropertyEditor(m)));
        }

        public string Name
        {
            get => _md.Name;
            set => _md.Name = value;
        }

        public IEnumerable<PropertyEditor> PropertyEditors => _props;


        public PropertyEditor CreateProperty()
        {
            var mp = new MDSerializableProperty();
            var a = new PropertyEditor(mp);
            _md.Properties.Add(mp);
            _props.Add(a);
            return a;
        }

        public MDType GetRef()
        {
            return MDTypes.Ref(_md.RefId);
        }

        public void Apply(IComponent com)
        {
            _com = com ?? throw new ArgumentNullException(nameof(com));

            Register();

            _tm.AddMD(_md.RefId, _com.Id, _md);
        }

        #region Register in type system

        private void Register()
        {
            var oType = _tm.Type();
            oType.IsObject = true;

            oType.Id = _md.RefId;
            oType.Name = _md.Name;
            oType.IsAsmAvaliable = true;
            oType.IsQueryAvaliable = false;
            oType.IsDbAffect = false;
            oType.GroupId = _md.RefId;

            oType.ComponentId = _com.Info.ComponentId;

            _tm.AddOrUpdateSetting(new ObjectSetting
            {
                ObjectId = oType.Id, SystemId = _inf.Counter.GetId(oType.Id),
                DatabaseName = $"Obj_{_inf.Counter.GetId(oType.Id)}"
            });

            _tm.Register(oType);

            foreach (var prop in _md.Properties)
            {
                var tProp = _tm.Property();
                tProp.Name = prop.Name;
                tProp.Id = prop.Id;
                tProp.ParentId = _md.RefId;

                tProp.SetType(prop.Type.GetTypeId(_tm, prop.IsArray));

                _tm.Register(tProp);
            }
        }

        #endregion
    }
}