using System.Collections.Generic;

namespace SassParser
{
    public interface IRuleList : IEnumerable<IRule>
    {
        IRule this[int index] { get; }
        int Length { get; }
    }
}