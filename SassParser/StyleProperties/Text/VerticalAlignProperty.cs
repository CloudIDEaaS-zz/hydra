
namespace SassParser
{
    using static Converters;

    internal sealed class VerticalAlignProperty : Property
    {
        private static readonly IValueConverter StyleConverter = LengthOrPercentConverter.Or(
            VerticalAlignmentConverter).OrDefault(VerticalAlignment.Baseline);

        internal VerticalAlignProperty(Token token) : base(PropertyNames.VerticalAlign, token, PropertyFlags.Animatable)
        {
        }

        internal override IValueConverter Converter => StyleConverter;
    }
}