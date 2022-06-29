
namespace SassParser
{
    internal sealed class PointerMediaFeature : MediaFeature
    {
        private static readonly IValueConverter TheConverter = Map.PointerAccuracies.ToConverter();

        public PointerMediaFeature(Token token) : base(FeatureNames.Pointer, token)
        {
        }

        internal override IValueConverter Converter => TheConverter;
    }
}