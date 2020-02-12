using System;
using System.Collections.Generic;
using System.Linq;
using ZenPlatform.Compiler.Contracts;
using ZenPlatform.Language.Ast;

namespace ZenPlatform.Compiler
{
    public class BindingClassTable
    {
        private class BindingClassRow
        {
            public string FullName { get; set; }
            public string Name { get; set; }
            public IType Type { get; set; }
            public BindingClass Class { get; set; }
        }

        private List<BindingClassRow> _rows;

        public BindingClassTable()
        {
            _rows = new List<BindingClassRow>();
        }

        public void AddBinding(string fullName, IType type, BindingClass @class)
        {
            _rows.Add(new BindingClassRow
            {
                FullName = fullName, Type = type,
                Class = @class
            });
        }

        public IType FindType(string ns, string typeName)
        {
            if (!string.IsNullOrEmpty(ns))
                ns = ns + ".";

            var results = _rows.Where(x => x.FullName == $"{ns}{typeName}").ToList();

            if (results.Count == 1)
                return results.First().Type;

            if (results.Count > 1)
                throw new Exception("Multiply types");

            throw new Exception("Type not found");
        }
    }
    
    /*
     FROM Entity SELECT Field >> list (IEnumerable<dynamic(Field)>) -> to List<dynamic> ... Add(dynamic)
     
     List`1
        Add(T a)
        Remove(T e)
     */
}