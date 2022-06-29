
namespace SassParser
{
    internal sealed class BorderSpacingProperty : Property
    {
        private static readonly IValueConverter StyleConverter =
            Converters.LengthConverter.Many(1, 2).OrDefault(Length.Zero);

        internal BorderSpacingProperty(Token token) : base(PropertyNames.BorderSpacing, token, PropertyFlags.Inherited)
        {
        }

        internal override IValueConverter Converter => StyleConverter;
    }
}