using DesignPatternCalculator.Expressions.Builder;
using DesignPatternCalculator.Core.Engine;
using DesignPatternCalculator.Core.Interfaces;

namespace DesignPatternCalculator.Calculator.Facade;

/// 복잡한 계산기를 간단한 인터페이스로 제공

public class CalculatorFacade
{
    private readonly CalculatorEngine _engine;
    private readonly IExpressionBuilder _builder;
    private string _currentExpression;

    public CalculatorFacade()
    {
        _engine = CalculatorEngine.Instance;
        _builder = new ExpressionBuilder(_engine.OperationFactory);
        _currentExpression = string.Empty;
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

    
    public double Calculate()
    {
        try
        {
            IExpression expression = _builder.Build();
            double result = _engine.Calculate(expression);
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

        string[] tokens = expression.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);

        foreach (string token in tokens)
        {
            if (double.TryParse(token, out double number))
            {
                EnterNumber(number);
            }
            else
            {
                EnterOperator(token);
            }
        }

        return Calculate();
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
    }


    
    public IEnumerable<string> GetSupportedOperators()
    {
        return _engine.OperationFactory.GetSupportedOperators();
    }
    
    public void SetLogging(bool enabled)
    {
        _engine.SetLogging(enabled);
    }
    
    public bool IsLoggingEnabled()
    {
        return _engine.IsLoggingEnabled();
    }

    private void UpdateCurrentExpression()
    {
        _currentExpression = _builder.GetCurrentExpression();
    }
}
