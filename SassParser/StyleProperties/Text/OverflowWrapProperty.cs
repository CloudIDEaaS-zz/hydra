
namespace SassParser
{
    internal sealed class OverflowWrapProperty : Property
    {
        private static readonly IValueConverter StyleConverter = Converters.OverflowWrapConverter;

        public OverflowWrapProperty(Token token) : base(PropertyNames.OverflowWrap, token)
        {
        }

        internal override IValueConverter Converter => StyleConverter;
    }
}