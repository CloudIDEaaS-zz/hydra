using Accord.Audio.Formats;
using CognitiveServices.Effects;
using CognitiveServices.Services;
using FFmpeg.NET;
using Microsoft.Azure.CognitiveServices.Vision.ComputerVision.Models;
using Microsoft.CognitiveServices.Speech;
using Microsoft.CognitiveServices.Speech.Audio;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using NAudio.Wave;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Schema;
using Newtonsoft.Json.Serialization;
using SharpDX.Multimedia;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Un4seen.Bass;
using Un4seen.Bass.AddOn.Tags;
using Utils;
using RiffChunk = SharpDX.Multimedia.RiffChunk;
using WaveFormat = SharpDX.Multimedia.WaveFormat;

namespace CognitiveServices
{
    public class CognitiveServices : ICognitiveServices
    {
        private ImageAnalyzer imageAnalyzer;
        private ILogger<CognitiveServices> logger;
        private SpeechConfig config;
        private string ssmlContent;
        private string workspaceFolder;
        private bool logSpeech;
        private List<SpeechLocal> speechLocales;
        private List<Style> styles;
        private List<Role> roles;
        private static RoleVoiceEffects roleVoiceEffects;
        private static DateTime voiceEffectsLastChange;

        static CognitiveServices()
        {
            Version version;
            LowHiWord lowHiWord;

            voiceEffectsLastChange = DateTime.MaxValue;

            CreateEffectsSchema();

            if (!Bass.BASS_Init(-1, 44100, BASSInit.BASS_DEVICE_DEFAULT, IntPtr.Zero))
            {
                var code = Bass.BASS_ErrorGetCode();
                DebugUtils.Break();
            }

            lowHiWord = new IntPtr(Bass.BASS_GetVersion()).ToLowHiWord();
            version = new Version(lowHiWord.Low, lowHiWord.High);

            Debug.WriteLine("Bass Version {0}", version);

            lowHiWord = new IntPtr(Un4seen.Bass.AddOn.Fx.BassFx.BASS_FX_GetVersion()).ToLowHiWord();
            version = new Version(lowHiWord.Low, lowHiWord.High);

            Debug.WriteLine("BassFx Version {0}", version);

            if (!Un4seen.Bass.AddOn.Fx.BassFx.LoadMe())
            {
                var code = Bass.BASS_ErrorGetCode();
                DebugUtils.Break();
            }
        }

