using System;
using System.Collections.Specialized;
using System.Text;

namespace AddinExpress.Installer.Prerequisites.Manifests
{
	internal class Exceptions
	{
		public Exceptions()
		{
		}

		public class BuildErrorsException : Exceptions.BuildException
		{
			public BuildErrorsException(string mainMessage, StringCollection msgs) : base(mainMessage, msgs)
			{
			}

			public BuildErrorsException(string mainMessage, StringCollection msgs, Exception innerException) : base(mainMessage, msgs, innerException)
			{
			}
		}

		public class BuildException : Exception
		{
			private StringCollection m_Messages;

			public string MainMessage
			{
				get
				{
					StringBuilder stringBuilder = new StringBuilder();
					stringBuilder.Append(this.Message);
					stringBuilder.Append("\r\n");
					return stringBuilder.ToString();
				}
			}

			public StringCollection Messages
			{
				get
				{
					return this.m_Messages;
				}
			}

			public BuildException(string mainMessage, StringCollection msgs) : base(mainMessage)
			{
				this.m_Messages = msgs;
			}

			public BuildException(string mainMessage, StringCollection msgs, Exception innerException) : base(mainMessage, innerException)
			{
				this.m_Messages = msgs;
			}

			public override string ToString()
			{
				StringBuilder stringBuilder = new StringBuilder();
				stringBuilder.Append(string.Concat(this.MainMessage, ":"));
				stringBuilder.Append("\r\n");
				if (this.m_Messages != null)
				{
					StringEnumerator enumerator = this.m_Messages.GetEnumerator();
					while (enumerator.MoveNext())
					{
						stringBuilder.Append(string.Concat("\t", enumerator.Current, "\r\n"));
					}
				}
				stringBuilder.Append("\r\n");
				stringBuilder.Append(this.StackTrace);
				stringBuilder.Append("InnerException: \r\n");
				stringBuilder.Append(base.InnerException.ToString());
				return stringBuilder.ToString();
			}
		}

		public class BuildWarningsException : Exceptions.BuildException
		{
			public BuildWarningsException(string mainMessage, StringCollection msgs) : base(mainMessage, msgs)
			{
			}

			public BuildWarningsException(string mainMessage, StringCollection msgs, Exception innerException) : base(mainMessage, msgs, innerException)
			{
			}
		}
	}
}