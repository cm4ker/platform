using System;
using System.Collections.Generic;
using Aquila.Configuration.Common;
using System.Linq;
using Aquila.Component.Shared.Configuration;
using Aquila.Component.Shared.Configuration.Editors;
using Aquila.Configuration.Common.TypeSystem;
using Aquila.Core.Contracts.Configuration.Store;
using Aquila.Core.Contracts.TypeSystem;

namespace Aquila.EntityComponent.Configuration.Editors
{
    public class ObjectEditor
    {
        private readonly IInfrastructure _inf;
        private readonly ITypeManager _tm;
        private IComponent _com;
        private MDEntity _md;
        private readonly List<PropertyEditor> _props;
        private readonly List<ModuleEditor> _modules;
        private readonly List<CommandEditor> _commands;
        private readonly List<TableEditor> _tables;
        private readonly List<InterfaceEditor> _interfaces;

        public IInfrastructure Infrastructure => _inf;

        public ObjectEditor(IInfrastructure inf)
        {
            _inf = inf;
            _tm = inf.TypeManager;

            _md = new MDEntity();
            _props = new List<PropertyEditor>();
            _modules = new List<ModuleEditor>();
            _commands = new List<CommandEditor>();
            _tables = new List<TableEditor>();
            _interfaces = new List<InterfaceEditor>();
        }

        public ObjectEditor(IInfrastructure inf, MDEntity md) : this(inf)
        {
            _md = md;

            _modules.AddRange(md.Modules.Select(m => new ModuleEditor(m)));
            _props.AddRange(md.Properties.Select(m => new PropertyEditor(m)));
            _commands.AddRange(md.Commands.Select(m => new CommandEditor(m)));
            _tables.AddRange(md.Tables.Select(m => new TableEditor(m)));
        }

        public string Name
        {
            get => _md.Name;
            set => _md.Name = value;
        }

        public IEnumerable<PropertyEditor> PropertyEditors => _props;
        public IEnumerable<ModuleEditor> ModuleEditors => _modules;
        public IEnumerable<CommandEditor> CommandEditors => _commands;
        public IEnumerable<TableEditor> TableEditors => _tables;
        public IEnumerable<InterfaceEditor> InterfaceEditors => _interfaces;

