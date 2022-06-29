using System.IO;
using System.Linq;


namespace SassParser
{
    internal sealed class PageRule : Rule, IPageRule
    {
        internal PageRule(Token token, StylesheetParser parser) : base(RuleType.Page, token, parser)
        {
            AppendChild(SimpleSelector.All);
            AppendChild(new StyleDeclaration(this, token));
        }

        public override void ToCss(TextWriter writer, IStyleFormatter formatter)
        {
            var rules = formatter.Block(Style);
            writer.Write(formatter.Rule("@page", SelectorText, rules));
        }

        public string SelectorText
        {
            get => Selector.Text;
            set => Selector = Parser.ParseSelector(value);
        }

        public ISelector Selector
        {
            get => Children.OfType<ISelector>().FirstOrDefault();
            set => ReplaceSingle(Selector, value);
        }

        public StyleDeclaration Style => Children.OfType<StyleDeclaration>().FirstOrDefault();
    }
}