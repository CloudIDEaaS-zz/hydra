
namespace SassParser
{
    internal sealed class LineHeightProperty : Property
    {
        private static readonly IValueConverter StyleConverter =
            Converters.LineHeightConverter.OrDefault(new Length(120f, Length.Unit.Percent));

        internal LineHeightProperty(Token token) : base(PropertyNames.LineHeight, token, PropertyFlags.Inherited | PropertyFlags.Animatable)
        {
        }

        internal override IValueConverter Converter => StyleConverter;
    }
}