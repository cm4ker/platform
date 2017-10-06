namespace SqlPlusDbSync.QueryCompiler.Queryes.Operator
{
    public abstract class BinaryOperator : OperatorBase, IBooleanResultOperator
    {
        private ISingleResultObject _operand1;
        private ISingleResultObject _operand2;

        protected BinaryOperator(ISingleResultObject operand1, ISingleResultObject operand2)
        {
            _operand1 = operand1;
            _operand2 = operand2;
        }

        protected BinaryOperator()
        {

        }

        public ISingleResultObject Operand1
        {
            get { return _operand1; }
            set { _operand1 = value; }
        }

        public ISingleResultObject Operand2
        {
            get { return _operand2; }
            set { _operand2 = value; }
        }
    }
}