
namespace SassParser
{
    internal sealed class BoxDecorationBreak : Property
    {
        private static readonly IValueConverter StyleConverter = Converters.BoxDecorationConverter.OrDefault(false);

        internal BoxDecorationBreak(Token token): base(PropertyNames.BoxDecorationBreak, token)
        {
        }

        internal override IValueConverter Converter => StyleConverter;
    }
}