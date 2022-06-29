
namespace SassParser
{
    internal sealed class FloatProperty : Property
    {
        private static readonly IValueConverter StyleConverter = Converters.FloatingConverter.OrDefault(Floating.None);

        internal FloatProperty(Token token) : base(PropertyNames.Float, token)
        {
        }

        internal override IValueConverter Converter => StyleConverter;
    }
}