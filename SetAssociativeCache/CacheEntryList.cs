using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SetAssociativeCache
{
    public delegate TKey SelectKeyToDeleteFunc<TKey, TValue> (List<CacheEntry<TKey, TValue>> list);

    public class CacheEntryList<TKey, TValue>
    {
        public CacheEntryList(int n, SelectKeyToDeleteFunc<TKey,TValue> selectDeleteIndexFunc
            , Comparer<TKey> keyComparer, Comparer<TValue> valueComparer) 
        {
            _capacity = n;
            _deleteSelector = selectDeleteIndexFunc;
            _wayData = new List<CacheEntry<TKey, TValue>>(n);
            _keyComparer = keyComparer;
            _valueComparer = valueComparer;
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
        private Comparer<TValue> _valueComparer;

        public void SetValue(TKey key, TValue value)
        {
            var foundEntry = _wayData.FirstOrDefault(p => _keyComparer(p.Key, key) == 0);

            if (foundEntry!=null)
            {
                if ( _valueComparer(foundEntry.Value, value ) != 0)
                    foundEntry.UpdateValue(value); 
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

                    OnMiss?.Invoke(this, EventArgs.Empty);
                }

                var entry = new CacheEntry<TKey, TValue>(key, value);

                _wayData.Add(entry);
            }
        }

        public IEnumerable<CacheEntryStat<TKey,TValue>> GenerateStatisticsList()
        {
            return _wayData.Select(ce => new CacheEntryStat<TKey, TValue>(ce));
        }

        public bool ContainsKey(TKey key)
        {
            var foundEntry = _wayData.FirstOrDefault(p => _keyComparer(p.Key, key) == 0);

            return foundEntry != null;
        }

        public TValue ReadValue(TKey key)
        {
            var foundEntry = _wayData.FirstOrDefault(p => _keyComparer(p.Key, key) == 0);

            if (foundEntry != null)
            {
                OnHit?.Invoke(this, EventArgs.Empty);
                return foundEntry.ReadValueAndUpdateStat();
            }
            else
            {
                OnMiss?.Invoke(this, EventArgs.Empty);
                return default(TValue);
            }
        }

        public string ToString(string tab)
        {
            if (_wayData.Count == 0)
                return $"{tab}--Empty--";

            var s = $"{tab}Index , Key , Value\r\n{tab}---------------------\r\n";

            for (int i = 0; i < _wayData.Count; i++)
            {
                s += $"{tab}{i} : {_wayData[i].Key.ToString()} , {_wayData[i].Value.ToString()}  \r\n";
            }
            return s;
        }
    }
}
