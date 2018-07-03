using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;

namespace ZenPlatform.Shared.ParenChildCollection
{
    /// <summary>
    /// Collection of child items. This collection automatically set the
    /// Parent property of the child items when they are added or removed
    /// </summary>
    /// <typeparam name="TParent">Type of the parent object</typeparam>
    /// <typeparam name="TChildren">Type of the child items</typeparam>
    public class ChildItemCollection<TParent, TChildren> : IList<TChildren>, INotifyCollectionChanged,
        INotifyPropertyChanged
        where TParent : class
        where TChildren : class, IChildItem<TParent>
    {
        private TParent _parent;
        private IList<TChildren> _collection;

        public ChildItemCollection(TParent parent)
        {
            this._parent = parent;
            this._collection = new List<TChildren>();
        }

        public ChildItemCollection(TParent parent, IList<TChildren> collection)
        {
            this._parent = parent;
            this._collection = collection;
        }


        #region IList<T> Members

        public int IndexOf(TChildren item)
        {
            return _collection.IndexOf(item);
        }

        public void Insert(int index, TChildren item)
        {
            if (item != null)
                item.Parent = _parent;
            _collection.Insert(index, item);
            CollectionChanged?.Invoke(this,
                new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, new[] {item}));
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("Items"));
        }

        public void InsertRange(int startIndex, IEnumerable<TChildren> items)
        {
            foreach (var item in items)
            {
                Insert(startIndex, item);
                startIndex++;
            }
        }

        public void RemoveAt(int index)
        {
            TChildren oldItem = _collection[index];
            _collection.RemoveAt(index);
            if (oldItem != null)
                oldItem.Parent = null;

            CollectionChanged?.Invoke(this,
                new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Remove, new[] {oldItem}));
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("Items"));
        }

        public TChildren this[int index]
        {
            get { return _collection[index]; }
            set
            {
                TChildren oldItem = _collection[index];
                if (value != null)
                    value.Parent = _parent;
                _collection[index] = value;
                if (oldItem != null)
                    oldItem.Parent = null;

                CollectionChanged?.Invoke(this,
                    new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Replace, new[] {value},
                        new[] {oldItem}));
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("Items"));
            }
        }

        #endregion

        #region ICollection<T> Members

        public void Add(TChildren item)
        {
            if (item != null)
                item.Parent = _parent;
            _collection.Add(item);
            CollectionChanged?.Invoke(this,
                new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, new[] {item}));
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("Items"));
        }

        public void AddRange(IEnumerable<TChildren> items)
        {
            foreach (var item in items)
            {
                Add(item);
            }
        }

        public void AddRange(params TChildren[] items)
        {
            foreach (var item in items)
            {
                Add(item);
            }
        }

        public void Clear()
        {
            foreach (TChildren item in _collection)
            {
                if (item != null)
                    item.Parent = null;
            }

            var oldItems = _collection.ToArray();
            _collection.Clear();
            CollectionChanged?.Invoke(this,
                new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Remove, oldItems));
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("Items"));
        }

        public bool Contains(TChildren item)
        {
            return _collection.Contains(item);
        }

        public void CopyTo(TChildren[] array, int arrayIndex)
        {
            _collection.CopyTo(array, arrayIndex);
            CollectionChanged?.Invoke(this,
                new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, array, arrayIndex));
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("Items"));
        }

        public int Count
        {
            get { return _collection.Count; }
        }

        public bool IsReadOnly
        {
            get { return _collection.IsReadOnly; }
        }

        public bool Remove(TChildren item)
        {
            bool b = _collection.Remove(item);
            if (item != null)
                item.Parent = null;
            CollectionChanged?.Invoke(this,
                new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Remove, new[] {item}));
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("Items"));
            return b;
        }

        #endregion

        #region IEnumerable<T> Members

        public IEnumerator<TChildren> GetEnumerator()
        {
            return _collection.GetEnumerator();
        }

        #endregion

        #region IEnumerable Members

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return (_collection as System.Collections.IEnumerable).GetEnumerator();
        }

        #endregion


        public event NotifyCollectionChangedEventHandler CollectionChanged;
        public event PropertyChangedEventHandler PropertyChanged;
    }
}