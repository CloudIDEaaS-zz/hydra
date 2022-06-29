using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Accord.DirectSound;
using System.Diagnostics;
using System.Runtime.InteropServices;
using Utils;

namespace CognitiveServices
{
    public class PitchDetection : ILockable
    {
        private AudioCaptureDevice source;
        private int SAMPLE_RATE = 44100;
        private int SAMPLE_COUNT = 10000;
        private int BLOCK_SIZE = 2048;
        private int STEP_SIZE = 512;
        private IManagedLockObject lockObject;
        private Stopwatch stopwatch;
        private Dictionary<TimeSpan, SignalInfo> latestPitches;

        [StructLayout(LayoutKind.Sequential)]
        unsafe public struct pyinc_pitch_range
        {
            public float* begin;
            public float* end;
        }

        [DllImport("LibPyin", CallingConvention = CallingConvention.Cdecl)]
        static extern void pyinc_init(int sample_rate, int block_size, int step_size);

        [DllImport("LibPyin", CallingConvention = CallingConvention.Cdecl)]
        unsafe static extern pyinc_pitch_range pyinc_feed(float* data, int size);

        [DllImport("LibPyin", CallingConvention = CallingConvention.Cdecl)]
        unsafe static extern void pyinc_clear();

        public PitchDetection()
        {
            lockObject = LockManager.CreateObject();
            stopwatch = new Stopwatch();
            latestPitches = new Dictionary<TimeSpan, SignalInfo>();
        }

        unsafe public void Start()
        {
            source = new AudioCaptureDevice();

            source.DesiredFrameSize = SAMPLE_COUNT;
            source.SampleRate = SAMPLE_RATE;
            source.NewFrame += Source_NewFrame;
            source.AudioSourceError += Source_AudioSourceError;

            stopwatch.Start();

            // Start
            source.Start();
        }

        unsafe public void Stop()
        {
            source.Stop();
            stopwatch.Stop();

            pyinc_clear();
        }

        public Dictionary<TimeSpan, SignalInfo> GetAndClearPitches()
        {
            using (this.Lock())
            {
                var latestPitches = this.latestPitches.ToDictionary(p => p.Key, p => p.Value);

                this.latestPitches.Clear();

                return latestPitches;
            }
        }

        private unsafe void Source_NewFrame(object sender, Accord.Audio.NewFrameEventArgs e)
        {
            var signal = e.Signal;
            var energy = signal.GetEnergy();
            var offset = stopwatch.Elapsed - signal.Duration;
            List<float> withinRange;

            if (energy > 10)
            {
                var results = new List<float>();
                float* ptr = (float*)signal.Data;
                float* res_ptr;
                pyinc_pitch_range pitches;

                pyinc_init(SAMPLE_RATE, BLOCK_SIZE, STEP_SIZE);

                pitches = pyinc_feed(ptr, SAMPLE_COUNT);
                res_ptr = pitches.begin;

                while (res_ptr != pitches.end)
                {
                    results.Add(*res_ptr);

                    res_ptr++;
                }

                withinRange = results.Where(f => f.IsBetween(40, 800)).ToList();

                if (withinRange.Count > 0)
                {
                    var withRemovedOutliers = withinRange.SkipOutliers(20, r => r);
                    var average = withRemovedOutliers.Average();

                    using (this.Lock())
                    {
                        latestPitches.Add(offset, new SignalInfo(offset, energy, average, signal.Length, signal.Duration));
                    }
                }

                pyinc_clear();
            }
        }

        private void Source_AudioSourceError(object sender, Accord.Audio.AudioSourceErrorEventArgs e)
        {
            if (e.Description != "Thread was being aborted.")
            {
                DebugUtils.Break();
            }
        }

        public IDisposable Lock()
        {
            return lockObject.Lock();
        }

        public T LockReturn<T>(Func<T> func)
        {
            using (this.Lock())
            {
                return func();
            }
        }

        public void LockSet(Action action)
        {
            using (this.Lock())
            {
                action();
            }
        }
    }
}
