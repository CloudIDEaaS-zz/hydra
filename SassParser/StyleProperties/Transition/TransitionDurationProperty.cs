
namespace SassParser
{
    internal sealed class TransitionDurationProperty : Property
    {
        private static readonly IValueConverter ListConverter = Converters.TimeConverter.FromList().OrDefault(Time.Zero);

        internal TransitionDurationProperty(Token token) : base(PropertyNames.TransitionDuration, token)
        {
        }

        internal override IValueConverter Converter => ListConverter;
    }
}