
namespace SassParser
{
    internal sealed class HoverMediaFeature : MediaFeature
    {
        private static readonly IValueConverter TheConverter = Map.HoverAbilities.ToConverter();

        public HoverMediaFeature(Token token) : base(FeatureNames.Hover, token)
        {
        }

        internal override IValueConverter Converter => TheConverter;
    }
}