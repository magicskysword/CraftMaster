using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace CraftMaster;

public class OrderedDictionary<TKey, TValue> : IDictionary<TKey, TValue>
{
    private readonly List<KeyValuePair<TKey, TValue>> _list;
    private readonly Dictionary<TKey, TValue> _dictionary;

    public OrderedDictionary()
    {
        _list = new List<KeyValuePair<TKey, TValue>>();
        _dictionary = new Dictionary<TKey, TValue>();
    }

    public OrderedDictionary(IEqualityComparer<TKey> comparer)
    {
        _list = new List<KeyValuePair<TKey, TValue>>();
        _dictionary = new Dictionary<TKey, TValue>(comparer);
    }
    
    public void Add(TKey key, TValue value)
    {
        _dictionary.Add(key, value);
        _list.Add(new KeyValuePair<TKey, TValue>(key, value));
    }

    public void Add(KeyValuePair<TKey, TValue> item)
    {
        _dictionary.Add(item.Key, item.Value);
        _list.Add(item);
    }
    
    public bool Remove(TKey key)
    {
        if (_dictionary.Remove(key))
        {
            _list.RemoveAll(kvp => kvp.Key.Equals(key));
            return true;
        }
        return false;
    }

    public bool TryGetValue(TKey key, out TValue value)
    {
        return _dictionary.TryGetValue(key, out value);
    }
    
    public void Clear()
    {
        _dictionary.Clear();
        _list.Clear();
    }

    public bool ContainsKey(TKey key)
    {
        return _dictionary.ContainsKey(key);
    }

    public bool Contains(KeyValuePair<TKey, TValue> item)
    {
        return _dictionary.Contains(item);
    }

    public void CopyTo(KeyValuePair<TKey, TValue>[] array, int arrayIndex)
    {
        ((IDictionary<TKey, TValue>)_dictionary).CopyTo(array, arrayIndex);
    }

    public int Count
    {
        get { return _dictionary.Count; }
    }

    public bool IsReadOnly
    {
        get { return ((IDictionary<TKey, TValue>)_dictionary).IsReadOnly; }
    }

    public bool Remove(KeyValuePair<TKey, TValue> item)
    {
        if (_dictionary.Remove(item.Key))
        {
            _list.RemoveAll(kvp => kvp.Key.Equals(item.Key));
            return true;
        }
        return false;
    }

    public IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator()
    {
        return _list.GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }

    public ICollection<TKey> Keys
    {
        get { return _list.ConvertAll(kvp => kvp.Key); }
    }

    public ICollection<TValue> Values
    {
        get { return _list.ConvertAll(kvp => kvp.Value); }
    }

    public TValue this[TKey key]
    {
        get { return _dictionary[key]; }
        set
        {
            if (_dictionary.ContainsKey(key))
            {
                var index = _list.FindIndex(kvp => kvp.Key.Equals(key));
                _list[index] = new KeyValuePair<TKey, TValue>(key, value);
            }
            else
            {
                _list.Add(new KeyValuePair<TKey, TValue>(key, value));
            }
            _dictionary[key] = value;
        }
    }
}