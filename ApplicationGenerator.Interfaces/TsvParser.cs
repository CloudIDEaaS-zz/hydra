using System.Collections.Generic;

namespace AbstraX
{
    public interface ITsvParser
    {
    }

    public interface ITsvParserHost<TRecord>
    {
        List<TRecord> ParseFile(string file);
    }

    public class TsvParser : ITsvParser
    {
    }
}