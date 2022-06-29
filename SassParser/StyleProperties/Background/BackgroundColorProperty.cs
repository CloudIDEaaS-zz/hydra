
namespace SassParser
{
    internal sealed class BackgroundColorProperty : Property
    {
        private static readonly IValueConverter StyleConverter = Converters.CurrentColorConverter.OrDefault();

        internal BackgroundColorProperty(Token token) : base(PropertyNames.BackgroundColor, token, PropertyFlags.Hashless | PropertyFlags.Animatable)
        {
        }

        internal override IValueConverter Converter => StyleConverter;
    }
}