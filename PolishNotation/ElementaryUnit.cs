namespace PolishNotation
{
    using Emums;

    internal class ElementaryUnit
    {
        private ElementaryUnitType type;

        private string value;

        public ElementaryUnitType Type { get { return type; } }

        public string Value { get { return value; } }

        public ElementaryUnit(ElementaryUnitType type, string value)
        {
            this.type = type;
            this.value = value;
        }
    }
}
