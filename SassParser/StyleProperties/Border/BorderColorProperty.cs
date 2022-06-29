
namespace SassParser
{
    internal sealed class BorderColorProperty : ShorthandProperty
    {
        private static readonly IValueConverter StyleConverter = Converters.CurrentColorConverter.Periodic(
            PropertyNames.BorderTopColor, PropertyNames.BorderRightColor, PropertyNames.BorderBottomColor,
            PropertyNames.BorderLeftColor).OrDefault();

        internal BorderColorProperty(Token token) : base(PropertyNames.BorderColor, token, PropertyFlags.Hashless | PropertyFlags.Animatable)
        {
        }

        internal override IValueConverter Converter => StyleConverter;
    }
}