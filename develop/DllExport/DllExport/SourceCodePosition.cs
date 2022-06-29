using System;

namespace DllExport
{
	public struct SourceCodePosition : IEquatable<SourceCodePosition>
	{
		private readonly int _Character;

		private readonly int _Line;

		public int Character
		{
			get
			{
				return this._Character;
			}
		}

		public int Line
		{
			get
			{
				return this._Line;
			}
		}

		public SourceCodePosition(int line, int character)
		{
			this._Line = line;
			this._Character = character;
		}

		public bool Equals(SourceCodePosition other)
		{
			if (other._Line != this._Line)
			{
				return false;
			}
			return other._Character == this._Character;
		}

		public override bool Equals(object obj)
		{
			if (object.ReferenceEquals(null, obj) || obj.GetType() != typeof(SourceCodePosition))
			{
				return false;
			}
			return this.Equals((SourceCodePosition)obj);
		}

		public static SourceCodePosition? FromText(string lineText, string columnText)
		{
			int temp;
			int num;
			int? line = null;
			int? column = null;
			if (int.TryParse(lineText, out temp))
			{
				line = new int?(temp);
			}
			if (int.TryParse(columnText, out temp))
			{
				column = new int?(temp);
			}
			if (!line.HasValue && !column.HasValue)
			{
				return null;
			}
			int? nullable = line;
			num = (nullable.HasValue ? nullable.GetValueOrDefault() : -1);
			int? nullable1 = column;
			return new SourceCodePosition?(new SourceCodePosition(num, (nullable1.HasValue ? nullable1.GetValueOrDefault() : -1)));
		}

		public override int GetHashCode()
		{
			return this._Line * 397 ^ this._Character;
		}

		public static bool operator ==(SourceCodePosition left, SourceCodePosition right)
		{
			return left.Equals(right);
		}

		public static bool operator !=(SourceCodePosition left, SourceCodePosition right)
		{
			return !left.Equals(right);
		}
	}
}