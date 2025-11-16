using DesignPatternCalculator.Core.Interfaces;

namespace DesignPatternCalculator.Operations.Unary;

public class CosOperation : IUnaryOperation
{
    public string Name => "cos";

    public double Execute(double operand)
    {
        // 라디안변환
        double radians = operand * Math.PI / 180.0;
        return Math.Cos(radians);
    }
}