        public CognitiveServices(IConfiguration configuration, IHostEnvironment hostEnvironment, ILogger<CognitiveServices> logger)
        {
            var environmentName = hostEnvironment.EnvironmentName;
            var workspaceFolderKey = $"{ environmentName }:WorkspaceFolder";
            var logSpeechKey = $"{ environmentName }:LogSpeech";

            LoadRoleVoiceEffects();

            speechLocales = new List<SpeechLocal>()
            {
                new SpeechLocal("English (Australia)", "en-AU", "White"),
                new SpeechLocal("English (Canada)", "en-CA", "White"),
                new SpeechLocal("English (Ghana)", "en-GH", "Black"),
                new SpeechLocal("English (Hong Kong)", "en-HK", "Asian"),
                new SpeechLocal("English (India)", "en-IN", "Indian"),
                new SpeechLocal("English (Ireland)", "en-IE", "White"),
                new SpeechLocal("English (Kenya)", "en-KE", "Black"),
                new SpeechLocal("English (New Zealand)", "en-NZ", "White"),
                new SpeechLocal("English (Nigeria)", "en-NG", "Black"),
                new SpeechLocal("English (Philippines)", "en-PH", "Latino"),
                new SpeechLocal("English (Singapore)", "en-SG", "Asian"),
                new SpeechLocal("English (South Africa)", "en-ZA", "White"),
                new SpeechLocal("English (Tanzania)", "en-TZ", "Black"),
                new SpeechLocal("English (United Kingdom)", "en-GB", "White"),
                new SpeechLocal("English (United States)", "en-US", "White")
            };

            styles = new List<Style>()
            {
                new Style("cheerful", "Joy", "Expresses a positive and happy tone."), 
                new Style("customerservice", "Joy", "Expresses a friendly and helpful tone for customer support."), 
                new Style("depressed", "Neutral", "Expresses a melancholic and despondent tone with lower pitch and energy."), 
                new Style("disgruntled", "Neutral", "Expresses a disdainful and complaining tone. Speech of this emotion displays displeasure and contempt."), 
                new Style("embarrassed", "Surprise", "Expresses an uncertain and hesitant tone when the speaker is feeling uncomfortable."), 
                new Style("empathetic", "", "Expresses a sense of caring and understanding."), 
                new Style("envious", "Neutral", "Expresses a tone of admiration when you desire something that someone else has."), 
                new Style("excited", "Surprise", "Expresses an upbeat and hopeful tone. It sounds like something great is happening and the speaker is really happy about that."), 
                new Style("fearful", "Neutral", "Expresses a scared and nervous tone,  with higher pitch,  higher vocal energy,  and faster rate. The speaker is in a state of tension and unease."), 
                new Style("friendly", "Joy", "Expresses a pleasant,  inviting,  and warm tone. It sounds sincere and caring."), 
                new Style("gentle", "Joy", "Expresses a mild,  polite,  and pleasant tone,  with lower pitch and vocal energy."), 
                new Style("hopeful", "Joy", "Expresses a warm and yearning tone. It sounds like something good will happen to the speaker."), 
                new Style("lyrical", "Joy", "Expresses emotions in a melodic and sentimental way."), 
                new Style("narration-professional", "Neutral", "Expresses a professional,  objective tone for content reading."), 
                new Style("narration-relaxed", "Neutral", "Express a soothing and melodious tone for content reading."), 
                new Style("newscast", "Neutral", "Expresses a formal and professional tone for narrating news."), 
                new Style("newscast-casual", "Neutral", "Expresses a versatile and casual tone for general news delivery."), 
                new Style("newscast-formal", "Neutral", "Expresses a formal,  confident,  and authoritative tone for news delivery."), 
                new Style("poetry-reading", "Neutral", "Expresses an emotional and rhythmic tone while reading a poem."), 
                new Style("sad", "Neutral", "Expresses a sorrowful tone."), 
                new Style("serious", "Neutral", "Expresses a strict and commanding tone. Speaker often sounds stiffer and much less relaxed with firm cadence."), 
                new Style("shouting", "Neutral", "Speaks like from a far distant or outside and to make self be clearly heard"), 
                new Style("sports-commentary", "Neutral", "Expresses a relaxed and interesting tone for broadcasting a sports event."), 
                new Style("sports-commentary-excited", "Surprise", "Expresses an intensive and energetic tone for broadcasting exciting moments in a sports event."), 
                new Style("whispering", "Surprise", "Speaks very softly and make a quiet and gentle sound"), 
                new Style("terrified", "Neutral", "Expresses a very scared tone,  with faster pace and a shakier voice. It sounds like the speaker is in an unsteady and frantic status."), 
                new Style("unfriendly", "Neutral", "Expresses a cold and indifferent tone.")
            };

            roles = new List<Role>()
            {
                new Role("Girl", 1, 16, "Female", "The voice imitates to a girl."), 
                new Role("Boy", 1, 16, "Male", "The voice imitates to a boy."), 
                new Role("YoungAdultFemale", 17, 40, "Female", "The voice imitates to a young adult female."), 
                new Role("YoungAdultMale", 17, 40, "Male", "The voice imitates to a young adult male."), 
                new Role("OlderAdultFemale", 41, 60, "Female", "The voice imitates to an older adult female."), 
                new Role("OlderAdultMale", 41, 60, "Male", "The voice imitates to an older adult male."), 
                new Role("SeniorFemale", 61, 120, "Female", "The voice imitates to a senior female."), 
                new Role("SeniorMale", 61, 120, "Male", "The voice imitates to a senior male.")
            };

            imageAnalyzer = new ImageAnalyzer();
            this.logger = logger;

            config = SpeechConfig.FromSubscription("f33235090d37448a87debc6fd8c493fc", "westus2");
            ssmlContent = typeof(CognitiveServices).ReadResource<string>("SSML.xml");

            workspaceFolder = configuration[workspaceFolderKey];
            logSpeech = configuration[logSpeechKey] == "True";
        }

