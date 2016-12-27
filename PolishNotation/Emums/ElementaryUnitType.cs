namespace PolishNotation.Emums
{
    public enum ElementaryUnitType
    {
        Digit,
        BinaryOperation,
        Comma,    //  Оператор разделителя аргументов функций и списков
        Operator, // +,-,*,/,%,^
        //UnaryOperation,
        //UnaryFunction,
        //BinaryFunction,
        UserFunctions,
        RoundBrackets, CurveBrackets,
        List,   // Просто списки
        ArgumentList, // Аргументы функций. Не может быть результатом вычисления.
        Variable,
        Constant
    }
}
