using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;

namespace ZenPlatform.EntityComponent
{
    public class TableCollection<TDtoRowType, TRowType, TParent> : IEnumerable<TRowType>
    {
        protected readonly IList<TDtoRowType> _dto;
        protected readonly IList<TRowType> _col;

        public TableCollection(TParent link, IList<TDtoRowType> dto)
        {
            _dto = dto;
            _col = new List<TRowType>();
            Link = link;
        }

        public TRowType this[int index] => _col[index];

        public TParent Link { get; }

        public virtual TRowType Add()
        {
            throw new NotImplementedException();
        }

        public virtual void Remove(TRowType item)
        {
            throw new NotImplementedException();
        }

        public TRowType Find(Func<TRowType, bool> criteria)
        {
            return _col.FirstOrDefault(criteria);
        }

        public IEnumerator<TRowType> GetEnumerator()
        {
            return _col.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }

    public class RO_TableCollection
    {
    }
}