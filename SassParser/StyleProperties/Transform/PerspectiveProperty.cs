
namespace SassParser
{
    internal sealed class PerspectiveProperty : Property
    {
        private static readonly IValueConverter StyleConverter =
            Converters.LengthConverter.OrNone().OrDefault(Length.Zero);

        internal PerspectiveProperty(Token token) : base(PropertyNames.Perspective, token, PropertyFlags.Animatable)
        {
        }

        internal override IValueConverter Converter => StyleConverter;
    }
}