        public ModuleEditor CreateModule()
        {
            var module = new MDProgramModule();
            var me = new ModuleEditor(module);

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

        public InterfaceEditor CreateInterface()
        {
            var im = new MDInterface();
            var e = new InterfaceEditor(im);

            _md.Interfaces.Add(im);
            _interfaces.Add(e);

            return e;
        }

        public TableEditor CreateTable()
        {
            var mp = new MDTable();
            var a = new TableEditor(mp);
            _md.Tables.Add(mp);
            _tables.Add(a);
            return a;
        }

        public MDType GetRef()
        {
            return MDTypes.Ref(_md.LinkId);
        }

        public void Apply(IComponent com)
        {
            _com = com ?? throw new ArgumentNullException(nameof(com));

            RegisterDto();
            RegisterObject();
            RegisterManager();
            RegisterLink();

            _tm.AddMD(_md.ObjectId, _com.Id, _md);
        }

        #region Register in type system

        private void RegisterManager()
        {
            var tm = _tm;

            var oType = tm.Type();
            oType.IsManager = true;
            oType.IsAsmAvailable = true;

            oType.Id = Guid.NewGuid();
            oType.Name = $"{_md.Name}Manager";
            oType.GroupId = _md.ObjectId;

            oType.ComponentId = _com.Info.ComponentId;

            _tm.AddOrUpdateSetting(new ObjectSetting
            {
                ObjectId = oType.Id, SystemId = _inf.Counter.GetId(oType.Id)
            });


            tm.Register(oType);
        }

        private void RegisterObject()
        {
            var oType = _tm.Type();
            oType.IsObject = true;

            oType.Id = _md.ObjectId;
            oType.Name = _md.Name;
            oType.IsAsmAvailable = true;
            oType.IsQueryAvailable = true;
            oType.IsDbAffect = true;
            oType.GroupId = _md.ObjectId;

            oType.ComponentId = _com.Info.ComponentId;

            _tm.AddOrUpdateSetting(new ObjectSetting
            {
                ObjectId = oType.Id, SystemId = _inf.Counter.GetId(oType.Id),
                DatabaseName = $"Obj_{_inf.Counter.GetId(oType.Id)}"
            });

            _tm.Register(oType);

            RegisterId(_md.ObjectId);
            RegisterName(_md.ObjectId);

            foreach (var prop in _md.Properties)
            {
                var tProp = _tm.Property();
                tProp.Name = prop.Name;
                tProp.Id = prop.Guid;
                tProp.ParentId = _md.ObjectId;

                tProp.SetType(_tm.HandleTypeSet(prop.Types.Select(x => x.GetTypeId(_tm))));

                _tm.Register(tProp);
            }

            foreach (var table in _md.Tables)
            {
                var tTable = _tm.NestedType();
                tTable.Name = table.Name;
                tTable.ParentId = _md.ObjectId;
                tTable.GroupId = table.Guid;
                tTable.Id = Guid.NewGuid();

                _tm.Register(tTable);

                var sysId = _inf.Counter.GetId(tTable.Id);

                _tm.AddOrUpdateSetting(new ObjectSetting
                    {ObjectId = tTable.Id, SystemId = sysId, DatabaseName = $"Tbl_{sysId}"});

                foreach (var prop in table.Properties)
                {
                    var tProp = _tm.Property();
                    tProp.Name = prop.Name;
                    tProp.Id = prop.Guid;
                    tProp.ParentId = tTable.Id;

                    tProp.SetType(_tm.HandleTypeSet(prop.Types.Select(x => x.GetTypeId(_tm))));

                    sysId = _inf.Counter.GetId(tProp.Id);

                    _tm.AddOrUpdateSetting(new ObjectSetting
                        {ObjectId = tProp.Id, SystemId = sysId, DatabaseName = $"Fld_{sysId}"});

                    _tm.Register(tProp);
                }
            }
        }

        private void RegisterDto()
        {
            var tm = _tm;

            var oType = tm.Type();
            oType.IsDto = true;

            oType.Id = _md.DtoId;
            oType.Name = "_" + _md.Name;
            oType.GroupId = _md.ObjectId;
            oType.IsAsmAvailable = true;
            oType.ComponentId = _com.Info.ComponentId;

            _tm.AddOrUpdateSetting(new ObjectSetting
            {
                ObjectId = oType.Id, SystemId = _inf.Counter.GetId(oType.Id)
            });

            tm.Register(oType);

            RegisterId(_md.DtoId);
            RegisterName(_md.DtoId);

            foreach (var prop in _md.Properties)
            {
                var tProp = _tm.Property();
                tProp.Name = prop.Name;
                tProp.Id = prop.Guid;
                tProp.ParentId = _md.DtoId;

                tProp.SetType(_tm.HandleTypeSet(prop.Types.Select(x => x.GetTypeId(_tm))));

                var sysId = _inf.Counter.GetId(tProp.Id);

                _tm.AddOrUpdateSetting(new ObjectSetting
                    {ObjectId = tProp.Id, SystemId = sysId, DatabaseName = $"Fld_{sysId}"});

                _tm.Register(tProp);
            }

            foreach (var table in _md.Tables)
            {
                var tTable = _tm.NestedType();
                tTable.Name = table.Name;
                tTable.ParentId = _md.DtoId;
                tTable.GroupId = table.Guid;
                tTable.Id = Guid.NewGuid();

                _tm.Register(tTable);

                var sysId = _inf.Counter.GetId(tTable.Id);

                _tm.AddOrUpdateSetting(new ObjectSetting
                    {ObjectId = tTable.Id, SystemId = sysId, DatabaseName = $"Tbl_{sysId}"});

                foreach (var prop in table.Properties)
                {
                    var tProp = _tm.Property();
                    tProp.Name = prop.Name;
                    tProp.Id = prop.Guid;
                    tProp.ParentId = tTable.Id;

                    tProp.SetType(_tm.HandleTypeSet(prop.Types.Select(x => x.GetTypeId(_tm))));

                    sysId = _inf.Counter.GetId(tProp.Id);

                    _tm.AddOrUpdateSetting(new ObjectSetting
                        {ObjectId = tProp.Id, SystemId = sysId, DatabaseName = $"Fld_{sysId}"});

                    _tm.Register(tProp);
                }
            }
        }

        private void RegisterLink()
        {
            var oType = _tm.Type();
            oType.IsLink = true;
            oType.IsQueryAvailable = false;
            oType.IsAsmAvailable = true;

            oType.GroupId = _md.ObjectId;

            oType.Id = _md.LinkId;
            oType.Name = _md.Name + "Link";

            oType.ComponentId = _com.Info.ComponentId;

            RegisterId(oType.Id);

            foreach (var prop in _md.Properties)
            {
                var tProp = _tm.Property();
                tProp.Name = prop.Name;
                tProp.Id = prop.Guid;
                tProp.ParentId = oType.Id;
                tProp.IsReadOnly = true;

                tProp.SetType(_tm.HandleTypeSet(prop.Types.Select(x => x.GetTypeId(_tm))));

                _tm.Register(tProp);
            }


            foreach (var table in _md.Tables)
            {
                var tTable = _tm.NestedType();
                tTable.Name = table.Name;
                tTable.ParentId = _md.LinkId;
                tTable.GroupId = table.Guid;
                tTable.Id = Guid.NewGuid();

                _tm.Register(tTable);

                var sysId = _inf.Counter.GetId(tTable.Id);

                _tm.AddOrUpdateSetting(new ObjectSetting
                    {ObjectId = tTable.Id, SystemId = sysId, DatabaseName = $"Tbl_{sysId}"});

                foreach (var prop in table.Properties)
                {
                    var tProp = _tm.Property();
                    tProp.Name = prop.Name;
                    tProp.Id = prop.Guid;
                    tProp.ParentId = tTable.Id;
                    tProp.IsReadOnly = true;

                    tProp.SetType(_tm.HandleTypeSet(prop.Types.Select(x => x.GetTypeId(_tm))));

                    _tm.Register(tProp);
                }
            }

            _tm.AddOrUpdateSetting(new ObjectSetting
            {
                ObjectId = oType.Id, SystemId = _inf.Counter.GetId(oType.Id)
            });

            _tm.Register(oType);
        }

        private void RegisterInterface()
        {
            foreach (var mdUx in _md.Interfaces)
            {
                var ux = _tm.CreateUX();
                ux.Name = mdUx.Name;
                ux.GroupId = mdUx.Guid;

                _tm.Register(ux);
            }
        }

        #endregion

        #region Register custom properties

        void RegisterId(Guid parentId)
        {
            var tProp = _tm.Property();
            tProp.Name = "Id";
            tProp.Id = Guid.Parse("7DB25AF5-1609-4B0E-A99C-60576336167D");
            tProp.ParentId = parentId;
            tProp.IsUnique = true;
            tProp.SetType(_tm.Guid.Id);

            var sysId = _inf.Counter.GetId(tProp.Id);

            _tm.AddOrUpdateSetting(new ObjectSetting
                {ObjectId = tProp.Id, SystemId = sysId, DatabaseName = $"Fld_{sysId}"});

            _tm.Register(tProp);
        }

        void RegisterName(Guid parentId)
        {
            var tProp = _tm.Property();
            tProp.Name = "Name";
            tProp.Id = Guid.Parse("583C34B4-5B80-4BF5-92BF-FCEBEA60BFC4");
            tProp.ParentId = parentId;


            var type = _tm.String.GetSpec();
            type.SetSize(300);
            _tm.Register(type);

            tProp.SetType(type.Id);

            var sysId = _inf.Counter.GetId(tProp.Id);

            _tm.AddOrUpdateSetting(new ObjectSetting
                {ObjectId = tProp.Id, SystemId = sysId, DatabaseName = $"Fld_{sysId}"});

            _tm.Register(tProp);
        }

        #endregion
    }
}