namespace DesignPatternCalculator.Core.Cache;

/// 캐시 구현
public class LRUCache<TKey, TValue> where TKey : notnull
{
    private readonly int _capacity;
    private readonly Dictionary<TKey, LinkedListNode<CacheItem>> _cacheMap;
    private readonly LinkedList<CacheItem> _lruList;

    private class CacheItem
    {
        public TKey Key { get; set; }
        public TValue Value { get; set; }

        public CacheItem(TKey key, TValue value)
        {
            Key = key;
            Value = value;
        }
    }

    public LRUCache(int capacity)
    {
        if (capacity <= 0)
            throw new ArgumentException("캐시 용량은 0보다 커야 합니다.", nameof(capacity));

        _capacity = capacity;
        _cacheMap = new Dictionary<TKey, LinkedListNode<CacheItem>>(capacity);
        _lruList = new LinkedList<CacheItem>();
    }

    public bool TryGetValue(TKey key, out TValue value)
    {
        if (_cacheMap.TryGetValue(key, out var node))
        {
            // 최근 사용으로 이동
            _lruList.Remove(node);
            _lruList.AddFirst(node);
            value = node.Value.Value;
            return true;
        }

        value = default!;
        return false;
    }

    public void Put(TKey key, TValue value)
    {
        if (_cacheMap.TryGetValue(key, out var existingNode))
        {
            // 기존 값 갱신
            existingNode.Value.Value = value;
            _lruList.Remove(existingNode);
            _lruList.AddFirst(existingNode);
        }
        else
        {
            // 용량 초과 시 가장 오래된 항목 제거
            if (_cacheMap.Count >= _capacity)
            {
                var lastNode = _lruList.Last;
                if (lastNode != null)
                {
                    _cacheMap.Remove(lastNode.Value.Key);
                    _lruList.RemoveLast();
                }
            }

            // 새 항목 추가
            var cacheItem = new CacheItem(key, value);
            var newNode = new LinkedListNode<CacheItem>(cacheItem);
            _lruList.AddFirst(newNode);
            _cacheMap[key] = newNode;
        }
    }

    public bool ContainsKey(TKey key)
    {
        return _cacheMap.ContainsKey(key);
    }

    public void Clear()
    {
        _cacheMap.Clear();
        _lruList.Clear();
    }
    public int Count => _cacheMap.Count;

    public int Capacity => _capacity;
}
