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
        private IDBFieldContainer _owner;
        private string _name;

        protected DBField(IDBFieldContainer owner, string name)
        {
            _owner = owner;
            _name = name;
        }

        public IDBFieldContainer Owner
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
            if (_owner is IDBTableDataSource)
                return new DBSelectField(_owner as IDBTableDataSource, _name);
            else
                throw
                    new System.Exception(
                        "Field owner must be IDBTableDataSource."); // to do ������� ��������� ����� ����������. ��� ���� ������ ��� �������.
        }

        /*
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
        */
        /*
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
        */

        public bool Equals(DBField field)
        {
            if (field == null || GetType() != field.GetType())
            {
                return false;
            }

            return this.Name == field.Name && this.Schema.Equals(field.Schema);
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