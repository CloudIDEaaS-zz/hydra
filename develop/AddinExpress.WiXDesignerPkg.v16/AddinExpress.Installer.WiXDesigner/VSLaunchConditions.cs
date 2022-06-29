using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Xml;

namespace AddinExpress.Installer.WiXDesigner
{
	internal class VSLaunchConditions : List<VSLaunchCondition>
	{
		private WiXProjectParser _project;

		public VSLaunchConditions(WiXProjectParser project)
		{
			this._project = project;
		}

		public VSLaunchCondition Add()
		{
			return this.Add("1=0", "<Error Message>");
		}

		public VSLaunchCondition Add(string condition, string errorMessage)
		{
			WiXEntity wiXEntity = null;
			WiXEntity wiXEntity1 = null;
			XmlElement xmlElement = null;
			XmlCDataSection xmlCDataSection = null;
			XmlNode xmlNodes = null;
			this.FindParent(ref wiXEntity, ref wiXEntity1, ref xmlNodes);
			if (wiXEntity == null || wiXEntity1 == null)
			{
				throw new Exception("Cannot create Launch Condition.");
			}
			xmlElement = Common.CreateXmlElementWithAttributes(wiXEntity.XmlNode.OwnerDocument, "Condition", wiXEntity.XmlNode.NamespaceURI, new string[] { "Message" }, new string[] { errorMessage }, "", false);
			xmlCDataSection = wiXEntity.XmlNode.OwnerDocument.CreateCDataSection(condition);
			xmlElement.AppendChild(xmlCDataSection);
			if (xmlNodes == null)
			{
				wiXEntity.XmlNode.AppendChild(xmlElement);
			}
			else
			{
				wiXEntity.XmlNode.InsertAfter(xmlElement, xmlNodes);
			}
			WiXCondition wiXCondition = new WiXCondition(this._project, wiXEntity1, wiXEntity, xmlElement);
			VSLaunchCondition vSLaunchCondition = new VSLaunchCondition(this._project, wiXCondition, this);
			base.Add(vSLaunchCondition);
			wiXEntity.SetDirty();
			return vSLaunchCondition;
		}

		private void FindParent(ref WiXEntity parent, ref WiXEntity owner, ref XmlNode insertAfter)
		{
			if (this._project.LaunchConditions.Count > 0)
			{
				parent = this._project.LaunchConditions[this._project.LaunchConditions.Count - 1].WiXElement.Parent as WiXEntity;
				owner = this._project.LaunchConditions[this._project.LaunchConditions.Count - 1].WiXElement.Owner as WiXEntity;
				insertAfter = this._project.LaunchConditions[this._project.LaunchConditions.Count - 1].WiXElement.XmlNode;
				return;
			}
			if (this._project.Searches.Count > 0)
			{
				parent = this._project.Searches[this._project.Searches.Count - 1].WiXProperty.Parent as WiXEntity;
				owner = this._project.Searches[this._project.Searches.Count - 1].WiXProperty.Owner as WiXEntity;
				insertAfter = this._project.Searches[this._project.Searches.Count - 1].WiXProperty.XmlNode;
				return;
			}
			if (this._project.ProjectType != WiXProjectType.Product)
			{
				parent = this._project.SupportedEntities.Find((WiXEntity e) => e is WiXFragment);
				if (parent != null)
				{
					owner = parent.Owner as WiXEntity;
				}
			}
			else
			{
				parent = this._project.SupportedEntities.Find((WiXEntity e) => e is WiXProduct);
				if (parent != null)
				{
					owner = parent.Owner as WiXEntity;
					WiXEntity wiXEntity = parent.ChildEntities.Find((WiXEntity e) => e.Name == "Package");
					if (wiXEntity != null)
					{
						if (parent.ChildEntities.Find((WiXEntity e) => e.Name == "Media") != null)
						{
							wiXEntity = parent.ChildEntities.Find((WiXEntity e) => e.Name == "Media");
						}
						if (parent.ChildEntities.Find((WiXEntity e) => e.Name == "MediaTemplate") != null)
						{
							wiXEntity = parent.ChildEntities.Find((WiXEntity e) => e.Name == "MediaTemplate");
						}
						insertAfter = wiXEntity.XmlNode;
						return;
					}
				}
			}
		}
	}
}