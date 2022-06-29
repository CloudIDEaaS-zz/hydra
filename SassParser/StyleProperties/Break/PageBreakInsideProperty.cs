
namespace SassParser
{
    internal sealed class PageBreakInsideProperty : Property
    {

        private static readonly IValueConverter StyleConverter =
            Converters.Assign(Keywords.Auto, BreakMode.Auto)
                .Or(Keywords.Avoid, BreakMode.Avoid)
                .OrDefault(BreakMode.Auto);

        internal PageBreakInsideProperty(Token token) : base(PropertyNames.PageBreakInside, token)
        {
        }

        internal override IValueConverter Converter => StyleConverter;
    }
}