
namespace SassParser
{
    internal sealed class DeviceAspectRatioMediaFeature : MediaFeature
    {
        public DeviceAspectRatioMediaFeature(string name, Token token) : base(name, token)
        {
        }

        internal override IValueConverter Converter => Converters.RatioConverter;
    }
}