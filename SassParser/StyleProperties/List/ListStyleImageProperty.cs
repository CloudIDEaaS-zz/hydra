
namespace SassParser
{
    internal sealed class ListStyleImageProperty : Property
    {
        private static readonly IValueConverter StyleConverter = Converters.OptionalImageSourceConverter.OrDefault();

        internal ListStyleImageProperty(Token token) : base(PropertyNames.ListStyleImage, token, PropertyFlags.Inherited)
        {
        }

        internal override IValueConverter Converter => StyleConverter;
    }
}