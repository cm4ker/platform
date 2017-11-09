//public class InvoiceEntity : DocumentEntityBase
//{
//    InvoiceDto _dto;
//    public InvoiceEntity([NotNull] Session session, InvoiceDto dto) : base(session)
//    {
//        _dto = dto;
//    }

//    public DateTime StartDate
//    {
//        get
//        {
//            return _dto.StartDate;
//        }

//        set
//        {
//            _dto.StartDate = value;
//            OnPropertyChanged();
//        }
//    }

//    public Decimal SomeNumber
//    {
//        get
//        {
//            return _dto.SomeNumber;
//        }

//        set
//        {
//            _dto.SomeNumber = value;
//            OnPropertyChanged();
//        }
//    }

//    public Contractor Contractor
//    {
//        get;
//        set;
//    }

//    public void Save()
//    {
//        throw new NotImplementedException();
//    }

//    public void Load()
//    {
//        throw new NotImplementedException();
//    }

//    public void Delete()
//    {
//        throw new NotImplementedException();
//    }
//}

//}