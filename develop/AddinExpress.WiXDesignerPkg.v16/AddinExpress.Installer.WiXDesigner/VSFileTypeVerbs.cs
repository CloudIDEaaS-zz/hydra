using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Xml;

namespace AddinExpress.Installer.WiXDesigner
{
	internal class VSFileTypeVerbs : List<VSFileTypeVerb>
	{
		private WiXProjectParser _project;

		private VSFileType _parent;

		public VSFileTypeVerbs(WiXProjectParser project, VSFileType parent)
		{
			this._project = project;
			this._parent = parent;
		}

		internal VSFileTypeVerb Add()
		{
			string i;
			string j;
			VSFileTypeVerb vSFileTypeVerb = null;
			int sequence = -2147483648;
			foreach (VSFileTypeVerb vSFileTypeVerb1 in this)
			{
				if (vSFileTypeVerb1.Sequence <= sequence)
				{
					continue;
				}
				sequence = vSFileTypeVerb1.Sequence;
			}
			if (sequence != -2147483648)
			{
				sequence++;
			}
			else
			{
				sequence = 1;
			}
			int num = 1;
			for (i = string.Concat("New Document Action #", num.ToString()); base.Find((VSFileTypeVerb e) => e.Name == i) != null; i = string.Concat("New Document Action #", num.ToString()))
			{
				num++;
			}
			num = 1;
			for (j = string.Concat("verb", num.ToString()); base.Find((VSFileTypeVerb e) => e.Verb == j) != null; j = string.Concat("verb", num.ToString()))
			{
				num++;
			}
			if (this._parent.ParentComponent != null && this._parent.WiXExtensions != null)
			{
				vSFileTypeVerb = new VSFileTypeVerb(this._project, this._parent);
				base.Add(vSFileTypeVerb);
				foreach (WiXExtension wiXExtension in this._parent.WiXExtensions)
				{
					XmlNode xmlNodes = Common.CreateXmlElementWithAttributes(wiXExtension.XmlNode.OwnerDocument, "Verb", wiXExtension.XmlNode.NamespaceURI, new string[] { "Id", "Command", "Sequence", "Argument" }, new string[] { j, i, sequence.ToString(), "\"%1\"" }, "", false);
					wiXExtension.XmlNode.AppendChild(xmlNodes);
					WiXVerb wiXVerb = new WiXVerb(this._project, wiXExtension.Owner, wiXExtension, xmlNodes);
					wiXExtension.SetDirty();
					vSFileTypeVerb.WiXElements.Add(wiXVerb);
				}
			}
			if (this._parent.ParentOutput != null)
			{
				VsWiXProject.ReferenceDescriptor.FileTypeVerb fileTypeVerb = new VsWiXProject.ReferenceDescriptor.FileTypeVerb(i, string.Empty, j, sequence.ToString())
				{
					Arguments = "\"%1\""
				};
				this._parent._parentFileType.Verbs.Add(fileTypeVerb);
				List<VsWiXProject.ReferenceDescriptor.FileType> fileTypes = new List<VsWiXProject.ReferenceDescriptor.FileType>();
				this._parent.CombineVDProjFileTypes(ref fileTypes);
				fileTypes.Add(this._parent._parentFileType);
				this._parent.ParentOutput.ReferenceDescriptor.SetProjectOutputProperty(this._parent.ParentOutput.Group, OutputGroupProperties.FileTypes, fileTypes);
				vSFileTypeVerb = new VSFileTypeVerb(this._project, this._parent, fileTypeVerb);
				base.Add(vSFileTypeVerb);
			}
			return vSFileTypeVerb;
		}

		internal void Parse()
		{
			if (this._parent.ParentComponent != null)
			{
				foreach (WiXExtension wiXExtension in this._parent.WiXExtensions)
				{
					List<WiXVerb> wiXVerbs = this._project.SupportedEntities.FindAll((WiXEntity e) => {
						if (!(e is WiXVerb))
						{
							return false;
						}
						return (e as WiXVerb).Parent == wiXExtension;
					}).ConvertAll<WiXVerb>((WiXEntity e) => e as WiXVerb);
					if (wiXVerbs == null || wiXVerbs.Count == 0)
					{
						wiXVerbs = wiXExtension.ChildEntities.FindAll((WiXEntity e) => e is WiXVerb).ConvertAll<WiXVerb>((WiXEntity e) => e as WiXVerb);
					}
					if (wiXVerbs == null || wiXVerbs.Count <= 0)
					{
						continue;
					}
					foreach (WiXVerb wiXVerb in wiXVerbs)
					{
						VSFileTypeVerb vSFileTypeVerb = base.Find((VSFileTypeVerb e) => e.Verb == wiXVerb.Id);
						if (vSFileTypeVerb == null)
						{
							vSFileTypeVerb = new VSFileTypeVerb(this._project, this._parent);
							base.Add(vSFileTypeVerb);
						}
						vSFileTypeVerb.WiXElements.Add(wiXVerb);
						this._project.SupportedEntities.Remove(wiXVerb);
					}
				}
			}
			if (this._parent.ParentOutput != null)
			{
				foreach (VsWiXProject.ReferenceDescriptor.FileTypeVerb verb in this._parent._parentFileType.Verbs)
				{
					base.Add(new VSFileTypeVerb(this._project, this._parent, verb));
				}
			}
		}
	}
}