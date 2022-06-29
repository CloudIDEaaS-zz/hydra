
namespace SassParser
{
    using static Converters;

    internal sealed class ColorIndexMediaFeature : MediaFeature
    {
        public ColorIndexMediaFeature(string name, Token token) : base(name, token)
        {
        }

        internal override IValueConverter Converter => 
            IsMinimum || IsMaximum 
                ? NaturalIntegerConverter 
                : NaturalIntegerConverter.Option(1);
    }
}