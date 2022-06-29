using System;
using System.Windows.Forms;

namespace AddinExpress.Installer.Prerequisites
{
	internal class ClipboardHelper
	{
		public ClipboardHelper()
		{
		}

		public static void CopyFromGrid(DataGridView gridView, string dataTypeName)
		{
			DataObject clipboardContent = gridView.GetClipboardContent();
			clipboardContent.SetText(string.Concat(dataTypeName, clipboardContent.GetText()));
			Clipboard.SetDataObject(clipboardContent);
		}

		public static bool EnablePasteContextMenu(string dataTypeName)
		{
			return Clipboard.GetText().StartsWith(dataTypeName);
		}

		public static void PasteToGrid(DataGridView gridView, string dataTypeName)
		{
			DataObject dataObject = (DataObject)Clipboard.GetDataObject();
			if (dataObject.GetText().StartsWith(dataTypeName))
			{
				string[] strArrays = dataObject.GetText().Remove(0, dataTypeName.Length).Split(new char[] { "\r\n".ToCharArray()[0] });
				int upperBound = strArrays.GetUpperBound(0);
				for (int i = strArrays.GetLowerBound(0); i <= upperBound; i++)
				{
					string[] strArrays1 = strArrays[i].Trim(new char[] { '\n' }).Trim(new char[] { '\t' }).Split(new char[] { "\t".ToCharArray()[0] });
					int num = gridView.Rows.Add();
					int upperBound1 = strArrays1.GetUpperBound(0);
					for (int j = strArrays1.GetLowerBound(0); j <= upperBound1; j++)
					{
						gridView.Rows[num].Cells[j].Value = strArrays1[j];
					}
				}
			}
		}
	}
}