
namespace SassParser
{
    internal sealed class StrokeLinejoinProperty : Property
    {
        private static readonly IValueConverter StyleConverter = Converters.StrokeLinejoinConverter;

        public StrokeLinejoinProperty(Token token) : base(PropertyNames.StrokeLinejoin, token, PropertyFlags.Animatable)
        {
        }

        internal override IValueConverter Converter => StyleConverter;
    }
}