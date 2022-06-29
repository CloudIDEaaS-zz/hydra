
namespace SassParser
{
    internal sealed class TextJustifyProperty : Property
    {
        private static readonly IValueConverter StyleConverter = Converters.TextJustifyConverter;

        public TextJustifyProperty(Token token) : base(PropertyNames.TextJustify, token)
        {
        }

        internal override IValueConverter Converter => StyleConverter;
    }
}