namespace RestApi.Business
{
    public class Calculator : ICalculator
    {
        private readonly IFoo _foo;
        public Calculator(IFoo foo)
        {
            _foo = foo;
        }

        public int Sum(int x, int y)
        {
            return x + y;
        }

        public int CalculateBecauseOfSomething(int x, int y, string input)
        {
            return _foo.DoSomething(input) ? x - y : y - x;
        }
    }
}