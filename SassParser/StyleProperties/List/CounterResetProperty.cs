
namespace SassParser
{
    using static Converters;

    internal sealed class CounterResetProperty : Property
    {
        private static readonly IValueConverter StyleConverter = Continuous(
            WithOrder(IdentifierConverter.Required(), IntegerConverter.Option(0))).OrDefault();

        internal CounterResetProperty(Token token) : base(PropertyNames.CounterReset, token)
        {
        }

        internal override IValueConverter Converter => StyleConverter;
    }
}