
namespace SassParser
{
    internal sealed class UnknownProperty : Property
    {
        internal UnknownProperty(string name, Token token) : base(name, token)
        {
        }

        internal override IValueConverter Converter => Converters.Any;
    }
}