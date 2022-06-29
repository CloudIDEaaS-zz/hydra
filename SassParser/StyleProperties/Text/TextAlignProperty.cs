
namespace SassParser
{
    internal sealed class TextAlignProperty : Property
    {
        private static readonly IValueConverter StyleConverter =
            Converters.HorizontalAlignmentConverter.OrDefault(HorizontalAlignment.Left);

        internal TextAlignProperty(Token token) : base(PropertyNames.TextAlign, token, PropertyFlags.Inherited)
        {
        }

        internal override IValueConverter Converter => StyleConverter;
    }
}