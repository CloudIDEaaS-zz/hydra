
namespace SassParser
{
    internal sealed class AnimationTimingFunctionProperty : Property
    {
        private static readonly IValueConverter ListConverter =
            Converters.TransitionConverter.FromList().OrDefault(Map.TimingFunctions[Keywords.Ease]);

        internal AnimationTimingFunctionProperty(Token token) : base(PropertyNames.AnimationTimingFunction, token)
        {
        }

        internal override IValueConverter Converter => ListConverter;
    }
}