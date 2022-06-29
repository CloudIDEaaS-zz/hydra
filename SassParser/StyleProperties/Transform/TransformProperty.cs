
namespace SassParser
{
    internal sealed class TransformProperty : Property
    {
        private static readonly IValueConverter StyleConverter =
            Converters.TransformConverter.Many().OrNone().OrDefault();

        internal TransformProperty(Token token) : base(PropertyNames.Transform, token, PropertyFlags.Animatable)
        {
        }

        internal override IValueConverter Converter => StyleConverter;
    }
}