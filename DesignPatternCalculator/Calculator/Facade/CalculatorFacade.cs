using DesignPatternCalculator.Expressions.Builder;
using DesignPatternCalculator.Core.Engine;
using DesignPatternCalculator.Core.Interfaces;
using DesignPatternCalculator.Memento;
using DesignPatternCalculator.Operations.Decorators;

namespace DesignPatternCalculator.Calculator.Facade;

/// 복잡한 계산기를 간단한 인터페이스로 제공
public class CalculatorFacade
{
    private readonly CalculatorEngine _engine;
    private readonly ExpressionBuilder _builder;
    private readonly CalculatorCaretaker _caretaker;
    private readonly CachingCalculatorDecorator _cacheDecorator;
    private string _currentExpression;
    private double _lastResult;
    private bool _cachingEnabled;

    public CalculatorFacade()
    {
        _engine = CalculatorEngine.Instance;
        _builder = new ExpressionBuilder(_engine.OperationFactory);
        _caretaker = new CalculatorCaretaker();
        _cacheDecorator = new CachingCalculatorDecorator(100, message =>
        {
            if (IsLoggingEnabled())
                Console.WriteLine(message);
        });
        _currentExpression = string.Empty;
        _lastResult = 0;
        _cachingEnabled = true; // 기본적으로 캐싱 활성화
    }

    public void EnterNumber(double number)
    {
        _builder.AddNumber(number);
        UpdateCurrentExpression();
    }

    public void EnterOperator(string operatorSymbol)
    {
        _builder.AddOperator(operatorSymbol);
        UpdateCurrentExpression();
    }


    public void EnterOpenParenthesis()
    {
        _builder.AddOpenParenthesis();
        UpdateCurrentExpression();
    }

    public void EnterCloseParenthesis()
    {
        _builder.AddCloseParenthesis();
        UpdateCurrentExpression();
    }

    public void EnterFunction(string functionName)
    {
        _builder.AddFunction(functionName);
        UpdateCurrentExpression();
    }

    public double Calculate()
    {
        try
        {
            string expressionString = _currentExpression;
            IExpression expression = _builder.Build();
            double result = _engine.Calculate(expression);

            // 메멘토 저장
            _caretaker.SaveState(new CalculatorMemento(expressionString, result));
            _lastResult = result;

            Clear();
            return result;
        }
        catch (Exception ex)
        {
            Clear();
            throw new InvalidOperationException($"계산 중 오류가 발생했습니다: {ex.Message}", ex);
        }
    }

    public double CalculateExpression(string expression)
    {
        if (string.IsNullOrWhiteSpace(expression))
        {
            throw new ArgumentException("수식이 비어있습니다.", nameof(expression));
        }

        Clear();
        _currentExpression = expression;

        try
        {
            double result;

            // 캐시 확인
            if (_cachingEnabled && _cacheDecorator.TryGetCachedResult(expression, out result))
            {
                // 캐시에서 가져온 경우에도 히스토리와 메멘토에 기록
                _engine.AddToHistory($"{expression} = {result} (cached)");
                _caretaker.SaveState(new CalculatorMemento(expression, result));
                _lastResult = result;
                Clear();
                return result;
            }

            // 캐시에 없으면 계산
            IExpression expr = _builder.ParseExpression(expression);
            result = expr.Evaluate();

            // 캐시에 저장
            if (_cachingEnabled)
            {
                _cacheDecorator.CacheResult(expression, result);
            }

            // 히스토리에 추가
            _engine.AddToHistory($"{expression} = {result}");

            // 메멘토 저장
            _caretaker.SaveState(new CalculatorMemento(expression, result));
            _lastResult = result;

            Clear();
            return result;
        }
        catch (Exception ex)
        {
            Clear();
            throw new InvalidOperationException($"계산 중 오류가 발생했습니다: {ex.Message}", ex);
        }
    }

    public string GetCurrentExpression()
    {
        return _currentExpression;
    }

    public void Clear()
    {
        _builder.Reset();
        _currentExpression = string.Empty;
    }

    public IReadOnlyList<string> GetHistory()
    {
        return _engine.GetHistory();
    }

    public void ClearHistory()
    {
        _engine.ClearHistory();
        _caretaker.Clear();
    }

    public IEnumerable<string> GetSupportedOperators()
    {
        return _engine.OperationFactory.GetSupportedOperators();
    }

    public IEnumerable<string> GetSupportedFunctions()
    {
        return _builder.GetSupportedFunctions();
    }

    public void SetLogging(bool enabled)
    {
        _engine.SetLogging(enabled);
    }

    public bool IsLoggingEnabled()
    {
        return _engine.IsLoggingEnabled();
    }

    public CalculatorMemento? Undo()
    {
        var memento = _caretaker.Undo();
        if (memento != null)
        {
            _lastResult = memento.Result;
        }
        return memento;
    }

    public CalculatorMemento? Redo()
    {
        var memento = _caretaker.Redo();
        if (memento != null)
        {
            _lastResult = memento.Result;
        }
        return memento;
    }


    public bool CanUndo => _caretaker.CanUndo;

    public bool CanRedo => _caretaker.CanRedo;

    public CalculatorMemento? GetCurrentState()
    {
        return _caretaker.GetCurrentState();
    }

    public double LastResult => _lastResult;

    public IEnumerable<CalculatorMemento> GetMementoHistory()
    {
        return _caretaker.GetHistory();
    }

    public void SetCaching(bool enabled)
    {
        _cachingEnabled = enabled;
    }

    public bool IsCachingEnabled => _cachingEnabled;

    public void ClearCache()
    {
        _cacheDecorator.ClearCache();
    }

    public string GetCacheStatistics()
    {
        return _cacheDecorator.GetStatistics();
    }

    public int CacheHits => _cacheDecorator.CacheHits;

    public int CacheMisses => _cacheDecorator.CacheMisses;

    public double CacheHitRate => _cacheDecorator.HitRate;

    private void UpdateCurrentExpression()
    {
        _currentExpression = _builder.GetCurrentExpression();
    }
}
