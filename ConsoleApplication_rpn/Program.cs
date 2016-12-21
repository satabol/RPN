using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PolishNotation;

namespace ConsoleApplication_rpn {
    class Program {
        static void Main( string[] args ) {
            //Expression rpn = Expression.ToPolishNotation(Expression.ToExpresion("log(12)+123+rotate(1,2,3,9-1)"));
            //Expression rpn = Expression.ToPolishNotation(Expression.ToExpresion("rotate(rnd(),1,2,3)+log(12, rnd(4,5,6) )"));
            //Expression rpn = Expression.ToPolishNotation(Expression.ToExpresion("rotate(rnd(),2,3,4)+rotate(11)+log(10,11,12)+sin(0.5)"));
            string str_formula = "-(((count_arguments(1,2,3, rnd(4) )))-log(10,10))";
            //str_formula = "((rnd(1,2,3,4)))";
            str_formula = "-(-5)";
            try {
                Expression rpn = Expression.ToPolishNotation(Expression.ToExpresion(str_formula));
                Console.WriteLine("rpn: " + rpn.ToString());
                Console.WriteLine("Результат: " + Expression.Calculate(rpn));
            }catch(Exception _ex) {
                Console.WriteLine(str_formula);
                Console.WriteLine(_ex.Message);
            }
            Console.ReadKey();
        }
    }
}
