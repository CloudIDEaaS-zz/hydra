namespace AbstraX.XPathBuilder
{
    public class XPathLiteral : IXPathOperand
    {
        public object Value { get; }
        public string Name => this.Value.ToString();

        public XPathLiteral(object value)
        {
            this.Value = value;
        }

        public override string ToString()
        {
            return this.Name;
        }
    }
}