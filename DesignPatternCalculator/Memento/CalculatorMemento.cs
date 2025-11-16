namespace DesignPatternCalculator.Memento;


/// 계산기 상태를 저장하는 메멘토
public class CalculatorMemento
{
    public string Expression { get; }
    public double Result { get; }
    public DateTime Timestamp { get; }

    public CalculatorMemento(string expression, double result)
    {
        Expression = expression;
        Result = result;
        Timestamp = DateTime.Now;
    }

    public override string ToString()
    {
        return $"{Expression} = {Result}";
    }
}
