
namespace SassParser
{
    internal abstract class ShorthandProperty : Property
    {
        protected ShorthandProperty(string name, Token token, PropertyFlags flags = PropertyFlags.None) : base(name, token, flags | PropertyFlags.Shorthand)
        {
        }

        public string Stringify(Property[] properties)
        {
            return Converter.Construct(properties)?.CssText;
        }

        public void Export(Property[] properties)
        {
            foreach (var property in properties)
            {
                if (DeclaredValue != null)
                {
                    var value = DeclaredValue.ExtractFor(property.Name);

                    if (property.TrySetValue(value))
                    {
                        property.IsImportant = IsImportant;
                    }
                }
            }
        }
    }
}