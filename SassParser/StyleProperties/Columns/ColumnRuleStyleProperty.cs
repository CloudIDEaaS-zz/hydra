
namespace SassParser
{
    internal sealed class ColumnRuleStyleProperty : Property
    {
        private static readonly IValueConverter StyleConverter = Converters.LineStyleConverter.OrDefault(LineStyle.None);

        internal ColumnRuleStyleProperty(Token token) : base(PropertyNames.ColumnRuleStyle, token)
        {
        }

        internal override IValueConverter Converter => StyleConverter;
    }
}