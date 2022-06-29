
namespace SassParser
{
    internal sealed class ScanMediaFeature : MediaFeature
    {

        private static readonly IValueConverter TheConverter = Converters.Toggle(Keywords.Interlace, Keywords.Progressive);

        public ScanMediaFeature(Token token) : base(FeatureNames.Scan, token)
        {
        }

        internal override IValueConverter Converter => TheConverter;
    }
}