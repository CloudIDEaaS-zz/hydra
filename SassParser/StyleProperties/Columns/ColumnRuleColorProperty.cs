
namespace SassParser
{
    internal sealed class ColumnRuleColorProperty : Property
    {
        private static readonly IValueConverter StyleConverter = Converters.ColorConverter.OrDefault(Color.Transparent);

        internal ColumnRuleColorProperty(Token token) : base(PropertyNames.ColumnRuleColor, token, PropertyFlags.Animatable)
        {
        }

        internal override IValueConverter Converter => StyleConverter;
    }
}