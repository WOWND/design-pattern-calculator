using DesignPatternCalculator.Core.Interfaces;
using DesignPatternCalculator.Operations.Basic;
using DesignPatternCalculator.Operations.Decorators;

namespace DesignPatternCalculator.Operations.Factory;


/// Factory 패턴 구현
/// 연산자 기호에 따라 연산 객체 생성
public class OperationFactory : IOperationFactory
{
    private readonly Dictionary<string, Func<IOperation>> _operationCreators;
    
    public bool EnableLogging { get; set; }

    public OperationFactory()
    {
        _operationCreators = new Dictionary<string, Func<IOperation>>
        {
            { "+", () => new AddOperation() },
            { "-", () => new SubtractOperation() },
            { "*", () => new MultiplyOperation() },
            { "/", () => new DivideOperation() },
            { "^", () => new PowerOperation() },
            { "%", () => new ModuloOperation() }
        };

        EnableLogging = false;
    }

    public IOperation CreateOperation(string operatorSymbol)
    {
        if (string.IsNullOrWhiteSpace(operatorSymbol))
        {
            throw new ArgumentException("연산자 기호는 null이거나 빈 문자열일 수 없습니다.", nameof(operatorSymbol));
        }

        if (!_operationCreators.ContainsKey(operatorSymbol))
        {
            throw new ArgumentException($"지원하지 않는 연산자입니다: {operatorSymbol}", nameof(operatorSymbol));
        }

        IOperation operation = _operationCreators[operatorSymbol]();


        if (EnableLogging)
        {
            operation = new LoggingOperationDecorator(operation);
        }

        return operation;
    }

    public IEnumerable<string> GetSupportedOperators()
    {
        return _operationCreators.Keys;
    }
}
