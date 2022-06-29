
namespace SassParser
{
    internal sealed class StrokeProperty : Property
    {
        private static readonly IValueConverter StyleConverter = Converters.PaintConverter;

        internal StrokeProperty(Token token) : base(PropertyNames.Stroke, token, PropertyFlags.Animatable)
        {
        }

        internal override IValueConverter Converter => StyleConverter;
    }
}