using System;
using System.Drawing;
using System.Runtime.InteropServices;

namespace AddinExpress.Installer.WiXDesigner
{
	internal static class Interop
	{
		[DllImport("uxtheme", CharSet=CharSet.None, ExactSpelling=false)]
		public static extern IntPtr BeginBufferedAnimation(IntPtr hwnd, IntPtr hdcTarget, ref Rectangle rcTarget, BP_BUFFERFORMAT dwFormat, IntPtr pPaintParams, ref BP_ANIMATIONPARAMS pAnimationParams, out IntPtr phdcFrom, out IntPtr phdcTo);

		[DllImport("uxtheme", CharSet=CharSet.None, ExactSpelling=false)]
		public static extern IntPtr BufferedPaintInit();

		[DllImport("uxtheme", CharSet=CharSet.None, ExactSpelling=false)]
		public static extern bool BufferedPaintRenderAnimation(IntPtr hwnd, IntPtr hdcTarget);

		[DllImport("uxtheme", CharSet=CharSet.None, ExactSpelling=false)]
		public static extern IntPtr BufferedPaintStopAllAnimations(IntPtr hwnd);

		[DllImport("uxtheme", CharSet=CharSet.None, ExactSpelling=false)]
		public static extern IntPtr BufferedPaintUnInit();

		[DllImport("uxtheme", CharSet=CharSet.None, ExactSpelling=false)]
		public static extern IntPtr EndBufferedAnimation(IntPtr hbpAnimation, bool fUpdateTarget);
	}
}