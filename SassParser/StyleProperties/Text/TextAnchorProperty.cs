
namespace SassParser
{
    internal sealed class TextAnchorProperty : Property
    {
        private static readonly IValueConverter StyleConverter = Converters.TextAnchorConverter;

        public TextAnchorProperty(Token token) : base(PropertyNames.TextAnchor, token)
        {
        }

        internal override IValueConverter Converter => StyleConverter;
    }
}