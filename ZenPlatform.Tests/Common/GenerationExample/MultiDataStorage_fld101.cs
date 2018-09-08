using System;

namespace DefaultNamespace
{
    public class MultiDataStorage_fld101
    {
        private readonly ТестоваяСущностьDto _dto;

        public MultiDataStorage_fld101(ТестоваяСущностьDto dto)
        {
            _dto = dto;
        }

        public object Value => Get();
        
        public object Get()
        {
            if (_dto.fld101_Binary != default(byte[]))
                return _dto.fld101_Binary;
            if (_dto.fld101_Boolean != default(bool))
                return _dto.fld101_Boolean;
            if (_dto.fld101_DateTime != default(DateTime))
                return _dto.fld101_DateTime;
            if (_dto.fld101_Guid != default(Guid))
                return _dto.fld101_Guid;

            return null;
        }

        private void ClearAll()
        {
            _dto.fld101_Binary = default(byte[]);
            _dto.fld101_Boolean = default(bool);
            _dto.fld101_DateTime = default(DateTime);
            _dto.fld101_Guid = default(Guid);
        }

        public void Clear()
        {
            ClearAll();
        }

        public bool HasValue()
        {
            return Get() == null;
        }

        public void Set(byte[] value)
        {
            ClearAll();
            _dto.fld101_Binary = value;
        }

        public void Set(bool value)
        {
            ClearAll();
            _dto.fld101_Boolean = value;
        }

        public void Set(DateTime value)
        {
            ClearAll();
            _dto.fld101_DateTime = value;
        }

        public void Set(Guid value)
        {
            ClearAll();
            _dto.fld101_Guid = value;
        }
    }
}