        private void LoadRoleVoiceEffects()
        {
            var assembly = Assembly.GetEntryAssembly();
            var voiceEffectsPath = Path.Combine(Path.GetDirectoryName(assembly.Location), "VoiceEffects.json");
            var voiceEffectsFile = new FileInfo(voiceEffectsPath);

            if (CognitiveServices.voiceEffectsLastChange <= voiceEffectsFile.LastWriteTimeUtc)
            {
                return;
            }

            CognitiveServices.voiceEffectsLastChange = voiceEffectsFile.LastWriteTimeUtc;

            using (var reader = new StreamReader(voiceEffectsPath))
            {
                CognitiveServices.roleVoiceEffects = JsonExtensions.ReadJson<RoleVoiceEffects>(reader, new CamelCaseNamingStrategy());

                assembly = typeof(BASSFXType).Assembly;

                foreach (var role in CognitiveServices.roleVoiceEffects.Roles)
                {
                    var effects = new List<object>();
                    var attributes = new List<object>();

                    if (role.Effects != null)
                    {
                        foreach (JObject effect in role.Effects)
                        {
                            foreach (var property in effect.Properties())
                            {
                                var typeName = property.Name;
                                var parms = ((JObject)property.Value).Properties().ToDictionary(p => p.Name, p => p.Value);
                                var effectType = assembly.GetType("Un4seen.Bass." + typeName);
                                var args = new List<object>();
                                ConstructorInfo constructor;
                                object effectInstance;

                                if (effectType == null)
                                {
                                    effectType = assembly.GetType("Un4seen.Bass.AddOn.Fx." + typeName);
                                }

                                if (effectType == null)
                                {
                                    DebugUtils.Break();
                                }

                                constructor = effectType.GetConstructors().Where(c => c.GetParameters().Length == parms.Count && !c.GetParameters().Any(p => !parms.Any(p2 => p2.Key == p.Name))).Single();

                                effectInstance = effectType.CreateInstance<object>(parms.Select(p => p.Value.ToObject(constructor.GetParameters().Single(p2 => p2.Name == p.Key).ParameterType)).ToArray());

                                effects.Add(effectInstance);
                            }
                        }
                    }

                    if (role.Attributes != null)
                    {
                        foreach (JObject attribute in role.Attributes)
                        {
                            var property = attribute.Properties().Single();
                            var name = property.Name;
                            var value = property.Value;
                            var keyValuePair = new KeyValuePair<BASSAttribute, float>(EnumUtils.GetValue<BASSAttribute>(name), (float) value);

                            attributes.Add(keyValuePair);
                        }
                    }

                    role.Effects = effects;
                    role.Attributes = attributes;
                }
            }
        }

        private IDisposable MuteServer()
        {
            try
            {
                var defaultMuted = AudioManager.GetMasterVolumeMute();

                AudioManager.SetMasterVolumeMute(true);

                return this.CreateDisposable(() =>
                {
                    //if (!defaultMuted)
                    //{
                    //    AudioManager.SetMasterVolumeMute(false);
                    //}

                    AudioManager.SetMasterVolumeMute(false);
                });
            }
            catch (Exception ex)
            {
                //When something happend that prevent us to iterate through the devices
            }

            return null;
        }

        public async Task<GeneratePhotoResponse> GenerateRandomPhoto(params KeyValuePair<string, string>[] parms)
        {
            var generatePhotoResponse = await GeneratePhotoService.FetchRandomPhotoAsync(parms);

            return generatePhotoResponse;
        }

        public async Task<ImageAnalysis> AnalyzePhoto(Image image)
        {
            var imageAnalysis = await imageAnalyzer.AnalyzeImage(image);

            return imageAnalysis;
        }

