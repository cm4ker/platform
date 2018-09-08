using System;
using ZenPlatform.Core.Sessions;
using ZenPlatform.Core.Annotations;
using ZenPlatform.EntityComponent;

namespace DefaultNamespace
{
    public partial class ТестоваяСущностьEntity : SingleEntity
    {
        private ТестоваяСущностьDto _dto;
        private MultiDataStorage_fld101 _mds_fld101;

        public ТестоваяСущностьEntity([NotNull] UserSession session, ТестоваяСущностьDto dto) : base(session)
        {
            _dto = dto;
            _mds_fld101 = new MultiDataStorage_fld101(_dto);
        }

        public MultiDataStorage_fld101 СоставнойТип => _mds_fld101;

        public Guid fld102_Guid { get; set; }

        public void Save()
        {
            Session.Entity().ТестоваяСущность.Save(this);
        }

        public void Load()
        {
            var key = Session.Entity().ТестоваяСущность.GetKey(this);
            var entity = Session.Entity().ТестоваяСущность.Load(key);
            _dto = entity._dto;
        }

        public void Delete()
        {
            Session.Entity().ТестоваяСущность.Delete(this);
        }
    }
}