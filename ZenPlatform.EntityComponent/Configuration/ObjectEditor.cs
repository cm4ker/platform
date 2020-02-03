using System;
using System.Collections.Generic;
using ZenPlatform.Configuration.Common;
using ZenPlatform.Configuration.Contracts;
using ZenPlatform.Configuration.Contracts.Store;
using ZenPlatform.Configuration.Contracts.TypeSystem;
using ZenPlatform.Configuration.TypeSystem;

namespace ZenPlatform.EntityComponent.Configuration
{
    public class ObjectEditor
    {
        private readonly IInfrastructure _inf;
        private IComponent _com;
        private MDEntity _md;
        private readonly List<PropertyEditor> _props;

        public ObjectEditor(IInfrastructure inf)
        {
            _inf = inf;

            _md = new MDEntity();
            _props = new List<PropertyEditor>();
        }

        public ObjectEditor(IInfrastructure inf, MDEntity md) : this(inf)
        {
            _md = md;
        }

        public string Name
        {
            get => _md.Name;
            set => _md.Name = value;
        }

        public IEnumerable<PropertyEditor> Editors => _props;

        public PropertyEditor CreateProperty()
        {
            var mp = new MDProperty();
            var a = new PropertyEditor(mp);
            _md.Properties.Add(mp);
            _props.Add(a);
            return a;
        }

        public void Apply(IComponent com)
        {
            _com = com;
            RegisterObject();
            RegisterDto();
            RegisterManager();
            RegisterLink();
        }

        private void RegisterManager()
        {
            var tm = _inf.TypeManager;

            var oType = tm.Type();
            oType.IsManager = true;

            oType.Id = Guid.NewGuid();
            oType.Name = $"{_md.Name}Manager";
            oType.GroupId = _md.ObjectId;

            oType.ComponentId = _com.Info.ComponentId;

            oType.SystemId = _inf.Counter.GetId(oType.Id);

            tm.Register(oType);
        }

        public MDType GetRef()
        {
            return MDTypes.Ref(_md.LinkId);
        }

        private void RegisterObject()
        {
            var oType = _inf.TypeManager.Type();
            oType.IsObject = true;

            oType.Id = _md.ObjectId;
            oType.Name = _md.Name;
            oType.IsCodeAvaliable = true;
            oType.IsQueryAvaliable = true;
            oType.GroupId = _md.ObjectId;

            oType.ComponentId = _com.Info.ComponentId;

            oType.SystemId = _inf.Counter.GetId(oType.Id);

            _inf.TypeManager.Register(oType);

            RegisterId(_md.ObjectId);

            foreach (var prop in _md.Properties)
            {
                var tProp = _inf.TypeManager.Property();
                tProp.Name = prop.Name;
                tProp.Id = prop.Guid;
                tProp.ParentId = _md.ObjectId;

                foreach (var pType in prop.Types)
                {
                    var tPropType = _inf.TypeManager.PropertyType();
                    tPropType.PropertyParentId = _md.ObjectId;
                    tPropType.PropertyId = tProp.Id;
                    tPropType.TypeId = pType.GetTypeId(_inf.TypeManager);
                    _inf.TypeManager.Register(tPropType);
                }

                _inf.TypeManager.Register(tProp);
            }
        }

        private void RegisterDto()
        {
            var tm = _inf.TypeManager;

            var oType = tm.Type();
            oType.IsDto = true;

            oType.Id = _md.DtoId;
            oType.Name = "_" + _md.Name;
            oType.GroupId = _md.ObjectId;

            oType.ComponentId = _com.Info.ComponentId;

            oType.SystemId = _inf.Counter.GetId(oType.Id);

            tm.Register(oType);

            RegisterId(_md.DtoId);

            foreach (var prop in _md.Properties)
            {
                var tProp = _inf.TypeManager.Property();
                tProp.Name = prop.Name;
                tProp.Id = prop.Guid;
                tProp.ParentId = _md.DtoId;

                foreach (var pType in prop.Types)
                {
                    var tPropType = _inf.TypeManager.PropertyType();
                    tPropType.PropertyParentId = _md.DtoId;
                    tPropType.PropertyId = tProp.Id;
                    tPropType.TypeId = pType.GetTypeId(_inf.TypeManager);
                    _inf.TypeManager.Register(tPropType);
                }

                var sysId = _inf.Counter.GetId(tProp.Id);

                _inf.TypeManager.AddOrUpdateSetting(new ObjectSetting
                    {ObjectId = tProp.Id, SystemId = sysId, DatabaseName = $"Fld_{sysId}"});

                _inf.TypeManager.Register(tProp);
            }
        }

        private void RegisterLink()
        {
            var oType = _inf.TypeManager.Type();
            oType.IsLink = true;
            oType.IsQueryAvaliable = true;

            oType.GroupId = _md.ObjectId;

            oType.Id = _md.LinkId;
            oType.Name = _md.Name + "Link";

            oType.ComponentId = _com.Info.ComponentId;

            oType.SystemId = _inf.Counter.GetId(oType.Id);

            _inf.TypeManager.Register(oType);
        }


        void RegisterId(Guid parentId)
        {
            var tProp = _inf.TypeManager.Property();
            tProp.Name = "Id";
            tProp.Id = Guid.Parse("7DB25AF5-1609-4B0E-A99C-60576336167D");
            tProp.ParentId = parentId;

            var tPropType = _inf.TypeManager.PropertyType();
            tPropType.PropertyParentId = parentId;
            tPropType.PropertyId = tProp.Id;
            tPropType.TypeId = _inf.TypeManager.Guid.Id;
            _inf.TypeManager.Register(tPropType);

            var sysId = _inf.Counter.GetId(tProp.Id);

            _inf.TypeManager.AddOrUpdateSetting(new ObjectSetting
                {ObjectId = tProp.Id, SystemId = sysId, DatabaseName = $"Fld_{sysId}"});

            _inf.TypeManager.Register(tProp);
        }
    }
}