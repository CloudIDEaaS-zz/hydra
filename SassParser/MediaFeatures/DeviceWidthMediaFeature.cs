
namespace SassParser
{
    internal sealed class DeviceWidthMediaFeature : MediaFeature
    {
        public DeviceWidthMediaFeature(string name, Token token) : base(name, token)
        {
        }

        internal override IValueConverter Converter => Converters.LengthConverter;
    }
}