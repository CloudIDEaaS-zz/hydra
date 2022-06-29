
namespace SassParser
{
    internal sealed class BorderImageOutsetProperty : Property
    {
        internal static readonly IValueConverter TheConverter = Converters.LengthOrPercentConverter.Periodic();
        private static readonly IValueConverter StyleConverter = TheConverter.OrDefault(Length.Zero);

        internal BorderImageOutsetProperty(Token token) : base(PropertyNames.BorderImageOutset, token)
        {
        }

        internal override IValueConverter Converter => StyleConverter;
    }
}