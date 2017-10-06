using System.Collections.Generic;

namespace SqlPlusDbSync.QueryCompiler.Queryes
{
    public class Table : ITable
    {
        private List<ISelectItem> _fields;

        private string _name;

        public Table(string tableName)
        {
            _name = tableName;
            _fields = new List<ISelectItem>();
        }

        public Table(string tableName, string alias) : this(tableName)
        {
            Alias = alias;
        }

        public Table()
        {

        }



        public Field CreateField()
        {
            var f = new Field(this);
            _fields.Add(f);
            return f;
        }

        public void RemoveField(Field field)
        {
            _fields.Remove(field);
        }

        public List<ISelectItem> GetFileds()
        {
            return _fields;
        }


        public string Alias { get; set; }

        public string Name
        {
            get { return _name; }
            set { _name = value; }
        }


        public string Compile()
        {
            if (!string.IsNullOrEmpty(Alias))
            {
                return $"[{_name}] AS {Alias}";
            }
            else
            {
                return $"[{_name}]";
            }
        }

        public string Compile(CompileOptions options)
        {
            throw new System.NotImplementedException();
        }
    }
}
