namespace PolishNotation
{
    using Emums;
    using System.Collections.Generic;
    using System.Linq;

    public class ElementaryUnit
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

        //public ElementaryUnit first_param=null; // Для функций важно знать, какой из аргументов будет первым, чтобы делать выборку из стека не по количеству аргументов, которые может принять в себя функция, а до первого аргумента (аргументы в стек помещаются в обратной последовательности, поэтому первый аргумент в обычной записи оказывается последним в стеке).

        public override string ToString() {
            string res = string.Empty;
            if (Type == ElementaryUnitType.List) {
                Stack<ElementaryUnit> expression = (Stack<ElementaryUnit>)value;
                res += "{";
                bool is_first = false;
                foreach (ElementaryUnit el in expression) {
                    if (is_first == true) {
                        res += ",";
                    }
                    switch (el.Type) {
                        //case ElementaryUnitType.Digit:
                        //    res += el.Value + " ";
                        //    break;
                        case ElementaryUnitType.List:
                            res += el.ToString();
                            break;
                        case ElementaryUnitType.ArgumentList:
                            res += el.ToString() + "";
                            break;
                        default:
                            //throw new Exception("Gotten wrong result. Type of result is '"+el.Type+"';'"+el.Value+"'");
                            res += el.Value.ToString() + "";
                            break;
                    }
                    is_first = true;
                }
                res += "}";
            } else if (Type == ElementaryUnitType.ArgumentList) {
                Stack<ElementaryUnit> eu = (Stack<ElementaryUnit>)Value;
                if (eu.Count==1){
                    if(eu.Peek().Type == ElementaryUnitType.ArgumentList) {
                        res += eu.Peek().ToString();
                    }else {
                        string _res = "";
                        foreach (ElementaryUnit el in eu) { 
                            //while (eu.Count > 0) {
                            _res += el.ToString();
                        }
                        if(eu.Count != 1) {
                            res += "(";
                            res += _res;
                            res += ")";
                        }else {
                            res += _res;
                        }
                    }
                }
            } else {
                res = value.ToString();
            }
            return res;
        }

    }
}
