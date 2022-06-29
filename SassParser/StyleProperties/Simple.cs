using System.IO;
// ReSharper disable UnusedMember.Global

namespace SassParser
{
    public class Simple : StylesheetNode
    {
        public string TextOrChar { get; }

        private readonly PropertyFlags _flags;

        internal Simple(string textOrChar, Token token) : base(token)
        {
            TextOrChar = textOrChar;
        }



        public override void ToCss(TextWriter writer, IStyleFormatter formatter)
        {
            writer.Write(this.TextOrChar);
        }
    }
}