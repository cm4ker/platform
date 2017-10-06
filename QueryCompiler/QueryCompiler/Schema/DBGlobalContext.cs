using System;
using System.Collections.Generic;

namespace QueryCompiler.Schema
{
    public class DBGlobalContext
    {
        private DBGlobalContext()
        {
            _variables = new Dictionary<string, object>();
        }

        private static DBGlobalContext _instance;
        private static object lockObject = new object();

        public Dictionary<string, object> _variables;

        public static DBGlobalContext Insatnce
        {
            get
            {
                lock (lockObject)
                {
                    _instance = _instance ?? new DBGlobalContext();
                }
                return _instance;
            }
        }

        public bool Exists(string name)
        {
            return _variables.ContainsKey(name);
        }

        public object this[string name]
        {
            get
            {
                if (_variables.TryGetValue(name, out var val))
                    return val;
                throw new Exception($"Cannot access value name: {name}");
            }
            set
            {
                _variables[name] = value;
            }
        }

    }
}