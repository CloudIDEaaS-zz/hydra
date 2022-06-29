
namespace SassParser
{
    internal sealed class SrcProperty : Property
    {
        public SrcProperty(Token token) : base(PropertyNames.Src, token)
        {
        }

        internal override IValueConverter Converter => Converters.Any;
    }
}