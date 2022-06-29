using System;
using System.Runtime.CompilerServices;
using System.Text;

namespace DllExport
{
	public sealed class SourceCodeRange
	{
		public SourceCodePosition EndPosition
		{
			get;
			private set;
		}

		public string FileName
		{
			get;
			private set;
		}

		public SourceCodePosition StartPosition
		{
			get;
			private set;
		}

		public SourceCodeRange(string fileName, SourceCodePosition startPosition, SourceCodePosition endPosition)
		{
			this.FileName = fileName;
			this.StartPosition = startPosition;
			this.EndPosition = endPosition;
		}

		private bool Equals(SourceCodeRange other)
		{
			if (!string.Equals(this.FileName, other.FileName) || !this.StartPosition.Equals(other.StartPosition))
			{
				return false;
			}
			return this.EndPosition.Equals(other.EndPosition);
		}

		public override bool Equals(object obj)
		{
			if (object.ReferenceEquals(null, obj))
			{
				return false;
			}
			if (object.ReferenceEquals(this, obj))
			{
				return true;
			}
			if (!(obj is SourceCodeRange))
			{
				return false;
			}
			return this.Equals((SourceCodeRange)obj);
		}

		private static bool ExtractLineParts(string line, out SourceCodePosition start, out SourceCodePosition end, out string fileName)
		{
			string str;
			start = new SourceCodePosition();
			end = new SourceCodePosition();
			fileName = null;
			line = line.TrimStart(new char[0]);
			if (!line.StartsWith(".line"))
			{
				return false;
			}
			line = line.Substring(5).Trim();
			if (string.IsNullOrEmpty(line))
			{
				return false;
			}
			int previousStart = 0;
			string startLine = null;
			string endLine = null;
			string startColumn = null;
			string endColumn = null;
			StringBuilder fileNameBuilder = new StringBuilder(line.Length);
			bool withinString = false;
			for (int i = 0; i < line.Length; i++)
			{
				char currentChar = line[i];
				if (currentChar == '\'')
				{
					if (!withinString)
					{
						string currentLine = line.Substring(previousStart, i - previousStart).Trim();
						if (endColumn == null)
						{
							endColumn = currentLine;
						}
					}
					withinString = !withinString;
				}
				else if (withinString)
				{
					fileNameBuilder.Append(currentChar);
				}
				else if (currentChar == ',' || currentChar == ':')
				{
					string currentLine = line.Substring(previousStart, i - previousStart).Trim();
					previousStart = i + 1;
					char chr = currentChar;
					if (chr != ',')
					{
						if (chr == ':')
						{
							if (endLine != null)
							{
								endColumn = currentLine;
							}
							else
							{
								endLine = currentLine;
							}
						}
					}
					else if (startLine != null)
					{
						startColumn = currentLine;
					}
					else
					{
						startLine = currentLine;
					}
				}
			}
			SourceCodePosition? nullable = SourceCodePosition.FromText(startLine, startColumn);
			start = (nullable.HasValue ? nullable.GetValueOrDefault() : start);
			SourceCodePosition? nullable1 = SourceCodePosition.FromText(endLine, endColumn);
			end = (nullable1.HasValue ? nullable1.GetValueOrDefault() : end);
			if (fileNameBuilder.Length > 0)
			{
				str = fileNameBuilder.ToString();
			}
			else
			{
				str = null;
			}
			fileName = str;
			return fileName != null;
		}

		public static SourceCodeRange FromMsIlLine(string line)
		{
			SourceCodePosition start;
			SourceCodePosition end;
			string fileName;
			if (!SourceCodeRange.ExtractLineParts(line, out start, out end, out fileName))
			{
				return null;
			}
			return new SourceCodeRange(fileName, start, end);
		}

		public override int GetHashCode()
		{
			int hashCode = (this.FileName != null ? this.FileName.GetHashCode() : 0);
			SourceCodePosition startPosition = this.StartPosition;
			hashCode = hashCode * 397 ^ startPosition.GetHashCode();
			return hashCode * 397 ^ this.EndPosition.GetHashCode();
		}
	}
}