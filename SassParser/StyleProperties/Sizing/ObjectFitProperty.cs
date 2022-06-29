
namespace SassParser
{
    internal sealed class ObjectFitProperty : Property
    {
        private static readonly IValueConverter StyleConverter =
            Converters.ObjectFittingConverter.OrDefault(ObjectFitting.Fill);

        internal ObjectFitProperty(Token token) : base(PropertyNames.ObjectFit, token)
        {
        }

        internal override IValueConverter Converter => StyleConverter;
    }
}