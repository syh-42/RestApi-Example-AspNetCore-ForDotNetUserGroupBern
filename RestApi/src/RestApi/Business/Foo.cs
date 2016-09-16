using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

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
