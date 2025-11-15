using DesignPatternCalculator.Core.Interfaces;

namespace DesignPatternCalculator.Operations.Basic;

public class DivideOperation : IOperation
{
    public string Symbol => "/";

    public double Execute(double left, double right)
    {
        if (right == 0)
        {
            throw new DivideByZeroException("0으로 나눌 수 없습니다.");
        }
        return left / right;
    }
}
