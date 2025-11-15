using DesignPatternCalculator.UI;

namespace DesignPatternCalculator
{
    class Program
    {
        static void Main(string[] args)
        {
            // 계산기 실행
            CalculatorUI calculator = new CalculatorUI();
            calculator.Run();
        }
    }
}