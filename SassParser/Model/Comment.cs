using System.IO;

namespace SassParser
{
    public sealed class Comment : StylesheetNode
    {
        public Comment(string data, Token token) : base(token)
        {
            Data = data;
        }

        public string Data { get; }

        public override void ToCss(TextWriter writer, IStyleFormatter formatter)
        {
            writer.Write(formatter.Comment(Data));
        }
    }
}