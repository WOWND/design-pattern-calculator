using DesignPatternCalculator.Core.Interfaces;

namespace DesignPatternCalculator.Operations.Unary;

public class SqrtOperation : IUnaryOperation
{
    public string Name => "sqrt";

    public double Execute(double operand)
    {
        if (operand < 0)
            throw new ArgumentException("음수의 제곱근은 계산할 수 없습니다");
        return Math.Sqrt(operand);
    }
}
