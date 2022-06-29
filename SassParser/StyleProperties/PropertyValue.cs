using System.IO;
// ReSharper disable UnusedMember.Global

namespace SassParser
{
    public class PropertyValue : StylesheetNode
    {
        public string Value { get; }


        internal PropertyValue(string value, Token token) : base(token)
        {
            Value = value;
        }

        public override void ToCss(TextWriter writer, IStyleFormatter formatter)
        {
            writer.Write(this.Value);
        }
    }
}