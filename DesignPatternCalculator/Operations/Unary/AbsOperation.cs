using DesignPatternCalculator.Core.Interfaces;

namespace DesignPatternCalculator.Operations.Unary;

public class AbsOperation : IUnaryOperation
{
    public string Name => "abs";

    public double Execute(double operand)
    {
        return Math.Abs(operand);
    }
}
