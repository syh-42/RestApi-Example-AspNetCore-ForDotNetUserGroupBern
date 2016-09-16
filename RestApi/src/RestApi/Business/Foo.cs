namespace RestApi.Business
{
    public class Foo : IFoo
    {
        public bool DoSomething(string input)
        {
            return input.Equals("ping");
        }
    }
}
