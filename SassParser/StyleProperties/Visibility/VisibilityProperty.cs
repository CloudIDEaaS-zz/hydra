
namespace SassParser
{
    internal sealed class VisibilityProperty : Property
    {
        private static readonly IValueConverter StyleConverter =
            Converters.VisibilityConverter.OrDefault(Visibility.Visible);

        internal VisibilityProperty(Token token) : base(PropertyNames.Visibility, token, PropertyFlags.Inherited | PropertyFlags.Animatable)
        {
        }

        internal override IValueConverter Converter => StyleConverter;
    }
}