
namespace SassParser
{
    internal sealed class FillRuleProperty : Property
    {
        private static readonly IValueConverter StyleConverter = Converters.FillRuleConverter.OrDefault(FillRule.Nonzero);

        public FillRuleProperty(Token token) : base(PropertyNames.FillRule, token, PropertyFlags.Animatable)
        {
        }

        internal override IValueConverter Converter => StyleConverter;
    }
}