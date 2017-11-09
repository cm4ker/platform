using System.Collections.Generic;
using System.Linq;
using ZenPlatform.QueryBuilder.Interfaces;
using ZenPlatform.QueryBuilder.Schema;

namespace ZenPlatform.QueryBuilder
{
    public class DBTable : IDBTableDataSource, IDbAliasedDbToken
    {
        private readonly string _name;
        private readonly List<DBField> _fields;
        private string _compileExpression;
        private string _alias;

        public DBTable(string name, string alias = "")
        {
            _name = name;
            _fields = new List<DBField>();
            _compileExpression = "[{Name}]";
            _alias = alias;

        }

        public DBTableField DeclareField(string name)
        {
            var result = new DBTableField(this, name);
            _fields.Add(result);
            return result;
        }

        public string Compile(bool recompile = false)
        {
            return StandartCompilers.CompileAliasedObject($"[{_name}]", _alias);
        }
        
        public string CompileExpression
        {
            get { return _compileExpression; }
            set { _compileExpression = value; }
        }

        public void SetAliase(string alias)
        {
            _alias = alias;
        }

        public string Alias
        {
            get { return string.IsNullOrEmpty(_alias) ? _name : _alias; }
        }

        public string Name
        {
            get { return _name; }
        }

        public object Clone()
        {
            return this.MemberwiseClone();
        }

        public List<DBField> Fields
        {
            get { return _fields; }
        }

        public void Validate()
        {

        }

        public DBField GetField(string name)
        {
            return _fields.First(x => (x as DBTableField).Name.ToLower() == name.ToLower());
        }
    }
}