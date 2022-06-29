
namespace SassParser
{
    internal sealed class WidthMediaFeature : MediaFeature
    {
        public WidthMediaFeature(string name, Token token) : base(name, token)
        {
        }

        internal override IValueConverter Converter => Converters.LengthConverter;
    }
}