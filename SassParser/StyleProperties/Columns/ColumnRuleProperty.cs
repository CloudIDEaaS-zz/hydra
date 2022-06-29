
namespace SassParser
{
    using static Converters;

    internal sealed class ColumnRuleProperty : ShorthandProperty
    {
        private static readonly IValueConverter StyleConverter = WithAny(
            ColorConverter.Option().For(PropertyNames.ColumnRuleColor),
            LineWidthConverter.Option().For(PropertyNames.ColumnRuleWidth),
            LineStyleConverter.Option().For(PropertyNames.ColumnRuleStyle)).OrDefault();

        internal ColumnRuleProperty(Token token) : base(PropertyNames.ColumnRule, token, PropertyFlags.Animatable)
        {
        }

        internal override IValueConverter Converter => StyleConverter;
    }
}