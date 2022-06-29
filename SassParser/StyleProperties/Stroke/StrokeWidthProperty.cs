
namespace SassParser
{
    internal sealed class StrokeWidthProperty : Property
    {
        private static readonly IValueConverter StyleConverter = Converters.LengthOrPercentConverter;

        internal StrokeWidthProperty(Token token) : base(PropertyNames.StrokeWidth, token, PropertyFlags.Animatable)
        {
        }

        internal override IValueConverter Converter => StyleConverter;
    }
}