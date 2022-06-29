
namespace SassParser
{
    internal sealed class FillProperty : Property
    {
        private static readonly IValueConverter StyleConverter = Converters.PaintConverter;

        internal FillProperty(Token token) : base(PropertyNames.Fill, token, PropertyFlags.Animatable)
        {
        }

        internal override IValueConverter Converter => StyleConverter;
    }
}