        public async Task<Stream> GenerateSpeech(string speechText, string gender = "Female", int age = 30, string ethnicity = "White", string emotion = "Joy", Dictionary<string, string> aspectsOut = null)
        {
            var task = new Task<Stream>(() =>
            {
                var resetEvent = new ManualResetEvent(false);
                var local = speechLocales.Randomize().FirstOrDefault(l => l.Ethnicity == ethnicity);
                var style = styles.Randomize().FirstOrDefault(s => s.Emotion == emotion);
                var role = roles.Randomize().FirstOrDefault(r => (age >= r.AgeMin && age <= r.AgeMax) && r.Gender == gender);
                string code = local.CultureCode;
                string ssml;
                Thread thread;
                MemoryStream mp3Stream = null;

                if (local != null)
                {
                    code = local.CultureCode;
                }
                else
                {
                    DebugUtils.Break();
                }

                thread = new Thread(async () =>
                {
                    SpeechSynthesisResult result;
                    var synthesizer = new SpeechSynthesizer(config);
                    var disposable = this.MuteServer();
                    var voicesResult = await synthesizer.GetVoicesAsync();
                    var voice = voicesResult.Voices.Where(v => v.Locale == code).Randomize().FirstOrDefault(v => v.Gender == (gender == "Male" ? SynthesisVoiceGender.Male : SynthesisVoiceGender.Female));

                    if (voice == null)
                    {
                        voice = voicesResult.Voices.Randomize().FirstOrDefault(v => v.Gender == (gender == "Male" ? SynthesisVoiceGender.Male : SynthesisVoiceGender.Female));
                    }

                    ssml = ssmlContent.Replace("${VOICENAME}", voice.ShortName)
                        .Replace("${TEXT}", speechText)
                        .Replace("${ROLE}", role.RoleName)
                        .Replace("${STYLE}", style.StyleName);

                    if (aspectsOut != null)
                    {
                        aspectsOut.Add("VoiceName", voice.ShortName);
                        aspectsOut.Add("Role", role.RoleName);
                        aspectsOut.Add("Style", style.StyleName);
                    }

                    Debug.WriteLine("Text: '{0}', {1}, {2}, {3}, {4}", speechText, age, role.RoleName, emotion, style.StyleName);

                    synthesizer.SynthesisCompleted += (s, e2) =>
                    {
                        result = e2.Result;

                        if (result.Reason == ResultReason.SynthesizingAudioCompleted)
                        {
                            var audioData = result.AudioData;
                            var audioProperties = result.GetProperties();

                            if (audioData.Length > 0)
                            {
                                mp3Stream = new MemoryStream();

                                LoadRoleVoiceEffects();

                                audioData = ApplyEffects(audioData, role, age, style, local);

                                if (audioData == null)
                                {
                                    DebugUtils.Break();
                                }

                                ProcessAudioData(audioProperties, audioData, mp3Stream);
                            }

                            synthesizer.Dispose();

                            resetEvent.Set();
                        }
                        else
                        {
                            DebugUtils.Break();
                        }

                        disposable.Dispose();
                        Thread.Sleep(100);
                    };

                    synthesizer.SynthesisCanceled += (s, e2) =>
                    {
                        throw new Exception("Voice synthesizer exception. Reason: " + e2.Result.Reason.ToString());
                    };

                    result = await synthesizer.SpeakSsmlAsync(ssml);

                    if (result.Reason != ResultReason.SynthesizingAudioCompleted)
                    {
                        throw new Exception("Voice synthesizer exception. Reason: " + result.Reason.ToString());
                    }
                });


                thread.SetApartmentState(ApartmentState.MTA);
                thread.Start();

                resetEvent.WaitOne();

                return mp3Stream;
            });

            task.Start();

            return await task;
        }

