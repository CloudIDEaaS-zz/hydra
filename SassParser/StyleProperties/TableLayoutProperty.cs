
namespace SassParser
{
    internal sealed class TableLayoutProperty : Property
    {
        private static readonly IValueConverter StyleConverter = Converters.TableLayoutConverter.OrDefault(false);

        internal TableLayoutProperty(Token token) : base(PropertyNames.TableLayout, token)
        {
        }

        internal override IValueConverter Converter => StyleConverter;
    }
}