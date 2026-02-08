using System.Collections;
using System.Collections.Concurrent;

namespace ZapretUpdater.Utils
{
    public class ConcurrentHashSet<T> : IEnumerable<T> where T : notnull
    {
        private readonly ConcurrentDictionary<T, byte> _dictionary;
        public ConcurrentHashSet()
        {
            _dictionary = new ConcurrentDictionary<T, byte>();
        }

        public ConcurrentHashSet(IEnumerable<T> collection, IEqualityComparer<T>? comparer = null)
        {
            _dictionary = new ConcurrentDictionary<T, byte>();
            foreach (var item in collection)
            {
                _dictionary.TryAdd(item, 0);
            }
        }

        public bool Add(T item)
        {
            return _dictionary.TryAdd(item, 0);
        }
        public IEnumerable<T> Distinct()
        {
            return _dictionary.Keys.Distinct();
        }
        public bool Remove(T item)
        {
            return _dictionary.TryRemove(item, out _);
        }
        public IEnumerable<T> Except(IEnumerable<T> items)
        {
            return _dictionary.Keys.Except(items);
        }
        public bool Contains(T item)
        {
            return _dictionary.ContainsKey(item);
        }
        public void Clear()
        {
            _dictionary.Clear();
        }

        public int Count => _dictionary.Count;
        public IEnumerable<T> ToEnumerable()
        {
            return _dictionary.Keys;
        }

        public IEnumerator<T> GetEnumerator()
        {
            return _dictionary.Keys.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}