        private byte[] ApplyEffects(byte[] audioData, Role role, int age, Style style, SpeechLocal local)
        {
            var pData = Marshal.AllocCoTaskMem(audioData.Length);
            long fullLength = 0;
            long lastLength = 0;
            long fullLengthSafe = 0;
            var lockObject = LockManager.CreateObject();
            var effects = roleVoiceEffects.Roles.Where(r => r.Effects != null && (r.AnyAge || (age >= r.AgeMin.Value && age <= r.AgeMax.Value))).SelectMany(r => r.Effects).ToList();
            var attributes = roleVoiceEffects.Roles.Where(r => r.Attributes != null && (r.AnyAge || (age >= r.AgeMin.Value && age <= r.AgeMax.Value))).SelectMany(r =>  r.Attributes.Cast<KeyValuePair<BASSAttribute, float>>()).ToList();
            var info = new BASS_CHANNELINFO();
            var tagInfo = new TAG_INFO();
            WavePcmFormatHeader waveHeader;
            WavePcmFormat wave;
            int dspHandle;

            Marshal.Copy(audioData, 0, pData, audioData.Length);
            waveHeader = Marshal.PtrToStructure<WavePcmFormatHeader>(pData);

            wave = new WavePcmFormat(new byte[0], waveHeader.numChannels, waveHeader.sampleRate, (ushort)waveHeader.bitsPerSample);

            var bassHandle = Bass.BASS_StreamCreateFile(pData, 0, audioData.Length, BASSFlag.BASS_DEFAULT);

            if (bassHandle == 0)
            {
                var code = Bass.BASS_ErrorGetCode();
                return null;
            }

            foreach (var attribute in attributes)
            {
                var bassAttribute = attribute.Key;
                var value = attribute.Value;

                if (bassAttribute == BASSAttribute.BASS_ATTRIB_TEMPO_PITCH)
                {
                    bassHandle = Bass.BASS_StreamCreateFile(pData, 0, audioData.Length, BASSFlag.BASS_STREAM_DECODE);

                    if (bassHandle == 0)
                    {
                        var code = Bass.BASS_ErrorGetCode();
                        return null;
                    }

                    bassHandle = Un4seen.Bass.AddOn.Fx.BassFx.BASS_FX_TempoCreate(bassHandle, BASSFlag.BASS_FX_FREESOURCE);

                    if (bassHandle == 0)
                    {
                        var code = Bass.BASS_ErrorGetCode();
                        return null;
                    }
                }

                if (!Bass.BASS_ChannelSetAttribute(bassHandle, bassAttribute, value))
                {
                    var code = Bass.BASS_ErrorGetCode();
                    return null;
                }
            }

            foreach (var effect in effects)
            {
                BASSFXType fxType;
                int fxHandle;

                if (effect.GetType().Name == "BASS_BFX_PITCHSHIFT")
                {
                    bassHandle = Bass.BASS_StreamCreateFile(pData, 0, audioData.Length, BASSFlag.BASS_STREAM_DECODE);

                    if (bassHandle == 0)
                    {
                        var code = Bass.BASS_ErrorGetCode();
                        return null;
                    }

                    bassHandle = Un4seen.Bass.AddOn.Fx.BassFx.BASS_FX_TempoCreate(bassHandle, BASSFlag.BASS_FX_FREESOURCE);

                    if (bassHandle == 0)
                    {
                        var code = Bass.BASS_ErrorGetCode();
                        return null;
                    }

                    if (!Bass.BASS_ChannelSetAttribute(bassHandle, BASSAttribute.BASS_ATTRIB_TEMPO_PITCH, 50))
                    {
                        var code = Bass.BASS_ErrorGetCode();
                        return null;
                    }
                }
                else
                {
                    fxType = EnumUtils.GetValue<BASSFXType>("BASS_FX_" + effect.GetType().Name.RemoveStart("BASS_"));
                    fxHandle = Bass.BASS_ChannelSetFX(bassHandle, fxType, 1);

                    if (fxHandle == 0)
                    {
                        var code = Bass.BASS_ErrorGetCode();
                        return null;
                    }

                    if (!Bass.BASS_FXSetParameters(fxHandle, effect))
                    {
                        var code = Bass.BASS_ErrorGetCode();
                        return null;
                    }
                }
            }

            if (!Bass.BASS_ChannelGetInfo(bassHandle, info))
            {
                var code = Bass.BASS_ErrorGetCode();
                return null;
            }


            BassTags.BASS_TAG_GetFromFile(bassHandle, tagInfo);

            dspHandle = Bass.BASS_ChannelSetDSP(bassHandle, (handle, channel, buffer, length, user) =>
            {
                using (lockObject.Lock())
                {
                    Array.Resize(ref wave.data, (int) fullLength + length);
                    Marshal.Copy(buffer, wave.data, (int) fullLength, length);

                    fullLength += length;
                }

            }, IntPtr.Zero, 1);

            if (dspHandle == 0)
            {
                var code = Bass.BASS_ErrorGetCode();
                return null;
            }

            if (!Bass.BASS_ChannelPlay(bassHandle, true))
            {
                var code = Bass.BASS_ErrorGetCode();
                return null;
            }

            Thread.Sleep((int) Math.Round(tagInfo.duration * 1000, 0));

            using (lockObject.Lock())
            {
                fullLengthSafe = fullLength;
            }

            while (lastLength < fullLengthSafe)
            {
                lastLength = fullLengthSafe;
                Thread.Sleep(200);

                using (lockObject.Lock())
                {
                    fullLengthSafe = fullLength;
                }
            }

            wave.subchunk2Size = (uint) fullLength;
            wave.blockAlign = waveHeader.blockAlign;
            wave.byteRate = waveHeader.byteRate;
            wave.chunkSize = waveHeader.chunkSize;

            Bass.BASS_StreamFree(bassHandle);

            Marshal.FreeCoTaskMem(pData);

            audioData = wave.ToBytesArray();

            Validate(audioData);

            return audioData;
        }

