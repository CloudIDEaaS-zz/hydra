
namespace SassParser
{
    internal sealed class UnknownMediaFeature : MediaFeature
    {
        public UnknownMediaFeature(string name, Token token) : base(name, token)
        {
        }

        internal override IValueConverter Converter => Converters.Any;
    }
}