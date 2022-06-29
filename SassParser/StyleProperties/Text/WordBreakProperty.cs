
namespace SassParser
{
    internal sealed class WordBreakProperty : Property
    {
        private static readonly IValueConverter StyleConverter = Converters.WordBreakConverter;

        public WordBreakProperty(Token token) : base(PropertyNames.WordBreak, token)
        {
        }
        internal override IValueConverter Converter => StyleConverter;
    }
}