        private unsafe bool Validate(byte[] audioData)
        {
            RiffParser parser;
            string fileFormatName;
            var audioStream = audioData.ToMemory();
            WaveFormat format;

            parser = new RiffParser(audioStream);

            // Parse Header
            if (!parser.MoveNext() || parser.Current == null)
            {
                return false;
            }

            // Check that WAVE or XWMA header is present
            fileFormatName = parser.Current.Type;

            if (fileFormatName != "WAVE" && fileFormatName != "XWMA")
            {
                return false;
            }

            // Parse inside the first chunk
            parser.Descend();

            // Get all the chunk
            var chunks = parser.GetAllChunks();

            // Get "fmt" chunk
            var fmtChunk = Chunk(chunks, "fmt ");

            try
            {
                format = WaveFormat.MarshalFrom(fmtChunk.GetData());
            }
            catch (InvalidOperationException ex)
            {
                return false;
            }

            // Check for "data" chunk
            var dataChunk = Chunk(chunks, "data");
            var startPositionOfData = dataChunk.DataPosition;
            var length = dataChunk.Size;

            return true;
        }

        private RiffChunk Chunk(IEnumerable<RiffChunk> chunks, string id)
        {
            RiffChunk chunk = null;

            foreach (var riffChunk in chunks)
            {
                if (riffChunk.Type == id)
                {
                    chunk = riffChunk;
                    break;
                }
            }

            if (chunk == null || chunk.Type != id)
            {
                return null;
            }

            return chunk;
        }

        private void ProcessAudioData(Dictionary<PropertyId, object> properties, byte[] audioData, Stream mp3File)
        {
            int position = 0;
            var inputStream = new MemoryStream(audioData);
            var waveDecoder = new WaveDecoder(inputStream);
            var frameCount = waveDecoder.Frames;

            if (frameCount > 0)
            {
                var signal = waveDecoder.Decode(position, frameCount);
                var floatArray = signal.ToFloat();
                var writer = new WaveFileWriter(mp3File, Mp3WaveFormat.CreateIeeeFloatWaveFormat(waveDecoder.SampleRate, waveDecoder.Channels));

                writer.WriteSamples(floatArray, 0, floatArray.Length);
                writer.Flush();

                mp3File.Rewind();

                if (logSpeech)
                {
                    var workspacePath = Path.Combine(workspaceFolder, DateTime.UtcNow.ToSortableDateTimeText(), Guid.NewGuid().ToString());
                    var workspaceDirectory = new DirectoryInfo(workspacePath);

                    if (!workspaceDirectory.Exists)
                    {
                        workspaceDirectory.Create();
                    }
                    else
                    {
                        workspaceDirectory.DeleteAndCreate();
                    }

                    using (var fileStream = File.OpenWrite(Path.Combine(workspaceDirectory.FullName, "SynthesizedSpeech.wav")))
                    {
                        using (var fileWriter = new BinaryWriter(fileStream))
                        {
                            fileWriter.Write(audioData);
                            fileWriter.Flush();
                        }
                    }

                    using (var fileStream = File.OpenWrite(Path.Combine(workspaceDirectory.FullName, "SynthesizedSpeech.mp3")))
                    {
                        using (var fileWriter = new BinaryWriter(fileStream))
                        {
                            fileWriter.Write(mp3File.ToArray());
                            fileWriter.Flush();
                        }
                    }

                    mp3File.Rewind();
                }
            }
        }

