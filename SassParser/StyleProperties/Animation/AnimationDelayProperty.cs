
namespace SassParser
{
    internal sealed class AnimationDelayProperty : Property
    {
        private static readonly IValueConverter ListConverter = Converters.TimeConverter.FromList().OrDefault(Time.Zero);

        internal AnimationDelayProperty(Token token) : base(PropertyNames.AnimationDelay, token)
        {
        }

        internal override IValueConverter Converter => ListConverter;
    }
}