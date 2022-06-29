
namespace SassParser
{
    internal sealed class OutlineStyleProperty : Property
    {
        private static readonly IValueConverter StyleConverter = Converters.LineStyleConverter.OrDefault(LineStyle.None);

        internal OutlineStyleProperty(Token token) : base(PropertyNames.OutlineStyle, token)
        {
        }

        internal override IValueConverter Converter => StyleConverter;
    }
}