
namespace SassParser
{
    using static Converters;

    internal sealed class MonochromeMediaFeature : MediaFeature
    {
        public MonochromeMediaFeature(string name, Token token) : base(name, token)
        {
        }

        internal override IValueConverter Converter => IsMinimum || IsMaximum ? NaturalIntegerConverter : NaturalIntegerConverter.Option(1);
    }
}