
namespace SassParser
{
    internal sealed class AnimationIterationCountProperty : Property
    {
        private static readonly IValueConverter ListConverter =
            Converters.PositiveOrInfiniteNumberConverter.FromList().OrDefault(1f);

        internal AnimationIterationCountProperty(Token token) : base(PropertyNames.AnimationIterationCount, token)
        {
        }

        internal override IValueConverter Converter => ListConverter;
    }
}