
namespace SassParser
{
    internal sealed class UnicodeRangeProperty : Property
    {
        public UnicodeRangeProperty(Token token) : base(PropertyNames.UnicodeRange, token)
        {
        }

        internal override IValueConverter Converter => Converters.Any;
    }
}