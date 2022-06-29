
namespace SassParser
{
    internal sealed class TransitionDelayProperty : Property
    {
        private static readonly IValueConverter ListConverter = Converters.TimeConverter.FromList().OrDefault(Time.Zero);

        internal TransitionDelayProperty(Token token) : base(PropertyNames.TransitionDelay, token)
        {
        }

        internal override IValueConverter Converter => ListConverter;
    }
}