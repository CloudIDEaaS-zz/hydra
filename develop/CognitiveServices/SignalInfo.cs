using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CognitiveServices
{
    public class SignalInfo
    {
        public TimeSpan Offset { get; }
        public double Loudness { get; }
        public float Pitch { get; }
        public int Length { get; }
        public TimeSpan Duration { get; }
        public float PercentOfStart { get; set; }

        public SignalInfo(TimeSpan offset, double energy, float average)
        {
            Offset = offset;
            Loudness = energy;
            Pitch = average;
        }

        public SignalInfo(TimeSpan offset, double energy, float average, int length, TimeSpan duration) : this(offset, energy, average)
        {
            Length = length;
            Duration = duration;
        }
    }
}
