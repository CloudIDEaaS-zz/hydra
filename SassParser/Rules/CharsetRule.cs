using System.IO;

namespace SassParser
{
    public sealed class CharsetRule : Rule
    {
        internal CharsetRule(Token token, StylesheetParser parser) : base(RuleType.Charset, token, parser)
        {
        }

        public string CharacterSet { get; set; }

        public override void ToCss(TextWriter writer, IStyleFormatter formatter)
        {
            writer.Write(formatter.Rule("@charset", CharacterSet.StylesheetString()));
        }

        protected override void ReplaceWith(IRule rule)
        {
            var newRule = rule as CharsetRule;
            CharacterSet = newRule?.CharacterSet;
            base.ReplaceWith(rule);
        }
    }
}