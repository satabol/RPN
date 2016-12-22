namespace PolishNotation
{
    using Emums;

    internal class ElementaryUnit
    {
        private ElementaryUnitType type;

        private object value;

        public ElementaryUnitType Type { get { return type; } }

        public object Value { get { return value; } }

        public ElementaryUnit(ElementaryUnitType type, object value, int _Position=-1 )
        {
            this.type = type;
            this.value = value;
            this.Position = _Position;
        }

        public int Position { get; set; }

        public ElementaryUnit first_param=null; // Для функций важно знать, какой из аргументов будет первым, чтобы делать выборку из стека не по количеству аргументов, которые может принять в себя функция, а до первого аргумента (аргументы в стек помещаются в обратной последовательности, поэтому первый аргумент в обычной записи оказывается последним в стеке).
    }
}
