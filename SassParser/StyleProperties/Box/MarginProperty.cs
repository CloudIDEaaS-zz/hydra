
namespace SassParser
{
    internal sealed class MarginProperty : ShorthandProperty
    {
        private static readonly IValueConverter StyleConverter = Converters.AutoLengthOrPercentConverter.Periodic(
                PropertyNames.MarginTop, PropertyNames.MarginRight, PropertyNames.MarginBottom, PropertyNames.MarginLeft)
            .OrDefault(Length.Zero);

        internal MarginProperty(Token token) : base(PropertyNames.Margin, token)
        {
        }

        internal override IValueConverter Converter => StyleConverter;
    }
}