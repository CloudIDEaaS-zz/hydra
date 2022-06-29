using System;

namespace AddinExpress.Installer.WiXDesigner
{
	internal static class GuidList
	{
		public const string guidWiXDesignerPkgString = "5A3C4C1E-8648-4005-2019-09DC4A2AFC21";

		public const string guidWiXDesignerPkgCmdSetString = "6F45E4A1-A326-444B-94B2-F6F931B5A4C9";

		public readonly static Guid guidWiXDesignerPkgCmdSet;

		static GuidList()
		{
			GuidList.guidWiXDesignerPkgCmdSet = new Guid("6F45E4A1-A326-444B-94B2-F6F931B5A4C9");
		}
	}
}