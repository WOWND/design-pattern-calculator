using DesignPatternCalculator.Core.Interfaces;

namespace DesignPatternCalculator.Operations.Unary;

public class LogOperation : IUnaryOperation
{
    public string Name => "log";

    public double Execute(double operand)
    {
        if (operand <= 0)
            throw new ArgumentException("0 이하의 로그는 계산할 수 없습니다");
        return Math.Log10(operand);
    }
}
