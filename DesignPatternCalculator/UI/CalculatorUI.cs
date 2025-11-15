using DesignPatternCalculator.Calculator.Facade;

namespace DesignPatternCalculator.UI;


/// 계산기 콘솔 UI
/// Facade 패턴을 사용하여 사용자 인터페이스 제공

public class CalculatorUI
{
    private readonly CalculatorFacade _calculator;
    private bool _isRunning;

    public CalculatorUI()
    {
        _calculator = new CalculatorFacade();
        _isRunning = true;
    }

    public void Run()
    {
        DisplayWelcomeMessage();

        while (_isRunning)
        {
            try
            {
                DisplayMenu();
                ProcessUserInput();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"\n오류: {ex.Message}");
            }
        }

        DisplayGoodbyeMessage();
    }

    private void DisplayWelcomeMessage()
    {
        Console.WriteLine("════════════════════════════════════════════════════════════");
        Console.WriteLine("                      디자인 패턴 계산기                        ");
    }

    private void DisplayMenu()
    {
        Console.WriteLine("════════════════════════════════════════════════════════════");
        Console.WriteLine("메뉴:");
        Console.WriteLine("  1. 수식 계산 (예: 10 + 20 * 3)");
        Console.WriteLine("  2. 대화형 계산");
        Console.WriteLine("  3. 계산 이력 보기");
        Console.WriteLine("  4. 이력 초기화");
        Console.WriteLine("  5. 지원 연산자 보기");
        Console.WriteLine($"  6. 로깅 {(_calculator.IsLoggingEnabled() ? "비활성화" : "활성화")} (현재: {(_calculator.IsLoggingEnabled() ? "ON" : "OFF")})");
        Console.WriteLine("  0. 종료");
        Console.WriteLine("════════════════════════════════════════════════════════════");
        Console.Write("\n선택: ");
    }

    private void ProcessUserInput()
    {
        string input = Console.ReadLine()?.Trim() ?? string.Empty;

        switch (input)
        {
            case "1":
                CalculateExpression();
                break;
            case "2":
                InteractiveCalculation();
                break;
            case "3":
                ShowHistory();
                break;
            case "4":
                ClearHistory();
                break;
            case "5":
                ShowSupportedOperators();
                break;
            case "6":
                ToggleLogging();
                break;
            case "0":
                _isRunning = false;
                break;
            default:
                Console.WriteLine("잘못된 선택입니다. 다시 시도해주세요.");
                break;
        }
    }

    private void CalculateExpression()
    {
        Console.WriteLine("\n[수식 계산 모드]");
        Console.WriteLine("수식을 입력하세요 (공백으로 구분, 예: 10 + 20 * 3):");
        Console.Write("> ");

        string expression = Console.ReadLine()?.Trim() ?? string.Empty;

        if (string.IsNullOrWhiteSpace(expression))
        {
            Console.WriteLine("수식이 입력되지 않았습니다.");
            return;
        }

        try
        {
            Console.WriteLine($"\n입력된 수식: {expression}");

            double result = _calculator.CalculateExpression(expression);

            Console.WriteLine($"결과: {result}");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"계산 실패: {ex.Message}");
        }
    }

    private void InteractiveCalculation()
    {
        Console.WriteLine("\n[대화형 계산 모드]");
        Console.WriteLine("숫자와 연산자를 번갈아 입력하세요.");
        Console.WriteLine("계산을 완료하려면 '=' 취소하려면 'c'를 입력하세요.");
        Console.WriteLine();

        _calculator.Clear();

        while (true)
        {
            Console.Write($"현재 수식: {_calculator.GetCurrentExpression()}\n> ");
            string input = Console.ReadLine()?.Trim() ?? string.Empty;

            if (string.IsNullOrWhiteSpace(input))
                continue;

            if (input.ToLower() == "c")
            {
                _calculator.Clear();
                Console.WriteLine("입력이 취소되었습니다.");
                break;
            }

            if (input == "=")
            {
                try
                {
                    double result = _calculator.Calculate();
                    Console.WriteLine($"\n결과: {result}");
                    break;
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"계산 실패: {ex.Message}");
                    break;
                }
            }

            try
            {
                if (double.TryParse(input, out double number))
                {
                    _calculator.EnterNumber(number);
                }
                else
                {
                    _calculator.EnterOperator(input);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"입력 오류: {ex.Message}");
            }
        }
    }

    private void ShowHistory()
    {
        Console.WriteLine("\n[계산 이력]");
        var history = _calculator.GetHistory();

        if (history.Count == 0)
        {
            Console.WriteLine("계산 이력이 없습니다.");
            return;
        }

        for (int i = 0; i < history.Count; i++)
        {
            Console.WriteLine($"{i + 1}. {history[i]}");
        }
    }

    private void ClearHistory()
    {
        _calculator.ClearHistory();
        Console.WriteLine("\n계산 이력이 초기화되었습니다.");
    }

    private void ShowSupportedOperators()
    {
        Console.WriteLine("\n[지원하는 연산자]");
        IEnumerable<string> operators = _calculator.GetSupportedOperators();
        foreach (string o in operators) {
            Console.WriteLine(o);
        }
    }

    private void ToggleLogging()
    {
        bool currentState = _calculator.IsLoggingEnabled();
        _calculator.SetLogging(!currentState);

        Console.WriteLine();
        Console.WriteLine($"[Decorator 패턴] 로깅 기능이 {(!currentState ? "활성화" : "비활성화")}되었습니다.");

        if (!currentState)
        {
            Console.WriteLine("이제 모든 연산 과정이 로그로 출력됩니다.");
        }
    }

    

    private void DisplayGoodbyeMessage()
    {
        Console.WriteLine();
        Console.WriteLine("계산기를 종료합니다.");
    }
}
