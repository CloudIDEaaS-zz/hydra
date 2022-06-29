using System;
using System.Xml;

namespace AddinExpress.Installer.WiXDesigner
{
	internal class WiXCustomVariable
	{
		private WiXProjectParser _project;

		private WiXCustomVariables _owner;

		private string _name = string.Empty;

		private string _value = string.Empty;

		internal string FullName
		{
			get
			{
				return string.Concat("$(var.", this._name, ")");
			}
		}

		internal string Name
		{
			get
			{
				return this._name;
			}
		}

		internal WiXProjectParser Project
		{
			get
			{
				return this._project;
			}
		}

		internal CustomVariableType Type
		{
			get
			{
				if (string.IsNullOrEmpty(this.Value))
				{
					return CustomVariableType.VarDefine;
				}
				return CustomVariableType.VarVariable;
			}
		}

		internal string Value
		{
			get
			{
				if (!this._value.Contains("$(var."))
				{
					return this._value;
				}
				string str = this._value;
				for (int i = str.IndexOf("$(var.", 0); i >= 0; i = str.IndexOf("$(var.", i + 1))
				{
					int num = str.IndexOf(")", i + 1);
					if (num < 0)
					{
						break;
					}
					string str1 = str.Substring(i, num + 1 - i);
					if (!string.IsNullOrEmpty(str1))
					{
						WiXCustomVariable wiXCustomVariable = this._owner.FindVariable(str1);
						if (wiXCustomVariable == null)
						{
							string referencedProjectVariable = this._project.ProjectManager.GetReferencedProjectVariable(str1);
							if (string.IsNullOrEmpty(referencedProjectVariable))
							{
								referencedProjectVariable = this._project.ProjectManager.GetBuildProjectProperty(str1);
								if (!string.IsNullOrEmpty(referencedProjectVariable))
								{
									str = str.Replace(str1, referencedProjectVariable);
								}
							}
							else
							{
								str = str.Replace(str1, referencedProjectVariable);
							}
						}
						else
						{
							str = str.Replace(str1, wiXCustomVariable.Value);
						}
					}
				}
				return str;
			}
		}

		internal string Value2
		{
			get
			{
				return this._value;
			}
		}

		internal WiXCustomVariable(WiXProjectParser project, WiXCustomVariables owner, WiXEntity entity)
		{
			this._project = project;
			this._owner = owner;
			if (!entity.XmlNode.InnerText.Contains("="))
			{
				this._name = entity.XmlNode.InnerText;
			}
			else
			{
				string[] strArrays = entity.XmlNode.InnerText.Trim().Split(new char[] { '=' });
				if (strArrays.Length != 0)
				{
					this._name = strArrays[0].Trim();
				}
				if ((int)strArrays.Length >= 1)
				{
					this._value = strArrays[1].Trim().Trim(new char[] { '\"', '\'' });
					return;
				}
			}
		}
	}
}