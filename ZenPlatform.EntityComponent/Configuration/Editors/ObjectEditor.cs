using System;
using System.Collections.Generic;
using ZenPlatform.Configuration.Common;
using ZenPlatform.Configuration.Contracts.Store;
using ZenPlatform.Configuration.Contracts.TypeSystem;
using ZenPlatform.Configuration.TypeSystem;

namespace ZenPlatform.EntityComponent.Configuration.Editors
{
    public class ObjectEditor
    {
        private readonly IInfrastructure _inf;
        private IComponent _com;
        private MDEntity _md;
        private readonly List<PropertyEditor> _props;
        private readonly List<ModuelEditor> _modules;
        private readonly List<CommandEditor> _commands;
        private readonly List<TableEditor> _tables;

        public IInfrastructure Infrastructure => _inf;

        public ObjectEditor(IInfrastructure inf)
        {
            _inf = inf;

            _md = new MDEntity();
            _props = new List<PropertyEditor>();
            _modules = new List<ModuelEditor>();
            _commands = new List<CommandEditor>();
            _tables = new List<TableEditor>();
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

        public IEnumerable<PropertyEditor> PropertyEditors => _props;
        public IEnumerable<ModuelEditor> ModuleEditors => _modules;
        public IEnumerable<CommandEditor> CommandEditors => _commands;
        public IEnumerable<TableEditor> TableEditors => _tables;

        public ModuelEditor CreateModule()
        {
            var module = new MDProgramModule();
            var me = new ModuelEditor(module);

            _md.Modules.Add(module);
            _modules.Add(me);

            return me;
        }


        public CommandEditor CreateCommand()
        {
            var command = new MDCommand();
            var me = new CommandEditor(command);

            _md.Commands.Add(command);
            _commands.Add(me);

            return me;
        }

        public PropertyEditor CreateProperty()
        {
            var mp = new MDProperty();
            var a = new PropertyEditor(mp);
            _md.Properties.Add(mp);
            _props.Add(a);
            return a;
        }

        public TableEditor CreateTable()
        {
            var mp = new MDTable();
            var a = new TableEditor(mp);
            _md.Tables.Add(mp);
            _tables.Add(a);
            return a;
        }

        public void Apply(IComponent com)
        {
            _com = com ?? throw new ArgumentNullException(nameof(com));

            RegisterDto();
            RegisterObject();
            RegisterManager();
            RegisterLink();

            _inf.TypeManager.AddMD(_md.ObjectId, _com.Id, _md);
        }

        private void RegisterManager()
        {
            var tm = _inf.TypeManager;

            var oType = tm.Type();
            oType.IsManager = true;
            oType.IsAsmAvaliable = true;

            oType.Id = Guid.NewGuid();
            oType.Name = $"{_md.Name}Manager";
            oType.GroupId = _md.ObjectId;

            oType.ComponentId = _com.Info.ComponentId;

            _inf.TypeManager.AddOrUpdateSetting(new ObjectSetting
            {
                ObjectId =  oType.Id, SystemId = _inf.Counter.GetId(oType.Id)
            });


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
            oType.IsAsmAvaliable = true;
            oType.IsQueryAvaliable = true;
            oType.IsDbAffect = true;
            oType.GroupId = _md.ObjectId;

            oType.ComponentId = _com.Info.ComponentId;

            _inf.TypeManager.AddOrUpdateSetting(new ObjectSetting
            {
                ObjectId = oType.Id, SystemId = _inf.Counter.GetId(oType.Id),
                DatabaseName = $"Obj_{_inf.Counter.GetId(oType.Id)}"
            });

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

            foreach (var table in _md.Tables)
            {
                var tTable = _inf.TypeManager.Table();
                tTable.Name = table.Name;
                tTable.ParentId = _md.ObjectId;
                tTable.GroupId = table.Guid;
                tTable.Id = Guid.NewGuid();

                _inf.TypeManager.Register(tTable);

                var sysId = _inf.Counter.GetId(tTable.Id);

                _inf.TypeManager.AddOrUpdateSetting(new ObjectSetting
                    {ObjectId = tTable.Id, SystemId = sysId, DatabaseName = $"Tbl_{sysId}"});

                foreach (var prop in table.Properties)
                {
                    var tProp = _inf.TypeManager.Property();
                    tProp.Name = prop.Name;
                    tProp.Id = prop.Guid;
                    tProp.ParentId = tTable.Id;

                    foreach (var pType in prop.Types)
                    {
                        var tPropType = _inf.TypeManager.PropertyType();
                        tPropType.PropertyParentId = tTable.Id;
                        tPropType.PropertyId = tProp.Id;
                        tPropType.TypeId = pType.GetTypeId(_inf.TypeManager);
                        _inf.TypeManager.Register(tPropType);
                    }

                    sysId = _inf.Counter.GetId(tProp.Id);

                    _inf.TypeManager.AddOrUpdateSetting(new ObjectSetting
                        {ObjectId = tProp.Id, SystemId = sysId, DatabaseName = $"Fld_{sysId}"});

                    _inf.TypeManager.Register(tProp);
                }
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
            oType.IsAsmAvaliable = true;
            oType.ComponentId = _com.Info.ComponentId;

            _inf.TypeManager.AddOrUpdateSetting(new ObjectSetting
            {
                ObjectId =  oType.Id, SystemId = _inf.Counter.GetId(oType.Id)
            });

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

            foreach (var table in _md.Tables)
            {
                var tTable = _inf.TypeManager.Table();
                tTable.Name = table.Name;
                tTable.ParentId = _md.DtoId;
                tTable.GroupId = table.Guid;
                tTable.Id = Guid.NewGuid();

                _inf.TypeManager.Register(tTable);

                var sysId = _inf.Counter.GetId(tTable.Id);

                _inf.TypeManager.AddOrUpdateSetting(new ObjectSetting
                    {ObjectId = tTable.Id, SystemId = sysId, DatabaseName = $"Tbl_{sysId}"});

                foreach (var prop in table.Properties)
                {
                    var tProp = _inf.TypeManager.Property();
                    tProp.Name = prop.Name;
                    tProp.Id = prop.Guid;
                    tProp.ParentId = tTable.Id;

                    foreach (var pType in prop.Types)
                    {
                        var tPropType = _inf.TypeManager.PropertyType();
                        tPropType.PropertyParentId = tTable.Id;
                        tPropType.PropertyId = tProp.Id;
                        tPropType.TypeId = pType.GetTypeId(_inf.TypeManager);
                        _inf.TypeManager.Register(tPropType);
                    }

                    sysId = _inf.Counter.GetId(tProp.Id);

                    _inf.TypeManager.AddOrUpdateSetting(new ObjectSetting
                        {ObjectId = tProp.Id, SystemId = sysId, DatabaseName = $"Fld_{sysId}"});

                    _inf.TypeManager.Register(tProp);
                }
            }
        }

        private void RegisterLink()
        {
            var oType = _inf.TypeManager.Type();
            oType.IsLink = true;
            oType.IsQueryAvaliable = true;
            oType.IsAsmAvaliable = true;

            oType.GroupId = _md.ObjectId;

            oType.Id = _md.LinkId;
            oType.Name = _md.Name + "Link";

            oType.ComponentId = _com.Info.ComponentId;

            _inf.TypeManager.AddOrUpdateSetting(new ObjectSetting
            {
                ObjectId =  oType.Id, SystemId = _inf.Counter.GetId( oType.Id)
            });

            _inf.TypeManager.Register(oType);
        }

        void RegisterId(Guid parentId)
        {
            var tProp = _inf.TypeManager.Property();
            tProp.Name = "Id";
            tProp.Id = Guid.Parse("7DB25AF5-1609-4B0E-A99C-60576336167D");
            tProp.ParentId = parentId;
            tProp.IsUnique = true;

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