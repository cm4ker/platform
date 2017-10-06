namespace SqlPlusDbSync.QueryCompiler.Queryes.Operator
{
    public class AndOperator : BinaryOperator
    {
        public AndOperator(IBooleanResultOperator operand1, IBooleanResultOperator operand2) : base(operand1, operand2)
        {
        }

        public AndOperator()
        {

        }

        public override string Compile()
        {
            return $"({Operand1.Compile()} AND {Operand2.Compile()})";
        }
    }
}