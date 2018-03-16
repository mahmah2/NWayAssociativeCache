using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace SetAssociativeCache
{
    public delegate TKey SelectKeyToDeleteFunc<TKey, TValue>(List<CacheEntry<TKey, TValue>> list);

    public class CacheEntryList<TKey, TValue>
    {
        public CacheEntryList(int n, SelectKeyToDeleteFunc<TKey, TValue> selectDeleteIndexFunc
            , Comparer<TKey> keyComparer, Comparer<TValue> valueComparer)
        {
            _capacity = n;
            _deleteSelector = selectDeleteIndexFunc;
            _wayData = new List<CacheEntry<TKey, TValue>>(n);
            _keyComparer = keyComparer;
            _valueComparer = valueComparer;

            _writeLock = new object();
        }

        public event EventHandler OnMiss;
        public event EventHandler OnHit;

        private int _capacity;

        private List<CacheEntry<TKey, TValue>> _wayData;

        private SelectKeyToDeleteFunc<TKey, TValue> _deleteSelector;

        private Comparer<TKey> _keyComparer;

        private Comparer<TValue> _valueComparer;

        private object _writeLock;

        public bool SetDeleteKeySelector(SelectKeyToDeleteFunc<TKey,TValue> func)
        {
            lock (_writeLock)
            {
                _deleteSelector = func;
                return true;
            }
        }

        public void SetValue(TKey key, TValue value)
        {
            var missHappened = false;
            lock (_writeLock)
            {
                var foundEntry = _wayData.FirstOrDefault(p => _keyComparer(p.Key, key) == 0);

                if (foundEntry != null)
                {
                    if (_valueComparer(foundEntry.Value, value) != 0)
                    {
                        foundEntry.UpdateValue(value);
                    }
                }
                else
                {
                    while (_wayData.Count >= _capacity)
                    {
                        var keyToRemove = _deleteSelector(_wayData);

                        var entryToRemove = _wayData.FirstOrDefault(p => _keyComparer(p.Key, keyToRemove) == 0);

                        if (entryToRemove == null)
                            throw new Exception($"Selected key to be deleted doesn't exist. key = {keyToRemove?.ToString()} ");

                        //Trace.WriteLine($"Removing key : {entryToRemove.Key.ToString()}");
                        //Trace.WriteLine(ToString("\t"));
                        //Trace.WriteLine($"----------------------------------------------");

                        _wayData.Remove(entryToRemove);

                        missHappened = true;
                    }

                    var entry = new CacheEntry<TKey, TValue>(key, value);

                    _wayData.Add(entry);
                }
            }

            if (missHappened)
            {
                OnMiss?.Invoke(this, EventArgs.Empty);
            }
        }

        public IEnumerable<CacheEntryStat<TKey, TValue>> GenerateStatisticsList()
        {
            return _wayData.Select(ce => new CacheEntryStat<TKey, TValue>(ce));
        }

        public bool ContainsKey(TKey key)
        {
            CacheEntry<TKey, TValue> foundEntry = default(CacheEntry<TKey, TValue>);

            lock (_writeLock)
            {
                foundEntry = _wayData.FirstOrDefault(p => _keyComparer(p.Key, key) == 0);
            }

            return foundEntry != null;
        }

        public TValue ReadValue(TKey key)
        {
            var missHappened = false;
            var hitHappened = false;

            try
            {
                lock (_writeLock)
                {
                    CacheEntry<TKey, TValue> foundEntry = default(CacheEntry<TKey, TValue>);

                    foundEntry = _wayData.FirstOrDefault(p => _keyComparer(p.Key, key) == 0);

                    if (foundEntry != null)
                    {
                        hitHappened = true;
                        return foundEntry.ReadValueAndUpdateStat();
                    }
                    else
                    {
                        missHappened = true;
                        return default(TValue);
                    }
                }
            }
            finally
            {
                if (hitHappened)
                    OnHit?.Invoke(this, EventArgs.Empty);

                if (missHappened)
                    OnMiss?.Invoke(this, EventArgs.Empty);
            }
        }

        public string ToString(string tab)
        {
            if (_wayData.Count == 0)
                return $"{tab}--Empty--";

            var s = $"{tab}Index , Key , Value\r\n{tab}---------------------\r\n";

            for (int i = 0; i < _wayData.Count; i++)
            {
                s += $"{tab}{i} : {_wayData[i].Key.ToString()} , {_wayData[i].Value.ToString()}, {_wayData[i].LastReadTick.ToString()}\r\n";
            }
            return s;
        }
    }
}
