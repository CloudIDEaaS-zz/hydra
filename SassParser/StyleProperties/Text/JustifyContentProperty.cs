namespace SassParser
{
    internal sealed class JustifyContentProperty : Property
    {
        private static readonly IValueConverter StyleConverter = Converters.JustifyContentConverter;

        public JustifyContentProperty(Token token) : base(PropertyNames.JustifyContent, token)
        {
        }

        internal override IValueConverter Converter => StyleConverter;
    }
}