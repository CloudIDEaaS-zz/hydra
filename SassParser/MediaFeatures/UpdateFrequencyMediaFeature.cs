
namespace SassParser
{
    internal sealed class UpdateFrequencyMediaFeature : MediaFeature
    {
        private static readonly IValueConverter TheConverter = Map.UpdateFrequencies.ToConverter();

        public UpdateFrequencyMediaFeature(Token token) : base(FeatureNames.UpdateFrequency, token)
        {
        }

        internal override IValueConverter Converter => TheConverter;
    }
}