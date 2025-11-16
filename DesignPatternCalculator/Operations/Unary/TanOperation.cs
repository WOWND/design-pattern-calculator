using DesignPatternCalculator.Core.Interfaces;

namespace DesignPatternCalculator.Operations.Unary;

public class TanOperation : IUnaryOperation
{
    public string Name => "tan";

    public double Execute(double operand)
    {
        // 라디안 변환
        double radians = operand * Math.PI / 180.0;
        return Math.Tan(radians);
    }
}
