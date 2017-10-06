namespace SqlPlusDbSync.QueryCompiler.Queryes
{
    public class Expression : ISelectItem, IGroupItem, ISingleResultObject
    {
        private readonly ITable _owner;
        private string _expression;
        public Expression(ITable owner)
        {
            _owner = owner;
        }

        public string Alias { get; set; }
        public ITable Owner
        {
            get { return null; }
            set { }
        }

        public void SetExpression(string expression)
        {
            _expression = expression;
        }


        public string Compile()
        {
            if (!string.IsNullOrEmpty(Alias))
                return $"[{Alias}] = {_expression.Replace("%TalbeAlias%", Alias)}";
            else
                return $"{_expression.Replace("%TalbeAlias%", Alias)}";
        }

        public string Compile(CompileOptions options)
        {
            throw new System.NotImplementedException();
        }
    }
}