
namespace SassParser
{
    internal sealed class ObjectPositionProperty : Property
    {
        private static readonly IValueConverter StyleConverter = Converters.PointConverter.OrDefault(Point.Center);

        internal ObjectPositionProperty(Token token) : base(PropertyNames.ObjectPosition, token, PropertyFlags.Animatable)
        {
        }

        internal override IValueConverter Converter => StyleConverter;
    }
}