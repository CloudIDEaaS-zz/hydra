using System;
using System.Collections.Generic;
using System.Xml;

namespace AddinExpress.Installer.WiXDesigner
{
	internal delegate void ViewXmlModelChangedEventHandler(int paneID, Dictionary<string, XmlDocument> documents);
}