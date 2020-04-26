using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;

namespace ZenPlatform.QueryBuilder
{
    public class DBParameterCollection : List<DBParameter>, ICloneable
    {
        public DBParameterCollection()
        {
        }

        public DBParameterCollection(IEnumerable<DBParameter> enumerator)
        {
            AddRange(enumerator.Distinct(), true);
        }

        public DBParameter this[string value]
        {
            get { return Find(x => x.Name == value); }
        }

        public bool HasParameter(string name)
        {
            return !(Find(x => x.Name.ToLower() == name.ToLower()) is null);
        }

        public void SetValueIfExists(string name, object value)
        {
            throw new Exception("Not implemented");
            //if (HasParameter(name))
            //    this[name].SetValue(value);
        }

        public new void Add(DBParameter param)
        {
            if (HasParameter(param.Name)) throw new Exception("Duplicate parameters");
            base.Add(param);
            OnChange?.Invoke(this, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, new[] { param }));
        }

        public void Add(DBParameter param, bool allowIntersect)
        {
            if (HasParameter(param.Name))
                if (allowIntersect) return;
                else throw new Exception("Duplicate parameters");
            base.Add(param);

            OnChange?.Invoke(this, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, new[] { param }));
        }

        public new void AddRange(IEnumerable<DBParameter> parameters)
        {
            foreach (var item in parameters)
            {
                Add(item);
            }
        }

        public new void Remove(DBParameter param)
        {
            base.Remove(param);
            OnChange?.Invoke(this, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Remove, new[] { param }));
        }

        public new void RemoveAll(Predicate<DBParameter> match)
        {
            var col = this.ToArray();
            foreach (var item in col)
            {
                if (match.Invoke(item))
                {
                    Remove(item);
                }
            }
        }

        public void AddRange(IEnumerable<DBParameter> parameters, bool allowIntersect)
        {
            foreach (var item in parameters)
            {
                Add(item, allowIntersect);
            }
        }

        public event NotifyCollectionChangedEventHandler OnChange;

        public object Clone()
        {
            var result = new DBParameterCollection();
            result.AddRange(this);
            return result;
        }
    }
}