using Accord.Audio.Formats;
using Microsoft.CognitiveServices.Speech;
using NAudio.Wave;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Utils;

namespace CognitiveServices
{
    public class CognitiveServices : ICognitiveServices
    {
        private SpeechConfig config;

        public CognitiveServices()
        {
            config = SpeechConfig.FromSubscription("f33235090d37448a87debc6fd8c493fc", "westus2");
        }

        public Task<byte[]> GenerateSpeech(string speechText)
        {
            byte[] mp3Bytes = null;

            var task = new Task<byte[]>(() =>
            {
                var resetEvent = new ManualResetEvent(false);
                Thread thread;

                thread = new Thread(async () =>
                {
                    var synthesizer = new SpeechSynthesizer(config);
                    SpeechSynthesisResult result;

                    result = await synthesizer.SpeakTextAsync(speechText);


                    synthesizer.SynthesisCompleted += (s, e2) =>
                    {
                        result = e2.Result;

                        if (result.Reason == ResultReason.SynthesizingAudioCompleted)
                        {
                            var audioData = result.AudioData;
                            var properties = result.GetProperties();

                            if (audioData.Length > 0)
                            {
                                var waveStream = new MemoryStream();

                                ProcessAudioData(properties, audioData, waveStream);

                                mp3Bytes = waveStream.ToArray();
                            }

                            synthesizer.Dispose();

                            resetEvent.Set();
                        }
                        else
                        {
                            DebugUtils.Break();
                        }
                    };

                    synthesizer.SynthesisCanceled += (s, e2) =>
                    {
                        DebugUtils.Break();
                    };

                    if (result.Reason != ResultReason.SynthesizingAudioCompleted)
                    {
                        DebugUtils.Break();
                    }
                });


                thread.SetApartmentState(ApartmentState.MTA);
                thread.Start();

                resetEvent.WaitOne();

                return mp3Bytes;
            });

            task.Start();

            return task;
        }

        private void ProcessAudioData(Dictionary<PropertyId, object> properties, byte[] audioData, Stream mp3File)
        {
            int position = 0;
            var inputStream = new MemoryStream(audioData);
            var waveDecoder = new WaveDecoder(inputStream);
            var guid = new Guid("{2af3c44f-c688-4a7b-9702-f0cc8a9bb576}");
            var frameCount = waveDecoder.Frames;

            if (frameCount > 0)
            {
                var signal = waveDecoder.Decode(position, frameCount);
                var floatArray = signal.ToFloat();

                using (var writer = new WaveFileWriter(mp3File, Mp3WaveFormat.CreateIeeeFloatWaveFormat(waveDecoder.SampleRate, waveDecoder.Channels)))
                {
                    writer.WriteSamples(floatArray, 0, floatArray.Length);
                    writer.Flush();
                }
            }
        }
    }
}
