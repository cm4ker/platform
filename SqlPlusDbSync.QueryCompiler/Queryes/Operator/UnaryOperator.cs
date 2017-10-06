namespace SqlPlusDbSync.QueryCompiler.Queryes.Operator
{
    public abstract class UnaryOperator : OperatorBase, ISingleResultObject
    {
        private ISingleResultObject _operand;

        protected UnaryOperator(ISingleResultObject operand)
        {
            _operand = operand;
        }

        protected UnaryOperator()
        {

        }

        protected ISingleResultObject Operand
        {
            get { return _operand; }
            set { _operand = value; }

        }
    }

    public class ParameterOperator : OperatorBase, ISingleResultObject
    {
        private string _paramName;

        public ParameterOperator()
        {

        }

        public ParameterOperator(string paramName)
        {
            _paramName = paramName;
        }

        public string ParamName
        {
            get { return _paramName; }
            set { _paramName = value; }
        }

        public override string Compile()
        {
            return '@' + _paramName.TrimStart('@');
        }
    }

    public class ConstantOperator : OperatorBase, ISingleResultObject
    {
        private object _constant;

        public ConstantOperator()
        {

        }

        public ConstantOperator(object constant)
        {
            _constant = constant;
        }

        public object Constant
        {
            get { return _constant; }
            set { _constant = value; }
        }

        public override string Compile()
        {
            return _constant.ToString();
        }
    }
}