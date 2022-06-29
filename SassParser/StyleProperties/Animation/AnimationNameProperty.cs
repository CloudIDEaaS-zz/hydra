
namespace SassParser
{
    internal sealed class AnimationNameProperty : Property
    {
        private static readonly IValueConverter ListConverter =
            Converters.IdentifierConverter.FromList().OrNone().OrDefault();

        internal AnimationNameProperty(Token token) : base(PropertyNames.AnimationName, token)
        {
        }

        internal override IValueConverter Converter => ListConverter;
    }
}