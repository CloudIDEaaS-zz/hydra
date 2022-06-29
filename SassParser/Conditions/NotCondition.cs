using System.IO;

namespace SassParser
{
    internal sealed class NotCondition : StylesheetNode, IConditionFunction
    {
        private IConditionFunction _content;
     
        public NotCondition(Token token) : base(token)
        {
        }

        public IConditionFunction Content
        {
            get => _content ?? new EmptyCondition(this.Token);
            set
            {
                if (_content != null)
                {
                    RemoveChild(_content);
                }

                _content = value;

                if (value != null)
                {
                    AppendChild(_content);
                }
            }
        }

        public bool Check()
        {
            return !Content.Check();
        }

        public override void ToCss(TextWriter writer, IStyleFormatter formatter)
        {
            writer.Write("not ");
            Content.ToCss(writer, formatter);
        }
    }
}