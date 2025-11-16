using DesignPatternCalculator.Core.Interfaces;
using DesignPatternCalculator.Expressions.Nodes;
using DesignPatternCalculator.Operations.Factory;

namespace DesignPatternCalculator.Expressions.Builder;


/// Builder 구현
/// 복잡한 수식을 단계적으로 구성
/// 연산자 우선순위를 고려한 후위 표기법 방식으로 처리
public class ExpressionBuilder : IExpressionBuilder
{
    private readonly IOperationFactory _operationFactory;
    private readonly UnaryOperationFactory _unaryOperationFactory;
    private readonly Stack<IExpression> _operandStack;
    private readonly Stack<string> _operatorStack;
    private readonly List<string> _expressionTokens;
    private readonly Dictionary<string, int> _precedence;

    public ExpressionBuilder(IOperationFactory operationFactory)
    {
        _operationFactory = operationFactory ?? throw new ArgumentNullException(nameof(operationFactory));
        _unaryOperationFactory = new UnaryOperationFactory();
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

    public IExpressionBuilder AddOpenParenthesis()
    {
        _operatorStack.Push("(");
        _expressionTokens.Add("(");
        return this;
    }

    public IExpressionBuilder AddCloseParenthesis()
    {
        while (_operatorStack.Count > 0 && _operatorStack.Peek() != "(")
        {
            ProcessOperator();
        }

        if (_operatorStack.Count == 0)
        {
            throw new InvalidOperationException("괄호가 맞지 않습니다. 여는 괄호를 찾을 수 없습니다.");
        }

        _operatorStack.Pop(); // 여는 괄호 제거

        // 함수 호출인 경우 처리
        if (_operatorStack.Count > 0 && _unaryOperationFactory.IsUnaryFunction(_operatorStack.Peek()))
        {
            ProcessUnaryFunction();
        }

        _expressionTokens.Add(")");
        return this;
    }

    public IExpressionBuilder AddFunction(string functionName)
    {
        if (!_unaryOperationFactory.IsUnaryFunction(functionName))
        {
            throw new ArgumentException($"지원하지 않는 함수입니다: {functionName}");
        }

        _operatorStack.Push(functionName);
        _expressionTokens.Add(functionName);
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
            string top = _operatorStack.Peek();
            if (top == "(")
            {
                throw new InvalidOperationException("괄호가 맞지 않습니다. 닫는 괄호가 없습니다.");
            }
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

    public IExpression ParseExpression(string expression)
    {
        Reset();
        var tokens = Tokenize(expression);

        for (int i = 0; i < tokens.Count; i++)
        {
            string token = tokens[i];

            if (token == "(")
            {
                AddOpenParenthesis();
            }
            else if (token == ")")
            {
                AddCloseParenthesis();
            }
            else if (_unaryOperationFactory.IsUnaryFunction(token))
            {
                AddFunction(token);
            }
            else if (_precedence.ContainsKey(token))
            {
                // 연산자 뒤나 수식 시작에서 -가 나오면 단항 마이너스
                if (token == "-" && IsUnaryMinus(i, tokens))
                {
                    // 다음 토큰이 숫자라면 음수로 처리
                    if (i + 1 < tokens.Count && double.TryParse(tokens[i + 1], out double num))
                    {
                        AddNumber(-num);
                        i++; // 숫자 토큰 건너뛰기
                    }
                    else
                    {
                        // -를 (0 - ...) 로 처리
                        AddNumber(0);
                        AddOperator("-");
                    }
                }
                else
                {
                    AddOperator(token);
                }
            }
            else if (double.TryParse(token, out double number))
            {
                AddNumber(number);
            }
            else
            {
                throw new ArgumentException($"알 수 없는 토큰입니다: {token}");
            }
        }

        return Build();
    }

    private List<string> Tokenize(string expression)
    {
        var tokens = new List<string>();
        var currentNumber = "";

        expression = expression.Replace(" ", "");

        for (int i = 0; i < expression.Length; i++)
        {
            char c = expression[i];

            if (char.IsDigit(c) || c == '.')
            {
                currentNumber += c;
            }
            else if (char.IsLetter(c))
            {
                // 현재 숫자가 있으면 먼저 추가
                if (currentNumber.Length > 0)
                {
                    tokens.Add(currentNumber);
                    currentNumber = "";
                }

                // 함수명 추출
                string functionName = "";
                while (i < expression.Length && char.IsLetter(expression[i]))
                {
                    functionName += expression[i];
                    i++;
                }
                i--;

                tokens.Add(functionName.ToLower());
            }
            else
            {
                if (currentNumber.Length > 0)
                {
                    tokens.Add(currentNumber);
                    currentNumber = "";
                }

                tokens.Add(c.ToString());
            }
        }

        if (currentNumber.Length > 0)
        {
            tokens.Add(currentNumber);
        }

        return tokens;
    }

    private bool IsUnaryMinus(int index, List<string> tokens)
    {
        if (index == 0) return true; // 수식 시작

        string prevToken = tokens[index - 1];
        
        return _precedence.ContainsKey(prevToken) || prevToken == "(";
    }

    private bool ShouldPopOperator(string currentOperator)
    {
        if (_operatorStack.Count == 0)
            return false;

        string topOperator = _operatorStack.Peek();

        if (topOperator == "(" || _unaryOperationFactory.IsUnaryFunction(topOperator))
            return false;

        if (!_precedence.ContainsKey(currentOperator) || !_precedence.ContainsKey(topOperator))
            return false;

        if (currentOperator == "^" && topOperator == "^")
            return false;

        return _precedence[topOperator] >= _precedence[currentOperator];
    }

    private void ProcessOperator()
    {
        if (_operatorStack.Count == 0)
            throw new InvalidOperationException("연산자 스택이 비어있습니다.");

        string operatorSymbol = _operatorStack.Peek();

        if (_unaryOperationFactory.IsUnaryFunction(operatorSymbol))
        {
            ProcessUnaryFunction();
        }
        else
        {
            if (_operandStack.Count < 2)
                throw new InvalidOperationException("피연산자가 부족합니다.");

            _operatorStack.Pop();
            IExpression right = _operandStack.Pop();
            IExpression left = _operandStack.Pop();

            IOperation operation = _operationFactory.CreateOperation(operatorSymbol);
            IExpression result = new BinaryOperationExpression(left, right, operation);

            _operandStack.Push(result);
        }
    }

    private void ProcessUnaryFunction()
    {
        if (_operatorStack.Count == 0 || _operandStack.Count < 1)
            throw new InvalidOperationException("단항 함수를 처리할 수 없습니다.");

        string functionName = _operatorStack.Pop();
        IExpression operand = _operandStack.Pop();

        IUnaryOperation operation = _unaryOperationFactory.CreateOperation(functionName);
        IExpression result = new UnaryOperationExpression(operand, operation);

        _operandStack.Push(result);
    }

    public IEnumerable<string> GetSupportedFunctions()
    {
        return _unaryOperationFactory.GetSupportedFunctions();
    }
}
