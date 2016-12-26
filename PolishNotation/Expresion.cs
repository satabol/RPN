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

        private static ElementaryUnit call_unary_function(string func_name, Stack<ElementaryUnit> a ) {
            if (a.Count != 1) {
                throw new Exception(func_name+"-function need one argument. It has " + a.Count + " argument(s).");
            }

            ElementaryUnit param1 = a.Pop();
            double _param1 = 0;
            double _res = 0;
            if (param1.Value is string) {
                string val1 = (string)param1.Value;
                _param1 = Convert.ToDouble(val1.Replace('.', ','));
            } else if (param1.Value is Double || param1.Value is int) {
                _param1 = (double)param1.Value;
            } else {
                throw new Exception("The first argument of '" + func_name + "' have to be a 'Digit'-type. It is " + param1.Type + " ");
            }

            if (unaryFunctions[func_name] == null) {
                throw new Exception("Not exist function '"+func_name+"' ");
            }
            _res = unaryFunctions[func_name](_param1);

            ElementaryUnit res = new ElementaryUnit(ElementaryUnitType.Digit, _res);
            return res;
        }

        private static ElementaryUnit call_binary_function( string func_name, Stack<ElementaryUnit> a ) {
            if (a.Count != 2) {
                throw new Exception(func_name+"-function need one argument. It has " + a.Count + " argument(s).");
            }

            double _res = 0;
            // Извлечение Pop даёт аргументы в правильной последовательности
            ElementaryUnit param1 = a.Pop();
            ElementaryUnit param2 = a.Pop();

            double _param1 = 0;
            double _param2 = 0;
            if (param1.Value is string) {
                string val1 = (string)param1.Value;
                _param1 = Convert.ToDouble(val1.Replace('.', ','));
            } else if (param1.Value is Double || param1.Value is int) {
                _param1 = (double)param1.Value;
            }else {
                throw new Exception("The first argument of '"+ func_name + "' have to be a 'Digit'-type. It is " + param1.Type + " ");
            }

            if (param2.Value is string) {
                string val2 = (string)param2.Value;
                _param2 = Convert.ToDouble(val2.Replace('.', ','));
            } else if (param2.Value is Double || param2.Value is int) {
                _param2 = (double)param2.Value;
            } else {
                throw new Exception("The second argument of '"+ func_name + "' have to be a 'Digit'-type. It is " + param2.Type + " ");
            }

            if (binaryFunctions[func_name] == null) {
                throw new Exception("Not exist function '" + func_name + "' ");
            }
            _res = binaryFunctions[func_name](_param1, _param2);

            ElementaryUnit res = new ElementaryUnit(ElementaryUnitType.Digit, _res);
            return res;
        }

        private static readonly Dictionary<string, Func<Stack<ElementaryUnit>, ElementaryUnit>> user_functions = new Dictionary<string, Func<Stack<ElementaryUnit>, ElementaryUnit>>
        {
            {
                "rnd", (a) => {
                    ElementaryUnit eu = new ElementaryUnit(ElementaryUnitType.Digit, (double)(new Random().Next()) );
                    return eu;
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
                    ElementaryUnit res = new ElementaryUnit(ElementaryUnitType.Digit, (double)a.Count );
                    return res;
                }
            },
            {
                "list_length", (a) => {
                    ElementaryUnit res = null;
                    if(a.Count>0) {
                        ElementaryUnit param1 = a.Pop();
                        if(param1.Value is Stack<ElementaryUnit>) {
                            //return ((Stack<ElementaryUnit>)param1.Value).Count;
                            res = new ElementaryUnit(ElementaryUnitType.Digit, ((Stack<ElementaryUnit>)param1.Value).Count );
                        }else {
                            throw new Exception("First argument is not a list: "+param1.Type );
                        }
                    }else {
                        //return 0;
                        res = new ElementaryUnit(ElementaryUnitType.Digit, 0 );
                    }
                    return res;
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

            ElementaryUnit prev_eu = null;
            for (var i = 0; i < expression.Length; i++)
            {
                char c = expression[i];

                //// Очень страннрое условие не разрешающее иметь имена функций, начинающихся на букву 'x'.
                //if (c == 'x' || c == 'X')
                //{
                //    prev_eu = new ElementaryUnit(Emums.ElementaryUnitType.Variable, Convert.ToString(c), i);
                //    result.Add(prev_eu);
                //    continue;
                //}

                if (binaryOperators.Contains(c))
                {
                    if ("+-".Contains(c) && (
                        prev_eu == null
                        || prev_eu.Type == ElementaryUnitType.Comma 
                        || prev_eu.Type==ElementaryUnitType.RoundBrackets&&prev_eu.ToString()=="("
                        || prev_eu.Type == ElementaryUnitType.CurveBrackets && prev_eu.ToString() == "{"
                        )) {
                        result.Add(new ElementaryUnit(Emums.ElementaryUnitType.Digit, 0, i) );
                    }
                    prev_eu = new ElementaryUnit(Emums.ElementaryUnitType.BinaryOperation, Convert.ToString(c), i);
                    result.Add(prev_eu);
                    continue;
                }

                //if (c == '(' || c == ')')
                if ("()".Contains(c) == true) {
                    prev_eu = new ElementaryUnit(Emums.ElementaryUnitType.RoundBrackets, Convert.ToString(c), i);
                    result.Add(prev_eu);
                    continue;
                }

                if ("{}".Contains(c) == true) {
                    prev_eu = new ElementaryUnit(Emums.ElementaryUnitType.CurveBrackets, Convert.ToString(c), i);
                    result.Add(prev_eu);
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
                    //i = j - 1;
                    i += buffer.Length-1;

                    if (constans.Keys.Contains(buffer)) {
                        prev_eu = new ElementaryUnit(Emums.ElementaryUnitType.Constant, buffer, i);
                    }else if (buffer.ToLower() == "x") {
                        prev_eu = new ElementaryUnit(Emums.ElementaryUnitType.Variable, buffer, i);
                    }else if (user_functions.Keys.Contains(buffer)) {
                        prev_eu = new ElementaryUnit(Emums.ElementaryUnitType.UserFunctions, buffer, i);
                    }else {
                        throw new Exception("Unknown function '"+buffer+"' at position "+i+" ");
                    }

                    result.Add(prev_eu);
                    continue;
                }

                if (char.IsDigit(c) )
                {
                    string buffer = string.Empty;
                    var j = i;
                    for (; j < expression.Length && (char.IsDigit(expression[j]) || expression[j] == '.'); j++)
                    {
                        buffer += expression[j];
                    }
                    //i = j - 1;
                    i += buffer.Length-1;
                    prev_eu = new ElementaryUnit(Emums.ElementaryUnitType.Digit, buffer, i);
                    result.Add(prev_eu);
                    continue;
                }

                if (c == ',') {
                    prev_eu = new ElementaryUnit(Emums.ElementaryUnitType.Comma, c.ToString(), i);
                    result.Add(prev_eu);
                    continue;
                }
                throw new Exception("Unknown char '" + c + "' at position " + i);
            }

            return new Expression(result);
        }

        private static List<string> list_operators_priority = new List<string> { "%", "^", "*/", "+-", };
        private static int index_operator_priority(ElementaryUnit eu ) {
            string str = eu.Value.ToString();
            int index = list_operators_priority.FindIndex(x => x.Contains(str));
            if (index < 0) {
                throw new Exception("Неизвестный оператор '"+str+"' в позиции "+eu.Position+";");
            }
            return index;
        }
        private static List<string> list_syntax_priority = new List<string> {  ",", "})", "{(", };
        private static int index_syntax_priority( string str ) {
            int index = list_syntax_priority.FindIndex(x => x.Contains(str));
            return index;
        }

        private static string oposite_bracket(string str ) {
            switch (str) {
                case "{":
                    return "}";
                case "}":
                    return "{";
                case "(":
                    return ")";
                case ")":
                    return "(";
            }
            throw new Exception("Неизвестная скобка '"+str+"'");
        }

        private static List<ElementaryUnitType> list_functions = new List<ElementaryUnitType> { ElementaryUnitType.UserFunctions };
        private static List<ElementaryUnitType> list_arguments = new List<ElementaryUnitType> { ElementaryUnitType.Digit, ElementaryUnitType.Variable, ElementaryUnitType.Constant, ElementaryUnitType.List };
        private static List<ElementaryUnitType> list_syntax = new List<ElementaryUnitType> { ElementaryUnitType.RoundBrackets, ElementaryUnitType.CurveBrackets, ElementaryUnitType.Comma};
        public static Expression ToPolishNotation( Expression expression ) {
            var result = new List<ElementaryUnit>();
            var buffer = new Stack<ElementaryUnit>();

            // Приоритеты операций:
            string[] operations_priority = new string[] { "})", "{(", ",", "%", "^", "*/", "+-" };



            ElementaryUnit prev_eu = null;
            foreach (ElementaryUnit el in expression) {
                if (list_arguments.Contains(el.Type) == true) {
                    prev_eu = el;
                    result.Add(el);
                    continue;
                }
                if (list_functions.Contains(el.Type) == true) {
                    prev_eu = el;
                    buffer.Push(el);
                    continue;
                }
                if(list_syntax.Contains(el.Type) == true) {
                    if( "({".Contains(el.Value.ToString())==true) {
                        buffer.Push(el);
                        result.Add(el);
                        continue;
                    }
                    // Достать всё из буфера до предыдущего синтаксического элемента не больше такого же веса
                    while (buffer.Count > 0) {
                        ElementaryUnit buffer_peek = buffer.Peek();
                        ElementaryUnit buffer_pop = null;
                        if (list_syntax.Contains(buffer_peek.Type) == false) {
                            buffer_pop =  buffer.Pop();
                            result.Add(buffer_pop);
                            continue;
                        }

                        if (")}".Contains( el.Value.ToString() )==true ) {
                            result.Add(el);
                            buffer_pop = buffer.Pop(); // Выкинуть открывающую скобку
                            if( oposite_bracket(el.Value.ToString())==buffer_pop.Value.ToString()==false) {
                                throw new Exception("Несогласованы скобки в позициях '"+ buffer_pop.Value.ToString() + "' в '"+buffer_pop.Position+"' и '"+ el.Value.ToString() + "' в '"+el.Position+"'");
                            }
                            // Если после закрывающей скобки в буфере есть функция, то перенести и её, т.к. в скобках были её аргументы:
                            if( buffer.Count>0 && ")".Contains(el.Value.ToString()) == true && buffer.Peek().Type == ElementaryUnitType.UserFunctions) {
                                result.Add(buffer.Pop());
                            }
                            break;
                        }
                        result.Add(el);
                        break;
                    }
                    continue;

                }

                // Обработка операторов:
                if (buffer.Count > 0) {
                    if (list_syntax.Contains(buffer.Peek().Type) == true) {
                    } else {
                        // Выдавливать из буфера в результат операторы, если они выше по приоритету, т.к. должны выполниться раньше
                        while (buffer.Count > 0 && list_syntax.Contains(buffer.Peek().Type)==false && index_operator_priority(el) >= index_operator_priority(buffer.Peek())) {
                            result.Add(buffer.Pop());
                        }
                    }
                }

                buffer.Push(el);
                continue;
            }
            while (buffer.Count != 0) {
                result.Add(buffer.Pop());
            }

            return new Expression(result);
        }

        public static Expression ToPolishNotation00(Expression expression)
        {
            var result = new List<ElementaryUnit>();
            var buffer = new Stack<ElementaryUnit>();

            // Приоритеты операций:
            string[] operations_priority = new string[] { "})", "{(", ",", "%", "^", "*/", "+-" };

            List<ElementaryUnitType> list_functions = new List<ElementaryUnitType> { ElementaryUnitType.UserFunctions };
            List<ElementaryUnitType> list_params = new List<ElementaryUnitType> { ElementaryUnitType.Digit, ElementaryUnitType.Variable, ElementaryUnitType.Constant};


            ElementaryUnit prev_eu = null;
            foreach (ElementaryUnit el in expression)
            {
                if (list_params.Contains(el.Type)==true) {
                    if (prev_eu!=null && prev_eu.Type == ElementaryUnitType.Digit) {
                        while (buffer.Count > 0 && !(buffer.Peek().Type == Emums.ElementaryUnitType.CurveBrackets && (string)(buffer.Peek().Value) == "{")) {
                            result.Add(buffer.Pop());
                        }
                        if (buffer.Count == 0 || !(buffer.Peek().Type == Emums.ElementaryUnitType.CurveBrackets && (string)(buffer.Peek().Value) == "{")) {
                            throw new Exception("Не найдена отрывающая фигурная скобка для элемента '" + el.ToString() + "' в позиции '" + el.Position + "'");
                        }
                    }
                    prev_eu = el;
                    result.Add(el);
                    continue;
                }
                if (list_functions.Contains(el.Type)==true) {
                    prev_eu = el;
                    buffer.Push(el);
                    continue;
                }

                //if (el.Type == Emums.ElementaryUnitType.Brackets || el.Type== Emums.ElementaryUnitType.CurveBrackets)
                if( new[]{ Emums.ElementaryUnitType.RoundBrackets, Emums.ElementaryUnitType.CurveBrackets }.Contains(el.Type)==true )
                {
                    if (")}".Contains(el.Value.ToString()) == true) // == ")" || el.Value.ToString() == "}")
                    {
                        string oposide_bracket = Convert.ToString("({"[")}".IndexOf(el.Value.ToString())]);
                        if (buffer.Count == 0) {
                            throw new Exception("Wrong bracket in position " + el.Position);
                        }
                        while (buffer.Peek().Value.ToString() != oposide_bracket) {
                            ElementaryUnit last_unit = buffer.Pop();
                            result.Add(last_unit);
                            if (buffer.Count == 0) {
                                throw new Exception("Wrong bracket in position " + el.Position);
                            }
                        }
                        prev_eu = el;
                        result.Add(el);
                        ElementaryUnit open_bracket = buffer.Pop();
                        if (buffer.Count > 0) {
                            if (list_functions.Contains(buffer.Peek().Type)) {
                                ElementaryUnit eu = buffer.Pop();
                                prev_eu = el;
                                result.Add(eu);
                            }
                        }
                        continue;
                    } else if ("{".Contains(el.Value.ToString()) == true) {
                        // Список может быть либо внутри функции, либо внутри другого списка. Скинуть все операции из буфера до открывающей круглой или фигурной скобки:
                        if (buffer.Count > 0) {
                            while (buffer.Count > 0 &&
                                !(buffer.Peek().Type == Emums.ElementaryUnitType.CurveBrackets && (string)(buffer.Peek().Value) == "{" || buffer.Peek().Type == Emums.ElementaryUnitType.RoundBrackets && (string)(buffer.Peek().Value) == "(")) {
                                result.Add(buffer.Pop());
                            }
                            if (buffer.Count == 0 || !(buffer.Peek().Type == Emums.ElementaryUnitType.CurveBrackets && (string)(buffer.Peek().Value) == "{" || buffer.Peek().Type == Emums.ElementaryUnitType.RoundBrackets && (string)(buffer.Peek().Value) == "(")) {
                                throw new Exception("Не найдена отрывающая круглая или фигурная скобка для элемента '" + el.ToString() + "' в позиции '" + el.Position + "'");
                            }
                        }
                        prev_eu = el;
                        result.Add(el);
                    }
                    prev_eu = el;
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
                    }
                    prev_eu = el;
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

        //private static Func<double,double> ToFunc(Expression expression)
        //{
        //    return (x) =>
        //    {
        //        return Calculate(expression, x);
        //    };
        //}

        private static Func<ElementaryUnit, ElementaryUnit> ToFunc( Expression expression ) {
            return ( x ) => {
                return Calculate(expression, x);
            };
        }

        //private static double Calculate( Expression expression, double x ) {
        //    return _Calculate(expression, x);
        //}

        private static ElementaryUnit Calculate( Expression expression, ElementaryUnit x ) {
            var stack = new Stack<ElementaryUnit>();
            List<ElementaryUnitType> list_params = new List<ElementaryUnitType> {
                ElementaryUnitType.Digit, ElementaryUnitType.Variable, ElementaryUnitType.Constant, ElementaryUnitType.RoundBrackets, ElementaryUnitType.CurveBrackets
            };
            foreach (ElementaryUnit el in expression) {

                if(list_syntax.Contains(el.Type) == true) {
                    if("({".Contains(el.Value.ToString()) == true) {
                        stack.Push(el);
                        continue;
                    }else if ("})".Contains(el.Value.ToString()) == true) {
                        Stack<ElementaryUnit> temp_list = new Stack<ElementaryUnit>();
                        while (stack.Count > 0) {
                            ElementaryUnit stack_pop = stack.Pop();
                            if (list_syntax.Contains(stack_pop.Type) == false) {
                                temp_list.Push(stack_pop);
                                continue;
                            } else if (stack_pop.Type == ElementaryUnitType.Comma) {
                                continue;
                            } else if (stack_pop.Value.ToString() == oposite_bracket(el.Value.ToString())) {
                                break;
                            } else {
                                throw new Exception("Ошибка обработки параметров в позиции " + el.Position + ". Проверьте скобки в позиции '" + el.Position + "' и '" + stack_pop.Position + "'");
                            }
                        }
                        ElementaryUnit _res = null;
                        switch (el.Value.ToString()) {
                            case "}":
                                _res = new ElementaryUnit(ElementaryUnitType.List, temp_list);
                                break;
                            case ")":
                                _res = new ElementaryUnit(ElementaryUnitType.ArgumentList, temp_list);
                                break;
                        }
                        stack.Push(_res);
                        continue;
                    }else {
                        //throw new Exception("Ошибка синтаксиса в позиции "+el.Position+" '"+el.Value.ToString()+"'");
                        stack.Push(el);  // Сюда попадает запятая
                        continue;
                    }
                }

                if(list_arguments.Contains(el.Type) == true) {
                    if (el.Type == Emums.ElementaryUnitType.Variable) {
                        stack.Push(x);
                        continue;
                    } else if (el.Type == Emums.ElementaryUnitType.Constant) {
                        stack.Push(new ElementaryUnit(ElementaryUnitType.Digit, constans[el.Value.ToString()].ToString(), el.Position));
                        continue;
                    }
                    stack.Push(el);
                    continue;
                }

                Stack<ElementaryUnit> stack_args = new Stack<ElementaryUnit>();
                ElementaryUnit res = null;
                string func_name = el.Value.ToString();

                if (el.Type == Emums.ElementaryUnitType.BinaryOperation) {
                    // Для бинарной операции нужно два операнда:
                    if (stack.Count >= 2) {
                        for (int i = 0; i <= 1; i++) {
                            ElementaryUnit stack_pop = stack.Pop();
                            stack_args.Push(stack_pop);
                            // Проверка типов для операции выполняется позже.
                        }
                    } else {
                        throw new Exception("Надостаточно операндов для выполнения '" + el.Value.ToString() + "' в позиции '" + el.Position + "'.");
                    }
                } else {
                    ElementaryUnit stack_pop = stack.Pop();
                    stack_args.Push(stack_pop);
                }

                if (el.Type == Emums.ElementaryUnitType.BinaryOperation) {
                    switch (stack_args.Count) {
                        case 2: {
                                // Извлечение Pop даёт аргументы в правильной последовательности
                                ElementaryUnit param1 = stack_args.Pop();
                                ElementaryUnit param2 = stack_args.Pop();

                                double _param1 = 0;
                                double _param2 = 0;
                                // Упростить аргументы. Если они были указаны внутри круглых скобок, то нужно их извлечь из скобок:
                                {
                                    List<ElementaryUnit> lst = new List<ElementaryUnit> { param1, param2 };
                                    for (int i = 0; i <= lst.Count - 1; i++) {
                                        ElementaryUnit param = lst.ElementAt(i);
                                        while (param.Type == ElementaryUnitType.ArgumentList) {
                                            Stack<ElementaryUnit> inner = ((Stack<ElementaryUnit>)param.Value);
                                            if (inner.Count != 1) {
                                                if (inner.Count == 0) {
                                                    throw new Exception((i + 1) + "-й аргумент в операции '" + el.Value.ToString() + "' не является правильный операндом. Нет операнда при извлечении из скобок.");
                                                } else {
                                                    throw new Exception((i + 1) + "-й аргумент в операции '" + el.Value.ToString() + "' не является правильный операндом. Количество операндов в кргулых скобках равно " + inner.Count + ". Должен быть 1.");
                                                }
                                            } else {
                                                param = inner.Pop();
                                            }
                                        }
                                        lst[i] = param;
                                    }
                                    param1 = lst[0];
                                    param2 = lst[1];

                                    List<double> lst_d = new List<double> { 0, 0 };
                                    for( int i=0; i<=lst.Count-1; i++) {
                                        ElementaryUnit param = lst.ElementAt(i);
                                        // Преобразовать в double оба параметра:
                                        if (param.Value is string) {
                                            string val1 = (string)param.Value;
                                            lst_d[i] = Convert.ToDouble(val1.Replace('.', ','));
                                        } else if (param.Value is Double || param.Value is int) {
                                            lst_d[i] = Convert.ToDouble(param.Value);
                                        } else {
                                            throw new Exception("The "+i+" argument of '" + func_name + "' have to be a 'Digit'-type. It is " + param.Type + " ");
                                        }
                                    }
                                    _param1 = lst_d[0];
                                    _param2 = lst_d[1];
                                }

                                double _res = 0;
                                switch (func_name) {
                                    case "+": _res = _param1 + _param2; break;
                                    case "-": _res = _param1 - _param2; break;
                                    case "/": _res = _param1 / _param2; break;
                                    case "*": _res = _param1 * _param2; break;
                                    case "^": _res = Math.Pow(_param1, _param2); break;
                                    case "%": _res = _param1 % _param2; break;
                                    default:
                                        throw new Exception("Unknown operator '" + func_name + "' at position " + el.Position);
                                }
                                res = new ElementaryUnit(ElementaryUnitType.Digit, _res);
                            }
                            break;
                        default:
                            throw new Exception("Operator '" + el.Value + "' must have 1 or 2 arguments. Count arguments: " + stack_args.Count + " ");
                    }
                } else {
                    Func<Stack<ElementaryUnit>, ElementaryUnit> func = user_functions[func_name];  // el.Value.ToString()
                    if (func != null) {
                        ElementaryUnit _param = stack_args.Pop();
                        Stack<ElementaryUnit> param = (Stack<ElementaryUnit>)_param.Value;
                        while (_param.Type == ElementaryUnitType.ArgumentList) {
                            Stack<ElementaryUnit> inner = ((Stack<ElementaryUnit>)_param.Value);
                            if (inner.Count>0 ) {
                                if (inner.Peek().Type == ElementaryUnitType.ArgumentList) {
                                    if (inner.Count == 1) {
                                        _param = inner.Pop();
                                        continue;
                                    }else {
                                        throw new Exception("Неверное количество аргументов при вызове функции '"+func_name+"'.");
                                    }
                                }
                            }
                            param = inner;
                            break;
                        }
                        //res = func(stack_args);
                        res = func( param );
                    } else {
                        throw new Exception("Неизвестная функция '" + func_name + "' в позиции " + el.Position);
                    }
                }
                //stack.Push(new ElementaryUnit(ElementaryUnitType.Digit, res.ToString()));
                stack.Push(res);
            }


            // Если вокруг последнего аргумента находятся скобки, то удалить все симметричные скобки:
            if (stack.Count > 0 && stack.Peek().Value.ToString() == ")") {
                // Найти предыдущую скобку и уничтожить её, сохранив аргументы:
                Stack<ElementaryUnit> temp = new Stack<ElementaryUnit>();
                // Выкинуть первую закрывающую скобку:
                stack.Pop();
                while (stack.Peek().Value.ToString() != "(") {
                    if (stack.Count == 0) {
                        throw new Exception("Неверное количество открывающих и закрывающих скобок. поз:" + stack.Peek().Position);
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
            ElementaryUnit eu_pop = stack.Pop();
            return eu_pop;
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
            string res = string.Empty;
            foreach(ElementaryUnit el in expression)
            {
                switch (el.Type) {
                    //case ElementaryUnitType.Digit:
                    //    res += el.Value + " ";
                    //    break;
                    case ElementaryUnitType.List:
                        res += "{";
                        List<ElementaryUnit> el_list = (List<ElementaryUnit>)el.Value;
                        for(int i=0; i<=el_list.Count-1; i++) {
                            ElementaryUnit el_list_i = el_list.ElementAt(i);
                            if(el_list_i.Type == ElementaryUnitType.Digit) {
                                res += el_list_i.Value.ToString();
                            }
                            if(i < el_list.Count - 1) {
                                res += ",";
                            }
                        }
                        res += "}";
                        break;
                    default:
                        //throw new Exception("Gotten wrong result. Type of result is '"+el.Type+"';'"+el.Value+"'");
                        res += el.Value + " ";
                        break;
                }
            }
            return res;
        }

        //public static Func<double,double> ToDelegate(string expression)
        //{
        //    Expression exp = ToExpresion(expression);
        //    Expression inversePolishNotation = ToPolishNotation(exp);
        //    return ToFunc(inversePolishNotation);
        //}

        public static Func<ElementaryUnit, ElementaryUnit> ToDelegate( string expression ) {
            Expression exp = ToExpresion(expression);
            Expression inversePolishNotation = ToPolishNotation(exp);
            return ToFunc(inversePolishNotation);
        }


        //public static double Calculate(string expression, double x = 0)
        //{
        //    var exp = ToExpresion(expression);
        //    var inversePolishNotation = ToPolishNotation(exp);
        //    return Calculate(inversePolishNotation, x);
        //}

        //public static double Calculate( Expression exp ) {
        //    var inversePolishNotation = exp;
        //    return Calculate(inversePolishNotation, 0);
        //}
        public static ElementaryUnit Calculate( Expression exp ) {
            var inversePolishNotation = exp;
            return Calculate(inversePolishNotation, new ElementaryUnit(ElementaryUnitType.Digit, 0) );
        }

        IEnumerator<ElementaryUnit> IEnumerable<ElementaryUnit>.GetEnumerator()
        {
            return expression.GetEnumerator();
        }
    }
}
