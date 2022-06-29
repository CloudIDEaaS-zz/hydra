
namespace SassParser
{
    internal sealed class MarginLeftProperty : Property
    {
        private static readonly IValueConverter StyleConverter = Converters.AutoLengthOrPercentConverter.OrDefault(Length.Zero);

        internal MarginLeftProperty(Token token) : base(PropertyNames.MarginLeft, token, PropertyFlags.Unitless | PropertyFlags.Animatable)
        {
        }

        internal override IValueConverter Converter => StyleConverter;
    }
}