namespace PolishNotation
{
    using Emums;
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;

    public class Expression:IEnumerable<ElementaryUnit>
    {
        private static readonly char[] binaryOperators = new[] { '+', '-', '*', '/', '^', '%' };

        public static Dictionary<string, double> constans = new Dictionary<string, double>
        {
            {"pi",Math.PI },
            {"e",Math.E }
        };

        private static readonly Dictionary<string, Func<double, double>> unaryFunctions = new Dictionary<string, Func<double, double>>
        {
            //{"sin", Math.Sin },
            //{"cos", Math.Cos },
            //{"tg", Math.Tan },
            //{"ctg", (number) => Math.Cos(number)/Math.Sin(number) },
            //{"sign", (number) => Math.Sign(number) },
            //{"sqrt", Math.Sqrt },
            //{"abs",Math.Abs },
            //{"acos",Math.Acos },
            //{"asin",Math.Asin },
            //{"atan", Math.Atan},
            //{"actg", (number) => 1/Math.Atan(number) },
            //{"lg",Math.Log10 },
            //{"ln", (number) => Math.Log(number) }
        };

        private static readonly Dictionary<string, Func<double, double, double>> binaryFunction = new Dictionary<string, Func<double, double, double>>
        {
            //{"log", (a,b) => Math.Log(a,b) }
        };

        private static readonly Dictionary<string, Func<Stack<ElementaryUnit>, double>> user_functions = new Dictionary<string, Func<Stack<ElementaryUnit>, double>>
        {
            {
                "rnd", (a) => {
                    return (double)(new Random().Next());
                }
            },
            {
                "sin", (a) => {
                    if( a.Count!=1 ) {
                        throw new Exception("sin-function need one argument. It has "+a.Count+" argument(s).");
                    }

                    ElementaryUnit param=a.Pop();
                    double _param = Convert.ToDouble( param.Value.Replace('.', ',') );
                    double res = Math.Sin(_param);
                    return res;
                }
            },
            {
                "cos", (a) => {
                    if( a.Count!=1 ) {
                        throw new Exception("cos-function need one argument. It has "+a.Count+" argument(s).");
                    }

                    ElementaryUnit param=a.Pop();
                    double _param = Convert.ToDouble( param.Value );
                    double res = Math.Cos(_param);
                    return res;
                }
            },
            {
                "tg", (a) => {
                    if( a.Count!=1 ) {
                        throw new Exception("tg-function need one argument. It has "+a.Count+" argument(s).");
                    }

                    ElementaryUnit param=a.Pop();
                    double _param = Convert.ToDouble( param.Value );
                    double res = Math.Tan(_param);
                    return res;
                }
            },
            {
                "ctg", (a) => {
                    if( a.Count!=1 ) {
                        throw new Exception("tg-function need one argument. It has "+a.Count+" argument(s).");
                    }

                    ElementaryUnit param=a.Pop();
                    double _param = Convert.ToDouble( param.Value );
                    double res = Math.Cos(_param)/Math.Sin(_param);
                    return res;
                }
            },
            {
                "sign", (a) => {
                    if( a.Count!=1 ) {
                        throw new Exception("sign-function need one argument. It has "+a.Count+" argument(s).");
                    }

                    ElementaryUnit param=a.Pop();
                    double _param = Convert.ToDouble( param.Value );
                    double res = Math.Sign(_param);
                    return res;
                }
            },
            {
                "sqrt", (a) => {
                    if( a.Count!=1 ) {
                        throw new Exception("sqrt-function need one argument. It has "+a.Count+" argument(s).");
                    }

                    ElementaryUnit param=a.Pop();
                    double _param = Convert.ToDouble( param.Value );
                    double res = Math.Sqrt(_param);
                    return res;
                }
            },
            {
                "abs", (a) => {
                    if( a.Count!=1 ) {
                        throw new Exception("abs-function need one argument. It has "+a.Count+" argument(s).");
                    }

                    ElementaryUnit param=a.Pop();
                    double _param = Convert.ToDouble( param.Value );
                    double res = Math.Abs(_param);
                    return res;
                }
            },
            {
                "acos", (a) => {
                    if( a.Count!=1 ) {
                        throw new Exception("acos-function need one argument. It has "+a.Count+" argument(s).");
                    }

                    ElementaryUnit param=a.Pop();
                    double _param = Convert.ToDouble( param.Value );
                    double res = Math.Acos(_param);
                    return res;
                }
            },
            {
                "asin", (a) => {
                    if( a.Count!=1 ) {
                        throw new Exception("asin-function need one argument. It has "+a.Count+" argument(s).");
                    }

                    ElementaryUnit param=a.Pop();
                    double _param = Convert.ToDouble( param.Value );
                    double res = Math.Asin(_param);
                    return res;
                }
            },
            {
                "atan", (a) => {
                    if( a.Count!=1 ) {
                        throw new Exception("atan-function need one argument. It has "+a.Count+" argument(s).");
                    }

                    ElementaryUnit param=a.Pop();
                    double _param = Convert.ToDouble( param.Value );
                    double res = Math.Atan(_param);
                    return res;
                }
            },
            {
                "actg", (a) => {
                    if( a.Count!=1 ) {
                        throw new Exception("actg-function need one argument. It has "+a.Count+" argument(s).");
                    }

                    ElementaryUnit param=a.Pop();
                    double _param = Convert.ToDouble( param.Value );
                    double res = 1/Math.Atan(_param);
                    return res;
                }
            },
            {
                "lg", (a) => {
                    if( a.Count!=1 ) {
                        throw new Exception("lg-function need one argument. It has "+a.Count+" argument(s).");
                    }

                    ElementaryUnit param=a.Pop();
                    double _param = Convert.ToDouble( param.Value );
                    double res = Math.Log10(_param);
                    return res;
                }
            },
            {
                "ln", (a) => {
                    if( a.Count!=1 ) {
                        throw new Exception("ln-function need one argument. It has "+a.Count+" argument(s).");
                    }

                    ElementaryUnit param=a.Pop();
                    double _param = Convert.ToDouble( param.Value );
                    double res = Math.Log(_param);
                    return res;
                }
            },
            {
                "log", (a) => {
                    if( a.Count!=2) {
                        throw new Exception("log-function need two arguments. It has "+a.Count+" agrument(s).");
                    }
                    // Извлечение Pop даёт аргументы в правильной последовательности
                    ElementaryUnit param1=a.Pop();
                    ElementaryUnit param2=a.Pop();
                    double _param1 = Convert.ToDouble( param1.Value );
                    double _param2 = Convert.ToDouble( param2.Value );
                    double res = Math.Log(_param1, _param2);
                    return res;
                }
            },
            {
                "count_arguments", (a) => {
                    return (double)a.Count;
                }
            },

        };

