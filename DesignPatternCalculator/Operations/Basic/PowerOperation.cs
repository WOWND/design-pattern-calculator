using DesignPatternCalculator.Core.Interfaces;

namespace DesignPatternCalculator.Operations.Basic;

public class PowerOperation : IOperation
{
    public string Symbol => "^";

    public double Execute(double left, double right)
    {
        return Math.Pow(left, right);
    }
}
