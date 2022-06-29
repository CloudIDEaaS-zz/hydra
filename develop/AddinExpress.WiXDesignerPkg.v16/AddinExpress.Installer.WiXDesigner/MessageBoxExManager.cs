using System;
using System.Collections;

namespace AddinExpress.Installer.WiXDesigner
{
	internal class MessageBoxExManager
	{
		private static Hashtable _messageBoxes;

		private static Hashtable _savedResponses;

		private static Hashtable _standardButtonsText;

		static MessageBoxExManager()
		{
			MessageBoxExManager._messageBoxes = new Hashtable();
			MessageBoxExManager._savedResponses = new Hashtable();
			MessageBoxExManager._standardButtonsText = new Hashtable();
			MessageBoxExManager._standardButtonsText[MessageBoxExButtons.Ok.ToString()] = "OK";
			MessageBoxExManager._standardButtonsText[MessageBoxExButtons.Cancel.ToString()] = "Cancel";
			MessageBoxExManager._standardButtonsText[MessageBoxExButtons.Yes.ToString()] = "Yes";
			MessageBoxExManager._standardButtonsText[MessageBoxExButtons.No.ToString()] = "No";
			MessageBoxExManager._standardButtonsText[MessageBoxExButtons.Abort.ToString()] = "Abort";
			MessageBoxExManager._standardButtonsText[MessageBoxExButtons.Retry.ToString()] = "Retry";
			MessageBoxExManager._standardButtonsText[MessageBoxExButtons.Ignore.ToString()] = "Ignore";
		}

		public MessageBoxExManager()
		{
		}

		public static MessageBoxEx CreateMessageBox(string name)
		{
			if (name != null && MessageBoxExManager._messageBoxes.ContainsKey(name))
			{
				throw new ArgumentException(string.Format("A MessageBox with the name {0} already exists.", name), "name");
			}
			MessageBoxEx messageBoxEx = new MessageBoxEx()
			{
				Name = name
			};
			if (messageBoxEx.Name != null)
			{
				MessageBoxExManager._messageBoxes[name] = messageBoxEx;
			}
			return messageBoxEx;
		}

		public static void DeleteMessageBox(string name)
		{
			if (name == null)
			{
				return;
			}
			if (MessageBoxExManager._messageBoxes.Contains(name))
			{
				(MessageBoxExManager._messageBoxes[name] as MessageBoxEx).Dispose();
				MessageBoxExManager._messageBoxes.Remove(name);
			}
		}

		internal static string GetLocalizedString(string key)
		{
			if (!MessageBoxExManager._standardButtonsText.ContainsKey(key))
			{
				return null;
			}
			return (string)MessageBoxExManager._standardButtonsText[key];
		}

		public static MessageBoxEx GetMessageBox(string name)
		{
			if (!MessageBoxExManager._messageBoxes.Contains(name))
			{
				return null;
			}
			return MessageBoxExManager._messageBoxes[name] as MessageBoxEx;
		}

		internal static string GetSavedResponse(MessageBoxEx msgBox)
		{
			string name = msgBox.Name;
			if (name == null)
			{
				return null;
			}
			if (!MessageBoxExManager._savedResponses.ContainsKey(name))
			{
				return null;
			}
			return MessageBoxExManager._savedResponses[msgBox.Name].ToString();
		}

		public static void ResetAllSavedResponses()
		{
			MessageBoxExManager._savedResponses.Clear();
		}

		public static void ResetSavedResponse(string messageBoxName)
		{
			if (messageBoxName == null)
			{
				return;
			}
			if (MessageBoxExManager._savedResponses.ContainsKey(messageBoxName))
			{
				MessageBoxExManager._savedResponses.Remove(messageBoxName);
			}
		}

		internal static void SetSavedResponse(MessageBoxEx msgBox, string response)
		{
			if (msgBox.Name == null)
			{
				return;
			}
			MessageBoxExManager._savedResponses[msgBox.Name] = response;
		}
	}
}