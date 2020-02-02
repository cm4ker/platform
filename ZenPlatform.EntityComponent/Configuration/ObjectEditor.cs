using System.Collections.Generic;
using ZenPlatform.Configuration.Common;
using ZenPlatform.Configuration.Contracts;
using ZenPlatform.Configuration.Contracts.Store;
using ZenPlatform.Configuration.Contracts.TypeSystem;

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

        public PropertyEditor CreateProperty()
        {
            var mp = new MDProperty();
            var a = new PropertyEditor(mp);
            _md.Properties.Add(mp);
            _props.Add(a);
            return a;
        }

        internal void Apply(IComponent com)
        {
            _com = com;
            RegisterObject();
            RegisterDto(_com, _inf.Counter, _inf.TypeManager, _md);
            RegisterLink(_com, _inf.Counter, _inf.TypeManager, _md);
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

            oType.ComponentId = _com.Info.ComponentId;

            oType.SystemId = _inf.Counter.GetId(oType.Id);

            _inf.TypeManager.Register(oType);

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

        private void RegisterDto(IComponent component, IUniqueCounter counter, ITypeManager tm, MDEntity md)
        {
            var oType = tm.Type();
            oType.IsDto = true;

            oType.Id = md.DtoId;
            oType.Name = "_" + md.Name;

            oType.ComponentId = component.Info.ComponentId;

            oType.SystemId = counter.GetId(oType.Id);

            tm.Register(oType);
        }

        private void RegisterLink(IComponent component, IUniqueCounter counter, ITypeManager tm, MDEntity md)
        {
            var oType = tm.Type();
            oType.IsLink = true;
            oType.IsQueryAvaliable = true;

            oType.Id = md.LinkId;
            oType.Name = md.Name + "Link";

            oType.ComponentId = component.Info.ComponentId;

            oType.SystemId = counter.GetId(oType.Id);

            tm.Register(oType);
        }
    }
}