
namespace SassParser
{
    using static Converters;

    internal sealed class ColorMediaFeature : MediaFeature
    {
        public ColorMediaFeature(string name, Token token) : base(name, token)
        {
        }

        internal override IValueConverter Converter => 
            IsMinimum || IsMaximum 
                ? PositiveIntegerConverter
                : PositiveIntegerConverter.Option(1);
    }
}