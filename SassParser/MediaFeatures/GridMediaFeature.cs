
namespace SassParser
{
    internal sealed class GridMediaFeature : MediaFeature
    {
        public GridMediaFeature(Token token) : base(FeatureNames.Grid, token)
        {
        }

        internal override IValueConverter Converter => Converters.BinaryConverter;
    }
}