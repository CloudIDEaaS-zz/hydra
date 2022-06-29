using System.IO;

namespace SassParser
{
    public interface IStyleFormattable
    {
        void ToCss(TextWriter writer, IStyleFormatter formatter);
    }
}