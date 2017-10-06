using QueryCompiler.Schema;

namespace QueryCompiler
{
    public class DBSelectField : DBField, IDBAliasedToken
    {
        private string _alias;

        public DBSelectField(IDBFieldContainer owner, string name, string alias = "") : base(owner, name)
        {
            _alias = alias;
            base.CompileExpression = "[{OwnerName}].[{Name}]";
        }

        public DBSelectField(IDBFieldContainer owner, string name, DBFieldSchema schema, string alias = "") : this(owner, name, alias)
        {
            Schema = schema;
        }

        public override string Compile(bool recompile = false)
        {
            var exp = base.CompileExpression;

            foreach (var tr in base.Transformations)
            {
                exp = tr.Apply(exp);
            }

            return StandartCompilers.CompileAliasedObject(exp, new { OwnerName = Owner.Alias, Name, Alias });
        }

        public void SetAliase(string alias)
        {
            _alias = alias;
        }

        public string Alias
        {
            get { return _alias; }
        }
    }
}