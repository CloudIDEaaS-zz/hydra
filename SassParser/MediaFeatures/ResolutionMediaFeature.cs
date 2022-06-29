
namespace SassParser
{
    internal sealed class ResolutionMediaFeature : MediaFeature
    {
        public ResolutionMediaFeature(string name, Token token) : base(name, token)
        {
        }

        internal override IValueConverter Converter => Converters.ResolutionConverter;
    }
}