        public static Expression ToExpresion(string expression)
        {
            expression = new string(expression.Where(c => c != ' ').ToArray());

            StringBuilder buf = new StringBuilder(Convert.ToString(expression[0]));

            for (int i = 1; i < expression.Length; i++)
            {
                //if (expression[i] == '-' && expression[i - 1] == '(')
                //{
                //    buf.Append("0");
                //}
                buf.Append(expression[i]);
            }

            expression = buf.ToString();

            //if (expression[0] == '-') expression = "0" + expression;

            List<ElementaryUnit> result = new List<ElementaryUnit>();

            for (var i = 0; i < expression.Length; i++)
            {
                char c = expression[i];

                if (c == 'x' || c == 'X')
                {
                    result.Add(new ElementaryUnit(Emums.ElementaryUnitType.Variable, Convert.ToString(c), i));
                    continue;
                }

                if (binaryOperators.Contains(c))
                {
                    result.Add(new ElementaryUnit(Emums.ElementaryUnitType.BinaryOperation, Convert.ToString(c), i));
                    continue;
                }

                if (c == '(' || c == ')')
                {
                    result.Add(new ElementaryUnit(Emums.ElementaryUnitType.Brackets, Convert.ToString(c), i));
                    continue;
                }

                if (char.IsLetter(c))
                {
                    string buffer = string.Empty;
                    var j = i;
                    for (; j < expression.Length && (char.IsLetter(expression[j]) || expression[j] == '_'); j++)
                    {
                        buffer += expression[j];
                    }
                    i = j - 1;

                    //if (unaryFunctions.Keys.Contains(buffer)) {
                    //    result.Add(new ElementaryUnit(Emums.ElementaryUnitType.UnaryFunction, buffer, i));
                    //} else if (binaryFunction.Keys.Contains(buffer)) {
                    //    result.Add(new ElementaryUnit(Emums.ElementaryUnitType.BinaryFunction, buffer, i));
                    //} else 
                    if (constans.Keys.Contains(buffer)) {
                        result.Add(new ElementaryUnit(Emums.ElementaryUnitType.Constant, buffer, i));
                    }else if (user_functions.Keys.Contains(buffer)) {
                        result.Add(new ElementaryUnit(Emums.ElementaryUnitType.UserFunctions, buffer, i));
                    }else {
                        throw new Exception("Неизвестная функция '"+buffer+"' в позиции "+i);
                    }
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
                    result.Add(new ElementaryUnit(Emums.ElementaryUnitType.Digit, buffer, i));
                }

            }

            return new Expression(result);
        }

