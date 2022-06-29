using System;
using System.Collections;
using System.Runtime.CompilerServices;
using System.Xml;

namespace AddinExpress.Installer.WiXDesigner
{
	internal interface IViewManager
	{
		void OnActivatePropertiesWindow();

		void OnSelectionChanged(ArrayList selectedObjects);

		void OnTextContentChanged(string filePath, string textLines);

		void OnXmlDocumentContentChanged(string filePath, XmlDocument document);

		event BeforeShowErrorEventHandler BeforeShowError;

		event BuildNotificationEventHandler BuildStarted;

		event BuildNotificationEventHandler BuildStopped;

		event FileAddedEventHandler FileAdded;

		event FileRemovedEventHandler FileRemoved;

		event LoadUserSettingsEventHandler LoadUserSettings;

		event ProjectNotificationEventHandler ProjectClosing;

		event ProjectNotificationEventHandler ProjectParentChanged;

		event ProjectNotificationEventHandler ProjectRenamed;

		event ReferenceAddedEventHandler ReferenceAdded;

		event ReferenceRefreshedEventHandler ReferenceRefreshed;

		event ReferenceRemovedEventHandler ReferenceRemoved;

		event ReferenceRenamedEventHandler ReferenceRenamed;

		event SaveUserSettingsEventHandler SaveUserSettings;

		event SolutionNotificationEventHandler SolutionLoaded;

		event ThemeChangedEventHandler ThemeChanged;

		event ToolWindowActivateEventHandler ToolWindowActivate;

		event ToolWindowBeforeCloseEventHandler ToolWindowBeforeClose;

		event ToolWindowCreatedEventHandler ToolWindowCreated;

		event ToolWindowDeactivateEventHandler ToolWindowDeactivate;
	}
}