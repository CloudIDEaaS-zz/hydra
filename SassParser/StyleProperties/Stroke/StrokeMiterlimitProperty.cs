
namespace SassParser
{
    internal sealed class StrokeMiterlimitProperty : Property
    {
        private static readonly IValueConverter StyleConverter = Converters.StrokeMiterlimitConverter;

        public StrokeMiterlimitProperty(Token token) : base(PropertyNames.StrokeMiterlimit, token, PropertyFlags.Animatable)
        {
        }

        internal override IValueConverter Converter => StyleConverter;
    }
}