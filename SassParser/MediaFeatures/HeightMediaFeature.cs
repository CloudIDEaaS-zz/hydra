
namespace SassParser
{
    internal sealed class HeightMediaFeature : MediaFeature
    {
        public HeightMediaFeature(string name, Token token) : base(name, token)
        {
        }

        internal override IValueConverter Converter => Converters.LengthConverter;
    }
}