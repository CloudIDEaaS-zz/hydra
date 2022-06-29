
namespace SassParser
{
    internal sealed class StrokeLinecapProperty : Property
    {
        private static readonly IValueConverter StyleConverter =
            Converters.StrokeLinecapConverter.OrDefault(StrokeLinecap.Butt);

        public StrokeLinecapProperty(Token token) : base(PropertyNames.StrokeLinecap, token, PropertyFlags.Animatable)
        {
        }

        internal override IValueConverter Converter => StyleConverter;
    }
}