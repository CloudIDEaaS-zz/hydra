
namespace SassParser
{
    internal sealed class BackgroundPositionProperty : Property
    {
        private static readonly IValueConverter ListConverter =
            Converters.PointConverter.FromList().OrDefault(Point.Center);

        internal BackgroundPositionProperty(Token token) : base(PropertyNames.BackgroundPosition, token, PropertyFlags.Animatable)
        {
        }

        internal override IValueConverter Converter => ListConverter;
    }
}