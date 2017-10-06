namespace SqlPlusDbSync.Platform.Configuration
{
    public class SOperand
    {
        private object _value;

        public SOperand(object value)
        {
            _value = value;
        }

        public object Value
        {
            get { return _value; }
            set { _value = value; }
        }
    }
}