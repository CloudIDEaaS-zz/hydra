using System.Collections.Generic;

namespace SassParser
{
    public interface IStylesheetNode : IStyleFormattable
    {
        Dictionary<string, List<object>> Properties { get; }
        IEnumerable<IStylesheetNode> Children { get; }
        StylesheetText StylesheetText { get; }
        TextPosition StartPosition { get; }
        TextPosition EndPosition { get; }
        IStylesheetNode ParentNode { get; set; }

    }
}