        public async Task<NameFromSpeechResults> GetNameFromSpeechBytes(byte[] bytes, Guid streamIdentifier)
        {
            TaskCompletionSource<int> stopRecognition;
            SpeechRecognizer recognizer;
            var name = string.Empty;
            var workspacePath = Path.Combine(workspaceFolder, DateTime.UtcNow.ToSortableDateTimeText(), streamIdentifier.ToString());
            var workspaceDirectory = new DirectoryInfo(workspacePath);
            var inputFile = new InputFile(Path.Combine(workspaceDirectory.FullName, "Name.webm"));
            var outputFile = new OutputFile(Path.Combine(workspaceDirectory.FullName, @"Name.wav"));
            var ffmpeg = new Engine(Path.Combine(workspaceFolder, @"ffmpeg.exe"));
            var errorBuilder = new StringBuilder();
            var resetEvent = new ManualResetEvent(false);
            MediaFile mediaFile;
            SpeechRecognitionResult result;
            AudioStreamFormat audioStreamFormat;
            string error = null;

            if (!workspaceDirectory.Exists)
            {
                workspaceDirectory.Create();
            }
            else
            {
                workspaceDirectory.DeleteAndCreate();
            }

            using (var fileStream = File.OpenWrite(inputFile.FileInfo.FullName))
            {
                using (var writer = new BinaryWriter(fileStream))
                {
                    writer.Write(bytes);
                    writer.Flush();
                }
            }

            ffmpeg.Error += (sender, e) =>
            {
                errorBuilder.Append(e.Exception.Message);
            };

            ffmpeg.Complete += (sender, e) =>
            {
                resetEvent.Set();
            };

            mediaFile = await ffmpeg.ConvertAsync(inputFile, outputFile, default).ConfigureAwait(false);

            if (!resetEvent.WaitOne(5000))
            {
                throw new Exception("Timeout converting audio. " + errorBuilder.ToString());
            }

            using (var fileStream = File.OpenRead(mediaFile.FileInfo.FullName))
            {
                audioStreamFormat = Helper.readWaveHeader(new BinaryReader(fileStream));
            }

            stopRecognition = new TaskCompletionSource<int>();

            using (var audioInput = AudioConfig.FromStreamInput(new PullAudioInputStream(new BinaryAudioStreamReader(new BinaryReader(File.OpenRead(mediaFile.FileInfo.FullName))), audioStreamFormat)))
            {
                using (recognizer = new SpeechRecognizer(config, audioInput))
                {
                    result = await recognizer.RecognizeOnceAsync();

                    if (result.Reason == ResultReason.RecognizedSpeech)
                    {
                        var text = result.Text;

                        name += text;
                    }
                    else if (result.Reason == ResultReason.NoMatch)
                    {
                        error = "Did not understand name";
                    }
                    else
                    {
                        error = $"Unexpected result { result.Reason }";
                    }
                }
            }

            name = name.RemoveEndIfMatches(".");

            return new NameFromSpeechResults(name, error);
        }

