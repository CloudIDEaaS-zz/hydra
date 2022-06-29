using System.IO;

namespace SassParser
{
    internal sealed class EmptyCondition : StylesheetNode, IConditionFunction
    {
        public EmptyCondition(Token token) : base(token)
        {
        }

        public bool Check()
        {
            return true;
        }

        public override void ToCss(TextWriter writer, IStyleFormatter formatter)
        {
        }
    }
}