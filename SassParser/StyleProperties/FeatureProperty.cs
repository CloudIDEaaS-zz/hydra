
namespace SassParser
{
    internal sealed class FeatureProperty : Property
    {
        internal FeatureProperty(MediaFeature feature, Token token) : base(feature.Name, token)
        {
            Feature = feature;
        }


        internal override IValueConverter Converter => Feature.Converter;

        internal MediaFeature Feature { get; }
    }
}