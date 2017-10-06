using System.Data;
using QueryCompiler.Interfaces;
using QueryCompiler.Schema;

namespace QueryCompiler
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

            CompileExpression = "{Name}";

            Schema = this.GetUnknownSchema();
        }

        public IDBAliasedFieldContainer Owner
        {
            get { return _owner; }
        }

        public DBFieldSchema Schema { get; set; }

        public string Name
        {
            get { return _name; }
        }

        public override string Compile(bool recompile = false)
        {
            return StandartCompilers.SimpleCompiler(CompileExpression, new { Name = _name });
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