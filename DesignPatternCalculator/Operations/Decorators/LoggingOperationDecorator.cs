using DesignPatternCalculator.Core.Interfaces;

namespace DesignPatternCalculator.Operations.Decorators;


/// Decorator 패턴 구상 클래스
/// 연산 실행 전후에 로깅 기능 추가
public class LoggingOperationDecorator : OperationDecorator
{
    private readonly Action<string> _logger;

    public LoggingOperationDecorator(IOperation operation, Action<string>? logger = null)
        : base(operation)
    {
        _logger = logger ?? Console.WriteLine;
    }

    public override double Execute(double left, double right)
    {
        _logger($"[LOG] 연산 시작: {left} {Symbol} {right}");

        try
        {
            double result = _wrappedOperation.Execute(left, right);
            _logger($"[LOG] 연산 완료: {left} {Symbol} {right} = {result}");
            return result;
        }
        catch (Exception ex)
        {
            _logger($"[ERROR] 연산 실패: {ex.Message}");
            throw;
        }
    }
}
