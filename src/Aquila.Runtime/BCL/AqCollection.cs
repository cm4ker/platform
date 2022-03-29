using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;

namespace Aquila.Core;

public class AqImmutableCollection<T> : IEnumerable<T>
{
    private List<T> _innerList;

    public bool IsInitialized => _innerList == null;

    protected void Reload()
    {
        _innerList = ReloadCore();
    }

    protected virtual List<T> ReloadCore()
    {
        throw new NotImplementedException();
    }

    public IEnumerator<T> GetEnumerator()
    {
        if (!IsInitialized)
            Reload();

        return _innerList.GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }
}

public class AqCollection<T> : IEnumerable<T>
{
    private readonly AqFactoryDelegate<T> _factory;
    private List<T> _innerList;

    public AqCollection(AqFactoryDelegate<T> factory, ImmutableArray<T> items)
    {
        _innerList = items.ToList();
        _factory = factory;
    }

    public T create()
    {
        var item = _factory();
        _innerList.Add(item);
        return item;
    }

    public void remove(T item)
    {
        _innerList.Remove(item);
    }

    public IEnumerator<T> GetEnumerator()
    {
        return _innerList.GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }
}