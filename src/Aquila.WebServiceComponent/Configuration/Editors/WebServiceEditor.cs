using System;
using System.Collections.Generic;
using System.Linq;
using Aquila.Component.Shared.Configuration;
using Aquila.Component.Shared.Configuration.Editors;
using Aquila.Configuration.Common;
using Aquila.Configuration.Common.TypeSystem;
using Aquila.Core.Contracts.Configuration.Store;
using Aquila.Core.Contracts.TypeSystem;

namespace Aquila.WebServiceComponent.Configuration.Editors
{
    public class WebServiceEditor
    {
        private readonly IInfrastructure _inf;
        private readonly ITypeManager _tm;
        private IComponent _com;
        private MDWebService _md;
        private readonly List<ModuleEditor> _modules;

        public IInfrastructure Infrastructure => _inf;

        public WebServiceEditor(IInfrastructure inf)
        {
            _inf = inf;
            _tm = inf.TypeManager;

            _md = new MDWebService();
            _modules = new List<ModuleEditor>();
        }

        public WebServiceEditor(IInfrastructure inf, MDWebService md) : this(inf)
        {
            _md = md;
            _modules.AddRange(md.Modules.Select(m => new ModuleEditor(m)));
        }

        public string Name
        {
            get => _md.Name;
            set => _md.Name = value;
        }

        public IEnumerable<ModuleEditor> ModuleEditors => _modules;

        public ModuleEditor CreateModule()
        {
            var mp = new MDProgramModule();
            var a = new ModuleEditor(mp);
            _md.Modules.Add(mp);
            _modules.Add(a);
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

            _tm.AddMD(oType.Id, oType.ComponentId, _md);
        }

        #endregion
    }
}