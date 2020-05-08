using System;
using Aquila.Core;
using Aquila.Core.Annotations;
using Aquila.DocumentComponent;

namespace DefaultNamespace
{
    public partial class ContractorEntity : DocumentEntity
    {
        ContractorDto _dto;
        public ContractorEntity([NotNull] Session session, ContractorDto dto): base (session)
        {
            _dto = dto;
        }

        public Guid Id
        {
            get
            {
                return _dto.Id;
            }

            set
            {
                _dto.Id = value;
                OnPropertyChanged();
            }
        }

        public String Name
        {
            get
            {
                return _dto.Name;
            }

            set
            {
                _dto.Name = value;
                OnPropertyChanged();
            }
        }

        public void Save()
        {
            Session.Document().Contractor.Save(this);
        }

        public void Load()
        {
            var key = Session.Document().Contractor.GetKey(this);
            var entity = Session.Document().Contractor.Load(key);
            _dto = entity._dto;
        }

        public void Delete()
        {
            Session.Document().Contractor.Delete(this);
        }
    }
}
