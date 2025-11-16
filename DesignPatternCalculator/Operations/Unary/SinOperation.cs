using DesignPatternCalculator.Core.Interfaces;

namespace DesignPatternCalculator.Operations.Unary;

public class SinOperation : IUnaryOperation
{
    public string Name => "sin";

    public double Execute(double operand)
    {
        // 라디안 변환
        double radians = operand * Math.PI / 180.0;
        return Math.Sin(radians);
    }
}
