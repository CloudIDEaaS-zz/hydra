
namespace SassParser
{
    internal sealed class AspectRatioMediaFeature : MediaFeature
    {
        public AspectRatioMediaFeature(string name, Token token) : base(name, token)
        {
        }

        internal override IValueConverter Converter => Converters.RatioConverter;
    }
}