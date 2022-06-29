
namespace SassParser
{
    internal sealed class ColumnGapProperty : Property
    {
        private static readonly IValueConverter StyleConverter =
            Converters.LengthOrNormalConverter.OrDefault(new Length(1f, Length.Unit.Em));

        internal ColumnGapProperty(Token token) : base(PropertyNames.ColumnGap, token, PropertyFlags.Animatable)
        {
        }

        internal override IValueConverter Converter => StyleConverter;
    }
}