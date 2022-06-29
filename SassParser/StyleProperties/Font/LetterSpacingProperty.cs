
namespace SassParser
{
    internal sealed class LetterSpacingProperty : Property
    {
        private static readonly IValueConverter StyleConverter = Converters.OptionalLengthConverter.OrDefault();

        internal LetterSpacingProperty(Token token) : base(PropertyNames.LetterSpacing, token, PropertyFlags.Inherited | PropertyFlags.Unitless)
        {
        }

        internal override IValueConverter Converter => StyleConverter;
    }
}