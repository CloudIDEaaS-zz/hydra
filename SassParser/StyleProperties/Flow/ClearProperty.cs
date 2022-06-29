
namespace SassParser
{
    internal sealed class ClearProperty : Property
    {
        private static readonly IValueConverter StyleConverter = Converters.ClearModeConverter.OrDefault(ClearMode.None);

        internal ClearProperty(Token token) : base(PropertyNames.Clear, token)
        {
        }

        internal override IValueConverter Converter => StyleConverter;
    }
}