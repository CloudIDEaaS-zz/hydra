using Microsoft.VisualStudio.OLE.Interop;
using System;
using System.IO;

namespace AddinExpress.Installer.WiXDesigner
{
	internal sealed class DataStreamFromComStream : Stream, IDisposable
	{
		private IStream comStream;

		public override bool CanRead
		{
			get
			{
				return true;
			}
		}

		public override bool CanSeek
		{
			get
			{
				return true;
			}
		}

		public override bool CanWrite
		{
			get
			{
				return true;
			}
		}

		public override long Length
		{
			get
			{
				long position = this.Position;
				long num = this.Seek((long)0, SeekOrigin.End);
				this.Position = position;
				return num - position;
			}
		}

		public override long Position
		{
			get
			{
				return this.Seek((long)0, SeekOrigin.Current);
			}
			set
			{
				this.Seek(value, SeekOrigin.Begin);
			}
		}

		public DataStreamFromComStream(IStream comStream)
		{
			this.comStream = comStream;
		}

		private void _NotImpl(string message)
		{
			throw new NotSupportedException();
		}

		public override void Close()
		{
			if (this.comStream != null)
			{
				this.Flush();
				this.comStream = null;
				GC.SuppressFinalize(this);
			}
		}

		public new void Dispose()
		{
			try
			{
				if (this.comStream != null)
				{
					this.Flush();
					this.comStream = null;
				}
			}
			finally
			{
				base.Dispose();
			}
		}

		protected override void Finalize()
		{
			try
			{
			}
			finally
			{
				base.Finalize();
			}
		}

		public override void Flush()
		{
			if (this.comStream != null)
			{
				try
				{
					this.comStream.Commit(0);
				}
				catch (Exception exception)
				{
				}
			}
		}

		public override int Read(byte[] buffer, int index, int count)
		{
			uint num;
			byte[] numArray = buffer;
			if (index != 0)
			{
				numArray = new byte[(int)buffer.Length - index];
				buffer.CopyTo(numArray, 0);
			}
			this.comStream.Read(numArray, (uint)count, out num);
			if (index != 0)
			{
				numArray.CopyTo(buffer, index);
			}
			return (int)num;
		}

		public override long Seek(long offset, SeekOrigin origin)
		{
			LARGE_INTEGER lARGEINTEGER = new LARGE_INTEGER();
			ULARGE_INTEGER[] uLARGEINTEGERArray = new ULARGE_INTEGER[] { new ULARGE_INTEGER() };
			lARGEINTEGER.QuadPart = offset;
			this.comStream.Seek(lARGEINTEGER, (uint)origin, uLARGEINTEGERArray);
			return (long)uLARGEINTEGERArray[0].QuadPart;
		}

		public override void SetLength(long value)
		{
			ULARGE_INTEGER uLARGEINTEGER = new ULARGE_INTEGER()
			{
				QuadPart = (ulong)value
			};
			this.comStream.SetSize(uLARGEINTEGER);
		}

		public override void Write(byte[] buffer, int index, int count)
		{
			uint num;
			if (count > 0)
			{
				byte[] numArray = buffer;
				if (index != 0)
				{
					numArray = new byte[(int)buffer.Length - index];
					buffer.CopyTo(numArray, 0);
				}
				this.comStream.Write(numArray, (uint)count, out num);
				if ((ulong)num != (long)count)
				{
					throw new IOException("Didn't write enough bytes to IStream!");
				}
				if (index != 0)
				{
					numArray.CopyTo(buffer, index);
				}
			}
		}
	}
}