        public static Expression ToPolishNotation(Expression expression)
        {
            var result = new List<ElementaryUnit>();
            var buffer = new Stack<ElementaryUnit>();

            // Приоритеты операций:
            string[] operations_priority = new string[] { "%", "^", "*/", "+-" };

            var firstLO = "+-";
            var secondLO = "*/";
            List<ElementaryUnitType> list_functions = new List<ElementaryUnitType> { ElementaryUnitType.UnaryFunction, ElementaryUnitType.BinaryFunction, ElementaryUnitType.UserFunctions };
            List<ElementaryUnitType> list_params = new List<ElementaryUnitType> { ElementaryUnitType.Digit, ElementaryUnitType.Variable, ElementaryUnitType.Constant};



            foreach (var el in expression)
            {
                //if (el.Type == Emums.ElementaryUnitType.Digit || el.Type == Emums.ElementaryUnitType.Variable || el.Type == Emums.ElementaryUnitType.Constant)
                if (list_params.Contains(el.Type) ) {
                    result.Add(el);
                    continue;
                }
                //if (el.Type == Emums.ElementaryUnitType.UnaryFunction || el.Type == Emums.ElementaryUnitType.BinaryFunction || el.Type == Emums.ElementaryUnitType.UserFunctions)
                if(list_functions.Contains(el.Type))
                {
                    buffer.Push(el);
                    continue;
                }

                if (el.Type == Emums.ElementaryUnitType.Brackets)
                {
                    if (el.Value == ")")
                    {
                        if(buffer.Count == 0) {
                            throw new Exception("Несогласованное количество скобок в позиции " + el.Position);
                        }
                        while (buffer.Peek().Value != "(")
                        {
                            ElementaryUnit last_unit = buffer.Pop();
                            result.Add(last_unit);
                            if (buffer.Count == 0) {
                                throw new Exception("Несогласованное количество скобок в позиции "+el.Position);
                            }
                        }
                        result.Add(el);
                        ElementaryUnit open_bracket = buffer.Pop();
                        //if (el.Type == Emums.ElementaryUnitType.UnaryFunction || el.Type == Emums.ElementaryUnitType.BinaryFunction || el.Type == Emums.ElementaryUnitType.UserFunctions)
                        if (buffer.Count > 0) {
                            if (list_functions.Contains(buffer.Peek().Type)) {
                                ElementaryUnit eu = buffer.Pop();
                                result.Add(eu);
                            }
                        }
                        continue;
                    }else {
                        result.Add(el);
                    }
                    buffer.Push(el);
                    continue;
                }

                if (el.Type == Emums.ElementaryUnitType.BinaryOperation) {
                    // Найти в какой позиции находится оператор по приоритету:
                    int pos_priority = -1;
                    // Строка более приоритетных операторов:
                    string more_priority_operators = "";
                    for (int i=0; i<= operations_priority.Length-1; i++) {
                        more_priority_operators += operations_priority[i];
                        if (operations_priority[i].Contains(el.Value)==true) {
                            pos_priority = i;
                            break;
                        }
                    }
                    if(pos_priority == -1) {
                        throw new Exception("Неизвестный оператор '"+el.Value+"' в позиции "+el.Position);
                    }
                    while (buffer.Count != 0 &&
                        (more_priority_operators.Contains(buffer.Peek().Value) || list_functions.Contains(buffer.Peek().Type))
                        ) {
                        result.Add(buffer.Pop());
                        if (buffer.Count == 0) break;
                    }

                    buffer.Push(el);
                    continue;
                }

                /*
                if (el.Type == Emums.ElementaryUnitType.BinaryOperation)
                {
                    if (el.Value == "^")
                    {
                        while (buffer.Count != 0 && 
                            (("^").Contains(buffer.Peek().Value) || list_functions.Contains(buffer.Peek().Type))
                            )
                        {
                            result.Add(buffer.Pop());
                            if (buffer.Count == 0) break;
                        }

                        buffer.Push(el);
                        continue;
                    }
                    if (secondLO.Contains(el.Value)) {
                        while ((buffer.Count != 0 &&
                            (("^" + secondLO).Contains(buffer.Peek().Value))
                            || list_functions.Contains(buffer.Peek().Type)
                            )) {
                            result.Add(buffer.Pop());
                        }

                        buffer.Push(el);
                        continue;
                    }
                    if (firstLO.Contains(el.Value))
                    {
                            while (buffer.Count != 0 && 
                                (("^" + secondLO + firstLO).Contains(buffer.Peek().Value)
                                || list_functions.Contains(buffer.Peek().Type)
                                ))

                            {
                                result.Add(buffer.Pop());
                            }
                        buffer.Push(el);
                        continue;
                    }
                }
                */
                throw new Exception("Неизвестная функция '"+el.Value+"' в позиции "+el.Position);
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
            var stack = new Stack<ElementaryUnit>();
            List<ElementaryUnitType> list_params = new List<ElementaryUnitType> { ElementaryUnitType.Digit, ElementaryUnitType.Variable, ElementaryUnitType.Constant, ElementaryUnitType.Brackets };
            foreach (var el in expression)
            {
                if (list_params.Contains( el.Type ) ) {
                    //stack.Push(Convert.ToDouble(el.Value));
                    if (stack.Count>0 && stack.Peek().Type==ElementaryUnitType.Brackets && stack.Peek().Value == ")" && el.Value==")") {
                        // Найти предыдущую скобку и уничтожить её, сохранив аргументы:
                        Stack<ElementaryUnit> temp = new Stack<ElementaryUnit>();
                        // Выкинуть первую закрывающую скобку:
                        stack.Pop();
                        while (stack.Peek().Value != "(") {
                            if(stack.Count == 0) {
                                throw new Exception("Неверное количество открывающих и закрывающих скобок.");
                            }
                            temp.Push(stack.Pop());
                        }
                        if(temp.Count != 1) {
                            throw new Exception("Количество аргументов при сокращении скобок должно быть равно 1. Например, выражение ((1,2,3))-wrong. Good: ((1)) or ((((-5))))");
                        }
                        // Выкинуть встретившуюся открывающую скобку
                        stack.Pop(); 
                        // И положить аргументы назад в стек:
                        while(temp.Count > 0) {
                            stack.Push(temp.Pop());
                        }
                        stack.Push(el);
                    } else if (el.Type == Emums.ElementaryUnitType.Variable) {
                        stack.Push(new ElementaryUnit(ElementaryUnitType.Digit, x.ToString(), el.Position));
                        continue;
                    }else if (el.Type == Emums.ElementaryUnitType.Constant) {
                        stack.Push(new ElementaryUnit(ElementaryUnitType.Digit, constans[el.Value].ToString(), el.Position) );
                        continue;
                    } else {
                        stack.Push(el);
                    }

                    
                    continue;
                }

                //if (el.Type == Emums.ElementaryUnitType.Digit)
                //{
                //    stack.Push(Convert.ToDouble(el.Value));
                //    continue;
                //}

                //if (el.Type == Emums.ElementaryUnitType.Constant)
                //{
                //    stack.Push(constans[el.Value]);
                //    continue;
                //}

                //if (el.Type == Emums.ElementaryUnitType.Variable)
                //{
                //    stack.Push(x);
                //    continue;
                //}


                // Выбрать из стека открывающие скобки, чтобы добраться до первого операнда:
                int brackets_count = 0;
                while(stack.Count>0 && stack.Peek().Type==ElementaryUnitType.Brackets && stack.Peek().Value == ")") {
                    stack.Pop();
                    brackets_count++;
                }

                // Выбрать список аргументов для операции:
                Stack<ElementaryUnit> stack_args = new Stack<ElementaryUnit>();
                if (el.Type == Emums.ElementaryUnitType.BinaryOperation) {
                    if (stack.Count > 0 &&  stack.Peek().Type == ElementaryUnitType.Digit) {
                        stack_args.Push(stack.Pop());
                    }
                    // Убрать лишние открывающие скобки
                    // TODO - сделать проверку, если стек кончится раньше, чем открывающие скобки
                    for (int i = 0; i <= brackets_count - 1; i++) {
                        if (stack.Peek().Type != ElementaryUnitType.Brackets || stack.Peek().Value != "(" || stack.Count == 0) {
                            throw new Exception("Неверное количество открывающих скобок");
                        }
                        stack.Pop();
                    }
                    // Выбрать из стека открывающие скобки, чтобы добраться до первого операнда:
                    brackets_count = 0;
                    while (stack.Count>0 && stack.Peek().Type == ElementaryUnitType.Brackets && stack.Peek().Value == ")") {
                        stack.Pop();
                        brackets_count++;
                    }

                    if (stack.Count > 0 && stack.Peek().Type == ElementaryUnitType.Digit) {
                        stack_args.Push(stack.Pop());
                    }
                } else {
                    while (stack.Count>0 && stack.Peek().Type != ElementaryUnitType.Brackets) {
                        stack_args.Push(stack.Pop());
                    }
                }
                // Убрать лишние открывающие скобки
                // TODO - сделать проверку, если стек кончится раньше, чем открывающие скобки
                for (int i = 0; i <= brackets_count - 1; i++) {
                    if (stack.Peek().Type != ElementaryUnitType.Brackets || stack.Peek().Value != "(" || stack.Count == 0) {
                        throw new Exception("Неверное количество открывающих скобок");
                    }
                    stack.Pop();
                }

                double res = 0;
                if (el.Type == Emums.ElementaryUnitType.BinaryOperation) {
                    switch (stack_args.Count) {
                        case 1: {
                                var _a = stack_args.Pop();
                                double a = Convert.ToDouble(_a.Value);
                                switch (el.Value) {
                                    case "+": res = a; break;
                                    case "-": res = -a; break;
                                    default: {
                                            throw new Exception("Операция "+el.Value+" не разрешена с аргументом "+_a.Value);
                                        }
                                }
                            }
                            break;
                        case 2: {
                                var _a = stack_args.Pop();
                                var _b = stack_args.Pop();
                                double a = Convert.ToDouble(_a.Value);
                                double b = Convert.ToDouble(_b.Value);
                                switch (el.Value) {
                                    case "+": res = a + b; break;
                                    case "-": res = a - b; break;
                                    case "/": res = a / b; break;
                                    case "*": res = a * b; break;
                                    case "^": res = Math.Pow(a, b); break;
                                    case "%": res = a % b; break;
                                }
                            }
                            break;
                        default:
                            throw new Exception("У операции '"+el.Value + "' должно быть 1 или 2 операнда");
                    }
                    //stack.Push(new ElementaryUnit(ElementaryUnitType.Digit, res.ToString() ) );
                } else {
                    Func<Stack<ElementaryUnit>, double> func = user_functions[el.Value];
                    if (func != null) {
                        res = func(stack_args);
                    }else {
                        throw new Exception("Неизвестная функция "+el.Value+" в позиции "+el.Position);
                    }
                }
                stack.Push(new ElementaryUnit(ElementaryUnitType.Digit, res.ToString()));

                //if (el.Type == Emums.ElementaryUnitType.UnaryFunction)
                //{
                //    if (el.Value == "log")
                //    {
                //        var a = stack.Pop();
                //        var b = stack.Pop();
                //        double _a = Convert.ToDouble(a.Value);
                //        double _b = Convert.ToDouble(b.Value);

                //        stack.Push( new ElementaryUnit( ElementaryUnitType.Digit, Math.Log(_b, _a ).ToString() ) );
                //    }
                //    var f = unaryFunctions[el.Value];

                //    var arg = stack.Pop();

                //    stack.Push(f(arg));
                //}

                //if (el.Type == Emums.ElementaryUnitType.BinaryFunction)
                //{
                //    var a = stack.Pop();
                //    var b = stack.Pop();

                //    stack.Push(binaryFunction[el.Value](b, a));
                //}
            }


            //return stack.Pop();
            // Если вокруг последнего аргумента находятся скобки, то удалить все симметричные скобки:
            if (stack.Count > 0 && stack.Peek().Value == ")") {
                // Найти предыдущую скобку и уничтожить её, сохранив аргументы:
                Stack<ElementaryUnit> temp = new Stack<ElementaryUnit>();
                // Выкинуть первую закрывающую скобку:
                stack.Pop();
                while (stack.Peek().Value != "(") {
                    if (stack.Count == 0) {
                        throw new Exception("Неверное количество открывающих и закрывающих скобок. поз:"+ stack.Peek().Position );
                    }
                    temp.Push(stack.Pop());
                }
                // Выкинуть встретившуюся открывающую скобку
                stack.Pop();
                // И положить аргументы назад в стек:
                while (temp.Count > 0) {
                    stack.Push(temp.Pop());
                }
            }
            if (stack.Count > 1) {
                throw new Exception("Проверьте синтаксис выражения. После завершения расчёта остались аргументы.");
            }
            return Convert.ToDouble(stack.Pop().Value);
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

        public static double Calculate( Expression exp ) {
            var inversePolishNotation = exp;
            return Calculate(inversePolishNotation, 0);
        }

        IEnumerator<ElementaryUnit> IEnumerable<ElementaryUnit>.GetEnumerator()
        {
            return expression.GetEnumerator();
        }
    }
}
