
namespace SassParser
{
    internal sealed class TextAlignLastProperty : Property
    {
        private static readonly IValueConverter StyleConverter = Converters.TextAlignLastConverter;

        public TextAlignLastProperty(Token token) : base(PropertyNames.TextAlignLast, token)
        {
        }

        internal override IValueConverter Converter => StyleConverter;
    }
}