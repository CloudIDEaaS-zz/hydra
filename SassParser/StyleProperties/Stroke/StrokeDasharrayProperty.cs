
namespace SassParser
{
    internal sealed class StrokeDasharrayProperty : Property
    {
        private static readonly IValueConverter StyleConverter = Converters.StrokeDasharrayConverter;

        public StrokeDasharrayProperty(Token token) : base(PropertyNames.StrokeDasharray, token, PropertyFlags.Animatable | PropertyFlags.Unitless)
        {
        }

        internal override IValueConverter Converter => StyleConverter;
    }
}