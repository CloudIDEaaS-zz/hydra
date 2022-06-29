
namespace SassParser
{
    using static Converters;

    internal sealed class CursorProperty : Property
    {
        private static readonly IValueConverter StyleConverter = ImageSourceConverter.Or(
            WithOrder(
                ImageSourceConverter.Required(),
                NumberConverter.Required(),
                NumberConverter.Required())).RequiresEnd(
            Map.Cursors.ToConverter()).OrDefault(SystemCursor.Auto);

        internal CursorProperty(Token token) : base(PropertyNames.Cursor, token, PropertyFlags.Inherited)
        {
        }

        internal override IValueConverter Converter => StyleConverter;
    }
}