
namespace SassParser
{
    internal sealed class StrokeDashoffsetProperty : Property
    {
        private static readonly IValueConverter StyleConverter = Converters.LengthOrPercentConverter;

        public StrokeDashoffsetProperty(Token token) : base(PropertyNames.StrokeDashoffset, token, PropertyFlags.Animatable)
        {
        }

        internal override IValueConverter Converter => StyleConverter;
    }
}