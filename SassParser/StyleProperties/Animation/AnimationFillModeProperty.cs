
namespace SassParser
{
    internal sealed class AnimationFillModeProperty : Property
    {
        private static readonly IValueConverter ListConverter =
            Converters.AnimationFillStyleConverter.FromList().OrDefault(AnimationFillStyle.None);

        internal AnimationFillModeProperty(Token token): base(PropertyNames.AnimationFillMode, token)
        {
        }

        internal override IValueConverter Converter => ListConverter;
    }
}