using System;
using System.Collections.Generic;

namespace SqlPlusDbSync.QueryCompiler.Queryes
{
    public class SelectQuery : SelectQueryBase, ITable
    {
        private string _alias;

        public List<ISelectItem> GetFileds()
        {
            var result = new List<ISelectItem>();
            foreach (var selectItem in SelectItems)
            {
                result.Add(new Field(this, selectItem.Alias));
            }
            return result;
        }

        public List<ISelectItem> GetFiledsWithNewAliasPreffix(string prefix)
        {
            var result = new List<ISelectItem>();
            foreach (var selectItem in SelectItems)
            {
                result.Add(new Field(this, selectItem.Alias, $"{prefix}.{selectItem.Alias}"));
            }
            return result;
        }

        public void AddField(ISelectItem field)
        {
            AddSelect(field);
        }

        public void AddFields(List<ISelectItem> fields)
        {
            fields.ForEach(x => AddSelect(x));
        }

        public void RemoveField(ISelectItem field)
        {
            throw new NotImplementedException();
        }

        public string Alias
        {
            get
            {
                if (!string.IsNullOrEmpty(_alias))
                    return _alias;
                return Table.Alias;
            }
            set { _alias = value; }
        }


        public override string Compile(CompileOptions options)
        {
            if (options.IsSubQuery)
            {
                return $"({Compile()}) AS {Alias}";
            }

            return base.Compile(options);
        }
    }
}