
namespace SassParser
{
    internal sealed class DevicePixelRatioFeature : MediaFeature
    {
        public DevicePixelRatioFeature(string name, Token token) : base(name, token)
        {
        }

        internal override IValueConverter Converter => Converters.NaturalNumberConverter;
    }
}