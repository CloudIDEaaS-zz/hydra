
namespace SassParser
{
    internal sealed class BackgroundRepeatProperty : Property
    {
        private static readonly IValueConverter ListConverter =
            Converters.BackgroundRepeatsConverter.FromList().OrDefault(BackgroundRepeat.Repeat);

        internal BackgroundRepeatProperty(Token token) : base(PropertyNames.BackgroundRepeat, token)
        {
        }

        internal override IValueConverter Converter => ListConverter;
    }
}