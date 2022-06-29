
namespace SassParser
{
    internal sealed class TransformStyleProperty : Property
    {
        private static readonly IValueConverter StyleConverter =
            Converters.Toggle(Keywords.Flat, Keywords.Preserve3d).OrDefault(true);

        internal TransformStyleProperty(Token token) : base(PropertyNames.TransformStyle, token)
        {
        }

        internal override IValueConverter Converter => StyleConverter;
    }
}