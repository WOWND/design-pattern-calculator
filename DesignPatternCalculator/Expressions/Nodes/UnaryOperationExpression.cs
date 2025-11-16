using DesignPatternCalculator.Core.Interfaces;

namespace DesignPatternCalculator.Expressions.Nodes;

/// 단항 연산 표현식 (Composite 패턴의 Composite)
public class UnaryOperationExpression : IExpression
{
    private readonly IExpression _operand;
    private readonly IUnaryOperation _operation;

    public UnaryOperationExpression(IExpression operand, IUnaryOperation operation)
    {
        _operand = operand ?? throw new ArgumentNullException(nameof(operand));
        _operation = operation ?? throw new ArgumentNullException(nameof(operation));
    }

    public double Evaluate()
    {
        double value = _operand.Evaluate();
        return _operation.Execute(value);
    }

    public override string ToString()
    {
        return $"{_operation.Name}({_operand})";
    }
}
