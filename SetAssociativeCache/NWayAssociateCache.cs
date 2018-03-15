using System;

namespace SetAssociativeCache
{
    public class NWayAssociateCache<TKey, TValue>
    {

        //public NWayAssociateCache(int n, int memoryLength, int blockSizeBytes, int cacheLengthBytes,
        public NWayAssociateCache(int n, int memoryLength, int blockSizeBytes, int cacheLengthBytes,
            KeyComparer keyComparer, ValueComparer valueComparer)
        {
            _n = n;
            _memoryLength = memoryLength;
            _blockSizeBytes = blockSizeBytes;
            _cacheLengthBytes = cacheLengthBytes;
            _keyComparer = keyComparer;
        }

        public delegate int KeyComparer(TKey v1, TKey v2);
        private KeyComparer _keyComparer;

        public delegate int ValueComparer(TKey v1, TKey v2);
        private ValueComparer _valueComparer;

        private int _n;
        private int _memoryLength;
        private int _blockSizeBytes;
        private int _cacheLengthBytes;

        public bool SaveValue(TKey key, TValue value)
        {

        }

    }
}
