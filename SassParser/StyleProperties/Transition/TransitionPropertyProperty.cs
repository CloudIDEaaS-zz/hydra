
namespace SassParser
{
    internal sealed class TransitionPropertyProperty : Property
    {
        private static readonly IValueConverter ListConverter =
            Converters.AnimatableConverter.FromList().OrNone().OrDefault(Keywords.All);

        internal TransitionPropertyProperty(Token token) : base(PropertyNames.TransitionProperty, token)
        {
        }

        internal override IValueConverter Converter => ListConverter;
    }
}