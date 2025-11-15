using DesignPatternCalculator.Core.Interfaces;
using DesignPatternCalculator.Expressions.Nodes;

namespace DesignPatternCalculator.Expressions.Builder;


/// Builder 구현
/// 복잡한 수식을 단계적으로 구성
/// 연산자 우선순위를 고려한 후위 표기법 방식으로 처리
public class ExpressionBuilder : IExpressionBuilder
{
    private readonly IOperationFactory _operationFactory;
    private readonly Stack<IExpression> _operandStack;
    private readonly Stack<string> _operatorStack;
    private readonly List<string> _expressionTokens;
    private readonly Dictionary<string, int> _precedence;

    public ExpressionBuilder(IOperationFactory operationFactory)
    {
        _operationFactory = operationFactory ?? throw new ArgumentNullException(nameof(operationFactory));
        _operandStack = new Stack<IExpression>();
        _operatorStack = new Stack<string>();
        _expressionTokens = new List<string>();

        // 연산자 우선순위
        _precedence = new Dictionary<string, int>
        {
            { "+", 1 },
            { "-", 1 },
            { "*", 2 },
            { "/", 2 },
            { "%", 2 },
            { "^", 3 }
        };
    }

    public IExpressionBuilder AddNumber(double number)
    {
        _operandStack.Push(new NumberExpression(number));
        _expressionTokens.Add(number.ToString());
        return this;
    }

    public IExpressionBuilder AddOperator(string operatorSymbol)
    {
        if (string.IsNullOrWhiteSpace(operatorSymbol))
        {
            throw new ArgumentException("연산자는 null이거나 빈 문자열일 수 없습니다.", nameof(operatorSymbol));
        }

        // 연산자 우선순위에 따라 처리
        while (_operatorStack.Count > 0 &&
               ShouldPopOperator(operatorSymbol))
        {
            ProcessOperator();
        }

        _operatorStack.Push(operatorSymbol);
        _expressionTokens.Add(operatorSymbol);
        return this;
    }

    public IExpressionBuilder Reset()
    {
        _operandStack.Clear();
        _operatorStack.Clear();
        _expressionTokens.Clear();
        return this;
    }

    public IExpression Build()
    {
        // 남은 연산자 모두 처리
        while (_operatorStack.Count > 0)
        {
            ProcessOperator();
        }

        if (_operandStack.Count != 1)
        {
            throw new InvalidOperationException("잘못된 수식입니다. 수식을 확인해주세요.");
        }

        IExpression result = _operandStack.Pop();
        Reset();
        return result;
    }

    public string GetCurrentExpression()
    {
        return string.Join(" ", _expressionTokens);
    }

    private bool ShouldPopOperator(string currentOperator)
    {
        if (_operatorStack.Count == 0)
            return false;

        string topOperator = _operatorStack.Peek();

        if (!_precedence.ContainsKey(currentOperator) || !_precedence.ContainsKey(topOperator))
            return false;

        // 우선순위가 같거나 낮으면 스택의 연산자를 먼저 처리
        return _precedence[topOperator] >= _precedence[currentOperator];
    }

    private void ProcessOperator()
    {
        if (_operatorStack.Count == 0)
            throw new InvalidOperationException("연산자 스택이 비어있습니다.");

        if (_operandStack.Count < 2)
            throw new InvalidOperationException("피연산자가 부족합니다.");

        string operatorSymbol = _operatorStack.Pop();
        IExpression right = _operandStack.Pop();
        IExpression left = _operandStack.Pop();

        IOperation operation = _operationFactory.CreateOperation(operatorSymbol);
        IExpression result = new BinaryOperationExpression(left, right, operation);

        _operandStack.Push(result);
    }
}
