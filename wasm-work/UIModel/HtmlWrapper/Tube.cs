using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace UIModel.HtmlWrapper
{
    public class Tube<T> : IEnumerable<T>
    {
        private List<T> _items;

        public Tube(int maxItems) : this()
        {
            MaxItems = maxItems;
        }

        public Tube()
        {
            _items = new List<T>();
        }

        public int MaxItems { get; set; }

        public void PushTop(T item)
        {
            _items.Insert(0, item);

            if (_items.Count > MaxItems)
            {
                OnSupersededNotify(_items.GetRange(_items.Count - 1, 1));
                _items.RemoveAt(_items.Count - 1);
            }
        }

        public void PushBottom(T item)
        {
            _items.Add(item);

            if (_items.Count > MaxItems)
            {
                OnSupersededNotify(_items.GetRange(0, 1));
                _items.RemoveAt(0);
            }
        }

        public void PushTop(IEnumerable<T> items)
        {
            _items.InsertRange(0, items);

            if (_items.Count > MaxItems)
            {
                var removed = _items.GetRange(MaxItems, _items.Count - MaxItems).ToList();
                OnSupersededNotify(removed);
                _items.RemoveRange(MaxItems, _items.Count - MaxItems);
            }
        }

        public event EventHandler<IEnumerable<T>> OnSuperseded;

        public void PushBottom(IEnumerable<T> items)
        {
            _items.AddRange(items);

            if (_items.Count > MaxItems)
            {
                var removed = _items.GetRange(0, _items.Count - MaxItems).ToList();
                OnSupersededNotify(removed);
                _items.RemoveRange(0, _items.Count - MaxItems);
            }
        }

        public T TakeTop()
        {
            if (_items.Count == 0) return default(T);

            var first = _items.First();
            _items.RemoveAt(0);
            return first;
        }

        public IEnumerable<T> TakeTopRange(int itemsCount)
        {
            if (_items.Count == 0) return _items;

            var count = Math.Min(itemsCount, Count);

            var first = _items.Take(count).ToList();

            _items.RemoveRange(0, itemsCount);

            return first;
        }

        public IEnumerable<T> TakeBottomRange(int itemsCount)
        {
            if (_items.Count == 0) return _items;

            var count = Math.Min(itemsCount, Count);

            var last = _items.TakeLast(count).ToList();

            _items.RemoveRange(_items.Count - count, count);
            return last;
        }

        public T TakeBottom()
        {
            var last = _items.Last();
            _items.RemoveAt(_items.Count - 1);
            return last;
        }

        public int Count => _items.Count;

        public void Clear()
        {
            _items.Clear();
        }

        public IEnumerator<T> GetEnumerator()
        {
            return _items.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        protected virtual void OnSupersededNotify(IEnumerable<T> e)
        {
            OnSuperseded?.Invoke(this, e);
        }
    }
}