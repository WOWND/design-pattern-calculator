namespace DesignPatternCalculator.Core.Interfaces;


/// Composite 패턴의 Component 인터페이스
public interface IExpression
{
    double Evaluate();
    
    string ToString();
}
