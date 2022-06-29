using System;
using System.IO;
using System.Linq;

namespace SassParser
{
    internal sealed class SupportsRule : ConditionRule, ISupportsRule
    {
        internal SupportsRule(Token token, StylesheetParser parser) : base(RuleType.Supports, token, parser)
        {
        }

        public override void ToCss(TextWriter writer, IStyleFormatter formatter)
        {
            var rules = formatter.Block(Rules);
            writer.Write(formatter.Rule("@supports", ConditionText, rules));
        }


        public string ConditionText
        {
            get => Condition.ToCss();
            set
            {
                var condition = Parser.ParseCondition(value);

                Condition = condition ?? throw new ParseException("Unable to parse condition");
            }
        }

        public IConditionFunction Condition
        {
            get => Children.OfType<IConditionFunction>().FirstOrDefault() ?? new EmptyCondition(null);
            set
            {
                if (value == null)
                {
                    return;
                }

                RemoveChild(Condition);
                AppendChild(value);
            }
        }
    }
}