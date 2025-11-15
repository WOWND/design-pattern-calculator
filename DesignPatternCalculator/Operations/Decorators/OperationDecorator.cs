using DesignPatternCalculator.Core.Interfaces;

namespace DesignPatternCalculator.Operations.Decorators;


/// Decorator 패턴 추상 Decorator
/// 연산에 기능을 추가
public abstract class OperationDecorator : IOperation
{
    protected readonly IOperation _wrappedOperation;

    protected OperationDecorator(IOperation operation)
    {
        _wrappedOperation = operation ?? throw new ArgumentNullException(nameof(operation));
    }

    public virtual string Symbol => _wrappedOperation.Symbol;

    public abstract double Execute(double left, double right);
}
