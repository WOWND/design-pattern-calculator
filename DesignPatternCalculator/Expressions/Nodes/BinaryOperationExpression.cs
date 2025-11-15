using DesignPatternCalculator.Core.Interfaces;

namespace DesignPatternCalculator.Expressions.Nodes;


/// Composite 패턴의 Composite
/// 수식 트리의 내부 노드 (연산)
public class BinaryOperationExpression : IExpression
{
    private readonly IExpression _left;
    private readonly IExpression _right;
    private readonly IOperation _operation;

    public BinaryOperationExpression(IExpression left, IExpression right, IOperation operation)
    {
        _left = left ?? throw new ArgumentNullException(nameof(left));
        _right = right ?? throw new ArgumentNullException(nameof(right));
        _operation = operation ?? throw new ArgumentNullException(nameof(operation));
    }

    public double Evaluate()
    {
        double leftValue = _left.Evaluate();
        double rightValue = _right.Evaluate();
        return _operation.Execute(leftValue, rightValue);
    }

    public override string ToString()
    {
        return $"({_left} {_operation.Symbol} {_right})";
    }
}
