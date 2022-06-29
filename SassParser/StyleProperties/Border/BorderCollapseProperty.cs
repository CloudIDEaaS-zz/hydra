
namespace SassParser
{
    internal sealed class BorderCollapseProperty : Property
    {
        private static readonly IValueConverter StyleConverter = Converters.BorderCollapseConverter.OrDefault(true);

        internal BorderCollapseProperty(Token token) : base(PropertyNames.BorderCollapse, token, PropertyFlags.Inherited)
        {
        }

        internal override IValueConverter Converter => StyleConverter;
    }
}