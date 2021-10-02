using System;

namespace GreetAndCalculate
{
    class GetUserInput : IGetUserInput
    {
        public string GetInput()
        {
            return Console.ReadLine();
        }
    }
}
