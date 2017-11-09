using System;
using System.Collections.Generic;
using System.Text;

namespace ZenPlatform.QueryBuilder.Queries
{
    /// <summary>
    /// Represents INSERT instruction
    /// </summary>
    public class DBInsertQuery : IDataChangeQuery
    {
        private string _compileExpression;
        private string _insertExpression;
        private string _valuesExpression;
        private DBValuesClause _values;
        private DBTable _insertTable;

        public DBInsertQuery()
        {
            _values = new DBValuesClause();
            _compileExpression = "{InsertExpression}\n{ValuesExpression}";
        }

        public DBTable InsertTable
        {
            get { return _insertTable; }
            set { _insertTable = value; }
        }

        

        public void AddField(DBTableField field)
        {
            if (_insertTable != null && field.Owner as DBTable != _insertTable)
            {
                throw new Exception("Trying to update fields in differend tables");
            }
            _insertTable = field.Owner as DBTable;
            _values.AddField(field);
        }

        public List<DBTableField> Fields => _values.Fields;

        public DBParameterCollection Parameters => new DBParameterCollection(_values.Parameters);


        public object Clone()
        {
            throw new NotImplementedException();
        }

        public string Compile(bool recompile = false)
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine($"{SQLTokens.INSERT} {SQLTokens.INTO}");

            sb.Append($"\t {InsertTable.Name}");
            sb.Append("(");
            foreach (var field in _values.Fields)
            {
                if (_values.Fields.IndexOf(field) > 0)
                    sb.Append($",");
                sb.Append($"{field.Name}");
            }
            sb.Append(")");
            _insertExpression = sb.ToString();
            _valuesExpression = _values.Compile();
            return $"{_insertExpression}\n{_valuesExpression}";
        }

        public string CompileExpression
        {
            get { return _compileExpression; }
            set { _compileExpression = value; }
        }
    }

    public class DBSetIdentityInsert : IQueryable
    {
        private readonly string _tableName;
        private readonly bool _isOn;

        public DBSetIdentityInsert(string tableName, bool isOn)
        {
            _tableName = tableName ?? throw new ArgumentNullException(nameof(tableName));
            _isOn = isOn;
        }

        public object Clone()
        {
            throw new NotImplementedException();
        }

        public string Compile(bool recompile = false)
        {
            var sb = new StringBuilder();
            sb.AppendFormat("{0} {1} {2} {3}", SQLTokens.SET, SQLTokens.IDENTITY_INSERT, _tableName,
                (_isOn) ? SQLTokens.ON : SQLTokens.OFF);
            return sb.ToString();
        }

        public string CompileExpression { get; set; }
    }
}