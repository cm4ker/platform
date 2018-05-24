using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Xml.Serialization;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ZenPlatform.Configuration.ConfigurationLoader.XmlConfiguration;

namespace ZenPlatform.Tests.Conf
{
    [TestClass]
    public class SerializationTesting
    {
        [TestMethod]
        public void Test()
        {
            using (var sw = new StringWriter())
            {
                var parent = new Parent();
                parent.Name = "Test object";

                parent.Children.Add(new Child() { Name = "Hello", Parent = parent });

                XmlSerializer serializer = new XmlSerializer(typeof(Parent));
                serializer.Serialize(sw, parent);

                using (var sr = new StringReader(sw.ToString()))
                {
                    var o = serializer.Deserialize(sr);
                }
            }
        }

    }

    /// <summary>
    /// Defines the contract for an object that has a parent object
    /// </summary>
    /// <typeparam name="TParent">Type of the parent object</typeparam>
    public interface IChildItem<TParent> where TParent : class
    {
        TParent Parent { get; set; }
    }

    /// <summary>
    /// Collection of child items. This collection automatically set the
    /// Parent property of the child items when they are added or removed
    /// </summary>
    /// <typeparam name="TParent">Type of the parent object</typeparam>
    /// <typeparam name="TChildren">Type of the child items</typeparam>
    public class ChildItemCollection<TParent, TChildren> : IList<TChildren>
        where TParent : class
        where TChildren : IChildItem<TParent>
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
        }

        public void RemoveAt(int index)
        {
            TChildren oldItem = _collection[index];
            _collection.RemoveAt(index);
            if (oldItem != null)
                oldItem.Parent = null;
        }

        public TChildren this[int index]
        {
            get
            {
                return _collection[index];
            }
            set
            {
                TChildren oldItem = _collection[index];
                if (value != null)
                    value.Parent = _parent;
                _collection[index] = value;
                if (oldItem != null)
                    oldItem.Parent = null;
            }
        }

        #endregion

        #region ICollection<T> Members

        public void Add(TChildren item)
        {
            if (item != null)
                item.Parent = _parent;
            _collection.Add(item);
        }

        public void Clear()
        {
            foreach (TChildren item in _collection)
            {
                if (item != null)
                    item.Parent = null;
            }
            _collection.Clear();
        }

        public bool Contains(TChildren item)
        {
            return _collection.Contains(item);
        }

        public void CopyTo(TChildren[] array, int arrayIndex)
        {
            _collection.CopyTo(array, arrayIndex);
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
    }

    public class Parent
    {
        public Parent()
        {
            this.Children = new ChildItemCollection<Parent, Child>(this);
        }

        public string Name { get; set; }

        public ChildItemCollection<Parent, Child> Children { get; private set; }
    }

    public class Child : IChildItem<Parent>
    {
        public string Name { get; set; }

        [XmlIgnore]
        public Parent Parent { get; set; }

        Parent IChildItem<Parent>.Parent
        {
            get
            {
                return this.Parent;
            }
            set
            {
                this.Parent = value;
            }
        }
    }
}
