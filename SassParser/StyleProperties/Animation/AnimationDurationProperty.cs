
namespace SassParser
{
    sealed class AnimationDurationProperty : Property
    {
        private static readonly IValueConverter ListConverter = Converters.TimeConverter.FromList().OrDefault(Time.Zero);

        internal AnimationDurationProperty(Token token) : base(PropertyNames.AnimationDuration, token)
        {
        }

        internal override IValueConverter Converter => ListConverter;
    }
}