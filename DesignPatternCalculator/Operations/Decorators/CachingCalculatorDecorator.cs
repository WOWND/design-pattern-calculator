using DesignPatternCalculator.Core.Cache;

namespace DesignPatternCalculator.Operations.Decorators;

/// Decorator 패턴 구상 클래스
/// 동일한 수식의 반복 계산을 캐시에서 가져옴
public class CachingCalculatorDecorator
{
    private readonly LRUCache<string, double> _cache;
    private readonly Action<string>? _logger;
    private int _cacheHits;
    private int _cacheMisses;

    public CachingCalculatorDecorator(int cacheCapacity = 100, Action<string>? logger = null)
    {
        _cache = new LRUCache<string, double>(cacheCapacity);
        _logger = logger;
        _cacheHits = 0;
        _cacheMisses = 0;
    }

    public bool TryGetCachedResult(string expression, out double result)
    {
        // 공백 제거하여 정규화
        string normalizedExpression = NormalizeExpression(expression);

        if (_cache.TryGetValue(normalizedExpression, out result))
        {
            _cacheHits++;
            _logger?.Invoke($"[CACHE HIT] {expression} = {result}");
            return true;
        }

        _cacheMisses++;
        result = 0;
        return false;
    }

    public void CacheResult(string expression, double result)
    {
        string normalizedExpression = NormalizeExpression(expression);
        _cache.Put(normalizedExpression, result);
        _logger?.Invoke($"[CACHE STORE] {expression} = {result}");
    }

    private string NormalizeExpression(string expression)
    {
        return expression.Replace(" ", "").ToLower();
    }


    public void ClearCache()
    {
        _cache.Clear();
        _cacheHits = 0;
        _cacheMisses = 0;
        _logger?.Invoke("[CACHE] Cache cleared");
    }

    public int CacheHits => _cacheHits;

    public int CacheMisses => _cacheMisses;

    public double HitRate
    {
        get
        {
            int total = _cacheHits + _cacheMisses;
            return total > 0 ? (double)_cacheHits / total * 100 : 0;
        }
    }

    public int CacheSize => _cache.Count;

    public int CacheCapacity => _cache.Capacity;

    public string GetStatistics()
    {
        return $"Cache Statistics: Hits={_cacheHits}, Misses={_cacheMisses}, " +
               $"Hit Rate={HitRate:F2}%, Size={CacheSize}/{CacheCapacity}";
    }
}
