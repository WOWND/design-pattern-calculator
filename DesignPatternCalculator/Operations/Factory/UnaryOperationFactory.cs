using DesignPatternCalculator.Core.Interfaces;
using DesignPatternCalculator.Operations.Unary;

namespace DesignPatternCalculator.Operations.Factory;


/// 단항 연산 팩토리
public class UnaryOperationFactory
{
    private readonly Dictionary<string, Func<IUnaryOperation>> _operationCreators;

    public UnaryOperationFactory()
    {
        _operationCreators = new Dictionary<string, Func<IUnaryOperation>>(StringComparer.OrdinalIgnoreCase)
        {
            { "sin", () => new SinOperation() },
            { "cos", () => new CosOperation() },
            { "tan", () => new TanOperation() },
            { "sqrt", () => new SqrtOperation() },
            { "log", () => new LogOperation() },
            { "ln", () => new LnOperation() },
            { "abs", () => new AbsOperation() }
        };
    }

    public IUnaryOperation CreateOperation(string functionName)
    {
        if (_operationCreators.TryGetValue(functionName, out var creator))
        {
            return creator();
        }

        throw new ArgumentException($"지원하지 않는 함수입니다: {functionName}");
    }

    public bool IsUnaryFunction(string token)
    {
        return _operationCreators.ContainsKey(token);
    }

    public IEnumerable<string> GetSupportedFunctions()
    {
        return _operationCreators.Keys;
    }
}
