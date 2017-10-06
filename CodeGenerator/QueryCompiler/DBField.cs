using System.Data;
using QueryCompiler.Schema;

namespace QueryCompiler
{
    /// <summary>
    /// Represents field. 
    /// Table field, Query field
    /// </summary>
    public abstract class DBField : DBClause
    {
        private IDBFieldContainer _owner;
        private string _name;

        protected DBField(IDBFieldContainer owner, string name)
        {
            _owner = owner;
            _name = name;
            CompileExpression = "{Name}";
        }

        public IDBFieldContainer Owner
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
            var result = DBClause.CreateSelectField(_owner, new DBFieldSchema(this.Schema.Type, this.Name), _name) as DBSelectField;

            return result;
        }

        public DBField Clone(IDBFieldContainer owner)
        {
            var result = this.MemberwiseClone() as DBField;
            result._owner = owner;
            owner.Fields.Add(result);
            return result;
        }

    }
}