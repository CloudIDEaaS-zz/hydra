
namespace SassParser
{
    using static Converters;

    internal sealed class FontWeightProperty : Property
    {
        private static readonly IValueConverter StyleConverter = FontWeightConverter.Or(
            WeightIntegerConverter).OrDefault(FontWeight.Normal);

        internal FontWeightProperty(Token token) : base(PropertyNames.FontWeight, token, PropertyFlags.Inherited | PropertyFlags.Animatable)
        {
        }

        internal override IValueConverter Converter => StyleConverter;
    }
}