        private static void CreateEffectsSchema()
        {
            var hydraSolutionPath = Path.GetFullPath(Environment.ExpandEnvironmentVariables("%HYDRASOLUTIONPATH%"));
            var effectSchemaPath = Path.Combine(hydraSolutionPath, @"CognitiveServices\EffectSchema\Schema.json");
            var effectSchemaFile = new FileInfo(effectSchemaPath);

            if (!effectSchemaFile.Exists)
            {
                List<string> names;
                var assembly = typeof(BASSFXType).Assembly;
                var effectsSchema = new JsonSchema
                {
                    Properties = new Dictionary<string, JsonSchema>()
                };
                var attributesSchema = new JsonSchema
                {
                    Properties = new Dictionary<string, JsonSchema>()
                };

                var jsonSchema = new JsonSchema
                {
                    Id = "https://schemas.cloudideaas.com/soundeffects.schema.json",
                    Properties = new Dictionary<string, JsonSchema>
                    {
                        { "Roles", new JsonSchema()
                            {
                                Type = JsonSchemaType.Array,
                                Items = new List<JsonSchema>
                                {
                                    new JsonSchema()
                                    {
                                        Properties = new Dictionary<string, JsonSchema>()
                                        {
                                            { "AgeMin", new JsonSchema()
                                                {
                                                    Type = JsonSchemaType.Integer,
                                                    Required = false
                                                }
                                            },
                                            { "AgeMax", new JsonSchema()
                                                {
                                                    Type = JsonSchemaType.Integer,
                                                    Required = false
                                                }
                                            },
                                            { "AnyAge", new JsonSchema()
                                                {
                                                    Type = JsonSchemaType.Boolean,
                                                    Required = false
                                                }
                                            },
                                            {
                                                "Effects", new JsonSchema()
                                                {
                                                    Type = JsonSchemaType.Array,
                                                    Required = true,
                                                    Items = new List<JsonSchema>
                                                    {
                                                        {
                                                          effectsSchema
                                                        }
                                                    }
                                                }
                                            },
                                            {
                                                "Attributes", new JsonSchema()
                                                {
                                                    Type = JsonSchemaType.Array,
                                                    Required = true,
                                                    Items = new List<JsonSchema>
                                                    {
                                                        {
                                                          attributesSchema
                                                        }
                                                    }
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    },
                    AllowAdditionalItems = true,
                    AllowAdditionalProperties = true
                };

                names = EnumUtils.GetNames<BASSFXType>().ToList();

                foreach (var name in names)
                {
                    var typeName = name.RemoveStart("BASS_FX_");
                    var effectType = assembly.GetType("Un4seen.Bass.BASS_" + typeName);

                    if (effectType == null)
                    {
                        effectType = assembly.GetType("Un4seen.Bass.AddOn.Fx.BASS_" + typeName);
                    }

                    if (effectType != null)
                    {
                        var documentation = effectType.GetAssemblyDocumentation();
                        var parms = effectType.GetConstructors().SelectMany(c => c.GetParameters()).DistinctBy(p => p.Name);
                        var typeSchema = new JsonSchema
                        {
                            Type = JsonSchemaType.Object,
                            Format = effectType.AssemblyQualifiedName,
                            Properties = new Dictionary<string, JsonSchema>(),
                            Required = false
                        };

                        effectsSchema.Properties.Add(effectType.Name, typeSchema);

                        foreach (var parm in parms)
                        {
                            typeSchema.Properties.Add(parm.Name, new JsonSchema
                            {
                                Type = parm.ParameterType.GetJsonType(),
                                Format = parm.ParameterType.FullName,
                                Required = !parm.IsOptional
                            });
                        }
                    }
                }

                names = EnumUtils.GetNames<BASSAttribute>().ToList();

                foreach (var name in names)
                {
                    attributesSchema.Properties.Add(name, new JsonSchema
                    {
                        Type = JsonSchemaType.Float,
                        Format = typeof(BASSAttribute).FullName,
                        Required = true
                    });
                }

                using (var writer = new StreamWriter(effectSchemaFile.FullName))
                using (var jsonWriter = new JsonTextWriter(writer))
                {
                    jsonSchema.WriteTo(jsonWriter);

                    jsonWriter.Flush();
                }
            }
        }
    }
}
