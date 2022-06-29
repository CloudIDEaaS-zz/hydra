
namespace SassParser
{
    internal sealed class TransitionTimingFunctionProperty : Property
    {
        private static readonly IValueConverter ListConverter =
            Converters.TransitionConverter.FromList().OrDefault(Map.TimingFunctions[Keywords.Ease]);

        internal TransitionTimingFunctionProperty(Token token) : base(PropertyNames.TransitionTimingFunction, token)
        {
        }

        internal override IValueConverter Converter => ListConverter;
    }
}