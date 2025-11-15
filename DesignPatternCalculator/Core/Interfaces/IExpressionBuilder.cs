namespace DesignPatternCalculator.Core.Interfaces;


/// 수식을 단계적으로 구성하는 빌더
public interface IExpressionBuilder
{
    IExpressionBuilder AddNumber(double number);
   IExpressionBuilder AddOperator(string operatorSymbol);

    IExpressionBuilder Reset();

    IExpression Build();

    string GetCurrentExpression();
}
