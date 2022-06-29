
namespace SassParser
{
    internal sealed class ClipProperty : Property
    {
        private static readonly IValueConverter StyleConverter = Converters.ShapeConverter.OrDefault();

        internal ClipProperty(Token token) : base(PropertyNames.Clip, token, PropertyFlags.Animatable)
        {
        }

        internal override IValueConverter Converter => StyleConverter;
    }
}