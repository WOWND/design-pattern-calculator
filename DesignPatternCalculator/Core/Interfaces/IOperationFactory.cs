namespace DesignPatternCalculator.Core.Interfaces;


/// Factory 패턴 인터페이스 (연산 객체 생성 팩토리)
public interface IOperationFactory
{
    
    /// 연산 기호에 해당하는 연산 객체 생성
    IOperation CreateOperation(string operatorSymbol);

    
    /// 지원하는 연산자 목록 반환
    IEnumerable<string> GetSupportedOperators();
}
