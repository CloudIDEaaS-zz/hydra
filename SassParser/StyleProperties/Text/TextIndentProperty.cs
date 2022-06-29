
namespace SassParser
{
    internal sealed class TextIndentProperty : Property
    {
        private static readonly IValueConverter StyleConverter =
            Converters.LengthOrPercentConverter.OrDefault(Length.Zero);

        internal TextIndentProperty(Token token) : base(PropertyNames.TextIndent, token, PropertyFlags.Inherited | PropertyFlags.Animatable)
        {
        }

        internal override IValueConverter Converter => StyleConverter;
    }
}