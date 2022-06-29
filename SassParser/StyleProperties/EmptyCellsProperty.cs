
namespace SassParser
{
    internal sealed class EmptyCellsProperty : Property
    {
        private static readonly IValueConverter StyleConverter = Converters.EmptyCellsConverter.OrDefault(true);

        internal EmptyCellsProperty(Token token) : base(PropertyNames.EmptyCells, token, PropertyFlags.Inherited)
        {
        }

        internal override IValueConverter Converter => StyleConverter;
    }
}