
namespace SassParser
{
    internal sealed class BackfaceVisibilityProperty : Property
    {
        private static readonly IValueConverter StyleConverter = Converters.BackfaceVisibilityConverter.OrDefault(true);

        internal BackfaceVisibilityProperty(Token token) : base(PropertyNames.BackfaceVisibility, token)
        {
        }

        internal override IValueConverter Converter => StyleConverter;
    }
}