
namespace SassParser
{
    using static Converters;

    internal sealed class CounterIncrementProperty : Property
    {
        private static readonly IValueConverter StyleConverter = Continuous(
            WithOrder(IdentifierConverter.Required(), IntegerConverter.Option(1))).OrDefault();

        internal CounterIncrementProperty(Token token) : base(PropertyNames.CounterIncrement, token)
        {
        }

        internal override IValueConverter Converter => StyleConverter;
    }
}