namespace PolishNotation
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;

    public class Expression:IEnumerable<ElementaryUnit>
    {
        private static readonly char[] binaryOperators = new[] { '+', '-', '*', '/', '^' };
        private static readonly Dictionary<string, Func<double, double>> unaryFunctions = new Dictionary<string, Func<double, double>>
        {
            {"sin", Math.Sin },
            {"cos", Math.Cos },
            {"tg", Math.Tan },
            {"ctg", (number) => Math.Cos(number)/Math.Sin(number) },
            {"sign", (number) => Math.Sign(number) },
            {"sqrt", Math.Sqrt },
            {"abs",Math.Abs },
            {"acos",Math.Acos },
            {"asin",Math.Asin },
            {"atan", Math.Atan},
            {"actg", (number) => 1/Math.Atan(number) },
            {"lg",Math.Log10 },
            {"ln", (number) => Math.Log(number) }
        };

        public static Dictionary<string, double> constans = new Dictionary<string, double>
        {
            {"pi",Math.PI },
            {"e",Math.E }
        };

        private static readonly Dictionary<string, Func<double, double, double>> binaryFunction = new Dictionary<string, Func<double, double, double>>
        {
            {"log", (a,b) => Math.Log(a,b) }
        };

        private static Expression ToExpresion(string expression)
        {
            expression = new string(expression.Where(c => c != ' ').ToArray());

            StringBuilder buf = new StringBuilder(Convert.ToString(expression[0]));

            for (int i = 1; i < expression.Length; i++)
            {
                if (expression[i] == '-' && expression[i - 1] == '(')
                {
                    buf.Append("0");
                }
                buf.Append(expression[i]);
            }

            expression = buf.ToString();

            if (expression[0] == '-') expression = "0" + expression;

            List<ElementaryUnit> result = new List<ElementaryUnit>();

            for (var i = 0; i < expression.Length; i++)
            {
                char c = expression[i];

                if (c == 'x' || c == 'X')
                {
                    result.Add(new ElementaryUnit(Emums.ElementaryUnitType.Variable, Convert.ToString(c)));
                    continue;
                }

                if (binaryOperators.Contains(c))
                {
                    result.Add(new ElementaryUnit(Emums.ElementaryUnitType.BinaryOperation, Convert.ToString(c)));
                    continue;
                }

                if (c == '(' || c == ')')
                {
                    result.Add(new ElementaryUnit(Emums.ElementaryUnitType.Brackets, Convert.ToString(c)));
                    continue;
                }

                if (char.IsLetter(c))
                {
                    string buffer = string.Empty;
                    var j = i;
                    for (; j < expression.Length && char.IsLetter(expression[j]); j++)
                    {
                        buffer += expression[j];
                    }
                    i = j - 1;

                    if (unaryFunctions.Keys.Contains(buffer))
                        result.Add(new ElementaryUnit(Emums.ElementaryUnitType.UnaryFunction, buffer));
                    if (binaryFunction.Keys.Contains(buffer))
                        result.Add(new ElementaryUnit(Emums.ElementaryUnitType.BinaryFunction, buffer));
                    if (constans.Keys.Contains(buffer))
                        result.Add(new ElementaryUnit(Emums.ElementaryUnitType.Constant, buffer));
                    continue;
                }

                if (char.IsDigit(c))
                {
                    string buffer = string.Empty;
                    var j = i;
                    for (; j < expression.Length && (char.IsDigit(expression[j]) || expression[j] == '.'); j++)
                    {
                        buffer += expression[j];
                    }
                    i = j - 1;
                    result.Add(new ElementaryUnit(Emums.ElementaryUnitType.Digit, buffer));
                }

            }

            return new Expression(result);
        }

        private static Expression ToPolishNotation(Expression expression)
        {
            var result = new List<ElementaryUnit>();
            var buffer = new Stack<ElementaryUnit>();

            var firstLO = "+-";
            var secondLO = "*/";

            foreach (var el in expression)
            {
                if (el.Type == Emums.ElementaryUnitType.Digit || el.Type == Emums.ElementaryUnitType.Variable || el.Type == Emums.ElementaryUnitType.Constant)
                {
                    result.Add(el);
                    continue;
                }
                if (el.Type == Emums.ElementaryUnitType.BinaryFunction || el.Type == Emums.ElementaryUnitType.UnaryFunction)
                {
                    buffer.Push(el);
                    continue;
                }

                if (el.Type == Emums.ElementaryUnitType.Brackets)
                {
                    if (el.Value == ")")
                    {
                        while (buffer.Peek().Value != "(")
                        {
                            result.Add(buffer.Pop());
                        }
                        if (el.Type == Emums.ElementaryUnitType.BinaryFunction || el.Type == Emums.ElementaryUnitType.UnaryFunction)
                        {
                            result.Add(buffer.Pop());
                        }
                        buffer.Pop();
                        continue;
                    }

                    buffer.Push(el);
                }

                if (el.Type == Emums.ElementaryUnitType.BinaryOperation)
                {
                    if (el.Value == "^")
                    {
                        while (buffer.Count != 0 && (buffer.Peek().Value == "^" || el.Type == Emums.ElementaryUnitType.BinaryFunction || el.Type == Emums.ElementaryUnitType.UnaryFunction))
                        {
                            result.Add(buffer.Pop());
                            if (buffer.Count == 0) break;
                        }

                        buffer.Push(el);
                        continue;
                    }
                    if (firstLO.Contains(el.Value))
                    {
                        if (buffer.Count != 0)
                            while ((firstLO + secondLO).Contains(buffer.Peek().Value) || buffer.Peek().Value == "^" || buffer.Peek().Type == Emums.ElementaryUnitType.BinaryFunction || buffer.Peek().Type == Emums.ElementaryUnitType.UnaryFunction)
                            {
                                result.Add(buffer.Pop());
                                if (buffer.Count == 0) break;
                            }
                        buffer.Push(el);
                        continue;
                    }
                    if (secondLO.Contains(el.Value))
                    {
                        if (buffer.Count != 0)
                            while ((buffer.Count != 0 && secondLO.Contains(buffer.Peek().Value)) || (buffer.Peek().Value == "^" && buffer.Count != 0) || buffer.Peek().Type == Emums.ElementaryUnitType.BinaryFunction || buffer.Peek().Type == Emums.ElementaryUnitType.UnaryFunction)
                            {
                                result.Add(buffer.Pop());
                                if (buffer.Count == 0) break;
                            }

                        buffer.Push(el);
                    }
                }
            }

            while (buffer.Count != 0)
                result.Add(buffer.Pop());

            return new Expression(result);
        }

        private static Func<double,double> ToFunc(Expression expression)
        {
            return (x) =>
            {
                return Calculate(expression, x);
            };
        }

        private static double Calculate(Expression expression, double x)
        {
            var stack = new Stack<double>();

            foreach (var el in expression)
            {
                if (el.Type == Emums.ElementaryUnitType.Digit)
                {
                    stack.Push(Convert.ToDouble(el.Value));
                    continue;
                }

                if (el.Type == Emums.ElementaryUnitType.Constant)
                {
                    stack.Push(constans[el.Value]);
                    continue;
                }

                if (el.Type == Emums.ElementaryUnitType.Variable)
                {
                    stack.Push(x);
                    continue;
                }

                if (el.Type == Emums.ElementaryUnitType.UnaryFunction)
                {
                    if (el.Value == "log")
                    {
                        var a = stack.Pop();

                        var b = stack.Pop();

                        stack.Push(Math.Log(b, a));

                        continue;
                    }
                    var f = unaryFunctions[el.Value];

                    var arg = stack.Pop();

                    stack.Push(f(arg));
                }

                if (el.Type == Emums.ElementaryUnitType.BinaryFunction)
                {
                    var a = stack.Pop();

                    var b = stack.Pop();

                    stack.Push(binaryFunction[el.Value](b, a));
                }

                if (el.Type == Emums.ElementaryUnitType.BinaryOperation)
                {
                    var a = stack.Pop();
                    var b = stack.Pop();
                    switch (el.Value)
                    {
                        case "+": stack.Push(a + b); break;
                        case "-": stack.Push(b - a); break;
                        case "/": stack.Push(b / a); break;
                        case "*": stack.Push(a * b); break;
                        case "^": stack.Push(Math.Pow(b, a)); break;
                    }
                }
            }


            return stack.Pop();
        }

        internal IEnumerator<ElementaryUnit> GetEnumerator()
        {
            return expression.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return expression.GetEnumerator();
        }

        internal Expression(List<ElementaryUnit> expresion)
        {
            this.expression = expresion;
        }

        internal List<ElementaryUnit> expression { get; }

        public override string ToString()
        {
            var res = string.Empty;
            foreach(var el in expression)
            {
                res += el.Value + " ";
            }
            return res;
        }

        public static Func<double,double> ToDelegate(string expression)
        {
            var exp = ToExpresion(expression);
            var inversePolishNotation = ToPolishNotation(exp);
            return ToFunc(inversePolishNotation);
        }

        public static double Calculate(string expression, double x = 0)
        {
            var exp = ToExpresion(expression);
            var inversePolishNotation = ToPolishNotation(exp);
            return Calculate(inversePolishNotation, x);
        }

        IEnumerator<ElementaryUnit> IEnumerable<ElementaryUnit>.GetEnumerator()
        {
            return expression.GetEnumerator();
        }
    }
}
