namespace SqlPlusDbSync.QueryCompiler.Queryes.Operator
{
    public class InOperator : OperatorBase, IBooleanResultOperator
    {
        private readonly ISelectItem _field;
        private readonly IQueryObject _query;

        public InOperator(ISelectItem field, IQueryObject query)
        {
            _field = field;
            _query = query;
        }

        public override string Compile()
        {
            return $"{_field.Compile()} IN ({_query.Compile()})";
        }
    }
}