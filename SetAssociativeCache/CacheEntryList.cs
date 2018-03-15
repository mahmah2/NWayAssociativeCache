using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SetAssociativeCache
{
    public delegate TKey SelectKeyToDeleteFunc<TKey, TValue> (List<CacheEntry<TKey, TValue>> list);

    public class CacheEntryList<TKey, TValue>
    {
        public CacheEntryList(int n, SelectKeyToDeleteFunc<TKey,TValue> selectDeleteIndexFunc, Comparer<TKey> keyComparer) 
        {
            _capacity = n;
            _deleteSelector = selectDeleteIndexFunc;
            _wayData = new List<CacheEntry<TKey, TValue>>(n);
            _keyComparer = keyComparer;
        }

        new public void Add(TKey key, TValue value)
        {
            var baseClass = this as Dictionary<TKey, TValue>;
            baseClass.Add(key, value);

            LastReadTime = DateTime.Now;
            ReadCount = 0;
        }

        public DateTime LastReadTime { get; set; }

        public event EventHandler OnMiss;
        public event EventHandler OnHit;

        public int ReadCount { get; set; }

        private int _capacity;

        private List<CacheEntry<TKey, TValue>> _wayData;

        private SelectKeyToDeleteFunc<TKey, TValue> _deleteSelector;

        private Comparer<TKey> _keyComparer;

        public void SetValue(TKey key, TValue value)
        {
            var foundEntry = _wayData.FirstOrDefault(p => _keyComparer(p.Key, key) == 0);

            if (foundEntry!=null)
            {
                foundEntry.Value = value; //todo : update value function
            }
            else
            {
                while(_wayData.Count >= _capacity) 
                {
                    var keyToRemove = _deleteSelector(_wayData);

                    var entryToRemove = _wayData.FirstOrDefault(p => _keyComparer(p.Key, keyToRemove) == 0);

                    if (entryToRemove == null)
                        throw new Exception($"Selected key to be deleted doesn't exist. key = {keyToRemove?.ToString()} ");

                    _wayData.Remove(entryToRemove);

                    OnMiss?.Invoke(null, EventArgs.Empty);
                }

                var entry = new CacheEntry<TKey, TValue>(key, value);

                _wayData.Add(entry);
            }
        }

        public List<CacheEntry<TKey,TValue>> GenerateInfoList()
        {
            return _wayData;
        }

        internal bool ContainsKey(TKey key)
        {
            var foundEntry = _wayData.FirstOrDefault(p => _keyComparer(p.Key, key) == 0);

            return foundEntry != null;
        }

        internal TValue ReadValue(TKey key)
        {
            var foundEntry = _wayData.FirstOrDefault(p => _keyComparer(p.Key, key) == 0);

            if (foundEntry != null)
            {
                OnHit?.Invoke(null, EventArgs.Empty);
                return foundEntry.ReadValueAndUpdateStat();
            }
            else
            {
                OnMiss?.Invoke(null, EventArgs.Empty);
                return default(TValue);
            }
        }
    }
}
