using DesignPatternCalculator.Core.Interfaces;

namespace DesignPatternCalculator.Expressions.Nodes;

/// Composite 패턴의 Leaf
/// 수식 트리의 단말 노드 (숫자)
public class NumberExpression : IExpression
{
    private readonly double _value;

    public NumberExpression(double value)
    {
        _value = value;
    }

    public double Evaluate()
    {
        return _value;
    }

    public override string ToString()
    {
        return _value.ToString();
    }
}
