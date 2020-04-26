using System;
using ZenPlatform.Core;
using ZenPlatform.Core.Annotations;
using ZenPlatform.DocumentComponent;

namespace DefaultNamespace
{
    public partial class InvoiceEntity : DocumentEntity
    {
        InvoiceDto _dto;
        public InvoiceEntity([NotNull] Session session, InvoiceDto dto) : base(session)
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

        public DateTime StartDate
        {
            get
            {
                return _dto.StartDate;
            }

            set
            {
                _dto.StartDate = value;
                OnPropertyChanged();
            }
        }

        public Decimal SomeNumber
        {
            get
            {
                return _dto.SomeNumber;
            }

            set
            {
                _dto.SomeNumber = value;
                OnPropertyChanged();
            }
        }

        public ContractorEntity Contractor
        {
            get
            {
                return Session.Document().Contractor.Load(_dto.Contractor_Ref);
            }

            set
            {
                _dto.Contractor_Ref = Session.Document().Contractor.GetKey(value);
                OnPropertyChanged();
            }
        }

        public void Save()
        {
            Session.Document().Invoice.Save(this);
        }

        public void Load()
        {
            var key = Session.Document().Invoice.GetKey(this);
            var entity = Session.Document().Invoice.Load(key);
            _dto = entity._dto;
        }

        public void Delete()
        {
            Session.Document().Invoice.Delete(this);
        }
    }
}
