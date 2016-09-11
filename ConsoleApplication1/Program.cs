using System;
using PolishNotation;

namespace ConsoleApplication1
{
    class Program
    {
        static void Main(string[] args)
        {
            var f = Expression.ToDelegate("x");

            for(double i = 0; i < 10; i++)
            {
                Console.Write(f(i));
                Console.Write(" ");
            }

            Console.ReadLine();
        }
    }
}
