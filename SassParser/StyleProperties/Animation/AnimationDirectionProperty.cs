
namespace SassParser
{
    internal sealed class AnimationDirectionProperty : Property
    {
        private static readonly IValueConverter ListConverter =
            Converters.AnimationDirectionConverter.FromList().OrDefault(AnimationDirection.Normal);

        internal AnimationDirectionProperty(Token token) : base(PropertyNames.AnimationDirection, token)
        {
        }

        internal override IValueConverter Converter => ListConverter;
    }
}