using System.IO;
// ReSharper disable UnusedMember.Global

namespace SassParser
{
    public class PropertyName : StylesheetNode


    {
        public string Name { get; }

        internal PropertyName(string name, Token token) : base(token)
        {
            Name = name;
        }

        public override void ToCss(TextWriter writer, IStyleFormatter formatter)
        {
            writer.Write(this.Name);
        }
    }
}