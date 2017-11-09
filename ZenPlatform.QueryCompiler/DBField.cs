using ZenPlatform.QueryBuilder.Interfaces;
using ZenPlatform.QueryBuilder.Schema;

namespace ZenPlatform.QueryBuilder
{
    /// <summary>
    /// Represents field. 
    /// Table field, Query field
    /// </summary>
    public abstract class DBField : DBClause
    {
        private IDBAliasedFieldContainer _owner;
        private string _name;

        protected DBField(IDBAliasedFieldContainer owner, string name)
        {
            _owner = owner;
            _name = name;
        }

        public IDBAliasedFieldContainer Owner
        {
            get { return _owner; }
        }

        public override string CompileExpression
        {
            get { return "{Name}"; }
        }

        public DBFieldSchema Schema { get; set; }

        public string Name
        {
            get { return _name; }
        }

        public override string Compile(bool recompile = false)
        {
            return $"{_name}";
        }

        public override object Clone()
        {
            var result = this.MemberwiseClone() as DBField;
            return result;
        }

        public DBSelectField ToSelectField()
        {


            var result = DBClause.CreateSelectField(_owner, new DBFieldSchema(
                this.Schema.Type,
                this.Name,
                this.Schema.ColumnSize,
                this.Schema.NumericPrecision,
                this.Schema.NumericScale,
                this.Schema.IsIdentity,
                this.Schema.IsKey,
                this.Schema.IsUnique,
                this.Schema.IsNullable), _name) as DBSelectField;

            return result;
        }

        public DBSelectField ToSelectField(IDBAliasedFieldContainer owner)
        {


            var result = DBClause.CreateSelectField(owner, new DBFieldSchema(
                this.Schema.Type,
                this.Name,
                this.Schema.ColumnSize,
                this.Schema.NumericPrecision,
                this.Schema.NumericScale,
                this.Schema.IsIdentity,
                this.Schema.IsKey,
                this.Schema.IsUnique,
                this.Schema.IsNullable), _name) as DBSelectField;

            return result;
        }

        public DBField Clone(IDBAliasedFieldContainer owner)
        {
            var result = this.MemberwiseClone() as DBField;
            result._owner = owner;
            owner.Fields.Add(result);
            return result;
        }

    }
}