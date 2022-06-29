
namespace SassParser
{
    internal sealed class MarginTopProperty : Property
    {
        private static readonly IValueConverter StyleConverter =
            Converters.AutoLengthOrPercentConverter.OrDefault(Length.Zero);
        
        internal MarginTopProperty(Token token) : base(PropertyNames.MarginTop, token, PropertyFlags.Unitless | PropertyFlags.Animatable)
        {
        }

        internal override IValueConverter Converter => StyleConverter;
    }
}