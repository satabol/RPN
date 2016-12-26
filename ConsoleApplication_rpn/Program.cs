using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PolishNotation;
using System.Windows.Forms;
using System.Globalization;

namespace ConsoleApplication_rpn {
    class Program {
        [STAThread]
        static void Main( string[] args ) {
            //Expression rpn = Expression.ToPolishNotation(Expression.ToExpresion("log(12)+123+rotate(1,2,3,9-1)"));
            //Expression rpn = Expression.ToPolishNotation(Expression.ToExpresion("rotate(rnd(),1,2,3)+log(12, rnd(4,5,6) )"));
            //Expression rpn = Expression.ToPolishNotation(Expression.ToExpresion("rotate(rnd(),2,3,4)+rotate(11)+log(10,11,12)+sin(0.5)"));
            string str_formula = "-(((count_arguments(1,2,3, rnd(4) )))-log(10,10))";
            Expression rpn = null;
            //str_formula = "((rnd(1,2,3,4)))";
            str_formula = "1+5*1*(2^2)"; // 21
            str_formula = "5%2+(2*2+1)^2/5"; // 6
            str_formula = "sum(1,2+sum(3,4))+sqrt(4)"; // 12
            str_formula = "sum(1,2+sum(3, list_length({1,2,(3)}) ))+sqrt(4)"; // 11
            str_formula = "((list_length({1,2,(3), ((sum(1,2)))})))+1+(1)"; // 6
            str_formula = "1+list_length({1,2,3})"; //4
            str_formula = "(((1)))+2"; // 3
            str_formula = "((( (((1)))+2 )*3))"; // 9
            try {
                //Console.WriteLine("Формула: " + str_formula);
                //Expression rpn = Expression.ToPolishNotation(Expression.ToExpresion(str_formula));
                //Console.WriteLine("rpn: " + rpn.ToString());
                //Console.WriteLine("Результат: " + Expression.Calculate(rpn));

                //str_formula = "5+2*3+0";
                //Console.WriteLine("Формула: " + str_formula);
                //rpn = Expression.ToPolishNotation(Expression.ToExpresion(str_formula));
                //Console.WriteLine("rpn: " + rpn.ToString());

                Console.WriteLine("\n\n Расчёт по формуле:");
                // Проверка формулы как функции:
                str_formula = "-(-x)*3-x+pi";
                str_formula = "-(-(-x))*3-x+pi";
                str_formula = "{(-x+1+sin(-x+1-0*2+(-4))*0),-(-(-x{))}";
                //str_formula = "(1)+2+sin(pi)";
                //str_formula = "{sin(x),log(x,x)}";
                //str_formula = "-(-x)*3-x";
                //str_formula = "{{1},2+3,{3-4,-4},{{5},6-7-1}}";

                //Console.WriteLine("=======");
                //IList<string> te = TokenizeExpression(str_formula);
                //for(int i=0; i<=te.Count-1; i++) {
                //    Console.WriteLine("" + te.ElementAt(i));
                //}

                //Console.WriteLine("=======");

                rpn = Expression.ToPolishNotation(Expression.ToExpresion(str_formula));
                Console.WriteLine("Формула: " + str_formula);
                Console.WriteLine("rpn: " + rpn.ToString());
                Func<ElementaryUnit, ElementaryUnit> func = Expression.ToDelegate(str_formula);
                ElementaryUnit res = func( new ElementaryUnit(PolishNotation.Emums.ElementaryUnitType.Digit, 10) );
                Console.WriteLine("результат: " + res.ToString() );
            } catch(Exception _ex) {
                Console.WriteLine(str_formula);
                string ex_info = Utilities.getExceptionInfo(_ex);
                //Clipboard.SetText(ex_info);
                Console.WriteLine(ex_info); // _ex.Message);

            }
            Console.ReadKey();
        }

        public static IList<string> TokenizeExpression( string expr ) {
            // TODO Add all your delimiters here
            var delimiters = new[] { '{', '}', '(', ')', '+', '-', '*', '/', '^', '%', ',' };
            var buffer = string.Empty;
            var ret = new List<string>();
            expr = expr.Replace(" ", "");
            foreach (var c in expr) {
                if (delimiters.Contains(c)) {
                    if (buffer.Length > 0) ret.Add(buffer);
                    ret.Add(c.ToString(CultureInfo.InvariantCulture));
                    buffer = string.Empty;
                } else {
                    buffer += c;
                }
            }
            return ret;
        }
    }
}
