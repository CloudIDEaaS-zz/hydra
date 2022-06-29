using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;

namespace CognitiveServices
{
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public class WavePcmFormatHeader
    {
        /* ChunkID          Contains the letters "RIFF" in ASCII form */
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
        public char[] chunkID = new char[] { 'R', 'I', 'F', 'F' };

        /* ChunkSize        36 + SubChunk2Size */
        [MarshalAs(UnmanagedType.U4, SizeConst = 4)]
        public uint chunkSize = 0;

        /* Format           The "WAVE" format name */
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
        public char[] format = new char[] { 'W', 'A', 'V', 'E' };

        /* Subchunk1ID      Contains the letters "fmt " */
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
        public char[] subchunk1ID = new char[] { 'f', 'm', 't', ' ' };

        /* Subchunk1Size    16 for PCM */
        [MarshalAs(UnmanagedType.U4, SizeConst = 4)]
        public uint subchunk1Size = 18;

        /* AudioFormat      PCM = 1 (i.e. Linear quantization) */
        [MarshalAs(UnmanagedType.U2, SizeConst = 2)]
        public ushort audioFormat = 1;

        /* NumChannels      Mono = 1, Stereo = 2, etc. */
        [MarshalAs(UnmanagedType.U2, SizeConst = 2)]
        public ushort numChannels = 1;

        /* SampleRate       8000, 44100, etc. */
        [MarshalAs(UnmanagedType.U4, SizeConst = 4)]
        public uint sampleRate = 44100;

        /* ByteRate         == SampleRate * NumChannels * BitsPerSample/8 */
        [MarshalAs(UnmanagedType.U4, SizeConst = 4)]
        public uint byteRate = 0;

        /* BlockAlign       == NumChannels * BitsPerSample/8 */
        [MarshalAs(UnmanagedType.U2, SizeConst = 2)]
        public ushort blockAlign = 0;

        /* BitsPerSample    8 bits = 8, 16 bits = 16, etc. */
        [MarshalAs(UnmanagedType.U2, SizeConst = 2)]
        public ushort bitsPerSample = 16;

        [MarshalAs(UnmanagedType.U2, SizeConst = 2)]
        public ushort extraParamSize = 0;

        /* Subchunk2ID      Contains the letters "data" */
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
        public char[] subchunk2ID = new char[] { 'd', 'a', 't', 'a' };

        /* Subchunk2Size    == NumSamples * NumChannels * BitsPerSample/8 */
        [MarshalAs(UnmanagedType.U4, SizeConst = 4)]
        public uint subchunk2Size = 0;
    }

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public class WavePcmFormat
    {
        /* ChunkID          Contains the letters "RIFF" in ASCII form */
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
        public char[] chunkID = new char[] { 'R', 'I', 'F', 'F' };

        /* ChunkSize        36 + SubChunk2Size */
        [MarshalAs(UnmanagedType.U4, SizeConst = 4)]
        public uint chunkSize = 0;

        /* Format           The "WAVE" format name */
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
        public char[] format = new char[] { 'W', 'A', 'V', 'E' };

        /* Subchunk1ID      Contains the letters "fmt " */
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
        public char[] subchunk1ID = new char[] { 'f', 'm', 't', ' ' };

        /* Subchunk1Size    16 for PCM */
        [MarshalAs(UnmanagedType.U4, SizeConst = 4)]
        public uint subchunk1Size = 18;

        /* AudioFormat      PCM = 1 (i.e. Linear quantization) */
        [MarshalAs(UnmanagedType.U2, SizeConst = 2)]
        public ushort audioFormat = 1;

        /* NumChannels      Mono = 1, Stereo = 2, etc. */
        [MarshalAs(UnmanagedType.U2, SizeConst = 2)]
        public ushort numChannels = 1;

        /* SampleRate       8000, 44100, etc. */
        [MarshalAs(UnmanagedType.U4, SizeConst = 4)]
        public uint sampleRate = 44100;

        /* ByteRate         == SampleRate * NumChannels * BitsPerSample/8 */
        [MarshalAs(UnmanagedType.U4, SizeConst = 4)]
        public uint byteRate = 0;

        /* BlockAlign       == NumChannels * BitsPerSample/8 */
        [MarshalAs(UnmanagedType.U2, SizeConst = 2)]
        public ushort blockAlign = 0;

        /* BitsPerSample    8 bits = 8, 16 bits = 16, etc. */
        [MarshalAs(UnmanagedType.U2, SizeConst = 2)]
        public ushort bitsPerSample = 16;

        [MarshalAs(UnmanagedType.U2, SizeConst = 2)]
        public ushort extraParamSize = 0;

        /* Subchunk2ID      Contains the letters "data" */
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
        public char[] subchunk2ID = new char[] { 'd', 'a', 't', 'a' };

        /* Subchunk2Size    == NumSamples * NumChannels * BitsPerSample/8 */
        [MarshalAs(UnmanagedType.U4, SizeConst = 4)]
        public uint subchunk2Size = 0;

        /* Data             The actual sound data. */
        public byte[] data = new byte[0];

        public WavePcmFormat(byte[] data, ushort numChannels = 2, uint sampleRate = 44100, ushort bitsPerSample = 16)
        {
            this.data = data;
            this.numChannels = numChannels;
            this.sampleRate = sampleRate;
            this.bitsPerSample = bitsPerSample;
        }

        private void CalculateSizes()
        {
            subchunk2Size = (uint)data.Length;
            blockAlign = (ushort)(numChannels * bitsPerSample / 8);
            byteRate = sampleRate * numChannels * bitsPerSample / 8;
            chunkSize = 38 + subchunk2Size;
        }

        public byte[] ToBytesArray()
        {
            CalculateSizes();

            int headerSize = Marshal.SizeOf(this);
            IntPtr headerPtr = Marshal.AllocHGlobal(headerSize);
            Marshal.StructureToPtr(this, headerPtr, false);
            byte[] rawData = new byte[headerSize + data.Length];
            Marshal.Copy(headerPtr, rawData, 0, headerSize);
            Marshal.FreeHGlobal(headerPtr);
            Array.Copy(data, 0, rawData, 46, data.Length);

            return rawData;
        }
    }
}
