
namespace SassParser
{
    internal sealed class DeviceHeightMediaFeature : MediaFeature
    {
        public DeviceHeightMediaFeature(string name, Token token) : base(name, token)
        {
        }

        internal override IValueConverter Converter => Converters.LengthConverter;
    }
}