namespace SqlPlusDbSync.QueryCompiler.Queryes
{
    public class Field : ISelectItem, IGroupItem, ISingleResultObject
    {
        private ITable _owner;
        private string _alias;

        public Field(ITable owner)
        {
            _owner = owner;
        }

        public Field(ITable owner, string name, string alias = null) : this(owner)
        {
            Name = name;
            _alias = alias;
        }

        public string Name { get; set; }

        public bool IsKey { get; set; }

        public string Alias
        {
            get
            {
                if (string.IsNullOrEmpty(_alias))
                    return Name;
                return _alias;

            }
            set
            {
                _alias = value;
            }
        }

        public ITable Owner
        {
            get
            {
                return _owner;
            }
        }


        public string Compile()
        {
            //if (!string.IsNullOrEmpty(Alias))
            //    return $"[{Alias}] = [{_owner?.Alias}].[{Name}]";
            //else
            return $"[{_owner?.Alias}].[{Name}]";
        }

        public string Compile(CompileOptions options)
        {
            throw new System.NotImplementedException();
        }
    }
}