using System.IO;

namespace SassParser
{
    internal sealed class UnknownSelector : StylesheetNode, ISelector
    {
        public Priority Specifity => Priority.Zero;

        public string Text => this.ToCss();

        public UnknownSelector(Token token) : base(token)
        {
        }

        public override void ToCss(TextWriter writer, IStyleFormatter formatter)
        {
            writer.Write(StylesheetText?.Text);
        }
    }
}