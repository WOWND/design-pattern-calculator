using DesignPatternCalculator.Core.Interfaces;
using DesignPatternCalculator.Operations.Factory;

namespace DesignPatternCalculator.Core.Engine;



/// 계산기 엔진 인스턴스를 하나만 유지
public sealed class CalculatorEngine
{
    private static readonly Lazy<CalculatorEngine> _instance = new Lazy<CalculatorEngine>(() => new CalculatorEngine());

    private readonly OperationFactory _operationFactory;
    private List<string> _history;

    private CalculatorEngine()
    {
        _operationFactory = new OperationFactory();
        _history = new List<string>();
    }

    public static CalculatorEngine Instance => _instance.Value;

    public IOperationFactory OperationFactory => _operationFactory;


    public double Calculate(IExpression expression)
    {
        if (expression == null)
        {
            throw new ArgumentNullException(nameof(expression));
        }

        string expressionString = expression.ToString();
        double result = expression.Evaluate();

        // 계산 이력 저장
        _history.Add($"{expressionString} = {result}");

        return result;
    }

    
    public IReadOnlyList<string> GetHistory()
    {
        return _history.AsReadOnly();
    }

    public void ClearHistory()
    {
        _history.Clear();
    }

    public void AddToHistory(string entry)
    {
        _history.Add(entry);
    }
    public void SetLogging(bool enabled)
    {
        _operationFactory.EnableLogging = enabled;
    }
    
    public bool IsLoggingEnabled()
    {
        return _operationFactory.EnableLogging;
    }

}
