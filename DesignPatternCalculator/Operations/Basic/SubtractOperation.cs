using DesignPatternCalculator.Core.Interfaces;

namespace DesignPatternCalculator.Operations.Basic;

public class SubtractOperation : IOperation
{
    public string Symbol => "-";

    public double Execute(double left, double right)
    {
        return left - right;
    }
}
