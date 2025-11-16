namespace DesignPatternCalculator.Core.Interfaces;


/// 단항 연산 인터페이스
public interface IUnaryOperation
{

    string Name { get; }

    double Execute(double operand);
}
