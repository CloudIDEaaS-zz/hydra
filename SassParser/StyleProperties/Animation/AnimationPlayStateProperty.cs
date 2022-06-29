
namespace SassParser
{
    internal sealed class AnimationPlayStateProperty : Property
    {
        private static readonly IValueConverter ListConverter =
            Converters.PlayStateConverter.FromList().OrDefault(PlayState.Running);

        internal AnimationPlayStateProperty(Token token) : base(PropertyNames.AnimationPlayState, token)
        {
        }

        internal override IValueConverter Converter => ListConverter;
    }
}