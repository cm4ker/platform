using System.Collections.Generic;

namespace ZenPlatform.QueryBuilder.Schema
{
    public class DBSchemaCache
    {
        Dictionary<string, List<DBFieldSchema>> _schema;
        private object _lockObject;
        public DBSchemaCache()
        {
            _lockObject = new object();
            _schema = new Dictionary<string, List<DBFieldSchema>>();

        }

        public void Add(string key, List<DBFieldSchema> value)
        {
            lock (_lockObject)
            {
                if (!_schema.TryGetValue(key, out _))
                    _schema[key] = value;
            }
        }

        //public List<DBFieldSchema> this[string name]
        //{
        //    get
        //    {
        //        if (_schema.TryGetValue(name, out var val))
        //            return val;
        //    }
        //    set
        //    {
        //        lock (_lockObject)
        //        {
        //            _schema[name] = value;
        //        }
        //    }
        //}
    }
}