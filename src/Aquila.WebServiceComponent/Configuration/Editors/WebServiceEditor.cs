using System;
using System.Collections.Generic;
using System.Linq;
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
        private readonly List<MethodEditor> _methods;

        public IInfrastructure Infrastructure => _inf;

        public WebServiceEditor(IInfrastructure inf)
        {
            _inf = inf;
            _tm = inf.TypeManager;

            _md = new MDWebService();
            _methods = new List<MethodEditor>();
        }

        public WebServiceEditor(IInfrastructure inf, MDWebService md) : this(inf)
        {
            _md = md;
            _methods.AddRange(md.Methods.Select(m => new MethodEditor(m)));
        }

        public string Name
        {
            get => _md.Name;
            set => _md.Name = value;
        }

        public string ModuleText
        {
            get => _md.Module;
            set => _md.Module = value;
        }

        public IEnumerable<MethodEditor> MethodEditors => _methods;


        public MethodEditor CreateMethod()
        {
            var mp = new MDMethod();
            var a = new MethodEditor(mp);
            _md.Methods.Add(mp);
            _methods.Add(a);
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

            foreach (var prop in _md.Methods)
            {
                var tMethod = _tm.Method();
                tMethod.Name = prop.Name;
                tMethod.Id = prop.Id;
                tMethod.ParentId = _md.RefId;

                _tm.Register(tMethod);
            }

            _tm.AddMD(oType.Id, oType.ComponentId, _md);
        }

        #endregion
    }
}