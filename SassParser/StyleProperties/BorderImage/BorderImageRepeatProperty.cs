
namespace SassParser
{
    internal sealed class BorderImageRepeatProperty : Property
    {
        internal static readonly IValueConverter TheConverter = Map.BorderRepeatModes.ToConverter().Many(1, 2);
        private static readonly IValueConverter StyleConverter = TheConverter.OrDefault(BorderRepeat.Stretch);

        internal BorderImageRepeatProperty(Token token) : base(PropertyNames.BorderImageRepeat, token)
        {
        }

        internal override IValueConverter Converter => StyleConverter;
    }
}