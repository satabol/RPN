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

        private static readonly Dictionary<string, Func<double, double, double>> binaryFunctions = new Dictionary<string, Func<double, double, double>>
        {
            {"log", (a,b) => Math.Log(a,b) },
            {"sum", (a,b) => a+b },
        };

        private static double call_unary_function(string func_name, Stack<ElementaryUnit> a ) {
            if (a.Count != 1) {
                throw new Exception(func_name+"-function need one argument. It has " + a.Count + " argument(s).");
            }

            double res = 0;
            ElementaryUnit param = a.Pop();
            if (param.Value is string) {
                string val = (string)param.Value;
                double _param = Convert.ToDouble(val.Replace('.', ','));
                //res = Math.Sin(_param);
                if (unaryFunctions[func_name] == null) {
                    throw new Exception("Not exist function '"+func_name+"' ");
                }
                res = unaryFunctions[func_name](_param);
            } else {
                throw new Exception("The argument have to be a 'double'-type. It is "+param.Type );
            }
            return res;
        }

        private static double call_binary_function( string func_name, Stack<ElementaryUnit> a ) {
            if (a.Count != 2) {
                throw new Exception(func_name+"-function need one argument. It has " + a.Count + " argument(s).");
            }

            // Извлечение Pop даёт аргументы в правильной последовательности
            ElementaryUnit param1 = a.Pop();
            ElementaryUnit param2 = a.Pop();

            double res = 0;
            if (param1.Value is string && param2.Value is string) {
                string val1 = (string)param1.Value;
                string val2 = (string)param2.Value;
                double _param1 = Convert.ToDouble(val1.Replace('.', ','));
                double _param2 = Convert.ToDouble(val2.Replace('.', ','));
                if (binaryFunctions[func_name] == null) {
                    throw new Exception("Not exist function '" + func_name + "' ");
                }
                res = binaryFunctions[func_name](_param1, _param2);
            } else {
                throw new Exception("The arguments have to be a 'double'-type. It is " + param1.Type+", "+param2.Type+" " );
            }
            return res;
        }

        private static readonly Dictionary<string, Func<Stack<ElementaryUnit>, double>> user_functions = new Dictionary<string, Func<Stack<ElementaryUnit>, double>>
        {
            {
                "rnd", (a) => {
                    return (double)(new Random().Next());
                }
            },
            {
                "sin", (a) => {
                    return call_unary_function( "sin", a);
                }
            },
            {
                "cos", (a) => {
                    return call_unary_function( "cos", a);
                }
            },
            {
                "tg", (a) => {
                    return call_unary_function( "tg", a);
                }
            },
            {
                "ctg", (a) => {
                    return call_unary_function( "ctg", a);
                }
            },
            {
                "sign", (a) => {
                    return call_unary_function( "sign", a);
                }
            },
            {
                "sqrt", (a) => {
                    return call_unary_function( "sqrt", a);
                }
            },
            {
                "abs", (a) => {
                    return call_unary_function( "abs", a);
                }
            },
            {
                "acos", (a) => {
                    return call_unary_function( "acos", a);
                }
            },
            {
                "asin", (a) => {
                    return call_unary_function( "asin", a);
                }
            },
            {
                "atan", (a) => {
                    return call_unary_function( "atan", a);
                }
            },
            {
                "actg", (a) => {
                    return call_unary_function( "actg", a);
                }
            },
            {
                "lg", (a) => {
                    return call_unary_function( "lg", a);
                }
            },
            {
                "ln", (a) => {
                    return call_unary_function( "ln", a);
                }
            },
            {
                "log", (a) => {
                    return call_binary_function( "log", a);
                }
            },
            {
                "sum", (a) => {
                    return call_binary_function( "sum", a);
                }
            },
            {
                "count_arguments", (a) => {
                    return (double)a.Count;
                }
            },
            {
                "list_length", (a) => {
                    if(a.Count>0) {
                        ElementaryUnit param1 = a.Pop();
                        if(param1.Value is Stack<ElementaryUnit>) {
                            return ((Stack<ElementaryUnit>)param1.Value).Count;
                        }else {
                            throw new Exception("First argument is not a list: "+param1.Type );
                        }
                    }else {
                        return 0;
                    }
                }
            },

        };

        public static Expression ToExpresion(string expression)
        {
            expression = new string(expression.Where(c => c != ' ').ToArray());

            StringBuilder buf = new StringBuilder(Convert.ToString(expression[0]));

            for (int i = 1; i < expression.Length; i++)
            {
                buf.Append(expression[i]);
            }

            expression = buf.ToString();

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

                //if (c == '(' || c == ')')
                if ("()".Contains(c) == true) {
                    result.Add(new ElementaryUnit(Emums.ElementaryUnitType.RoundBrackets, Convert.ToString(c), i));
                    continue;
                }

                if ("{}".Contains(c) == true) {
                    result.Add(new ElementaryUnit(Emums.ElementaryUnitType.CurveBrackets, Convert.ToString(c), i));
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

                    if (constans.Keys.Contains(buffer)) {
                        result.Add(new ElementaryUnit(Emums.ElementaryUnitType.Constant, buffer, i));
                    }else if (user_functions.Keys.Contains(buffer)) {
                        result.Add(new ElementaryUnit(Emums.ElementaryUnitType.UserFunctions, buffer, i));
                    }else {
                        throw new Exception("Unknown function '"+buffer+"' at position "+i+" ");
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

            List<ElementaryUnitType> list_functions = new List<ElementaryUnitType> { ElementaryUnitType.UserFunctions };
            List<ElementaryUnitType> list_params = new List<ElementaryUnitType> { ElementaryUnitType.Digit, ElementaryUnitType.Variable, ElementaryUnitType.Constant};



            foreach (var el in expression)
            {
                if (list_params.Contains(el.Type)==true) {
                    result.Add(el);
                    continue;
                }
                if (list_functions.Contains(el.Type)==true) {
                    buffer.Push(el);
                    continue;
                }

                //if (el.Type == Emums.ElementaryUnitType.Brackets || el.Type== Emums.ElementaryUnitType.CurveBrackets)
                if( new[]{ Emums.ElementaryUnitType.RoundBrackets, Emums.ElementaryUnitType.CurveBrackets }.Contains(el.Type)==true )
                {
                    if (")}".Contains( el.Value.ToString() )==true) // == ")" || el.Value.ToString() == "}")
                    {
                        string oposide_bracket = Convert.ToString( "({"[")}".IndexOf(el.Value.ToString())] );
                        if(buffer.Count == 0) {
                            throw new Exception("Wrong bracket in position " + el.Position);
                        }
                        while (buffer.Peek().Value.ToString() != oposide_bracket)
                        {
                            ElementaryUnit last_unit = buffer.Pop();
                            result.Add(last_unit);
                            if (buffer.Count == 0) {
                                throw new Exception("Wrong bracket in position "+el.Position);
                            }
                        }
                        result.Add(el);
                        ElementaryUnit open_bracket = buffer.Pop();
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
                        if (operations_priority[i].Contains(el.Value.ToString() )==true) {
                            pos_priority = i;
                            break;
                        }
                    }
                    if(pos_priority == -1) {
                        throw new Exception("Unknown operator '"+el.Value+"' in position "+el.Position);
                    }
                    // Сначала надо выполнить более приоритетные операторы и функции, поэтому перенести их из буфера в результирующий массив
                    while (buffer.Count != 0 &&
                        (more_priority_operators.Contains( buffer.Peek().Value.ToString() ) || list_functions.Contains(buffer.Peek().Type))
                        ) {
                        result.Add(buffer.Pop());
                        //if (buffer.Count == 0) break;
                    }

                    buffer.Push(el);
                    continue;
                }
                throw new Exception("Неизвестная функция '"+el.Value+"' в позиции "+el.Position);
            }
            while (buffer.Count != 0) {
                result.Add(buffer.Pop());
            }

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
            List<ElementaryUnitType> list_params = new List<ElementaryUnitType> { ElementaryUnitType.Digit, ElementaryUnitType.Variable, ElementaryUnitType.Constant, ElementaryUnitType.RoundBrackets, ElementaryUnitType.CurveBrackets };
            foreach (var el in expression)
            {
                // Процедура сокращения круглых скобок, если они объявлены сами по себе, без функции.
                // Небольшое следствие: Одна круглая закрывающая скобка всегда попадёт в стек, но вторая уже никак не може попать, потому что
                // должно сработать следующее условие:
                if (stack.Count > 0 && stack.Peek().Type == ElementaryUnitType.RoundBrackets && stack.Peek().Value.ToString() == ")" && el.Type!=ElementaryUnitType.UserFunctions) { // .ToString() == ")") {
                    // Найти предыдущую скобку и уничтожить её, сохранив аргументы:
                    Stack<ElementaryUnit> temp = new Stack<ElementaryUnit>();
                    // Выкинуть первую закрывающую скобку:
                    ElementaryUnit stack_pop = stack.Pop();
                    // Перекидывать во временный стек все аргументы до открывающей круглой скобки
                    while (stack.Peek().Value.ToString() != "(") {
                        if (stack.Count == 0) {
                            throw new Exception("Неверное количество открывающих и закрывающих скобок.");
                        }
                        temp.Push(stack.Pop());
                    }
                    // Внутри круглых скобок не должно быть больше одного аргумента.
                    if (temp.Count != 1) {
                        throw new Exception("Внутри круглых скобок должен быть только один аргумент. Например, выражение ((1,2,3))|()-wrong. Good: ((1)) or ((((-5))))");
                    }
                    // Выкинуть встретившуюся открывающую круглую скобку
                    stack.Pop();
                    // И положить аргументы назад в стек:
                    while (temp.Count > 0) {
                        stack.Push(temp.Pop());
                    }
                }
                if (list_params.Contains( el.Type ) ) {


                    if (stack.Count > 0 && el.Value.ToString() == "}") {
                        // Найти открывающую фигурную скобку и сформировать из аргументов список:
                        // Найти предыдущую скобку и уничтожить её, сохранив аргументы:
                        Stack<ElementaryUnit> temp = new Stack<ElementaryUnit>();
                        // Выкинуть первую закрывающую скобку:
                        //stack.Pop(); - Для фигурной ничего не надо выкидывать, т.к. никакой скобки сейчас нет
                        while (stack.Peek().Value.ToString() != "{") {
                            if (stack.Count == 0) {
                                throw new Exception("Неверное количество открывающих и закрывающих фигурных скобок.");
                            }
                            temp.Push(stack.Pop());
                        }
                        // Выкинуть встретившуюся открывающую фигурную скобку
                        stack.Pop();
                        ElementaryUnit list = new ElementaryUnit(ElementaryUnitType.List, temp);
                        stack.Push(list);
                    } else if (el.Type == Emums.ElementaryUnitType.Variable) {
                        stack.Push(new ElementaryUnit(ElementaryUnitType.Digit, x.ToString(), el.Position));
                        continue;
                    } else if (el.Type == Emums.ElementaryUnitType.Constant) {
                        stack.Push(new ElementaryUnit(ElementaryUnitType.Digit, constans[el.Value.ToString()].ToString(), el.Position));
                        continue;
                    } else {
                        stack.Push(el);
                    }

                    
                    continue;
                }

                // Выбрать из стека открывающую скобки, чтобы добраться до первого операнда (Больше одной открывающей скобки быть не должно, потому что их не пропустит первое условие в цикле):
                int brackets_count = 0;
                if(stack.Count > 0 && stack.Peek().Type == ElementaryUnitType.RoundBrackets && stack.Peek().Value.ToString() == ")") {
                    stack.Pop();
                    brackets_count++;
                }

                // Выбрать список аргументов для операции:
                Stack<ElementaryUnit> stack_args = new Stack<ElementaryUnit>();
                if (el.Type == Emums.ElementaryUnitType.BinaryOperation) {
                    // Для бинарной операции нужно не больше двух операндов: Не больше потому что иногда плюс/минус стоит справа от скобки и первого операнда быть не может.
                    for( int i=0; i<=1 && stack.Count > 0 && stack.Peek().Type != ElementaryUnitType.RoundBrackets; i++) {
                            stack_args.Push(stack.Pop());
                    }
                } else {
                    while (stack.Count > 0 && stack.Peek().Type != ElementaryUnitType.RoundBrackets) {
                        stack_args.Push(stack.Pop());
                    }
                }

                // Убрать лишнюю открывающую скобку
                if (stack.Count > 0 && brackets_count>0 && stack.Peek().Type == ElementaryUnitType.RoundBrackets && stack.Peek().Value.ToString() == "(") {
                    stack.Pop();
                }

                double res = 0;
                if (el.Type == Emums.ElementaryUnitType.BinaryOperation) {
                    switch (stack_args.Count) {
                        case 1: {
                                var _a = stack_args.Pop();
                                double a = Convert.ToDouble(_a.Value);
                                switch (el.Value.ToString()) {
                                    case "+": res = a; break;
                                    case "-": res = -a; break;
                                    default: {
                                            throw new Exception("Operator '"+el.Value+"' not allowed with '"+_a.Value+"'");
                                        }
                                }
                            }
                            break;
                        case 2: {
                                ElementaryUnit _a = stack_args.Pop();
                                ElementaryUnit _b = stack_args.Pop();
                                if (_a.Value is string && _b.Value is string) {
                                    double a = Convert.ToDouble(_a.Value);
                                    double b = Convert.ToDouble(_b.Value);
                                    switch (el.Value.ToString()) {
                                        case "+": res = a + b; break;
                                        case "-": res = a - b; break;
                                        case "/": res = a / b; break;
                                        case "*": res = a * b; break;
                                        case "^": res = Math.Pow(a, b); break;
                                        case "%": res = a % b; break;
                                        default:
                                            throw new Exception("Unknown operator '" + el.Value.ToString() + "' at position " + el.Position);
                                    }
                                }else {
                                    throw new Exception("Неверные типы аргументов при выполнении операции '"+el.Value.ToString()+"'. Нельзя обработать "+_a.Type+ "'" + el.Value.ToString() + "'"+_b.Type+" ");
                                }
                            }
                            break;
                        default:
                            throw new Exception("Operator '"+el.Value + "' must have 1 or 2 arguments. Count arguments: "+ stack_args.Count+" ");
                    }
                } else {
                    Func<Stack<ElementaryUnit>, double> func = user_functions[el.Value.ToString()];
                    if (func != null) {
                        res = func(stack_args);
                    }else {
                        throw new Exception("Неизвестная функция "+el.Value+" в позиции "+el.Position);
                    }
                }
                stack.Push(new ElementaryUnit(ElementaryUnitType.Digit, res.ToString()));
            }


            // Если вокруг последнего аргумента находятся скобки, то удалить все симметричные скобки:
            if (stack.Count > 0 && stack.Peek().Value.ToString() == ")") {
                // Найти предыдущую скобку и уничтожить её, сохранив аргументы:
                Stack<ElementaryUnit> temp = new Stack<ElementaryUnit>();
                // Выкинуть первую закрывающую скобку:
                stack.Pop();
                while (stack.Peek().Value.ToString() != "(") {
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
