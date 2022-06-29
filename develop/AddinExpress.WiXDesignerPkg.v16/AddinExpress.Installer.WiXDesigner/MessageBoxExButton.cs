using System;

namespace AddinExpress.Installer.WiXDesigner
{
	internal class MessageBoxExButton
	{
		private string _text;

		private string _value;

		private string _helpText;

		private bool _isCancelButton;

		private bool defaultButton;

		public string HelpText
		{
			get
			{
				return this._helpText;
			}
			set
			{
				this._helpText = value;
			}
		}

		public bool IsCancelButton
		{
			get
			{
				return this._isCancelButton;
			}
			set
			{
				this._isCancelButton = value;
			}
		}

		public bool IsDefaultButton
		{
			get
			{
				return this.defaultButton;
			}
			set
			{
				this.defaultButton = value;
			}
		}

		public string Text
		{
			get
			{
				return this._text;
			}
			set
			{
				this._text = value;
			}
		}

		public string Value
		{
			get
			{
				return this._value;
			}
			set
			{
				this._value = value;
			}
		}

		public MessageBoxExButton()
		{
		}
	}
}