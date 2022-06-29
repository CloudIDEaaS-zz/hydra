using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Forms;
using System.Xml;

namespace AddinExpress.Installer.WiXDesigner
{
	internal class VSFileTypes : List<VSFileType>
	{
		private WiXProjectParser _project;

		public VSFileTypes(WiXProjectParser project)
		{
			this._project = project;
		}

		internal VSFileType Add()
		{
			VSFileType vSFileType;
			string i;
			FormSelectItemInProject formSelectItemInProject = new FormSelectItemInProject();
			try
			{
				formSelectItemInProject.Initialize(null, string.Empty, this._project.FileSystem, new string[] { "Executable Files (*.exe)", "All Files (*.*)" });
				if (formSelectItemInProject.ShowDialog() == DialogResult.OK)
				{
					int num = 1;
					for (i = string.Concat("New Document Type #", num.ToString()); base.Find((VSFileType e) => e.Name == i) != null; i = string.Concat("New Document Type #", num.ToString()))
					{
						num++;
					}
					if (formSelectItemInProject.SelectedFile is VSProjectOutputVDProj)
					{
						VsWiXProject.ReferenceDescriptor.FileType fileType = new VsWiXProject.ReferenceDescriptor.FileType(i, string.Empty, "myext", string.Empty, string.Empty);
						VsWiXProject.ReferenceDescriptor.FileTypeVerb fileTypeVerb = new VsWiXProject.ReferenceDescriptor.FileTypeVerb("&Open", string.Empty, "open", "1")
						{
							Arguments = "\"%1\""
						};
						fileType.AddVerb(fileTypeVerb);
						List<VsWiXProject.ReferenceDescriptor.FileType> fileTypes = new List<VsWiXProject.ReferenceDescriptor.FileType>()
						{
							fileType
						};
						foreach (VSFileType vSFileType1 in this)
						{
							if (vSFileType1.ParentOutput != formSelectItemInProject.SelectedFile as VSProjectOutputVDProj)
							{
								continue;
							}
							fileTypes.Add(vSFileType1._parentFileType);
						}
						(formSelectItemInProject.SelectedFile as VSProjectOutputVDProj).ReferenceDescriptor.SetProjectOutputProperty((formSelectItemInProject.SelectedFile as VSProjectOutputVDProj).Group, OutputGroupProperties.FileTypes, fileTypes);
						VSFileType vSFileType2 = new VSFileType(this._project, formSelectItemInProject.SelectedFile as VSProjectOutputVDProj, fileType);
						base.Add(vSFileType2);
						vSFileType = vSFileType2;
						return vSFileType;
					}
					else if (formSelectItemInProject.SelectedFile is VSFile)
					{
						VSFile selectedFile = formSelectItemInProject.SelectedFile as VSFile;
						XmlNode xmlNodes = Common.CreateXmlElementWithAttributes(selectedFile.WiXElement.XmlNode.OwnerDocument, "ProgId", selectedFile.WiXElement.XmlNode.NamespaceURI, new string[] { "Id", "Advertise" }, new string[] { i, "yes" }, "", false);
						selectedFile.ParentComponent.WiXElement.XmlNode.InsertAfter(xmlNodes, selectedFile.WiXElement.XmlNode);
						WiXProgId wiXProgId = new WiXProgId(this._project, selectedFile.WiXElement.Owner, selectedFile.ParentComponent.WiXElement, xmlNodes);
						XmlNode xmlNodes1 = Common.CreateXmlElementWithAttributes(selectedFile.WiXElement.XmlNode.OwnerDocument, "Extension", selectedFile.WiXElement.XmlNode.NamespaceURI, new string[] { "Id" }, new string[] { "myext" }, "", false);
						xmlNodes.AppendChild(xmlNodes1);
						WiXExtension wiXExtension = new WiXExtension(this._project, wiXProgId.Owner, wiXProgId, xmlNodes1);
						XmlNode xmlNodes2 = Common.CreateXmlElementWithAttributes(selectedFile.WiXElement.XmlNode.OwnerDocument, "Verb", selectedFile.WiXElement.XmlNode.NamespaceURI, new string[] { "Id", "Command", "Sequence", "Argument" }, new string[] { "open", "&Open", "1", "\"%1\"" }, "", false);
						xmlNodes1.AppendChild(xmlNodes2);
						WiXVerb wiXVerb = new WiXVerb(this._project, wiXExtension.Owner, wiXExtension, xmlNodes2);
						wiXProgId.Parent.SetDirty();
						VSFileType vSFileType3 = new VSFileType(this._project, selectedFile.ParentComponent, wiXProgId);
						base.Add(vSFileType3);
						vSFileType = vSFileType3;
						return vSFileType;
					}
				}
				return null;
			}
			finally
			{
				formSelectItemInProject.Dispose();
			}
			return vSFileType;
		}
	}
}