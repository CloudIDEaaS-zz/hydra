using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Utils
{
    public delegate void OnBufferDump(object sender, BufferDumpEventArgs e);

    public class BufferDumpEventArgs
    {
        public byte[] BufferContents { get; }

        public BufferDumpEventArgs(byte[] bufferContents)
        {
            this.BufferContents = bufferContents;
        }
    }

    [DebuggerDisplay(" { DebugInfo } ")]
    public class ManagedBuffer : IDisposable
    {
        private MemoryStream stream;
        private BufferedStream buffer;
        public int MaxSize { get; }
        public int CurrentLength { get; set; }

        public event OnBufferDump OnBufferDump;

        public ManagedBuffer(int size)
        {
            stream = new MemoryStream();
            buffer = new BufferedStream(stream, size);
            this.MaxSize = size;
        }

        public void Dispose()
        {
            if (this.CurrentLength > 0)
            {
                byte[] bytes;

                buffer.Rewind();

                bytes = new byte[this.CurrentLength];
                buffer.Read(bytes, 0, (int)this.CurrentLength);

                if (OnBufferDump != null)
                {
                    OnBufferDump(this, new BufferDumpEventArgs(bytes));
                }

                buffer.Rewind();
                this.CurrentLength = 0;
            }

            stream.Dispose();
        }

        public override string ToString()
        {
            return base.ToString();
        }

        public byte[] BufferContents
        {
            get
            {
                using (new StreamReset(stream))
                {
                    byte[] bytes;

                    stream.Rewind();

                    bytes = new byte[this.CurrentLength];
                    stream.Read(bytes, 0, this.CurrentLength);

                    return bytes;
                }
            }
        }

        public string DebugInfo
        {
            get
            {
                return ASCIIEncoding.ASCII.GetString(this.BufferContents);
            }
        }

        public void WriteLine(string value)
        {
            this.Write(value + "\r\n");
        }

        public void Write(string value)
        {
            var valueLength = (int) value.Length;
            var remainingLength = this.MaxSize - (int) buffer.Position;
            string leftValue;
            string rightValue;
            int leftLength;
            int rightLength;

            if (remainingLength < valueLength)
            {
                byte[] bytes;

                leftLength = Math.Min(valueLength, remainingLength);
                rightLength = valueLength - leftLength;

                leftValue = value.Left((int) leftLength);
                rightValue = value.Right((int)rightLength);

                buffer.Write(ASCIIEncoding.ASCII.GetBytes(leftValue), 0, leftLength);
                buffer.Flush();

                this.CurrentLength += leftLength;

                buffer.Rewind();

                bytes = new byte[this.CurrentLength];
                buffer.Read(bytes, 0, (int)this.CurrentLength);

                OnBufferDump(this, new BufferDumpEventArgs(bytes));

                buffer.Rewind();
                this.CurrentLength = 0;

                this.Write(rightValue);
            }
            else
            {
                var bytes = ASCIIEncoding.ASCII.GetBytes(value);

                buffer.Write(bytes, 0, valueLength);
                buffer.Flush();

                this.CurrentLength += valueLength;
            }
        }
    }
}
