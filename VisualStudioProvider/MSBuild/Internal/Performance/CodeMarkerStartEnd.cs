namespace Microsoft.Internal.Performance
{
    using System;

    internal sealed class CodeMarkerStartEnd : IDisposable
    {
        private Microsoft.Internal.Performance.CodeMarkerEvent _end;

        public CodeMarkerStartEnd(Microsoft.Internal.Performance.CodeMarkerEvent begin, Microsoft.Internal.Performance.CodeMarkerEvent end)
        {
            Microsoft.Internal.Performance.CodeMarkers.Instance.CodeMarker(begin);
            this._end = end;
        }

        public void Dispose()
        {
            if (this._end != ((Microsoft.Internal.Performance.CodeMarkerEvent) 0))
            {
                Microsoft.Internal.Performance.CodeMarkers.Instance.CodeMarker(this._end);
                this._end = (Microsoft.Internal.Performance.CodeMarkerEvent) 0;
            }
        }
    }
}

