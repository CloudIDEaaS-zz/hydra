
namespace SassParser
{
    internal sealed class OrientationMediaFeature : MediaFeature
    {
        private static readonly IValueConverter TheConverter = Converters.Toggle(Keywords.Portrait, Keywords.Landscape);

        public OrientationMediaFeature(Token token) : base(FeatureNames.Orientation, token)
        {
        }

        internal override IValueConverter Converter => TheConverter;
    }
}