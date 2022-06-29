using System.IO;

namespace SassParser
{
    internal sealed class CompoundSelector : Selectors, ISelector
    {
        public CompoundSelector(Token token) : base(token)
        {
        }

        public override void ToCss(TextWriter writer, IStyleFormatter formatter)
        {
            foreach (var selector in _selectors)
            {
                writer.Write(selector.Text);
            }
        }
    }
}