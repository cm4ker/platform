using ZenPlatform.QueryBuilder.Interfaces;
using ZenPlatform.QueryBuilder.Schema;

namespace ZenPlatform.QueryBuilder
{
    public class DBSelectField : DBField, IDbAliasedDbToken
    {
        private string _alias;

        public DBSelectField(IDBAliasedFieldContainer owner, string name, string alias = "") : base(owner, name)
        {
            _alias = alias;
            base.CompileExpression = "[{OwnerName}].[{Name}]";
        }

        public DBSelectField(IDBAliasedFieldContainer owner, string name, DBFieldSchema schema, string alias = "") : this(owner, name, alias)
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

            if (Owner != null)
                return StandartCompilers.CompileAliasedObject($"[{Owner.Alias}].[{Name}]", Alias);
            else
                return $"{Name} AS {Alias}";
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

    public class DBSelectConstantField : DBField, IDbAliasedDbToken
    {
        private string _alias;
        private DBClause _constant;

        public DBSelectConstantField(object value, string alias) : base(null, alias)
        {
            _alias = alias;
            base.CompileExpression = "{Constant}";
            _constant = new DBConstant(value);
        }

        public override string Compile(bool recompile = false)
        {
            var exp = base.CompileExpression;

            foreach (var tr in base.Transformations)
            {
                exp = tr.Apply(exp);
            }

            return StandartCompilers.CompileAliasedObject(_constant.Compile(), Alias);
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