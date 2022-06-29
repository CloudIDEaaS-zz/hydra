using System.Collections.Generic;

namespace SassParser
{
    public interface IGradient : IImageSource
    {
        IEnumerable<GradientStop> Stops { get; }
        bool IsRepeating { get; }
    }
}