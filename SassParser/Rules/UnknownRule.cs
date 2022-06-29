using System.IO;

namespace SassParser
{
    internal sealed class UnknownRule : Rule
    {
        public UnknownRule(string name, Token token, StylesheetParser parser) : base(RuleType.Unknown, token, parser)
        {
            Name = name;
        }

        public string Name { get; }

        public override void ToCss(TextWriter writer, IStyleFormatter formatter)
        {
            writer.Write(StylesheetText?.Text);
        }
    }
}