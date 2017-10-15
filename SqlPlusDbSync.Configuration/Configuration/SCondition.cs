using SqlPlusDbSync.Configuration.Configuration;

namespace SqlPlusDbSync.Platform.Configuration
{
    public class SCondition
    {
        private readonly PType _o1;
        private readonly string _fieldName1;
        private readonly PType _o2;
        private readonly string _fieldName2;

        public SCondition()
        {
        }

        public SCondition(PType o1, string fieldName1, PType o2, string fieldName2)
        {
            _o1 = o1;
            _fieldName1 = fieldName1;
            _o2 = o2;
            _fieldName2 = fieldName2;
        }


        public PType Object1
        {
            get { return _o1; }
        }
        public PType Object2
        {
            get { return _o2; }
        }

        public SField Field1
        {
            get { return Object1.Fields.Find(x => x.Name.ToLower() == _fieldName1.ToLower()); }
        }
        public SField Field2
        {
            get { return Object2.Fields.Find(x => x.Name.ToLower() == _fieldName2.ToLower()); }
        }
    }
}