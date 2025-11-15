namespace DesignPatternCalculator.Core.Interfaces;


/// 연산을 나타내는 인터페이스 (Factory 패턴에서 생성될 인터페이스)
public interface IOperation
{
   
    /// 연산 기호
    string Symbol { get; }

    
    /// 연산 수행
    double Execute(double left, double right);
}
