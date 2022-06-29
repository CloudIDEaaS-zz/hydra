
namespace SassParser
{
    internal sealed class ColumnRuleWidthProperty : Property
    {
        private static readonly IValueConverter StyleConverter = Converters.LineWidthConverter.OrDefault(Length.Medium);

        internal ColumnRuleWidthProperty(Token token) : base(PropertyNames.ColumnRuleWidth, token, PropertyFlags.Animatable)
        {
        }

        internal override IValueConverter Converter => StyleConverter;
    }
}