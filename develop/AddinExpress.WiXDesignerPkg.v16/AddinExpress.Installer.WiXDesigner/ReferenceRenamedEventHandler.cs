using System;

namespace AddinExpress.Installer.WiXDesigner
{
	internal delegate void ReferenceRenamedEventHandler(VsWiXProject.ReferenceDescriptor oldReference, VsWiXProject.ReferenceDescriptor newReference);
}