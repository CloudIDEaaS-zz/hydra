using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Runtime.InteropServices;
using System.Drawing;
using System.Reflection;
using System.Diagnostics;
using System.IO;
using System.ComponentModel;
using Utils.GlyphDrawing;

namespace Utils
{
    public static class ControlExtensions
    {
        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool IsWindowVisible(IntPtr hWnd);
        [DllImport("user32.dll")]
        public static extern bool FlashWindow(IntPtr hwnd, bool bInvert);
        [DllImport("user32.dll")]
        static extern bool FlashWindowEx(ref FLASHWINFO pwfi);
        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = false)]
        private static extern IntPtr SendMessage(IntPtr hWnd, WindowsMessage msg, IntPtr w, IntPtr l);
        [DllImport("user32.dll")]
        public static extern IntPtr FindWindow(string lpClassName, string lpWindowName);
        [DllImport("user32.dll")]
        public static extern IntPtr FindWindowEx(IntPtr hwndParent, IntPtr hwndChildAfter, string lpszClass, string lpszWindow);
        [DllImport("user32.dll")]
        private static extern bool GetClientRect(IntPtr hWnd, out RECT lpRect);
        [DllImport("user32.dll")]
        private static extern IntPtr SendMessage(IntPtr hWnd, WindowsMessage msg, int wParam, int lParam);
        [DllImport("user32.dll", SetLastError = true)]
        private static extern bool PostMessage(IntPtr hWnd, WindowsMessage Msg, int wParam, int lParam);
        [DllImport("user32.dll", SetLastError = true)]
        private static extern bool PostMessage(IntPtr hWnd, WindowsMessage Msg, IntPtr wParam, IntPtr lParam);
        [DllImport("user32.dll")]
        private static extern IntPtr SetFocus(IntPtr hWnd);
        [DllImport("user32.dll")]
        public static extern IntPtr GetFocus();
        [DllImport("user32")]
        private static extern IntPtr SetWindowLong(IntPtr hWnd, WindowLongIndex nIndex, IntPtr newProc);
        [DllImport("user32")]
        private static extern IntPtr SetWindowLong(IntPtr hWnd, WindowLongIndex nIndex, WndProc newProc);
        [DllImport("user32")]
        private static extern IntPtr SetWindowLong(IntPtr hWnd, WindowLongIndex nIndex, WindowStyles styles);
        [DllImport("user32")]
        private static extern IntPtr SetWindowLong(IntPtr hWnd, WindowLongIndex nIndex, uint styles);

        public static void FlashWindow(IntPtr hwndConsole, object p, int v1, int v2)
        {
            throw new NotImplementedException();
        }

        [DllImport("user32.dll")]
        private static extern IntPtr DefWindowProc(IntPtr hWnd, int uMsg, int wParam, int lParam);
        [DllImport("user32")]
        private static extern IntPtr CallWindowProc(IntPtr lpPrevWndFunc, IntPtr hWnd, int Msg, int wParam, int lParam);
        public delegate IntPtr WndProc(IntPtr hWnd, int Msg, int wParam, int lParam);
        [DllImport("user32.dll", ExactSpelling = true)]
        public static extern IntPtr SetTimer(IntPtr hWnd, int nIDEvent, uint uElapse, [MarshalAs(UnmanagedType.FunctionPtr)] TimerProc lpTimerFunc);
        [DllImport("user32.dll", ExactSpelling = true)]
        public static extern IntPtr SetTimer(IntPtr hWnd, int nIDEvent, uint uElapse, IntPtr lpTimerFunc);
        [DllImport("user32.dll", ExactSpelling = true)]
        public static extern bool KillTimer(IntPtr hWnd, int uIDEvent);
        [DllImport("user32.dll")]
        public static extern bool PeekMessage(out NativeMessage lpMsg, IntPtr hWnd, uint wMsgFilterMin, uint wMsgFilterMax, uint wRemoveMsg);
        [DllImport("user32.dll", SetLastError = true)]
        static extern uint GetWindowThreadProcessId(IntPtr hWnd, out uint lpdwProcessId);
        [DllImport("user32.dll", SetLastError = true)]
        static extern IntPtr SetParent(IntPtr hWndChild, IntPtr hWndNewParent);
        [DllImport("user32.dll", SetLastError = true)]
        static extern bool SetWindowPos(IntPtr hWnd, IntPtr hWndInsertAfter, int x, int y, int cx, int cy, SetWindowPosFlags uFlags);
        [DllImport("user32.dll")]
        public static extern IntPtr GetActiveWindow();
        [DllImport("user32.dll")]
        public static extern IntPtr GetForegroundWindow();
        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        public static extern int GetWindowText(IntPtr hWnd, StringBuilder lpString, int nMaxCount);
        [DllImport("user32.dll")]
        public static extern IntPtr WindowFromPoint(Point p);
        [DllImport("kernel32.dll")]
        public static extern IntPtr GetConsoleWindow();
        [DllImport("user32.dll")]
        public static extern IntPtr GetParent(IntPtr hWnd);
        [DllImport("user32")]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool EnumChildWindows(IntPtr window, EnumWindowProc callback, IntPtr i);
        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool EnableWindow(IntPtr hWnd, bool bEnable);
        [DllImport("user32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool AddClipboardFormatListener(IntPtr hwnd);
        [DllImport("user32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool RemoveClipboardFormatListener(IntPtr hwnd);
        [DllImport("user32.dll", SetLastError = true, CharSet = CharSet.Auto)]
        static extern int GetClassName(IntPtr hWnd, StringBuilder lpClassName, int nMaxCount);
        [DllImport("user32.dll")]
        public static extern bool RedrawWindow(IntPtr hWnd, ref RECT lprcUpdate, IntPtr hrgnUpdate, RedrawWindowFlags flags);
        [DllImport("user32.dll")]
        static extern int GetMessage(out NativeMessage lpMsg, IntPtr hWnd, uint wMsgFilterMin, uint wMsgFilterMax); private static int MOD_ALT = 0x1;
        [DllImport("user32.dll")]
        static extern int SetWindowLong(IntPtr hWnd, int nIndex, int dwNewLong);
        [DllImport("user32.dll")]
        static extern int CallWindowProc(int lpPrevWndFunc, IntPtr hWnd, uint Msg, int wParam, IntPtr lParam);
        [DllImport("user32.dll", SetLastError = true)]
        static extern bool GetWindowRect(int hwnd, out Rectangle lpRect);
        [DllImport("User32.dll")]
        static extern bool MoveWindow(IntPtr handle, int x, int y, int width, int height, bool redraw);
        [DllImport("user32.dll", SetLastError = true)]
        public static extern bool GetWindowRect(IntPtr hwnd, out RECT lpRect);
        [DllImport("user32.dll")]
        internal static extern uint SendInput(uint nInputs, [MarshalAs(UnmanagedType.LPArray), In] INPUT[] pInputs, int cbSize);
        [DllImport("user32.dll")]
        internal static extern bool ShowWindowAsync(IntPtr hWnd, ShowWindowCommands nCmdShow);
        [DllImport("user32.dll", SetLastError = false)]
        public static extern IntPtr GetDesktopWindow();
        [DllImport("user32.dll")]
        static extern bool ClientToScreen(IntPtr hWnd, ref Point lpPoint);
        [DllImport("user32.dll")]
        public static extern int GetSystemMetrics(SystemMetric smIndex);
        [DllImport("user32.dll")]
        public static extern bool CreateCaret(IntPtr hWnd, IntPtr hBitmap, int nWidth, int nHeight);
        [DllImport("user32.dll")]
        public static extern bool ShowCaret(IntPtr hWnd);
        [DllImport("user32.dll")]
        public static extern bool HideCaret(IntPtr hWnd);
        [DllImport("user32.dll")]
        public static extern bool DestroyCaret();
        [DllImport("user32.dll", SetLastError=true)]
        public static extern bool SetCaretPos(int x, int y);
        [DllImport("gdi32.dll", CharSet = CharSet.Auto)]
        static extern bool GetTextMetrics(IntPtr hdc, out TEXTMETRIC lptm);
        [DllImport("gdi32.dll", EntryPoint = "SelectObject")]
        public static extern IntPtr SelectObject([In] IntPtr hdc, [In] IntPtr hgdiobj);
        [DllImport("gdi32.dll", EntryPoint = "DeleteObject")]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool DeleteObject([In] IntPtr hObject);
        [DllImport("gdi32.dll", CharSet = CharSet.Auto)]
        public static extern bool TextOut(IntPtr hdc, int nXStart, int nYStart, string lpString, int cbString);
        [DllImport("user32.dll")]
        public static extern IntPtr GetCapture();
        [DllImport("user32.dll")]
        public static extern IntPtr SetCapture(IntPtr hWnd);
        [DllImport("user32.dll")]
        static extern bool ReleaseCapture();
        [DllImport("user32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
        static extern bool DestroyWindow(IntPtr hwnd);
        internal delegate int WndProcDelegate(IntPtr hWnd, uint msg, int wParam, IntPtr lParam);
        [DllImport("user32.dll", SetLastError = true)]
        public static extern bool TrackMouseEvent(ref TRACKMOUSEEVENT lpEventTrack);
        private static int MOD_CONTROL = 0x2;
        private static int MOD_SHIFT = 0x4;
        private static int MOD_WIN = 0x8;
        private static int WM_HOTKEY = 0x312;

        [DllImport("user32.dll")]
        private static extern bool RegisterHotKey(IntPtr hWnd, int id, int fsModifiers, int vlc);

        [DllImport("user32.dll")]
        public static extern bool UnregisterHotKey(IntPtr hWnd, int id);

        public delegate void TimerProc(IntPtr hWnd, uint uMsg, int nIDEvent, uint dwTime);
        public delegate bool EnumWindowProc(IntPtr hWnd, IntPtr parameter); 
        private const int GWL_WNDPROC = -4;
        private static bool enableClickOnActivateNoReenter;
        private static GlyphManager glyphManager;

        static ControlExtensions()
        {
            glyphManager = new GlyphManager();
        }

        public enum BeepType : uint
        {
            SimpleBeep = 0xFFFFFFFF,
            OK = 0x00,
            Question = 0x20,
            Exclamation = 0x30,
            Asterisk = 0x40,
        }

        [DllImport("User32.dll", ExactSpelling = true)]
        private static extern bool MessageBeep(uint type);

        public static TEXTMETRIC GetTextMetrics(this Graphics graphics, Font font)
        {
            IntPtr hDC = graphics.GetHdc();
            TEXTMETRIC textMetric;
            IntPtr hFont = font.ToHfont();

            try
            {
                IntPtr hFontPrevious = SelectObject(hDC, hFont);
                bool result = GetTextMetrics(hDC, out textMetric);
                SelectObject(hDC, hFontPrevious);
            }
            finally
            {
                DeleteObject(hFont);
            }

            return textMetric;
        }

        public static void Destroy(this Control control)
        {
            DestroyWindow(control.Handle);
        }

        public static void AllowTabRename(this TabControl tabControl)
        {
            tabControl.MouseDoubleClick += tabControl_MouseDoubleClick;

            glyphManager.RenamableTabControls.Add(tabControl);

            tabControl.Disposed += (sender, e) =>
            {
                glyphManager.RenamableTabControls.Remove(tabControl);
            };
        }

        public static void MoveWindow(this Control control, int x, int y, int width, int height, bool redraw)
        {
            MoveWindow(control.Handle, x, y, width, height, redraw);
        }

        public static Rectangle GetWindowRect(this Control control)
        {
            Rectangle rect;

            GetWindowRect((int) control.Handle, out rect);

            return rect;
        }

        public static void AllowDeleteTab(this TabControl tabControl)
        {
            foreach (var tabPage in tabControl.TabPages.Cast<TabPage>())
            {
                tabPage.AddTabPageDeleteGlyph();
            }

            tabControl.ControlAdded += (sender, e) =>
            {
                if (e.Control is TabPage)
                {
                    var tabPage = (TabPage)e.Control;

                    tabPage.AddTabPageDeleteGlyph();
                }
            };

            tabControl.ControlRemoved += (sender, e) =>
            {
                if (e.Control is TabPage)
                {
                    var tabPage = (TabPage)e.Control;
                    var glyph = tabPage.GetDeleteGlyph(tabControl);

                    if (glyph != null)
                    {
                        glyphManager.Glyphs.RemoveFromBaseDictionaryListIfExist(tabControl, glyph);
                        tabControl.Refresh();
                    }
                }
            };

            tabControl.MouseMove += (sender, e) =>
            {
                var refresh = false;

                for (var x = 0; x < tabControl.TabCount; x++)
                {
                    var tabRect = tabControl.GetTabRect(x);
                    var tabPage = tabControl.TabPages[x];
                    var glyph = tabPage.GetDeleteGlyph();

                    if (tabRect.Contains(e.Location))
                    {
                        if (glyph != null && !glyph.FocusedOrHovered)
                        {
                            glyph.FocusedOrHovered = true;
                            refresh = true;
                        }
                    }
                    else
                    {
                        if (glyph != null && glyph.FocusedOrHovered)
                        {
                            if (tabControl.SelectedTab != tabPage)
                            {
                                glyph.FocusedOrHovered = false;
                                refresh = false;
                            }
                        }
                    }
                }

                if (refresh)
                {
                    tabControl.Refresh();
                }
            };

            tabControl.MouseLeave += (sender, e) =>
            {
                var refresh = false;

                foreach (var tabPage in tabControl.TabPages.Cast<TabPage>())
                {
                    var glyph = tabPage.GetDeleteGlyph();

                    if (glyph != null && glyph.FocusedOrHovered)
                    {
                        if (tabControl.SelectedTab != tabPage)
                        {
                            glyph.FocusedOrHovered = false;
                            refresh = true;
                        }
                    }
                }

                if (refresh)
                {
                    tabControl.Refresh();
                }
            };

            tabControl.SelectedIndexChanged += (sender, e) =>
            {
                foreach (var tabPage in tabControl.TabPages.Cast<TabPage>())
                {
                    var glyph = tabPage.GetDeleteGlyph();

                    if (tabControl.SelectedTab == tabPage)
                    {
                        glyph.FocusedOrHovered = true;
                    }
                    else
                    {
                        glyph.FocusedOrHovered = false;
                    }

                    if (glyph != null)
                    {
                        tabPage.PositionDeleteGlyph(glyph);
                    }
                }
            };
        }

        private static void AddTabPageDeleteGlyph(this TabPage tabPage)
        {
            var glyph = new ControlGlyph();
            var index = tabPage.GetTabPageIndex();
            var tabControl = (TabControl) tabPage.Parent;
            var tabRect = tabControl.GetTabRect(index);
            var spacer = " ".Repeat(4);

            glyph.ImageChar = (char)0xcd;
            glyph.Font = new Font("Wingdings 2", 8);
            glyph.Size = new Size(12, 12);
            glyph.ShowOnFocusHoverOnly = true;
            glyph.Name = string.Format("TabControlDeleteButton{0}", tabPage.Handle);

            if (!tabPage.Text.EndsWith(spacer))
            {
                var addGlyph = tabControl.GetAddGlyph();

                tabPage.Text += spacer;
                tabRect = tabControl.GetTabRect(index);

                tabControl.PositionAddGlyph(addGlyph);
            }

            glyph.Click += (sender, e) =>
            {
                tabControl.TabPages.Remove(tabPage);
            };

            if (tabControl.SelectedTab == tabPage)
            {
                glyph.Offset = new Point(0, 0);
            }
            else
            {
                glyph.Offset = new Point(-2, 2);
            }

            if (tabControl.SelectedTab == tabPage)
            {
                glyph.FocusedOrHovered = true;
            }
            else
            {
                glyph.FocusedOrHovered = false;
            }

            glyph.TextOffset = new Point(-1, 1);
            glyph.Brush = new SolidBrush(SystemColors.ControlDark);
            glyph.Location = new Point(tabRect.Right - 12, tabRect.Y + 2);

            glyphManager.Glyphs.AddToBaseDictionaryListCreateIfNotExist(tabControl, glyph);

            tabControl.Refresh();
        }

        public static int GetTabPageIndex(this TabPage tabPage)
        {
            var tabControl = (TabControl)tabPage.Parent;
            var index = tabControl.TabPages.IndexOf(tabPage);

            return index;
        }

        private static void PositionDeleteGlyph(this TabPage tabPage, ControlGlyph glyph)
        {
            var tabControl = (TabControl)tabPage.Parent;
            var index = tabPage.GetTabPageIndex();

            if (tabControl.TabPages.Count > index)
            {
                var tabRect = tabControl.GetTabRect(index);

                if (tabControl.SelectedTab == tabPage)
                {
                    glyph.Offset = new Point(0, 0);
                }
                else
                {
                    glyph.Offset = new Point(-2, 2);
                }

                glyph.Location = new Point(tabRect.Right - 12, tabRect.Y + 2);
            }

            tabControl.Refresh();
        }

        private static ControlGlyph GetDeleteGlyph(this TabPage tabPage)
        {
            var tabControl = (TabControl) tabPage.Parent;
            var name = string.Format("TabControlDeleteButton{0}", tabPage.Handle);

            if (glyphManager.Glyphs.ContainsKey(tabControl))
            {
                return glyphManager.Glyphs[tabControl].SingleOrDefault(g => g.Name == name);
            }
            else
            {
                return null;
            }
        }

        private static ControlGlyph GetDeleteGlyph(this TabPage tabPage, TabControl tabControl)
        {
            var name = string.Format("TabControlDeleteButton{0}", tabPage.Handle);

            if (glyphManager.Glyphs.ContainsKey(tabControl))
            {
                return glyphManager.Glyphs[tabControl].SingleOrDefault(g => g.Name == name);
            }
            else
            {
                return null;
            }
        }

        public static void AllowAddTab(this TabControl tabControl)
        {
            var lastTabRect = tabControl.ClientRectangle;
            var glyph = new ControlGlyph();

            glyph.ImageChar = (char)0xca;
            glyph.Font = new Font("Wingdings 2", 10);
            glyph.Size = new Size(14, 14);
            glyph.Name = "TabControlAddButton";
            glyph.Offset = new Point(-3, 3);
            glyph.TextOffset = new Point(-1, 1);
            glyph.Brush = new SolidBrush(Color.Green);

            glyphManager.Glyphs.AddToBaseDictionaryListCreateIfNotExist(tabControl, glyph);

            for (var x = 0; x < tabControl.TabCount; x++)
            {
                lastTabRect = tabControl.GetTabRect(x);
            }

            lastTabRect.Location = new Point(lastTabRect.Right + 10, lastTabRect.Top - 2);
            lastTabRect.Width = glyph.Size.Width;
            lastTabRect.Height = glyph.Size.Height;

            tabControl.PositionAddGlyph(glyph);

            tabControl.Resize += (sender, e) =>
            {
                tabControl.PositionAddGlyph(glyph);
            };

            tabControl.ControlAdded += (sender, e) =>
            {
                tabControl.PositionAddGlyph(glyph);
            };

            tabControl.ControlRemoved += (sender, e) =>
            {
                tabControl.DelayInvoke(1, () =>
                {
                    tabControl.PositionAddGlyph(glyph);
                    tabControl.Refresh();
                });
            };

            glyph.Click += (sender, e) =>
            {
                var tabPage = new TabPage();

                tabControl.TabPages.Add(tabPage);
                tabControl.SelectedTab = tabPage;

                tabControl.PositionAddGlyph(glyph);

                if (glyphManager.RenamableTabControls.Contains(tabControl))
                {
                    var tabRect = tabControl.GetTabRect(tabPage.GetTabPageIndex());
                    var point = new Point(tabRect.X + 1, tabRect.Y + 1);
                    var args = new MouseEventArgs(MouseButtons.Left, 1, point.X, point.Y, 0);

                    CheckForTabRenameRequest(tabControl, args, true);
                }
            };
        }

        private static ControlGlyph GetAddGlyph(this TabControl tabControl)
        {
            if (glyphManager.Glyphs.ContainsKey(tabControl))
            {
                return glyphManager.Glyphs[tabControl].SingleOrDefault(g => g.Name == "TabControlAddButton");
            }
            else
            {
                return null;
            }
        }

        private static TextEdit GetTextEdit(this TabControl tabControl)
        {
            TextEdit textEdit = null;

            EnumChildWindows(tabControl.Handle, (hWndChild, parameter) =>
            {
                var control = Control.FromChildHandle(hWndChild);

                if (control is TextEdit)
                {
                    textEdit = (TextEdit)control;
                    return false;
                }

                return true;

            }, IntPtr.Zero);

            return textEdit;
        }

        private static void PositionAddGlyph(this TabControl tabControl, ControlGlyph glyph)
        {
            var lastTabRect = tabControl.GetLastTabRect();

            lastTabRect.Location = new Point(lastTabRect.Right + 10, lastTabRect.Top - 2);
            lastTabRect.Width = glyph.Size.Width;
            lastTabRect.Height = glyph.Size.Height;

            glyph.Location = lastTabRect.Location;
        }

        private static Rectangle GetLastTabRect(this TabControl tabControl)
        {
            var lastTabRect = tabControl.ClientRectangle;

            lastTabRect.X = -8;
            lastTabRect.Width = 0;
            lastTabRect.Height = 0; 

            for (var x = 0; x < tabControl.TabCount; x++)
            {
                lastTabRect = tabControl.GetTabRect(x);
            }

            return lastTabRect;
        }

        private static void tabControl_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            var tabControl = (TabControl)sender;

            CheckForTabRenameRequest(tabControl, e, true);
        }

        private static void CheckForTabRenameRequest(TabControl tabControl, MouseEventArgs e, bool immediateRename = false)
        {
            for (var x = 0; x < tabControl.TabCount; ++x)
            {
                var tabRect = tabControl.GetTabRect(x);

                if (tabControl.GetTabRect(x).Contains(new Point(e.X, e.Y)))
                {
                    var tab = tabControl.TabPages[x];

                    if (immediateRename)
                    {
                        BeginRenameTab(tabControl, tab, tabRect);
                    }
                    else
                    {
                        tabControl.DelayInvoke(1000, () =>
                        {
                            var point = Cursor.Position;

                            point = tabControl.PointToClient(point);

                            if (tabRect.Contains(point))
                            {
                                BeginRenameTab(tabControl, tab, tabRect);
                            }
                        });
                    }

                    break;
                }
            }
        }

        private static void BeginRenameTab(TabControl tabControl, TabPage tabPage, Rectangle tabRect)
        {
            var textEdit = new TextEdit();
            var addGlyph = tabControl.GetAddGlyph();
            var deleteGlyph = tabPage.GetDeleteGlyph();
            var destroying = false;
            Action<bool> destroyEdit;

            tabPage.Text = " ".Repeat(20);
            tabRect = tabControl.GetTabRect(tabControl.TabPages.IndexOf(tabPage));

            if (addGlyph != null)
            {
                tabControl.PositionAddGlyph(addGlyph);
                tabControl.Refresh();
            }

            tabRect.Inflate(-4, -2);

            textEdit.CreateControl();
            textEdit.SetAsChildOf(tabControl);

            textEdit.SetWindowPos(tabRect, ControlExtensions.SetWindowPosFlags.ShowWindow);
            textEdit.BringToFront();
            textEdit.Focus();

            destroyEdit = (restoreOnEmpty) =>
            {
                if (destroying)
                {
                    return;
                }

                destroying = true;

                textEdit = tabControl.GetTextEdit();

                if (textEdit != null)
                {
                    if (textEdit.Text.IsNullOrEmpty() && restoreOnEmpty)
                    {
                        tabPage.Text = string.Empty;
                    }
                    else
                    {
                        if (deleteGlyph != null)
                        {
                            tabPage.Text = textEdit.Text + " ".Repeat(4);
                        }
                        else
                        {
                            tabPage.Text = textEdit.Text;
                        }
                    }
                }

                // Guess destroy doesn't always mean destroy

                while (textEdit != null)
                {
                    textEdit.SetAsChildOf(null);
                    textEdit.Destroy();

                    textEdit = tabControl.GetTextEdit();
                }

                tabControl.Invalidate();
                tabControl.GetParentForm().Refresh();

                if (addGlyph != null)
                {
                    tabControl.PositionAddGlyph(addGlyph);
                    tabControl.Refresh();
                }

                if (deleteGlyph != null)
                {
                    tabPage.PositionDeleteGlyph(deleteGlyph);
                }
            };

            textEdit.LostFocus += (sender, e) =>
            {
                destroyEdit(true);
            };

            textEdit.KeyDown += (sender, e) =>
            {
                if (e.KeyCode == Keys.Enter)
                {
                    destroyEdit(false);
                }
                else if (e.KeyCode == Keys.Escape)
                {
                    destroyEdit(true);
                }
            };
        }

        public static Control GetCapture(this Control control)
        {
            var hwnd = GetCapture();
            var ctrl = Control.FromHandle(hwnd);

            if (ctrl == null)
            {
                ctrl = Control.FromChildHandle(hwnd);
            }

            return ctrl;
        }

        public static bool SetCapture(this Control control)
        {
            var hwnd = control.Handle;

            return SetCapture(hwnd) != IntPtr.Zero;
        }

        public static bool ReleaseCapture(this Control control)
        {
            var hwnd = control.Handle;

            if (GetCapture() == hwnd)
            {
                return ReleaseCapture();
            }

            return false;
        }

        public static bool TextOut(this Graphics graphics, int x, int y, string str, int cb)
        {
            return TextOut(graphics.GetHdc(), x, y, str, cb);
        }

        public static bool TextOut(this Graphics graphics, Font font, int x, int y, string str)
        {
            return graphics.TextOut(font, x, y, str, str.Length);
        }

        public static bool TextOut(this Graphics graphics, Font font, int x, int y, string str, int cb)
        {
            IntPtr hDC = graphics.GetHdc();
            IntPtr hFont = font.ToHfont();

            IntPtr hFontPrevious = SelectObject(hDC, hFont);
            var result = TextOut(hDC, x, y, str, cb);

            SelectObject(hDC, hFontPrevious);
            DeleteObject(hFont);

            return result;
        }

        public static bool Flash(this Form form, FlashWindowFlags flags, int count = 0, int timeout = 0)
        {
            return FlashWindow(form.Handle, flags, count, timeout);
        }

        public static bool Flash(IntPtr handle, FlashWindowFlags flags, int count = 0, int timeout = 0)
        {
            return FlashWindow(handle, flags, count, timeout);
        }

        public static bool Flash(this Form form, FlashWindowFlags flags)
        {
            return FlashWindow(form.Handle, flags, 0, 0);
        }

        public static bool Flash(this Form form, bool invert = false)
        {
            return FlashWindow(form.Handle, invert);
        }

        public static bool FlashWindow(IntPtr hwnd, FlashWindowFlags flags, int count, int timeout)
        {
            var flashWindowInfo = new FLASHWINFO
            {
                hwnd = hwnd,
                dwFlags = flags,
                uCount = (uint)count,
                dwTimeout = (uint)timeout
            };

            flashWindowInfo.cbSize = (uint)Marshal.SizeOf(flashWindowInfo);

            return FlashWindowEx(ref flashWindowInfo);
        }

        public static void SetStatusFormat(this StatusStrip statusStrip, string format, params object[] args)
        {
            var properties = StatusStripProperties.RegisteredStatusStrips.AddToDictionaryIfNotExist(statusStrip, () => new StatusStripProperties(statusStrip));

            statusStrip.Invoke(() =>
            {
                properties.StatusLabel.Text = string.Format(format, args);

                Application.DoEvents();
            });
        }

        public static void SetStatus(this StatusStrip statusStrip, string statusText, Color foreColor, Color backColor)
        {
            var properties = StatusStripProperties.RegisteredStatusStrips.AddToDictionaryIfNotExist(statusStrip, () => new StatusStripProperties(statusStrip));

            statusStrip.Invoke(() =>
            {
                properties.StatusLabel.Text = statusText;
                properties.StatusLabel.ForeColor = foreColor;
                properties.StatusLabel.BackColor = backColor;

                Application.DoEvents();
            });
        }

        public static void SetStatusFormat(this StatusStrip statusStrip, string format, int progressPercent, params object[] args)
        {
            var properties = StatusStripProperties.RegisteredStatusStrips.AddToDictionaryIfNotExist(statusStrip, () => new StatusStripProperties(statusStrip));

            statusStrip.Invoke(() =>
            {
                properties.StatusLabel.Text = string.Format(format, args);
                properties.ProgressBar.Value = Math.Min(progressPercent, 100);

                Application.DoEvents();
            });
        }

        public static void SetStatus(this StatusStrip statusStrip, string statusText, int progressPercent = 0)
        {
            var properties = StatusStripProperties.RegisteredStatusStrips.AddToDictionaryIfNotExist(statusStrip, () => new StatusStripProperties(statusStrip));

            if (properties.PendingTempStatus)
            {
                properties.CancelTempStatus = true;
            }

            statusStrip.Invoke(() =>
            {
                properties.StatusLabel.Text = statusText;
                properties.ProgressBar.Value = Math.Min(progressPercent, 100);

                if (!properties.ProgressBar.Visible)
                {
                    properties.ProgressBar.Visible = true;
                }

                Application.DoEvents();
            });
        }

        public static void SetTemporaryStatusFormat(this StatusStrip statusStrip, string format, int progressPercent, int delay, params object[] args)
        {
            var properties = StatusStripProperties.RegisteredStatusStrips.AddToDictionaryIfNotExist(statusStrip, () => new StatusStripProperties(statusStrip));

            properties.PendingTempStatus = true;

            statusStrip.Invoke(() =>
            {
                var oneTimeTimer = new OneTimeTimer(delay);

                properties.StatusLabel.Text = string.Format(format, args);
                properties.ProgressBar.Value = Math.Min(progressPercent, 100);

                Application.DoEvents();

                oneTimeTimer.Start(() =>
                {
                    if (properties.CancelTempStatus)
                    {
                        properties.CancelTempStatus = false;
                        properties.PendingTempStatus = false;
                    }
                    else
                    {
                        statusStrip.ResetStatus();
                        properties.PendingTempStatus = false;
                    }
            });

            });
        }

        public static void SetTemporaryStatusFormat(this StatusStrip statusStrip, string format, int delay, params object[] args)
        {
            var properties = StatusStripProperties.RegisteredStatusStrips.AddToDictionaryIfNotExist(statusStrip, () => new StatusStripProperties(statusStrip));

            properties.PendingTempStatus = true;

            statusStrip.Invoke(() =>
            {
                var oneTimeTimer = new OneTimeTimer(delay);

                properties.StatusLabel.Text = string.Format(format, args);

                Application.DoEvents();

                oneTimeTimer.Start(() =>
                {
                    if (properties.CancelTempStatus)
                    {
                        properties.CancelTempStatus = false;
                        properties.PendingTempStatus = false;
                    }
                    else
                    {
                        statusStrip.ResetStatus();
                        properties.PendingTempStatus = false;
                    }
                });

            });
        }

        public static void SetTemporaryStatus(this StatusStrip statusStrip, string statusText, int delay, int progressPercent = 0)
        {
            var properties = StatusStripProperties.RegisteredStatusStrips.AddToDictionaryIfNotExist(statusStrip, () => new StatusStripProperties(statusStrip));

            properties.PendingTempStatus = true;

            statusStrip.Invoke(() =>
            {
                var oneTimeTimer = new OneTimeTimer(delay);

                properties.StatusLabel.Text = statusText;
                properties.ProgressBar.Value = progressPercent;

                Application.DoEvents();
                statusStrip.Refresh();

                oneTimeTimer.Start(() =>
                {
                    if (properties.CancelTempStatus)
                    {
                        properties.CancelTempStatus = false;
                        properties.PendingTempStatus = false;
                    }
                    else
                    {
                        statusStrip.ResetStatus();
                        properties.PendingTempStatus = false;
                    }
                });
            });
        }

        public static void PushStatus(this StatusStrip statusStrip, string statusText, Color foreColor, Color backColor)
        {
            var properties = StatusStripProperties.RegisteredStatusStrips.AddToDictionaryIfNotExist(statusStrip, () => new StatusStripProperties(statusStrip));

            statusStrip.Invoke(() =>
            {
                var newStatusEntry = new StatusEntry(statusText, foreColor, backColor);
                var currentStatusEntry = new StatusEntry(properties.StatusLabel.Text, statusStrip.ForeColor, statusStrip.BackColor);

                if (properties.StatusStack.Count > 0)
                {
                    var stackStatusEntry = properties.StatusStack.Peek();

                    if (stackStatusEntry != currentStatusEntry)
                    {
                        properties.StatusStack.Push(currentStatusEntry);
                    }
                }
                else
                {
                    properties.StatusStack.Push(currentStatusEntry);
                }

                properties.StatusStack.Push(newStatusEntry);

                properties.StatusLabel.Text = statusText;

                statusStrip.ForeColor = foreColor;
                statusStrip.BackColor = backColor;

                Application.DoEvents();
            });
        }

        public static void PushStatus(this StatusStrip statusStrip, string statusText)
        {
            var properties = StatusStripProperties.RegisteredStatusStrips.AddToDictionaryIfNotExist(statusStrip, () => new StatusStripProperties(statusStrip));

            statusStrip.Invoke(() =>
            {
                var newStatusEntry = new StatusEntry(statusText);
                var currentStatusEntry = new StatusEntry(properties.StatusLabel.Text, statusStrip.ForeColor, statusStrip.BackColor);

                if (properties.StatusStack.Count > 0)
                {
                    var stackStatusEntry = properties.StatusStack.Peek();

                    if (stackStatusEntry != currentStatusEntry)
                    {
                        properties.StatusStack.Push(currentStatusEntry);
                    }
                }
                else
                {
                    properties.StatusStack.Push(currentStatusEntry);
                }

                properties.StatusStack.Push(newStatusEntry);

                properties.StatusLabel.Text = statusText;

                Application.DoEvents();
            });
        }

        public static void PopPushStatus(this StatusStrip statusStrip, string oldStatus, string statusText)
        {
            statusStrip.PopStatus(oldStatus);
            statusStrip.PushStatus(statusText);
        }

        public static void PopStatus(this StatusStrip statusStrip, string statusText, Color foreColor, Color backColor)
        {
            var properties = StatusStripProperties.RegisteredStatusStrips.AddToDictionaryIfNotExist(statusStrip, () => new StatusStripProperties(statusStrip));

            statusStrip.Invoke(() =>
            {
                var popStatusEntry = new StatusEntry(statusText);

                if (properties.StatusStack.Peek() == popStatusEntry)
                {
                    properties.StatusStack.Pop();
                }
                else
                {
                    properties.PendingPopStatusQueue.Enqueue(popStatusEntry);
                }

                if (properties.PendingPopStatusQueue.Count > 0)
                {
                    if (properties.StatusStack.Peek() == properties.PendingPopStatusQueue.Peek())
                    {
                        properties.PendingPopStatusQueue.Dequeue();
                        properties.StatusStack.Pop();
                    }
                }

                if (properties.StatusStack.Count > 0)
                {
                    var currentStatusEntry = properties.StatusStack.Pop();

                    properties.StatusLabel.Text = currentStatusEntry.StatusText;

                    if (currentStatusEntry.ForeColor != Color.Transparent)
                    {
                        statusStrip.ForeColor = currentStatusEntry.ForeColor;
                    }

                    if (currentStatusEntry.BackColor != Color.Transparent)
                    {
                        statusStrip.BackColor = currentStatusEntry.BackColor;
                    }
                }
                else
                {
                    DebugUtils.Break();
                }

                Application.DoEvents();
            });
        }

        public static void PopStatus(this StatusStrip statusStrip, string statusText)
        {
            statusStrip.PopStatus(statusText, Color.Transparent, Color.Transparent);
        }

        public static void ResetStatus(this StatusStrip statusStrip, string statusText = "")
        {
            var properties = StatusStripProperties.RegisteredStatusStrips.AddToDictionaryIfNotExist(statusStrip, () => new StatusStripProperties(statusStrip));

            statusStrip.Invoke(() =>
            {
                properties.StatusLabel.Text = statusText;
                statusStrip.ForeColor = properties.StatusStripDefaultForeColor;
                statusStrip.BackColor = properties.StatusStripDefaultBackColor;
                properties.ProgressBar.Value = 0;
                properties.ProgressBar.Visible = false;

                Application.DoEvents();
            });
        }

        public static void ClearStatus(this StatusStrip statusStrip)
        {
            var properties = StatusStripProperties.RegisteredStatusStrips.AddToDictionaryIfNotExist(statusStrip, () => new StatusStripProperties(statusStrip));

            statusStrip.Invoke(() =>
            {
                properties.StatusLabel.Text = string.Empty;
                statusStrip.ForeColor = properties.StatusStripDefaultForeColor;
                statusStrip.BackColor = properties.StatusStripDefaultBackColor;
                properties.ProgressBar.Value = 0;
                properties.ProgressBar.Visible = false;

                Application.DoEvents();
            });
        }

        public static bool RegisterHotKey(this Control control, Keys key, int id)
        {
            return RegisterHotKey(control.Handle, key, id);
        }

        public static bool RegisterHotKey(IntPtr window, Keys key, int id)
        {
            int modifiers = 0;

            if ((key & Keys.Alt) == Keys.Alt)
            {
                modifiers = modifiers | MOD_ALT;
            }

            if ((key & Keys.Control) == Keys.Control)
            {
                modifiers = modifiers | MOD_CONTROL;
            }

            if ((key & Keys.Shift) == Keys.Shift)
            {
                modifiers = modifiers | MOD_SHIFT;
            }

            Keys k = key & ~Keys.Control & ~Keys.Shift & ~Keys.Alt;
            
            return RegisterHotKey(window, id, modifiers, (int)k);
        }

        public static void UnregisterHotKey(this Control control, int id)
        {
            try
            {
                UnregisterHotKey(control.Handle, id);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }

        public static NativeMessageWithResult GetMessage(this Control control)
        {
            NativeMessage msg;
            int ret;

            ret = GetMessage(out msg, IntPtr.Zero, 0, 0);

            return new NativeMessageWithResult
            {
                handle = msg.handle,
                message = msg.msg,
                wParam = msg.wParam,
                lParam = msg.lParam,
                time = TimeSpan.FromTicks(msg.time),
                point = msg.point,
                returnValue = ret
            };
        }

        public static void Beep(this Control control, BeepType type = BeepType.SimpleBeep)
        {
            MessageBeep((uint)type);
        }

        public static List<IntPtr> GetChildWindows(IntPtr parent)
        {
            List<IntPtr> result = new List<IntPtr>();
            GCHandle listHandle = GCHandle.Alloc(result);

            try
            {
                EnumWindowProc childProc = new EnumWindowProc(EnumWindow);
                EnumChildWindows(parent, childProc, GCHandle.ToIntPtr(listHandle));
            }
            finally
            {
                if (listHandle.IsAllocated)
                    listHandle.Free();
            }
        
            return result;
        }

        public static IntPtr GetInternetExplorerServerWindow(this WebBrowser webBrowser)
        {
            var internetExplorerServer = IntPtr.Zero;
            var foundShellView = false;

            foreach (var hwndChild in ControlExtensions.GetChildWindows(webBrowser.Handle))
            {
                if (foundShellView)
                {
                    internetExplorerServer = hwndChild;
                }
                else
                {
                    var name = ControlExtensions.GetClassName(hwndChild);

                    if (name == "Shell DocObject View")
                    {
                        foundShellView = true;
                    }
                }
            }

            return internetExplorerServer;
        }

        public static string GetClassName(IntPtr handle)
        {
            var builder = new StringBuilder();
            string className;

            GetClassName(handle, builder, 255);

            className = builder.ToString();

            return className;
        }

        private static bool EnumWindow(IntPtr handle, IntPtr pointer)
        {
            GCHandle gch = GCHandle.FromIntPtr(pointer);
            List<IntPtr> list = gch.Target as List<IntPtr>;
        
            if (list == null)
            {
                throw new InvalidCastException("GCHandle Target could not be cast as List<IntPtr>");
            }
            list.Add(handle);
            //  You can modify this to check to see if you want to cancel the operation, then return a null here
            return true;
        }

        public static IEnumerable<IntPtr> GetAncestorWindows(IntPtr hwnd)
        {
            hwnd = GetParent(hwnd);

            while (hwnd != IntPtr.Zero)
            {
                yield return hwnd;

                hwnd = GetParent(hwnd);
            }
        }

        public static bool EnableWindow(this Control control, bool enable)
        {
            return EnableWindow(control.Handle, enable);
        }

        public static IDisposable Wait(this Control control)
        {
            return WaitCursor.Wait(control);
        }

        [Flags()]
        public enum SetWindowPosFlags : uint
        {
            AsynchronousWindowPosition = 0x4000,
            DeferErase = 0x2000,
            DrawFrame = 0x0020,
            FrameChanged = 0x0020,
            HideWindow = 0x0080,
            DoNotActivate = 0x0010,
            DoNotCopyBits = 0x0100,
            IgnoreMove = 0x0002,
            DoNotChangeOwnerZOrder = 0x0200,
            DoNotRedraw = 0x0008,
            DoNotReposition = 0x0200,
            DoNotSendChangingEvent = 0x0400,
            IgnoreResize = 0x0001,
            IgnoreZOrder = 0x0004,
            ShowWindow = 0x0040,
        }

        internal enum GetWindowCmd : uint
        {
            GW_HWNDFIRST = 0,
            GW_HWNDLAST = 1,
            GW_HWNDNEXT = 2,
            GW_HWNDPREV = 3,
            GW_OWNER = 4,
            GW_CHILD = 5,
            GW_ENABLEDPOPUP = 6
        }

        public static class HWND
        {
            public static IntPtr NoTopMost = new IntPtr(-2);
            public static IntPtr TopMost = new IntPtr(-1);
            public static IntPtr Top = new IntPtr(0);
            public static IntPtr Bottom = new IntPtr(1);
        }

        public static void ActivateOrSetFocus(this Control selectControl)
        {
            Form selectForm;

            if (selectControl is Form)
            {
                selectForm = (Form)selectControl;

                if (selectForm != Form.ActiveForm)
                {
                    selectForm.Activate();
                }
            }
            else
            {
                selectForm = selectControl.GetParentForm();

                if (selectForm != Form.ActiveForm)
                {
                    selectForm.Activate();
                }

                if (selectForm.ActiveControl != selectControl)
                {
                    foreach (var ancestorControl in selectControl.GetAncestors())
                    {
                        ancestorControl.Select();

                        if (ancestorControl is TabPage)
                        {
                            var tabPage = (TabPage)ancestorControl;
                            var tabControl = tabPage.GetAncestors().OfType<TabControl>().First();

                            tabControl.SelectedTab = tabPage;
                        }
                    }

                    selectControl.SetFocus();
                }
            }
        }

        public static IDisposable CaptureClipboardEvents(this TreeView treeVew)
        {
            var hEdit = treeVew.GetEditControl(m =>
            {
                var message = (ControlExtensions.WindowsMessage)m.Msg;

                switch (message)
                {
                    case ControlExtensions.WindowsMessage.KEYUP:

                        var ch = (char)m.WParam;

                        switch (ch)
                        {
                            case 'C':
                            case 'X':

                                if (Keys.ControlKey.IsPressed())
                                {
                                    var builder = new StringBuilder(255);

                                    ControlExtensions.GetWindowText(m.HWnd, builder, 255);

                                    Clipboard.SetText(builder.ToString());
                                }

                                break;
                        }

                        break;
                }

                return true;
            });

            return hEdit;
        }

        public static NativeWindow GetEditControl(this TreeView treeView, Func<Message, bool> msgProc)
        {
            var hEdit = treeView.SendMessage((WindowsMessage) 0x1100 + 15);
            var textBox = new NativeWindow(hEdit, msgProc);

            return textBox;
        }

        public static void CenterOver(this Control control, Control controlToCenterTo)
        {
            var midPoint = new Point(controlToCenterTo.Width / 2, controlToCenterTo.Height / 2);

            midPoint = controlToCenterTo.PointToScreen(midPoint);

            control.Left = midPoint.X - (control.Width / 2);
            control.Top = midPoint.Y - (control.Height / 2);
        }

        public static void CenterOver(this Control control, IntPtr controlToCenterTo)
        {
            Point midPoint;
            Rectangle rect;

            GetWindowRect((int) controlToCenterTo, out rect);
            midPoint = new Point(rect.Width / 2, rect.Height / 2);

            ClientToScreen(controlToCenterTo, ref midPoint);

            control.Left = midPoint.X - (control.Width / 2);
            control.Top = midPoint.Y - (control.Height / 2);
        }

        public static void CenterOver(this Control control, Screen screen = null)
        {
            var midPoint = new Point(screen.Bounds.Left + (screen.Bounds.Width / 2), screen.Bounds.Top + (screen.Bounds.Height / 2));

            control.Left = midPoint.X - (control.Width / 2);
            control.Top = midPoint.Y - (control.Height / 2);
        }

        public static string GetActiveWindowText()
        {
            var hwnd = GetActiveWindow();
            var builder = new StringBuilder(255);

            if (hwnd != IntPtr.Zero)
            {
                GetWindowText(hwnd, builder, 255);
                return builder.ToString();
            }

            return "[No Window Found]";
        }

        public static string GetForegroundWindowText()
        {
            var hwnd = GetForegroundWindow();
            var builder = new StringBuilder(255);

            if (hwnd != IntPtr.Zero)
            {
                GetWindowText(hwnd, builder, 255);
                return builder.ToString();
            }

            return "[No Window Found]";
        }

        public static string GetFocusWindowText()
        {
            var hwnd = GetFocus();
            var builder = new StringBuilder(255);

            if (hwnd != IntPtr.Zero)
            {
                GetWindowText(hwnd, builder, 255);
                return builder.ToString();
            }

            return "[No Window Found]";
        }

        public static bool SetAsChildOf(this Form form, Control parent, bool asControl = false)
        {
            var result = false;
            var handle = form.Handle;

            if (asControl)
            {
                SetWindowLong(form.Handle, WindowLongIndex.GWL_STYLE, WindowStyles.WS_CHILD);
            }

            parent.Invoke(() =>
            {
                result = SetParent(handle, parent.Handle) != IntPtr.Zero;
            });

            return result;
        }

        public static bool SetAsChildOf(this Control controlChild, Control parent)
        {
            var result = false;
            var handle = controlChild.Handle;

            if (parent == null)
            {
                result = SetParent(handle, IntPtr.Zero) != IntPtr.Zero;
                return result;
            }

            parent.Invoke(() =>
            {
                result = SetParent(handle, parent.Handle) != IntPtr.Zero;
            });

            return result;
        }

        public static IntPtr SubclassWindow(IntPtr handle, WndProc wndProc)
        {
            return SetWindowLong(handle, WindowLongIndex.GWL_WNDPROC, wndProc);
        }

        public static bool InDesignMode(this Control control, bool skipDevEnvCheck = false)
        {
            if (((Component) control).GetPrivatePropertyValue<bool>("DesignMode"))
            {
                return true;
            }
            else if (LicenseManager.UsageMode == LicenseUsageMode.Designtime)
            {
                return true;
            }
            else if (!skipDevEnvCheck && System.Diagnostics.Process.GetCurrentProcess().ProcessName == "devenv")
            {
                return true;
            }

            return false;
        }

        public static void SetLabelColumnWidth(this PropertyGrid grid, int width)
        {
            var view = (Control)grid.GetType().GetField("gridView", BindingFlags.Instance | BindingFlags.NonPublic).GetValue(grid);
            var methodInfo = view.GetType().GetMethod("MoveSplitterTo", BindingFlags.Instance | BindingFlags.NonPublic);

            methodInfo.Invoke(view, new object[] { width });

            view.Invalidate();
        }

        public static void UnSublassWindow(IntPtr handle, WndProc oldWndProc)
        {
            SetWindowLong(handle, WindowLongIndex.GWL_WNDPROC, oldWndProc);
        }

        public static void DoEvents(this Control control)
        {
            Application.DoEvents();
        }

        public static void DoEventsSleep(this Control control, int milliseconds)
        {
            var startTime = DateTime.Now;

            while (DateTime.Now - startTime < TimeSpan.FromMilliseconds(milliseconds))
            {
                Application.DoEvents();
            }
        }

        public static void DoEventsSleep(int milliseconds)
        {
            var startTime = DateTime.Now;

            while (DateTime.Now - startTime < TimeSpan.FromMilliseconds(milliseconds))
            {
                Application.DoEvents();
        }
        }

        public static Screen GetSecondaryMonitor(this Form form)
        {
            var leftScreen = Screen.AllScreens.SingleOrDefault(s => s != Screen.PrimaryScreen);

            if (leftScreen != null)
            {
                return leftScreen;
            }
            else
            {
                return Screen.AllScreens.First();
            }
        }

        public static void ShowInSecondaryMonitor(this Form form, FormWindowState state = FormWindowState.Maximized)
        {
            var leftScreen = Screen.AllScreens.SingleOrDefault(s => s != Screen.PrimaryScreen);

            if (leftScreen != null)
            {
                form.WindowState = FormWindowState.Normal;

                form.Location = new Point(leftScreen.Bounds.Left, 0);

                form.WindowState = state;

                if (state == FormWindowState.Normal)
                {
                    form.CenterOver(leftScreen);
                }
            }
        }

        public static void SetWindowPos(this Control control, Rectangle rect, SetWindowPosFlags flags)
        {
            var hwnd = control.Handle;

            SetWindowPos(hwnd, IntPtr.Zero, rect.Left, rect.Top, rect.Width, rect.Height, flags);
        }

        public static void ShowConsoleInSecondaryMonitor(FormWindowState state = FormWindowState.Maximized)
        {
            var leftScreen = Screen.AllScreens.SingleOrDefault(s => s != Screen.PrimaryScreen);

            if (leftScreen != null)
            {
                var hwndConsole = GetConsoleWindow();

                if (state == FormWindowState.Maximized)
                {
                    SetWindowPos(hwndConsole, IntPtr.Zero, leftScreen.Bounds.Left, 0, leftScreen.Bounds.Width, leftScreen.Bounds.Height, SetWindowPosFlags.IgnoreResize);
                }
                else
                {
                    SetWindowPos(hwndConsole, IntPtr.Zero, leftScreen.Bounds.Left, 0, 0, 0, SetWindowPosFlags.IgnoreResize);
                }
            }
        }

        public static void AddColumnSortingBehavior(this ListView listView, List<ListViewItem> itemCache = null)
        {
            var lvwColumnSorter = new ListViewColumnSorter();
            ColumnClickEventHandler columnClick = null;

            columnClick = (sender, e) =>
            {
                if (e.Column == lvwColumnSorter.SortColumn)
                {
                    if (lvwColumnSorter.Order == SortOrder.Ascending)
                    {
                        lvwColumnSorter.Order = SortOrder.Descending;
                    }
                    else
                    {
                        lvwColumnSorter.Order = SortOrder.Ascending;
                    }
                }
                else
                {
                    lvwColumnSorter.SortColumn = e.Column;
                    lvwColumnSorter.Order = SortOrder.Ascending;
                }

                if (itemCache != null)
                {
                    itemCache.Sort(lvwColumnSorter);
                    listView.Refresh();
                }
                else
                {
                    listView.Sort();
                }
            };

            listView.Disposed += (sender, e) =>
            {
                listView.ColumnClick -= columnClick;
            };

            listView.ListViewItemSorter = lvwColumnSorter;
            listView.ColumnClick += columnClick;
        }

        public static int GetVisibleRowCount(this ListView listView)
        {
            return (int) listView.SendMessage(WindowsMessage.LVM_GETCOUNTPERPAGE, 0, 0);
        }

        public static void AddSubItem(this ListViewItem item, ColumnHeader header, string text)
        {
            var index = header.DisplayIndex;

            item.SubItems.Insert(index, new ListViewItem.ListViewSubItem(item, text));
        }

        public static void UpdateSubItem(this ListViewItem item, ColumnHeader header, string text)
        {
            var index = header.DisplayIndex;
            var subItem = item.SubItems.Cast<ListViewItem.ListViewSubItem>().ElementAt(header.DisplayIndex);

            subItem.Text = text;
        }

        public static Utils.NativeWindow GetMessages(this Control control, Func<Message, bool> msgProc)
        {
            var nativeWindow = new Utils.NativeWindow(msgProc);

            nativeWindow.AssignHandle(control.Handle);

            return nativeWindow;
        }

        public static Utils.NativeWindow GetMessages(this Control control, Func<Message, bool> msgProc, Action<Message> msgPostProc)
        {
            var nativeWindow = new Utils.NativeWindow(msgProc, msgPostProc);

            nativeWindow.AssignHandle(control.Handle);

            return nativeWindow;
        }

        public static Utils.NativeWindow GetMessages(IntPtr hwnd, Func<Message, bool> msgProc)
        {
            var nativeWindow = new Utils.NativeWindow(msgProc);

            nativeWindow.AssignHandle(hwnd);

            return nativeWindow;
        }

        public static IDisposable HookMessages(IntPtr hwnd, Func<Message, bool> msgProc)
        {
            int prevWndFunc = 0;
            int delegatePtr;
            var hook = new WindowsHook();

            hook.OnMessage += (hWnd, msg, wParam, lParam) =>
            {
                if (msgProc(new Message{ HWnd = hWnd, Msg = (int) msg, WParam = (IntPtr) wParam, LParam = lParam }))
                {
                    return CallWindowProc(prevWndFunc, hWnd, msg, wParam, lParam);
                }

                return 0;
            };

            delegatePtr = hook.DelegatePtr;
            prevWndFunc = SetWindowLong(hwnd, (int)Utils.ControlExtensions.WindowLongIndex.GWL_WNDPROC, delegatePtr);

            return prevWndFunc.AsDisposable(() =>
            {
                prevWndFunc = SetWindowLong(hwnd, (int)Utils.ControlExtensions.WindowLongIndex.GWL_WNDPROC, prevWndFunc);
            });
        }

        public static Utils.NativeWindow GetMessages(this Form form, Func<Message, bool> msgProc)
        {
            var nativeWindow = new Utils.NativeWindow(msgProc);

            nativeWindow.AssignHandle(form.Handle);

            return nativeWindow;
        }

        public static void EnableClickOnActivate(this Control control)
        {
            Form parentForm;
            Action checkForParent = null;

            checkForParent = () =>
            {
                parentForm = control.GetParentForm();

                if (parentForm != null)
                {
                    var window = parentForm.GetMessages((m) =>
                    {
                        var message = (WindowsMessage)m.Msg;

                        if (message == WindowsMessage.MOUSEACTIVATE || message == WindowsMessage.CHILDACTIVATE || message == WindowsMessage.NCACTIVATE)
                        {
                            if (m.WParam == (IntPtr)1)
                            {
                                var mousePosition = control.PointToClient(Control.MousePosition);

                                if (control.Bounds.Contains(mousePosition))
                                {
                                    control.ClickWindow();
                                }
                            }
                        }

                        return true;
                    });

                    control.HandleDestroyed += (sender, e) =>
                    {
                        window.ReleaseHandle();
                    };
                }
                else
                {
                    control.BeginInvoke(checkForParent);
                }
            };

            control.BeginInvoke(checkForParent);
        }

        public static void EnableClickOnActivate(this Button button)
        {
            Form parentForm;
            Action checkForParent = null;

            checkForParent = () =>
            {
                parentForm = button.GetParentForm();

                if (parentForm != null)
                {
                    var window = parentForm.GetMessages((m) =>
                    {
                        var message = (WindowsMessage)m.Msg;

                        if (message == WindowsMessage.MOUSEACTIVATE || message == WindowsMessage.CHILDACTIVATE || message == WindowsMessage.NCACTIVATE)
                        {
                            if (m.WParam == (IntPtr)1)
                            {
                                var mousePosition = button.PointToClient(Control.MousePosition);

                                if (button.Bounds.Contains(mousePosition))
                                {
                                    button.PerformClick();
                                }
                            }
                        }

                        return true;
                    });

                    button.HandleDestroyed += (sender, e) =>
                    {
                        window.ReleaseHandle();
                    };
                }
                else
                {
                    button.BeginInvoke(checkForParent);
                }
            };

            button.BeginInvoke(checkForParent);
        }

        public static void EnableClickOnActivate(this CheckBox checkBox)
        {
            Form parentForm;
            Action checkForParent = null;

            checkForParent = () =>
            {
                parentForm = checkBox.GetParentForm();

                if (parentForm != null)
                {
                    var window = parentForm.GetMessages((m) =>
                    {
                        var message = (WindowsMessage)m.Msg;

                        if (message == WindowsMessage.MOUSEACTIVATE || message == WindowsMessage.CHILDACTIVATE || message == WindowsMessage.NCACTIVATE)
                        {
                            var mousePosition = checkBox.PointToClient(Control.MousePosition);

                            if (checkBox.Bounds.Contains(mousePosition))
                            {
                                checkBox.ClickWindow();
                            }
                        }

                        return true;
                    });

                    checkBox.HandleDestroyed += (sender, e) =>
                    {
                        window.ReleaseHandle();
                    };
                }
                else
                {
                    checkBox.BeginInvoke(checkForParent);
                }
            };

            checkBox.BeginInvoke(checkForParent);
        }

        public static void EnableClickOnActivate(this ToolStrip toolStrip)
        {
            Form parentForm;
            Action checkForParent = null;

            checkForParent = () =>
            {
                parentForm = toolStrip.GetParentForm();

                if (parentForm != null)
                {
                    var window = parentForm.GetMessages((m) =>
                    {
                        var message = (WindowsMessage)m.Msg;

                        if (message == WindowsMessage.MOUSEACTIVATE || message == WindowsMessage.CHILDACTIVATE || message == WindowsMessage.NCACTIVATE)
                        {
                            if (enableClickOnActivateNoReenter)
                            {
                                return true;
                            }

                            if (m.WParam == (IntPtr) 1 && m.HWnd == parentForm.Handle)
                            {
                                var mousePosition = toolStrip.PointToClient(Control.MousePosition);

                                foreach (var item in toolStrip.Items.Cast<ToolStripItem>())
                                {
                                    if (item.Bounds.Contains(mousePosition))
                                    {
                                        enableClickOnActivateNoReenter = true;
                                        item.PerformClick();
                                        enableClickOnActivateNoReenter = false;
                                    }
                                }
                            }
                        }

                        return true;
                    });

                    toolStrip.HandleDestroyed += (sender, e) =>
                    {
                        window.ReleaseHandle();
                    };
                }
                else
                {
                    toolStrip.BeginInvoke(checkForParent);
                }
            };

            toolStrip.BeginInvoke(checkForParent);
        }

        public static void EnableClickOnActivate(this ToolStrip toolStrip, Control parent)
        {
            var window = parent.GetMessages((m) =>
            {
                var message = (WindowsMessage)m.Msg;

                Debug.WriteLine(message);

                if (message == WindowsMessage.PARENTNOTIFY)
                {
                    var loHi = m.LParam.ToLowHiWord();
                    var point = new Point(loHi.Low, loHi.High);
                    var parentMessage = (WindowsMessage)m.WParam;
                }
                else if (message == WindowsMessage.MOUSEACTIVATE || message == WindowsMessage.CHILDACTIVATE || message == WindowsMessage.NCACTIVATE)
                {
                    if (enableClickOnActivateNoReenter)
                    {
                        return true;
                    }

                    if (m.WParam == (IntPtr)1 && m.HWnd == parent.Handle)
                    {
                        var mousePosition = toolStrip.PointToClient(Control.MousePosition);

                        foreach (var item in toolStrip.Items.Cast<ToolStripItem>())
                        {
                            if (item.Bounds.Contains(mousePosition))
                            {
                                enableClickOnActivateNoReenter = true;
                                item.PerformClick();
                                enableClickOnActivateNoReenter = false;
                            }
                        }
                    }
                }

                return true;
            });

            toolStrip.HandleDestroyed += (sender, e) =>
            {
                window.ReleaseHandle();
            };
        }
        public static Point TranslatePoint(this Control control, Point point, Control fromControl)
        {
            point = fromControl.PointToScreen(point);
            point = control.PointToClient(point);

            return point;
        }

        public static Form GetParentForm(this Control control)
        {
            Form frm = null;

            while (frm == null && control != null)
            {
                control = control.Parent;

                if (control is Form)
                {
                    frm = (Form)control;
                }
                else if (control is ContainerControl)
                {
                    var containerControl = (ContainerControl)control;
                    frm = containerControl.ParentForm;
                }
            }

            return frm;
        }

        public static void RefreshTray(this NotifyIcon icon)
        {
            RefreshTrayArea();
        }

        public static void RefreshTrayArea()
        {
            var systemTrayContainerHandle = FindWindow("Shell_TrayWnd", null);
            var systemTrayHandle = FindWindowEx(systemTrayContainerHandle, IntPtr.Zero, "TrayNotifyWnd", null);
            var sysPagerHandle = FindWindowEx(systemTrayHandle, IntPtr.Zero, "SysPager", null);
            var notificationAreaHandle = FindWindowEx(sysPagerHandle, IntPtr.Zero, "ToolbarWindow32", "Notification Area");

            if (notificationAreaHandle == IntPtr.Zero)
            {
                var notifyIconOverflowWindowHandle = FindWindow("NotifyIconOverflowWindow", null);
                var overflowNotificationAreaHandle = FindWindowEx(notifyIconOverflowWindowHandle, IntPtr.Zero, "ToolbarWindow32", "Overflow Notification Area");

                notificationAreaHandle = FindWindowEx(sysPagerHandle, IntPtr.Zero, "ToolbarWindow32", "User Promoted Notification Area");

                RefreshTrayArea(overflowNotificationAreaHandle);
            }

            RefreshTrayArea(notificationAreaHandle);
        }

        private static void RefreshTrayArea(IntPtr windowHandle)
        {
            RECT rect;

            GetClientRect(windowHandle, out rect);

            for (var x = 0; x < rect.Right; x += 5)
            {
                for (var y = 0; y < rect.Bottom; y += 5)
                {
                    SendMessage(windowHandle, WindowsMessage.MOUSEMOVE, 0, (y << 16) + x);
                }
            }
        }

        public static TreeNode FindDeepestNodeFromPath(this TreeNode treeNode, ref string path)
        {
            TreeNode existingNode = null;
            var parts = path.Split('\\').ToList();
            var findNode = treeNode;

            while (parts.Count > 0 && findNode != null)
            {
                var part = parts.First();

                findNode = findNode.Nodes.Cast<TreeNode>().SingleOrDefault(n => n.Text == part);

                if (findNode != null)
                {
                    existingNode = findNode;
                    parts.RemoveAt(0);

                    path = path.RemoveStart(part).RemoveStartIfMatches("\\");
                }
            }

            return existingNode;
        }

        public static TreeNode Add(this TreeView treeView, TreeNode child)
        {
            treeView.Nodes.Add(child);
            return child;
        }

        public static TreeNode Add(this TreeNode treeNode, TreeNode child)
        {
            treeNode.Nodes.Add(child);
            return child;
        }

        public static TreeNode Add(this TreeNode treeNode, string text, object tag = null)
        {
            var child = new TreeNode(text) { Tag = tag };

            treeNode.Nodes.Add(child);

            return child;
        }

        public static TreeNode Insert(this TreeNode treeNode, int index, TreeNode child)
        {
            treeNode.Nodes.Insert(index, child);
            return child;
        }

        public static void SelectNode(this TreeNode treeNode)
        {
            var treeView = treeNode.TreeView;

            treeView.SelectedNode = treeNode;
        }

        public static void Remove(this TreeView treeView, TreeNode child)
        {
            treeView.Nodes.Remove(child);
        }

        public static void Remove(this TreeNode treeNode, TreeNode child)
        {
            treeNode.Nodes.Remove(child);
        }

        public static void Clear(this TreeView treeView)
        {
            treeView.Nodes.Clear();
        }

        public static void Clear(this TreeNode treeNode)
        {
            treeNode.Nodes.Clear();
        }

        public static void Replace(this TreeView treeView, TreeNode oldChild, TreeNode newChild)
        {
            var index = oldChild.Index;

            treeView.Nodes.RemoveAt(index);
            treeView.Nodes.Insert(index, newChild);
        }

        public static void Replace(this TreeNode treeNode, TreeNode oldChild, TreeNode newChild)
        {
            var index = oldChild.Index;

            treeNode.Nodes.RemoveAt(index);
            treeNode.Nodes.Insert(index, newChild);
        }

        public static IEnumerable<TreeNode> GetSiblings(this TreeNode treeNode, bool includeSelf = false)
        {
            if (treeNode.Parent == null)
            {
                var list = new List<TreeNode>();

                if (includeSelf)
                {
                    list.Add(treeNode);
                }

                return list;
            }
            else
            {
                if (includeSelf)
                {
                    return treeNode.Parent.Nodes.Cast<TreeNode>();
                }
                else
                {
                    return treeNode.Parent.Nodes.Cast<TreeNode>().Where(n => n != treeNode);
                }
            }
        }

        public static IEnumerable<TreeNode> GetSiblings(this TreeNode treeNode, Func<TreeNode, bool> filter, bool includeSelf = false)
        {
            return treeNode.GetSiblings(includeSelf).Where(t => filter(t));
        }

        public static IEnumerable<TreeNode> GetAllNodes(this TreeView treeView)
        {
            var list = new List<TreeNode>();

            foreach (TreeNode node in treeView.Nodes)
            {
                list.Add(node);

                node.AddAllNodes(list);
            }

            return list;
        }

        public static IEnumerable<Control> GetAllControls(this Control control)
        {
            var list = new List<Control>();

            foreach (Control childControl in control.Controls)
            {
                list.Add(childControl);

                childControl.AddAllControls(list);
            }

            return list;
        }

        private static void AddAllControls(this Control control, List<Control> list)
        {
            foreach (Control childControl in control.Controls)
            {
                list.Add(childControl);

                childControl.AddAllControls(list);
            }
        }

        public static IEnumerable<Control> GetAllControls(this Control control, Func<Control, bool> filter)
        {
            var list = new List<Control>();

            foreach (Control childControl in control.Controls)
            {
                if (filter(childControl))
                {
                    list.Add(childControl);
                }

                childControl.AddAllControls(list, filter);
            }

            return list;
        }

        private static void AddAllControls(this Control control, List<Control> list, Func<Control, bool> filter)
        {
            foreach (Control childControl in control.Controls)
            {
                if (filter(childControl))
                {
                    list.Add(childControl);
                }

                childControl.AddAllControls(list, filter);
            }
        }

        public static IEnumerable<Control> GetAncestors(this Control control)
        {
            var parent = control.Parent;

            while (parent != null)
            {
                yield return parent;
                parent = parent.Parent;
            }
        }

        public static IEnumerable<Control> GetAncestors(this Control control, Func<Control, bool> filter)
        {
            var parent = control.Parent;

            while (parent != null)
            {
                if (filter(parent))
                {
                    yield return parent;
                }

                parent = parent.Parent;
            }
        }

        public static IEnumerable<TreeNode> GetAllNodes(this TreeNode treeNode)
        {
            var list = new List<TreeNode>();

            foreach (TreeNode node in treeNode.Nodes)
            {
                list.Add(node);

                node.AddAllNodes(list);
            }

            return list;
        }

        private static void AddAllNodes(this TreeNode treeNode, List<TreeNode> list)
        {
            foreach (TreeNode node in treeNode.Nodes)
            {
                list.Add(node);

                node.AddAllNodes(list);
            }
        }

        public static IEnumerable<TreeNode> GetAllNodes(this TreeView treeView, Func<TreeNode, bool> filter)
        {
            var list = new List<TreeNode>();

            foreach (TreeNode node in treeView.Nodes)
            {
                if (filter(node))
                {
                    list.Add(node);
                }

                node.AddAllNodes(list, filter);
            }

            return list;
        }

        public static IEnumerable<TreeNode> GetAllNodes(this TreeNode treeNode, Func<TreeNode, bool> filter)
        {
            var list = new List<TreeNode>();

            foreach (TreeNode node in treeNode.Nodes)
            {
                if (filter(node))
                {
                    list.Add(node);
                }

                node.AddAllNodes(list, filter);
            }

            return list;
        }

        private static void AddAllNodes(this TreeNode treeNode, List<TreeNode> list, Func<TreeNode, bool> filter)
        {
            foreach (TreeNode node in treeNode.Nodes)
            {
                if (filter(node))
                {
                    list.Add(node);
                }

                node.AddAllNodes(list, filter);
            }
        }

        public static IEnumerable<TreeNode> GetAllNodesFilteredDownwards(this TreeNode treeNode, Func<TreeNode, bool> filter)
        {
            var list = new List<TreeNode>();

            foreach (TreeNode node in treeNode.Nodes)
            {
                if (filter(node))
                {
                    list.Add(node);
                    node.AddAllNodesFilteredDownwards(list, filter);
                }
            }

            return list;
        }

        private static void AddAllNodesFilteredDownwards(this TreeNode treeNode, List<TreeNode> list, Func<TreeNode, bool> filter)
        {
            foreach (TreeNode node in treeNode.Nodes)
            {
                if (filter(node))
                {
                    list.Add(node);
                    node.AddAllNodesFilteredDownwards(list, filter);
                }
            }
        }

        public static IEnumerable<TreeNode> GetAllNodesWhile(this TreeNode treeNode, Func<TreeNode, bool> filter)
        {
            var list = new List<TreeNode>();

            foreach (TreeNode node in treeNode.Nodes)
            {
                if (filter(node))
                {
                    list.Add(node);
                    node.AddAllNodes(list, filter);
                }
            }

            return list;
        }

        private static void AddAllNodesWhile(this TreeNode treeNode, List<TreeNode> list, Func<TreeNode, bool> filter)
        {
            foreach (TreeNode node in treeNode.Nodes)
            {
                if (filter(node))
                {
                    list.Add(node);
                    node.AddAllNodes(list, filter);
                }
            }
        }

        public static string GetImageKey(this ImageList imageList, int index)
        {
            var imageTypes = new List<string>() { "bmp", "gif", "jpg", "jpeg", "png", "ico" };
            var key = imageList.Images.Keys[index];
            
            return key;
        }

        public static int GetImageIndex(this ImageList imageList, string name)
        {
            var imageTypes = new List<string>() { "bmp", "gif", "jpg", "jpeg", "png", "ico" };
            var index = imageList.Images.Keys.IndexOf(name);

            if (index == -1)
            {
                foreach (var imageType in imageTypes)
                {
                    index = imageList.Images.Keys.IndexOf(name + "." + imageType);

                    if (index != -1)
                    {
                        return index;
                    }
                }
            }

            return index;
        }

        public static LowHiWord ToLowHiWord(this IntPtr ptr)
        {
            return new LowHiWord { Number = (uint) ptr };
        }

        public static LowHiWordSigned ToLowHiWordSigned(this IntPtr ptr)
        {
            return new LowHiWordSigned { Number = (uint)ptr };
        }

        public static IntPtr MakeLParam(short low, short high)
        {
            return (IntPtr)new LowHiWordSigned { Low = low, High = high }.Number;
        }

        public static IntPtr MakeLParam(ushort low, ushort high)
        {
            return (IntPtr)new LowHiWord { Low = low, High = high }.Number;
        }

        public static IntPtr MakeLParam(int low, int high)
        {
            return (IntPtr)new LowHiWordSigned { Low = (short)low, High = (short)high }.Number;
        }

        public static IntPtr MakeLParam(uint low, uint high)
        {
            return (IntPtr)new LowHiWord { Low = (ushort)low, High = (ushort)high }.Number;
        }

        public static void SetState(this ProgressBar pBar, ProgressBarState state)
        {
            SendMessage(pBar.Handle, (WindowsMessage)1040, (IntPtr)state, IntPtr.Zero);
        }

        public static void ClickWindowRestorePosition(this Control control, int offsetLeft = 1, int offsetTop = 1)
        {
            var position = Cursor.Position;

            ClickWindow(control.Handle.ToInt32(), offsetLeft, offsetTop);

            Cursor.Position = position;
        }

        public static void ClickWindow(this Control control, int offsetLeft = 1, int offsetTop = 1)
        {
            ClickWindow(control.Handle.ToInt32(), offsetLeft, offsetTop);
        }

        public static void ClickWindow(int hwnd, int offsetLeft = 1, int offsetTop = 1)
        {
            ClickWindow((IntPtr)hwnd, offsetLeft, offsetTop);
        }

        public static Point PixelToAbsolute(this Point point)
        {
            var x = (65535 * point.X) / Screen.PrimaryScreen.Bounds.Width;
            var y = (65535 * point.Y) / Screen.PrimaryScreen.Bounds.Height;

            return new Point(x, y);
        }

        public static void ClickWindow(IntPtr hwnd, int offsetLeft = 1, int offsetTop = 1)
        {
            RECT rect;
            Point point;

            GetWindowRect(hwnd, out rect);

            point = new Point(rect.X + offsetLeft, rect.Y + offsetTop);
            point = point.PixelToAbsolute();

            var input = new INPUT
            {
                type = InputType.Mouse,
                U = new InputUnion
                {
                    mi = new MOUSEINPUT
                    {
                        dx = point.X,
                        dy = point.Y,
                        dwFlags = MOUSEEVENTF.ABSOLUTE | MOUSEEVENTF.LEFTDOWN | MOUSEEVENTF.MOVE
                    }
                }
            };

            var returnValue = SendInput(1, new INPUT[] { input }, Marshal.SizeOf(typeof(INPUT)));

            input.U.mi.dwFlags = MOUSEEVENTF.LEFTUP | MOUSEEVENTF.ABSOLUTE;

            returnValue = SendInput(1, new INPUT[] { input }, Marshal.SizeOf(typeof(INPUT)));

            //Debug.WriteLine("Position: {{{0}, {1}}}", input.U.mi.dx, input.U.mi.dy);
        }

        public static void ClickWindowRelative(this Control control, int offsetLeft = 1, int offsetTop = 1)
        {
            ClickWindowRelative(control.Handle.ToInt32(), offsetLeft, offsetTop);
        }

        public static void ClickWindowRelative(int hwnd, int offsetLeft = 1, int offsetTop = 1)
        {
            ClickWindowRelative((IntPtr)hwnd, offsetLeft, offsetTop);
        }

        public static void ClickWindowRelative(IntPtr hwnd, int offsetLeft = 1, int offsetTop = 1)
        {
            var input = new INPUT
            {
                type = InputType.Mouse,
                U = new InputUnion
                {
                    mi = new MOUSEINPUT
                    {
                        dx = offsetLeft,
                        dy = offsetTop,
                        dwFlags = MOUSEEVENTF.LEFTDOWN | MOUSEEVENTF.MOVE
                    }
                }
            };

            var returnValue = SendInput(1, new INPUT[] { input }, Marshal.SizeOf(typeof(INPUT)));

            input.U.mi.dwFlags = MOUSEEVENTF.LEFTUP | MOUSEEVENTF.ABSOLUTE | MOUSEEVENTF.MOVE;

            returnValue = SendInput(1, new INPUT[] { input }, Marshal.SizeOf(typeof(INPUT)));
        }

        public static void TypeKey(this Control control, Keys key)
        {
            var shift = key.HasFlag(Keys.ShiftKey);

            TypeKey(control, (VirtualKeyShort)key, shift);
        }

        private static void TypeKey(this Control control, VirtualKeyShort key, bool shift = false)
        {
            TypeKey(control.Handle.ToInt32(), key, shift);
        }

        public static void TypeKey(int hwnd, Keys key)
        {
            TypeKey(hwnd, (VirtualKeyShort)key);
        }

        public static void TypeKey(IntPtr hwnd, Keys key)
        {
            var shift = key.HasFlag(Keys.ShiftKey);

            TypeKey((int)hwnd, (VirtualKeyShort)key, shift);
        }

        private static void TypeKey(int hwnd, VirtualKeyShort key, bool shift = false)
        {
            if (shift)
            {
                TypeKeyDown(hwnd, VirtualKeyShort.SHIFT);
            }

            var input = new INPUT
            {
                type = InputType.Keyboard,
                U = new InputUnion
                {
                    ki = new KEYBDINPUT
                    {
                        wVk = key,
                        dwFlags = 0
                    }
                }
            };

            var returnValue = SendInput(1, new INPUT[] { input }, Marshal.SizeOf(typeof(INPUT)));

            input.U.ki.dwFlags = KEYEVENTF.KEYUP;

            returnValue = SendInput(1, new INPUT[] { input }, Marshal.SizeOf(typeof(INPUT)));

            if (shift)
            {
                TypeKeyUp(hwnd, VirtualKeyShort.SHIFT);
            }
        }

        private static void TypeKeyDown(int hwnd, VirtualKeyShort key)
        {
            var rect = new Rectangle();

            GetWindowRect(hwnd, out rect);

            var input = new INPUT
            {
                type = InputType.Keyboard,
                U = new InputUnion
                {
                    ki = new KEYBDINPUT
                    {
                        wVk = key,
                        dwFlags = 0
                    }
                }
            };

            var returnValue = SendInput(1, new INPUT[] { input }, Marshal.SizeOf(typeof(INPUT)));
        }

        private static void TypeKeyUp(int hwnd, VirtualKeyShort key)
        {
            var rect = new Rectangle();

            GetWindowRect(hwnd, out rect);

            var input = new INPUT
            {
                type = InputType.Keyboard,
                U = new InputUnion
                {
                    ki = new KEYBDINPUT
                    {
                        wVk = key,
                        dwFlags = KEYEVENTF.KEYUP
                    }
                }
            };

            var returnValue = SendInput(1, new INPUT[] { input }, Marshal.SizeOf(typeof(INPUT)));
        }

        public static IEnumerable<TreeNode> GetAncestors(this TreeNode childNode)
        {
            var node = childNode.Parent;

            while (node != null)
            {
                yield return node;
                node = node.Parent;
            }
        }

        public static IEnumerable<TreeNode> GetAncestorsAndSelf(this TreeNode childNode)
        {
            var node = childNode.Parent;

            yield return childNode;

            while (node != null)
            {
                yield return node;
                node = node.Parent;
            }
        }

        public static IEnumerable<TreeNode> GetAncestors(this TreeNode childNode, Func<TreeNode, bool> filter)
        {
            var node = childNode.Parent;

            while (node != null)
            {
                if (filter(node))
                {
                    yield return node;
                }

                node = node.Parent;
            }
        }

        public static IEnumerable<TreeNode> GetAncestorsAndSelf(this TreeNode childNode, Func<TreeNode, bool> filter)
        {
            var node = childNode.Parent;

            if (filter(childNode))
            {
                yield return childNode;
            }

            while (node != null)
            {
                if (filter(node))
                {
                    yield return node;
                }

                node = node.Parent;
            }
        }

        public static int GetLevel(this TreeNode childNode)
        {
            var level = 1;
            var node = childNode.Parent;

            while (node != null)
            {
                level++;
                node = node.Parent;
            }

            return level;
        }

        public static TreeNode GetAncestor(this TreeNode childNode, Func<TreeNode, bool> filter)
        {
            var node = childNode.Parent;

            while (node != null)
            {
                if (filter(node))
                {
                    return node;
                }

                node = node.Parent;
            }

            return null;
        }

        public static string GetCommonCaption(this Form form, string formTitle = null)
        {
            var caption = string.Empty;
            var callingAssembly = Assembly.GetCallingAssembly();
            var attributes = callingAssembly.GetAttributes();
            var module = Process.GetCurrentProcess().MainModule.ModuleName;

            if (formTitle == null)
            {
                caption = string.Format("{0} v{1} {2}", attributes.Product, attributes.Version, attributes.Copyright);
            }
            else
            {
                caption = string.Format("{0} v{1} {2}", formTitle, attributes.Version, attributes.Copyright);
            }

            return caption;
        }

        public static string GetCommonCaption(this object obj)
        {
            return obj.GetType().GetCommonCaption();
        }

        public static string GetCommonCaption(this Type type)
        {
            var callingAssembly = type.Assembly;

            return type.GetCommonCaption(callingAssembly);
        }

        private static string GetCommonCaption(this Type type, Assembly callingAssembly)
        {
            var caption = string.Empty;
            var attributes = callingAssembly.GetAttributes();
            var module = Process.GetCurrentProcess().MainModule.ModuleName;

            caption = string.Format("{0}.{1} v{2} - {3}", Path.GetFileNameWithoutExtension(callingAssembly.ManifestModule.Name), type.Name, attributes.Version, Path.GetFileNameWithoutExtension(module));

            return caption;
        }

        public static void Invoke(this Control control, Action action)
        {
            try
            {
                if (!control.IsDisposed)
                {
                    control.Invoke(action);
                }
            }
            catch
            {
            }
        }

        public static void DelayInvoke(this Control control, int milliseconds, Action action)
        {
            try
            {
                var timer = new OneTimeTimer(milliseconds);

                timer.Start(() =>
                {
                    if (!control.IsDisposed)
                    {
                    control.Invoke(action);
                    }
                });
            }
            catch
            {
            }
        }

        public static void BeginInvoke(this Control control, Action action)
        {
            control.BeginInvoke(action);
        }

        public static void BeginInvoke(this Control control, Action action, int milliseconds)
        {
            var oneTimeTimer = new OneTimeTimer(milliseconds);

            oneTimeTimer.Start(() => control.BeginInvoke(action));
        }

        public static IntPtr SendMessage<TLPARAM>(this Control control, WindowsMessage msg, int w, TLPARAM l) where TLPARAM : struct
        {
            var size = l.SizeOf();
            var ptr = Marshal.AllocCoTaskMem(size);
            IntPtr result;

            Marshal.StructureToPtr(l, ptr, false);

            result = SendMessage(control.Handle, msg, (IntPtr) w, ptr);

            Marshal.FreeCoTaskMem(ptr);

            return result;
        }

        public static IntPtr SendMessage(this Control control, WindowsMessage msg, int w = 0, int l = 0)
        {
            return SendMessage(control.Handle, msg, w, l);
        }

        public static IntPtr SendMessage(this Control control, WindowsMessage msg, IntPtr w, IntPtr l)
        {
            return SendMessage(control.Handle, msg, w, l);
        }

        public static void PostMessage(this Control control, WindowsMessage msg, int w = 0, int l = 0)
        {
            PostMessage(control.Handle, msg, w, l);
        }

        public static void PostMessage(this Control control, WindowsMessage msg, IntPtr w, IntPtr l)
        {
            PostMessage(control.Handle, msg, w, l);
        }

        public static bool PeekMessage(this Control control, out NativeMessage nativeMessage, WindowsMessage msgFilterMin = WindowsMessage.NULL, WindowsMessage msgFilterMax = WindowsMessage.NULL, bool removeMsg = false)
        {
            nativeMessage = new NativeMessage();

            return PeekMessage(out nativeMessage, control.Handle, (uint)msgFilterMin, (uint)msgFilterMax, (uint) (removeMsg ? 1 : 0));
        }

        public static Control SetFocus(this Control control)
        {
            var hPreviousFocus = SetFocus(control.Handle);

            return Control.FromHandle(hPreviousFocus);
        }

        public static Control GetFocus(this Control control)
        {
            var hPreviousFocus = GetFocus();    

            return Control.FromHandle(hPreviousFocus);
        }

        public static void AllowUIProcessing(this Control control, float minutes = 1, bool debugBreak = true)
        {
            var start = DateTime.Now;

            while (DateTime.Now - start < TimeSpan.FromMinutes(minutes))
            {
                NativeMessage nativeMessage;

                if (System.Windows.Forms.Keys.Pause.IsPressed())
                {
                    break;
                }

                if (control.PeekMessage(out nativeMessage))
                {
                    start = DateTime.Now;
                }

                Application.DoEvents();
            }

            if (debugBreak)
            {
                Debugger.Break();
            }
        }

        public static void AllowUIProcessing(IntPtr hwnd, float minutes = 1, bool debugBreak = true)
        {
            var start = DateTime.Now;

            while (DateTime.Now - start < TimeSpan.FromMinutes(minutes))
            {
                NativeMessage nativeMessage;

                if (System.Windows.Forms.Keys.Pause.IsPressed())
                {
                    break;
                }

                if (PeekMessage(out nativeMessage, hwnd, 0, 0, 0))
                {
                    start = DateTime.Now;
                }

                Application.DoEvents();
            }

            if (debugBreak)
            {
                Debugger.Break();
            }
        }

        public enum SysCommands : int
        {
            SC_SIZE = 0xF000,
            SC_MOVE = 0xF010,
            SC_MINIMIZE = 0xF020,
            SC_MAXIMIZE = 0xF030,
            SC_NEXTWINDOW = 0xF040,
            SC_PREVWINDOW = 0xF050,
            SC_CLOSE = 0xF060,
            SC_VSCROLL = 0xF070,
            SC_HSCROLL = 0xF080,
            SC_MOUSEMENU = 0xF090,
            SC_KEYMENU = 0xF100,
            SC_ARRANGE = 0xF110,
            SC_RESTORE = 0xF120,
            SC_TASKLIST = 0xF130,
            SC_SCREENSAVE = 0xF140,
            SC_HOTKEY = 0xF150,
            SC_DEFAULT = 0xF160,
            SC_MONITORPOWER = 0xF170,
            SC_CONTEXTHELP = 0xF180,
            SC_SEPARATOR = 0xF00F,
            SCF_ISSECURE = 0x00000001,
            SC_ICON = SC_MINIMIZE,
            SC_ZOOM = SC_MAXIMIZE,
        }

        public enum WindowsMessage : uint
        {
            NULL = 0x0000,
            CREATE = 0x0001,
            DESTROY = 0x0002,
            MOVE = 0x0003,
            SIZE = 0x0005,
            ACTIVATE = 0x0006,
            SETFOCUS = 0x0007,
            KILLFOCUS = 0x0008,
            ENABLE = 0x000A,
            SETREDRAW = 0x000B,
            SETTEXT = 0x000C,
            GETTEXT = 0x000D,
            GETTEXTLENGTH = 0x000E,
            PAINT = 0x000F,
            CLOSE = 0x0010,
            QUERYENDSESSION = 0x0011,
            QUERYOPEN = 0x0013,
            ENDSESSION = 0x0016,
            QUIT = 0x0012,
            ERASEBKGND = 0x0014,
            SYSCOLORCHANGE = 0x0015,
            SHOWWINDOW = 0x0018,
            WININICHANGE = 0x001A,
            SETTINGCHANGE = WININICHANGE,
            DEVMODECHANGE = 0x001B,
            ACTIVATEAPP = 0x001C,
            FONTCHANGE = 0x001D,
            TIMECHANGE = 0x001E,
            CANCELMODE = 0x001F,
            SETCURSOR = 0x0020,
            MOUSEACTIVATE = 0x0021,
            CHILDACTIVATE = 0x0022,
            QUEUESYNC = 0x0023,
            GETMINMAXINFO = 0x0024,
            PAINTICON = 0x0026,
            ICONERASEBKGND = 0x0027,
            NEXTDLGCTL = 0x0028,
            SPOOLERSTATUS = 0x002A,
            DRAWITEM = 0x002B,
            MEASUREITEM = 0x002C,
            DELETEITEM = 0x002D,
            VKEYTOITEM = 0x002E,
            CHARTOITEM = 0x002F,
            SETFONT = 0x0030,
            GETFONT = 0x0031,
            SETHOTKEY = 0x0032,
            GETHOTKEY = 0x0033,
            QUERYDRAGICON = 0x0037,
            COMPAREITEM = 0x0039,
            GETOBJECT = 0x003D,
            COMPACTING = 0x0041,
            [Obsolete]
            COMMNOTIFY = 0x0044,
            WINDOWPOSCHANGING = 0x0046,
            WINDOWPOSCHANGED = 0x0047,
            [Obsolete]
            POWER = 0x0048,
            COPYDATA = 0x004A,
            CANCELJOURNAL = 0x004B,
            NOTIFY = 0x004E,
            INPUTLANGCHANGEREQUEST = 0x0050,
            INPUTLANGCHANGE = 0x0051,
            TCARD = 0x0052,
            HELP = 0x0053,
            USERCHANGED = 0x0054,
            NOTIFYFORMAT = 0x0055,
            CONTEXTMENU = 0x007B,
            STYLECHANGING = 0x007C,
            STYLECHANGED = 0x007D,
            DISPLAYCHANGE = 0x007E,
            GETICON = 0x007F,
            SETICON = 0x0080,
            NCCREATE = 0x0081,
            NCDESTROY = 0x0082,
            NCCALCSIZE = 0x0083,
            NCHITTEST = 0x0084,
            NCPAINT = 0x0085,
            NCACTIVATE = 0x0086,
            GETDLGCODE = 0x0087,
            SYNCPAINT = 0x0088,
            NCMOUSEMOVE = 0x00A0,
            NCLBUTTONDOWN = 0x00A1,
            NCLBUTTONUP = 0x00A2,
            NCLBUTTONDBLCLK = 0x00A3,
            NCRBUTTONDOWN = 0x00A4,
            NCRBUTTONUP = 0x00A5,
            NCRBUTTONDBLCLK = 0x00A6,
            NCMBUTTONDOWN = 0x00A7,
            NCMBUTTONUP = 0x00A8,
            NCMBUTTONDBLCLK = 0x00A9,
            NCXBUTTONDOWN = 0x00AB,
            NCXBUTTONUP = 0x00AC,
            NCXBUTTONDBLCLK = 0x00AD,
            INPUT_DEVICE_CHANGE = 0x00FE,
            INPUT = 0x00FF,
            KEYFIRST = 0x0100,
            KEYDOWN = 0x0100,
            KEYUP = 0x0101,
            CHAR = 0x0102,
            DEADCHAR = 0x0103,
            SYSKEYDOWN = 0x0104,
            SYSKEYUP = 0x0105,
            SYSCHAR = 0x0106,
            SYSDEADCHAR = 0x0107,
            UNICHAR = 0x0109,
            KEYLAST = 0x0109,
            IME_STARTCOMPOSITION = 0x010D,
            IME_ENDCOMPOSITION = 0x010E,
            IME_COMPOSITION = 0x010F,
            IME_KEYLAST = 0x010F,
            INITDIALOG = 0x0110,
            COMMAND = 0x0111,
            SYSCOMMAND = 0x0112,
            TIMER = 0x0113,
            HSCROLL = 0x0114,
            VSCROLL = 0x0115,
            INITMENU = 0x0116,
            INITMENUPOPUP = 0x0117,
            MENUSELECT = 0x011F,
            MENUCHAR = 0x0120,
            ENTERIDLE = 0x0121,
            MENURBUTTONUP = 0x0122,
            MENUDRAG = 0x0123,
            MENUGETOBJECT = 0x0124,
            UNINITMENUPOPUP = 0x0125,
            MENUCOMMAND = 0x0126,
            CHANGEUISTATE = 0x0127,
            UPDATEUISTATE = 0x0128,
            QUERYUISTATE = 0x0129,
            CTLCOLORMSGBOX = 0x0132,
            CTLCOLOREDIT = 0x0133,
            CTLCOLORLISTBOX = 0x0134,
            CTLCOLORBTN = 0x0135,
            CTLCOLORDLG = 0x0136,
            CTLCOLORSCROLLBAR = 0x0137,
            CTLCOLORSTATIC = 0x0138,
            MOUSEFIRST = 0x0200,
            MOUSEMOVE = 0x0200,
            LBUTTONDOWN = 0x0201,
            LBUTTONUP = 0x0202,
            LBUTTONDBLCLK = 0x0203,
            RBUTTONDOWN = 0x0204,
            RBUTTONUP = 0x0205,
            RBUTTONDBLCLK = 0x0206,
            MBUTTONDOWN = 0x0207,
            MBUTTONUP = 0x0208,
            MBUTTONDBLCLK = 0x0209,
            MOUSEWHEEL = 0x020A,
            XBUTTONDOWN = 0x020B,
            XBUTTONUP = 0x020C,
            XBUTTONDBLCLK = 0x020D,
            MOUSEHWHEEL = 0x020E,
            MOUSELAST = 0x020E,
            PARENTNOTIFY = 0x0210,
            ENTERMENULOOP = 0x0211,
            EXITMENULOOP = 0x0212,
            NEXTMENU = 0x0213,
            SIZING = 0x0214,
            CAPTURECHANGED = 0x0215,
            MOVING = 0x0216,
            POWERBROADCAST = 0x0218,
            DEVICECHANGE = 0x0219,
            MDICREATE = 0x0220,
            MDIDESTROY = 0x0221,
            MDIACTIVATE = 0x0222,
            MDIRESTORE = 0x0223,
            MDINEXT = 0x0224,
            MDIMAXIMIZE = 0x0225,
            MDITILE = 0x0226,
            MDICASCADE = 0x0227,
            MDIICONARRANGE = 0x0228,
            MDIGETACTIVE = 0x0229,
            MDISETMENU = 0x0230,
            ENTERSIZEMOVE = 0x0231,
            EXITSIZEMOVE = 0x0232,
            DROPFILES = 0x0233,
            MDIREFRESHMENU = 0x0234,
            IME_SETCONTEXT = 0x0281,
            IME_NOTIFY = 0x0282,
            IME_CONTROL = 0x0283,
            IME_COMPOSITIONFULL = 0x0284,
            IME_SELECT = 0x0285,
            IME_CHAR = 0x0286,
            IME_REQUEST = 0x0288,
            IME_KEYDOWN = 0x0290,
            IME_KEYUP = 0x0291,
            MOUSEHOVER = 0x02A1,
            MOUSELEAVE = 0x02A3,
            NCMOUSEHOVER = 0x02A0,
            NCMOUSELEAVE = 0x02A2,
            WTSSESSION_CHANGE = 0x02B1,
            TABLET_FIRST = 0x02c0,
            TABLET_LAST = 0x02df,
            CUT = 0x0300,
            COPY = 0x0301,
            PASTE = 0x0302,
            CLEAR = 0x0303,
            UNDO = 0x0304,
            RENDERFORMAT = 0x0305,
            RENDERALLFORMATS = 0x0306,
            DESTROYCLIPBOARD = 0x0307,
            DRAWCLIPBOARD = 0x0308,
            PAINTCLIPBOARD = 0x0309,
            VSCROLLCLIPBOARD = 0x030A,
            SIZECLIPBOARD = 0x030B,
            ASKCBFORMATNAME = 0x030C,
            CHANGECBCHAIN = 0x030D,
            HSCROLLCLIPBOARD = 0x030E,
            QUERYNEWPALETTE = 0x030F,
            PALETTEISCHANGING = 0x0310,
            PALETTECHANGED = 0x0311,
            HOTKEY = 0x0312,
            PRINT = 0x0317,
            PRINTCLIENT = 0x0318,
            APPCOMMAND = 0x0319,
            THEMECHANGED = 0x031A,
            CLIPBOARDUPDATE = 0x031D,
            DWMCOMPOSITIONCHANGED = 0x031E,
            DWMNCRENDERINGCHANGED = 0x031F,
            DWMCOLORIZATIONCOLORCHANGED = 0x0320,
            DWMWINDOWMAXIMIZEDCHANGE = 0x0321,
            GETTITLEBARINFOEX = 0x033F,
            HANDHELDFIRST = 0x0358,
            HANDHELDLAST = 0x035F,
            AFXFIRST = 0x0360,
            AFXLAST = 0x037F,
            PENWINFIRST = 0x0380,
            PENWINLAST = 0x038F,
            SETSCROLLINFO = 0xE9,
            APP = 0x8000,
            USER = 0x0400,
            CPL_LAUNCH = USER + 0x1000,
            CPL_LAUNCHED = USER + 0x1001,
            SYSTIMER = 0x118,
            HSHELL_ACCESSIBILITYSTATE = 11,
            HSHELL_ACTIVATESHELLWINDOW = 3,
            HSHELL_APPCOMMAND = 12,
            HSHELL_GETMINRECT = 5,
            HSHELL_LANGUAGE = 8,
            HSHELL_REDRAW = 6,
            HSHELL_TASKMAN = 7,
            HSHELL_WINDOWCREATED = 1,
            HSHELL_WINDOWDESTROYED = 2,
            HSHELL_WINDOWACTIVATED = 4,
            HSHELL_WINDOWREPLACED = 13,
            CB_SHOWDROPDOWN = 0x014F,
            AC_SEARCHCOMPLETE = 0xC413,
            LVM_APPROXIMATEVIEWRECT = 0x1040,
            LVM_ARRANGE = 0x1016,
            LVM_CANCELEDITLABEL = 0x10b3,
            LVM_CREATEDRAGIMAGE = 0x1021,
            LVM_DELETEALLITEMS = 0x1009,
            LVM_DELETECOLUMN = 0x101c,
            LVM_DELETEITEM = 0x1008,
            LVM_EDITLABELA = 0x1017,
            LVM_EDITLABELW = 0x1076,
            LVM_ENABLEGROUPVIEW = 0x109d,
            LVM_ENSUREVISIBLE = 0x1013,
            LVM_FINDITEMA = 0x100d,
            LVM_FINDITEMW = 0x1053,
            LVM_FIRST = 0x1000,
            LVM_GETBKCOLOR = 0x1000,
            LVM_GETBKIMAGEA = 0x1045,
            LVM_GETBKIMAGEW = 0x108b,
            LVM_GETCALLBACKMASK = 0x100a,
            LVM_GETCOLUMNA = 0x1019,
            LVM_GETCOLUMNORDERARRAY = 0x103b,
            LVM_GETCOLUMNW = 0x105f,
            LVM_GETCOLUMNWIDTH = 0x101d,
            LVM_GETCOUNTPERPAGE = 0x1028,
            LVM_GETEDITCONTROL = 0x1018,
            LVM_GETEXTENDEDLISTVIEWSTYLE = 0x1037,
            LVM_GETGROUPINFO = 0x1095,
            LVM_GETGROUPMETRICS = 0x109c,
            LVM_GETHOTCURSOR = 0x103f,
            LVM_GETHOTITEM = 0x103d,
            LVM_GETHOVERTIME = 0x1048,
            LVM_GETIMAGELIST = 0x1002,
            LVM_GETINSERTMARK = 0x10a7,
            LVM_GETINSERTMARKCOLOR = 0x10ab,
            LVM_GETINSERTMARKRECT = 0x10a9,
            LVM_GETISEARCHSTRINGA = 0x1034,
            LVM_GETISEARCHSTRINGW = 0x1075,
            LVM_GETITEMA = 0x1005,
            LVM_GETITEMCOUNT = 0x1004,
            LVM_GETITEMPOSITION = 0x1010,
            LVM_GETITEMRECT = 0x100e,
            LVM_GETITEMSPACING = 0x1033,
            LVM_GETITEMSTATE = 0x102c,
            LVM_GETITEMTEXTA = 0x102d,
            LVM_GETITEMTEXTW = 0x1073,
            LVM_GETITEMW = 0x104b,
            LVM_GETNUMBEROFWORKAREAS = 0x1049,
            LVM_GETORIGIN = 0x1029,
            LVM_GETOUTLINECOLOR = 0x10b0,
            LVM_GETSELECTEDCOLUMN = 0x10ae,
            LVM_GETSELECTEDCOUNT = 0x1032,
            LVM_GETSELECTIONMARK = 0x1042,
            LVM_GETSTRINGWIDTHA = 0x1011,
            LVM_GETSTRINGWIDTHW = 0x1057,
            LVM_GETSUBITEMRECT = 0x1038,
            LVM_GETTEXTBKCOLOR = 0x1025,
            LVM_GETTEXTCOLOR = 0x1023,
            LVM_GETTILEINFO = 0x10a5,
            LVM_GETTILEVIEWINFO = 0x10a3,
            LVM_GETTOOLTIPS = 0x104e,
            LVM_GETTOPINDEX = 0x1027,
            LVM_GETUNICODEFORMAT = 0x2006,
            LVM_GETVIEW = 0x108f,
            LVM_GETVIEWRECT = 0x1022,
            LVM_GETWORKAREAS = 0x1046,
            LVM_HASGROUP = 0x10a1,
            LVM_HITTEST = 0x1012,
            LVM_INSERTCOLUMNA = 0x101b,
            LVM_INSERTCOLUMNW = 0x1061,
            LVM_INSERTGROUP = 0x1091,
            LVM_INSERTGROUPSORTED = 0x109f,
            LVM_INSERTITEMA = 0x1007,
            LVM_INSERTITEMW = 0x104d,
            LVM_INSERTMARKHITTEST = 0x10a8,
            LVM_ISGROUPVIEWENABLED = 0x10af,
            LVM_MAPIDTOINDEX = 0x10b5,
            LVM_MAPINDEXTOID = 0x10b4,
            LVM_MOVEGROUP = 0x1097,
            LVM_MOVEITEMTOGROUP = 0x109a,
            LVM_REDRAWITEMS = 0x1015,
            LVM_REMOVEALLGROUPS = 0x10a0,
            LVM_REMOVEGROUP = 0x1096,
            LVM_SCROLL = 0x1014,
            LVM_SETBKCOLOR = 0x1001,
            LVM_SETBKIMAGEA = 0x1044,
            LVM_SETBKIMAGEW = 0x108a,
            LVM_SETCALLBACKMASK = 0x100b,
            LVM_SETCOLUMNA = 0x101a,
            LVM_SETCOLUMNORDERARRAY = 0x103a,
            LVM_SETCOLUMNW = 0x1060,
            LVM_SETCOLUMNWIDTH = 0x101e,
            LVM_SETEXTENDEDLISTVIEWSTYLE = 0x1036,
            LVM_SETGROUPINFO = 0x1093,
            LVM_SETGROUPMETRICS = 0x109b,
            LVM_SETHOTCURSOR = 0x103e,
            LVM_SETHOTITEM = 0x103c,
            LVM_SETHOVERTIME = 0x1047,
            LVM_SETICONSPACING = 0x1035,
            LVM_SETIMAGELIST = 0x1003,
            LVM_SETINFOTIP = 0x10ad,
            LVM_SETINSERTMARK = 0x10a6,
            LVM_SETINSERTMARKCOLOR = 0x10aa,
            LVM_SETITEMA = 0x1006,
            LVM_SETITEMCOUNT = 0x102f,
            LVM_SETITEMPOSITION = 0x100f,
            LVM_SETITEMPOSITION32 = 0x1031,
            LVM_SETITEMSTATE = 0x102b,
            LVM_SETITEMTEXTA = 0x102e,
            LVM_SETITEMTEXTW = 0x1074,
            LVM_SETITEMW = 0x104c,
            LVM_SETOUTLINECOLOR = 0x10b1,
            LVM_SETSELECTEDCOLUMN = 0x108c,
            LVM_SETSELECTIONMARK = 0x1043,
            LVM_SETTEXTBKCOLOR = 0x1026,
            LVM_SETTEXTCOLOR = 0x1024,
            LVM_SETTILEINFO = 0x10a4,
            LVM_SETTILEVIEWINFO = 0x10a2,
            LVM_SETTILEWIDTH = 0x108d,
            LVM_SETTOOLTIPS = 0x104a,
            LVM_SETUNICODEFORMAT = 0x2005,
            LVM_SETVIEW = 0x108e,
            LVM_SETWORKAREAS = 0x1041,
            LVM_SORTGROUPS = 0x109e,
            LVM_SORTITEMS = 0x1030,
            LVM_SORTITEMSEX = 0x1051,
            LVM_SUBITEMHITTEST = 0x1039,
            LVM_UPDATE = 0x102a
        }

        [Flags]
        public enum WindowStylesEx : uint
        {
            WS_EX_ACCEPTFILES = 0x00000010,
            WS_EX_APPWINDOW = 0x00040000,
            WS_EX_CLIENTEDGE = 0x00000200,
            WS_EX_COMPOSITED = 0x02000000,
            WS_EX_CONTEXTHELP = 0x00000400,
            WS_EX_CONTROLPARENT = 0x00010000,
            WS_EX_DLGMODALFRAME = 0x00000001,
            WS_EX_LAYERED = 0x00080000,
            WS_EX_LAYOUTRTL = 0x00400000,
            WS_EX_LEFT = 0x00000000,
            WS_EX_LEFTSCROLLBAR = 0x00004000,
            WS_EX_LTRREADING = 0x00000000,
            WS_EX_MDICHILD = 0x00000040,
            WS_EX_NOACTIVATE = 0x08000000,
            WS_EX_NOINHERITLAYOUT = 0x00100000,
            WS_EX_NOPARENTNOTIFY = 0x00000004,
            WS_EX_OVERLAPPEDWINDOW = WS_EX_WINDOWEDGE | WS_EX_CLIENTEDGE,
            WS_EX_PALETTEWINDOW = WS_EX_WINDOWEDGE | WS_EX_TOOLWINDOW | WS_EX_TOPMOST,
            WS_EX_RIGHT = 0x00001000,
            WS_EX_RIGHTSCROLLBAR = 0x00000000,
            WS_EX_RTLREADING = 0x00002000,
            WS_EX_STATICEDGE = 0x00020000,
            WS_EX_TOOLWINDOW = 0x00000080,
            WS_EX_TOPMOST = 0x00000008,
            WS_EX_TRANSPARENT = 0x00000020,
            WS_EX_WINDOWEDGE = 0x00000100
        }

        [Flags]
        public enum WindowLongIndex : int
        {
            GWL_EXSTYLE = -20,
            GWLP_HINSTANCE = -6,
            GWLP_HWNDPARENT = -8,
            GWL_ID = -12,
            GWL_STYLE = -16,
            GWL_USERDATA = -21,
            GWL_WNDPROC = -4,
            DWLP_USER = 0x8,
            DWLP_MSGRESULT = 0x0,
            DWLP_DLGPROC = 0x4
        }

        [Flags]
        internal enum KEYEVENTF : uint
        {
            EXTENDEDKEY = 0x0001,
            KEYUP = 0x0002,
            SCANCODE = 0x0008,
            UNICODE = 0x0004
        }

        internal enum VirtualKeyShort : short
        {
            LBUTTON = 0x01,
            RBUTTON = 0x02,
            CANCEL = 0x03,
            MBUTTON = 0x04,
            XBUTTON1 = 0x05,
            XBUTTON2 = 0x06,
            BACK = 0x08,
            TAB = 0x09,
            CLEAR = 0x0C,
            RETURN = 0x0D,
            SHIFT = 0x10,
            CONTROL = 0x11,
            MENU = 0x12,
            PAUSE = 0x13,
            CAPITAL = 0x14,
            KANA = 0x15,
            HANGUL = 0x15,
            JUNJA = 0x17,
            FINAL = 0x18,
            HANJA = 0x19,
            KANJI = 0x19,
            ESCAPE = 0x1B,
            CONVERT = 0x1C,
            NONCONVERT = 0x1D,
            ACCEPT = 0x1E,
            MODECHANGE = 0x1F,
            SPACE = 0x20,
            PRIOR = 0x21,
            NEXT = 0x22,
            END = 0x23,
            HOME = 0x24,
            LEFT = 0x25,
            UP = 0x26,
            RIGHT = 0x27,
            DOWN = 0x28,
            SELECT = 0x29,
            PRINT = 0x2A,
            EXECUTE = 0x2B,
            SNAPSHOT = 0x2C,
            INSERT = 0x2D,
            DELETE = 0x2E,
            HELP = 0x2F,
            KEY_0 = 0x30,
            KEY_1 = 0x31,
            KEY_2 = 0x32,
            KEY_3 = 0x33,
            KEY_4 = 0x34,
            KEY_5 = 0x35,
            KEY_6 = 0x36,
            KEY_7 = 0x37,
            KEY_8 = 0x38,
            KEY_9 = 0x39,
            KEY_A = 0x41,
            KEY_B = 0x42,
            KEY_C = 0x43,
            KEY_D = 0x44,
            KEY_E = 0x45,
            KEY_F = 0x46,
            KEY_G = 0x47,
            KEY_H = 0x48,
            KEY_I = 0x49,
            KEY_J = 0x4A,
            KEY_K = 0x4B,
            KEY_L = 0x4C,
            KEY_M = 0x4D,
            KEY_N = 0x4E,
            KEY_O = 0x4F,
            KEY_P = 0x50,
            KEY_Q = 0x51,
            KEY_R = 0x52,
            KEY_S = 0x53,
            KEY_T = 0x54,
            KEY_U = 0x55,
            KEY_V = 0x56,
            KEY_W = 0x57,
            KEY_X = 0x58,
            KEY_Y = 0x59,
            KEY_Z = 0x5A,
            LWIN = 0x5B,
            RWIN = 0x5C,
            APPS = 0x5D,
            SLEEP = 0x5F,
            NUMPAD0 = 0x60,
            NUMPAD1 = 0x61,
            NUMPAD2 = 0x62,
            NUMPAD3 = 0x63,
            NUMPAD4 = 0x64,
            NUMPAD5 = 0x65,
            NUMPAD6 = 0x66,
            NUMPAD7 = 0x67,
            NUMPAD8 = 0x68,
            NUMPAD9 = 0x69,
            MULTIPLY = 0x6A,
            ADD = 0x6B,
            SEPARATOR = 0x6C,
            SUBTRACT = 0x6D,
            DECIMAL = 0x6E,
            DIVIDE = 0x6F,
            F1 = 0x70,
            F2 = 0x71,
            F3 = 0x72,
            F4 = 0x73,
            F5 = 0x74,
            F6 = 0x75,
            F7 = 0x76,
            F8 = 0x77,
            F9 = 0x78,
            F10 = 0x79,
            F11 = 0x7A,
            F12 = 0x7B,
            F13 = 0x7C,
            F14 = 0x7D,
            F15 = 0x7E,
            F16 = 0x7F,
            F17 = 0x80,
            F18 = 0x81,
            F19 = 0x82,
            F20 = 0x83,
            F21 = 0x84,
            F22 = 0x85,
            F23 = 0x86,
            F24 = 0x87,
            NUMLOCK = 0x90,
            SCROLL = 0x91,
            LSHIFT = 0xA0,
            RSHIFT = 0xA1,
            LCONTROL = 0xA2,
            RCONTROL = 0xA3,
            LMENU = 0xA4,
            RMENU = 0xA5,
            BROWSER_BACK = 0xA6,
            BROWSER_FORWARD = 0xA7,
            BROWSER_REFRESH = 0xA8,
            BROWSER_STOP = 0xA9,
            BROWSER_SEARCH = 0xAA,
            BROWSER_FAVORITES = 0xAB,
            BROWSER_HOME = 0xAC,
            VOLUME_MUTE = 0xAD,
            VOLUME_DOWN = 0xAE,
            VOLUME_UP = 0xAF,
            MEDIA_NEXT_TRACK = 0xB0,
            MEDIA_PREV_TRACK = 0xB1,
            MEDIA_STOP = 0xB2,
            MEDIA_PLAY_PAUSE = 0xB3,
            LAUNCH_MAIL = 0xB4,
            LAUNCH_MEDIA_SELECT = 0xB5,
            LAUNCH_APP1 = 0xB6,
            LAUNCH_APP2 = 0xB7,
            OEM_1 = 0xBA,
            OEM_PLUS = 0xBB,
            OEM_COMMA = 0xBC,
            OEM_MINUS = 0xBD,
            OEM_PERIOD = 0xBE,
            OEM_2 = 0xBF,
            OEM_3 = 0xC0,
            OEM_4 = 0xDB,
            OEM_5 = 0xDC,
            OEM_6 = 0xDD,
            OEM_7 = 0xDE,
            OEM_8 = 0xDF,
            OEM_102 = 0xE2,
            PROCESSKEY = 0xE5,
            PACKET = 0xE7,
            ATTN = 0xF6,
            CRSEL = 0xF7,
            EXSEL = 0xF8,
            EREOF = 0xF9,
            PLAY = 0xFA,
            ZOOM = 0xFB,
            NONAME = 0xFC,
            PA1 = 0xFD,
            OEM_CLEAR = 0xFE
        }

        internal enum ScanCodeShort : short
        {
            LBUTTON = 0,
            RBUTTON = 0,
            CANCEL = 70,
            MBUTTON = 0,
            XBUTTON1 = 0,
            XBUTTON2 = 0,
            BACK = 14,
            TAB = 15,
            CLEAR = 76,
            RETURN = 28,
            SHIFT = 42,
            CONTROL = 29,
            MENU = 56,
            PAUSE = 0,
            CAPITAL = 58,
            KANA = 0,
            HANGUL = 0,
            JUNJA = 0,
            FINAL = 0,
            HANJA = 0,
            KANJI = 0,
            ESCAPE = 1,
            CONVERT = 0,
            NONCONVERT = 0,
            ACCEPT = 0,
            MODECHANGE = 0,
            SPACE = 57,
            PRIOR = 73,
            NEXT = 81,
            END = 79,
            HOME = 71,
            LEFT = 75,
            UP = 72,
            RIGHT = 77,
            DOWN = 80,
            SELECT = 0,
            PRINT = 0,
            EXECUTE = 0,
            SNAPSHOT = 84,
            INSERT = 82,
            DELETE = 83,
            HELP = 99,
            KEY_0 = 11,
            KEY_1 = 2,
            KEY_2 = 3,
            KEY_3 = 4,
            KEY_4 = 5,
            KEY_5 = 6,
            KEY_6 = 7,
            KEY_7 = 8,
            KEY_8 = 9,
            KEY_9 = 10,
            KEY_A = 30,
            KEY_B = 48,
            KEY_C = 46,
            KEY_D = 32,
            KEY_E = 18,
            KEY_F = 33,
            KEY_G = 34,
            KEY_H = 35,
            KEY_I = 23,
            KEY_J = 36,
            KEY_K = 37,
            KEY_L = 38,
            KEY_M = 50,
            KEY_N = 49,
            KEY_O = 24,
            KEY_P = 25,
            KEY_Q = 16,
            KEY_R = 19,
            KEY_S = 31,
            KEY_T = 20,
            KEY_U = 22,
            KEY_V = 47,
            KEY_W = 17,
            KEY_X = 45,
            KEY_Y = 21,
            KEY_Z = 44,
            LWIN = 91,
            RWIN = 92,
            APPS = 93,
            SLEEP = 95,
            NUMPAD0 = 82,
            NUMPAD1 = 79,
            NUMPAD2 = 80,
            NUMPAD3 = 81,
            NUMPAD4 = 75,
            NUMPAD5 = 76,
            NUMPAD6 = 77,
            NUMPAD7 = 71,
            NUMPAD8 = 72,
            NUMPAD9 = 73,
            MULTIPLY = 55,
            ADD = 78,
            SEPARATOR = 0,
            SUBTRACT = 74,
            DECIMAL = 83,
            DIVIDE = 53,
            F1 = 59,
            F2 = 60,
            F3 = 61,
            F4 = 62,
            F5 = 63,
            F6 = 64,
            F7 = 65,
            F8 = 66,
            F9 = 67,
            F10 = 68,
            F11 = 87,
            F12 = 88,
            F13 = 100,
            F14 = 101,
            F15 = 102,
            F16 = 103,
            F17 = 104,
            F18 = 105,
            F19 = 106,
            F20 = 107,
            F21 = 108,
            F22 = 109,
            F23 = 110,
            F24 = 118,
            NUMLOCK = 69,
            SCROLL = 70,
            LSHIFT = 42,
            RSHIFT = 54,
            LCONTROL = 29,
            RCONTROL = 29,
            LMENU = 56,
            RMENU = 56,
            BROWSER_BACK = 106,
            BROWSER_FORWARD = 105,
            BROWSER_REFRESH = 103,
            BROWSER_STOP = 104,
            BROWSER_SEARCH = 101,
            BROWSER_FAVORITES = 102,
            BROWSER_HOME = 50,
            VOLUME_MUTE = 32,
            VOLUME_DOWN = 46,
            VOLUME_UP = 48,
            MEDIA_NEXT_TRACK = 25,
            MEDIA_PREV_TRACK = 16,
            MEDIA_STOP = 36,
            MEDIA_PLAY_PAUSE = 34,
            LAUNCH_MAIL = 108,
            LAUNCH_MEDIA_SELECT = 109,
            LAUNCH_APP1 = 107,
            LAUNCH_APP2 = 33,
            OEM_1 = 39,
            OEM_PLUS = 13,
            OEM_COMMA = 51,
            OEM_MINUS = 12,
            OEM_PERIOD = 52,
            OEM_2 = 53,
            OEM_3 = 41,
            OEM_4 = 26,
            OEM_5 = 43,
            OEM_6 = 27,
            OEM_7 = 40,
            OEM_8 = 0,
            OEM_102 = 86,
            PROCESSKEY = 0,
            PACKET = 0,
            ATTN = 0,
            CRSEL = 0,
            EXSEL = 0,
            EREOF = 93,
            PLAY = 0,
            ZOOM = 98,
            NONAME = 0,
            PA1 = 0,
            OEM_CLEAR = 0,
        }

        [Flags]
        internal enum MOUSEEVENTF : uint
        {
            ABSOLUTE = 0x8000,
            HWHEEL = 0x01000,
            MOVE = 0x0001,
            MOVE_NOCOALESCE = 0x2000,
            LEFTDOWN = 0x0002,
            LEFTUP = 0x0004,
            RIGHTDOWN = 0x0008,
            RIGHTUP = 0x0010,
            MIDDLEDOWN = 0x0020,
            MIDDLEUP = 0x0040,
            VIRTUALDESK = 0x4000,
            WHEEL = 0x0800,
            XDOWN = 0x0080,
            XUP = 0x0100
        }

        internal enum InputType : uint
        {
            Mouse = 0,
            Keyboard = 1,
            Hardware = 3
        }

        [StructLayout(LayoutKind.Sequential)]
        internal struct INPUT
        {
            internal InputType type;
            internal InputUnion U;
            internal static int Size
            {
                get { return Marshal.SizeOf(typeof(INPUT)); }
            }
        }

        [StructLayout(LayoutKind.Sequential)]
        internal struct KEYBDINPUT
        {
            internal VirtualKeyShort wVk;
            internal ScanCodeShort wScan;
            internal KEYEVENTF dwFlags;
            internal int time;
            internal UIntPtr dwExtraInfo;
        }

        [StructLayout(LayoutKind.Sequential)]
        internal struct MOUSEINPUT
        {
            internal int dx;
            internal int dy;
            internal int mouseData;
            internal MOUSEEVENTF dwFlags;
            internal uint time;
            internal UIntPtr dwExtraInfo;
        }

        [StructLayout(LayoutKind.Sequential)]
        internal struct HARDWAREINPUT
        {
            internal int uMsg;
            internal short wParamL;
            internal short wParamH;
        }

        [StructLayout(LayoutKind.Explicit)]
        internal struct InputUnion
        {
            [FieldOffset(0)]
            internal MOUSEINPUT mi;
            [FieldOffset(0)]
            internal KEYBDINPUT ki;
            [FieldOffset(0)]
            internal HARDWAREINPUT hi;
        }

        public enum ShowWindowCommands
        {
            Hide = 0,
            Normal = 1,
            ShowMinimized = 2,
            Maximize = 3, // is this the right value?
            ShowMaximized = 3,
            ShowNoActivate = 4,
            Show = 5,
            Minimize = 6,
            ShowMinNoActive = 7,
            ShowNA = 8,
            Restore = 9,
            ShowDefault = 10,
            ForceMinimize = 11
        }
    }

    public enum ProgressBarState
    {
        Normal = 1,
        Error,
        Warning
    }

    public enum ToAsciiFlags : uint
    {
        MenuIsActive = 1,
        MenuIsInactive = 0
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct NativeMessage
    {
        public IntPtr handle;
        public ControlExtensions.WindowsMessage msg;
        public IntPtr wParam;
        public IntPtr lParam;
        public uint time;
        public Point point;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct NativeMessageWithResult
    {
        public IntPtr handle;
        public ControlExtensions.WindowsMessage message;
        public IntPtr wParam;
        public IntPtr lParam;
        public TimeSpan time;
        public Point point;
        public int returnValue;
    }

    public enum ScrollBarOrientation : int
    {
        SB_HORZ = 0x0,
        SB_VERT = 0x1,
        SB_CTL = 0x2,
        SB_BOTH = 0x3
    }

    [Flags]
    enum WindowStyles : uint
    {
        WS_OVERLAPPED = 0x00000000,
        WS_POPUP = 0x80000000,
        WS_CHILD = 0x40000000,
        WS_MINIMIZE = 0x20000000,
        WS_VISIBLE = 0x10000000,
        WS_DISABLED = 0x08000000,
        WS_CLIPSIBLINGS = 0x04000000,
        WS_CLIPCHILDREN = 0x02000000,
        WS_MAXIMIZE = 0x01000000,
        WS_BORDER = 0x00800000,
        WS_DLGFRAME = 0x00400000,
        WS_VSCROLL = 0x00200000,
        WS_HSCROLL = 0x00100000,
        WS_SYSMENU = 0x00080000,
        WS_THICKFRAME = 0x00040000,
        WS_GROUP = 0x00020000,
        WS_TABSTOP = 0x00010000,

        WS_MINIMIZEBOX = 0x00020000,
        WS_MAXIMIZEBOX = 0x00010000,

        WS_CAPTION = WS_BORDER | WS_DLGFRAME,
        WS_TILED = WS_OVERLAPPED,
        WS_ICONIC = WS_MINIMIZE,
        WS_SIZEBOX = WS_THICKFRAME,
        WS_TILEDWINDOW = WS_OVERLAPPEDWINDOW,

        WS_OVERLAPPEDWINDOW = WS_OVERLAPPED | WS_CAPTION | WS_SYSMENU | WS_THICKFRAME | WS_MINIMIZEBOX | WS_MAXIMIZEBOX,
        WS_POPUPWINDOW = WS_POPUP | WS_BORDER | WS_SYSMENU,
        WS_CHILDWINDOW = WS_CHILD,

        //Extended Window Styles

        WS_EX_DLGMODALFRAME = 0x00000001,
        WS_EX_NOPARENTNOTIFY = 0x00000004,
        WS_EX_TOPMOST = 0x00000008,
        WS_EX_ACCEPTFILES = 0x00000010,
        WS_EX_TRANSPARENT = 0x00000020,

        //#if(WINVER >= 0x0400)

        WS_EX_MDICHILD = 0x00000040,
        WS_EX_TOOLWINDOW = 0x00000080,
        WS_EX_WINDOWEDGE = 0x00000100,
        WS_EX_CLIENTEDGE = 0x00000200,
        WS_EX_CONTEXTHELP = 0x00000400,

        WS_EX_RIGHT = 0x00001000,
        WS_EX_LEFT = 0x00000000,
        WS_EX_RTLREADING = 0x00002000,
        WS_EX_LTRREADING = 0x00000000,
        WS_EX_LEFTSCROLLBAR = 0x00004000,
        WS_EX_RIGHTSCROLLBAR = 0x00000000,

        WS_EX_CONTROLPARENT = 0x00010000,
        WS_EX_STATICEDGE = 0x00020000,
        WS_EX_APPWINDOW = 0x00040000,

        WS_EX_OVERLAPPEDWINDOW = (WS_EX_WINDOWEDGE | WS_EX_CLIENTEDGE),
        WS_EX_PALETTEWINDOW = (WS_EX_WINDOWEDGE | WS_EX_TOOLWINDOW | WS_EX_TOPMOST),
        //#endif /* WINVER >= 0x0400 */

        //#if(WIN32WINNT >= 0x0500)

        WS_EX_LAYERED = 0x00080000,
        //#endif /* WIN32WINNT >= 0x0500 */

        //#if(WINVER >= 0x0500)

        WS_EX_NOINHERITLAYOUT = 0x00100000, // Disable inheritence of mirroring by children
        WS_EX_LAYOUTRTL = 0x00400000, // Right to left mirroring
        //#endif /* WINVER >= 0x0500 */

        //#if(WIN32WINNT >= 0x0500)

        WS_EX_COMPOSITED = 0x02000000,
        WS_EX_NOACTIVATE = 0x08000000
        //#endif /* WIN32WINNT >= 0x0500 */
    }

    [Flags]
    public enum WindowStylesEx : uint
    {
        WS_EX_ACCEPTFILES = 0x00000010,
        WS_EX_APPWINDOW = 0x00040000,
        WS_EX_CLIENTEDGE = 0x00000200,
        WS_EX_COMPOSITED = 0x02000000,
        WS_EX_CONTEXTHELP = 0x00000400,
        WS_EX_CONTROLPARENT = 0x00010000,
        WS_EX_DLGMODALFRAME = 0x00000001,
        WS_EX_LAYERED = 0x00080000,
        WS_EX_LAYOUTRTL = 0x00400000,
        WS_EX_LEFT = 0x00000000,
        WS_EX_LEFTSCROLLBAR = 0x00004000,
        WS_EX_LTRREADING = 0x00000000,
        WS_EX_MDICHILD = 0x00000040,
        WS_EX_NOACTIVATE = 0x08000000,
        WS_EX_NOINHERITLAYOUT = 0x00100000,
        WS_EX_NOPARENTNOTIFY = 0x00000004,
        WS_EX_OVERLAPPEDWINDOW = WS_EX_WINDOWEDGE | WS_EX_CLIENTEDGE,
        WS_EX_PALETTEWINDOW = WS_EX_WINDOWEDGE | WS_EX_TOOLWINDOW | WS_EX_TOPMOST,
        WS_EX_RIGHT = 0x00001000,
        WS_EX_RIGHTSCROLLBAR = 0x00000000,
        WS_EX_RTLREADING = 0x00002000,
        WS_EX_STATICEDGE = 0x00020000,
        WS_EX_TOOLWINDOW = 0x00000080,
        WS_EX_TOPMOST = 0x00000008,
        WS_EX_TRANSPARENT = 0x00000020,
        WS_EX_WINDOWEDGE = 0x00000100
    }

    [Flags()]
    public enum RedrawWindowFlags : uint
    {
        Invalidate = 0x1,
        InternalPaint = 0x2,
        Erase = 0x4,
        Validate = 0x8,
        NoInternalPaint = 0x10,
        NoErase = 0x20,
        NoChildren = 0x40,
        AllChildren = 0x80,
        UpdateNow = 0x100,
        EraseNow = 0x200,
        Frame = 0x400,
        NoFrame = 0x800
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct FLASHWINFO
    {
        public UInt32 cbSize;
        public IntPtr hwnd;
        public FlashWindowFlags dwFlags;
        public UInt32 uCount;
        public UInt32 dwTimeout;
    }

    public enum SystemMetric
    {
        SM_CXSCREEN = 0,  // 0x00
        SM_CYSCREEN = 1,  // 0x01
        SM_CXVSCROLL = 2,  // 0x02
        SM_CYHSCROLL = 3,  // 0x03
        SM_CYCAPTION = 4,  // 0x04
        SM_CXBORDER = 5,  // 0x05
        SM_CYBORDER = 6,  // 0x06
        SM_CXDLGFRAME = 7,  // 0x07
        SM_CXFIXEDFRAME = 7,  // 0x07
        SM_CYDLGFRAME = 8,  // 0x08
        SM_CYFIXEDFRAME = 8,  // 0x08
        SM_CYVTHUMB = 9,  // 0x09
        SM_CXHTHUMB = 10, // 0x0A
        SM_CXICON = 11, // 0x0B
        SM_CYICON = 12, // 0x0C
        SM_CXCURSOR = 13, // 0x0D
        SM_CYCURSOR = 14, // 0x0E
        SM_CYMENU = 15, // 0x0F
        SM_CXFULLSCREEN = 16, // 0x10
        SM_CYFULLSCREEN = 17, // 0x11
        SM_CYKANJIWINDOW = 18, // 0x12
        SM_MOUSEPRESENT = 19, // 0x13
        SM_CYVSCROLL = 20, // 0x14
        SM_CXHSCROLL = 21, // 0x15
        SM_DEBUG = 22, // 0x16
        SM_SWAPBUTTON = 23, // 0x17
        SM_CXMIN = 28, // 0x1C
        SM_CYMIN = 29, // 0x1D
        SM_CXSIZE = 30, // 0x1E
        SM_CYSIZE = 31, // 0x1F
        SM_CXSIZEFRAME = 32, // 0x20
        SM_CXFRAME = 32, // 0x20
        SM_CYSIZEFRAME = 33, // 0x21
        SM_CYFRAME = 33, // 0x21
        SM_CXMINTRACK = 34, // 0x22
        SM_CYMINTRACK = 35, // 0x23
        SM_CXDOUBLECLK = 36, // 0x24
        SM_CYDOUBLECLK = 37, // 0x25
        SM_CXICONSPACING = 38, // 0x26
        SM_CYICONSPACING = 39, // 0x27
        SM_MENUDROPALIGNMENT = 40, // 0x28
        SM_PENWINDOWS = 41, // 0x29
        SM_DBCSENABLED = 42, // 0x2A
        SM_CMOUSEBUTTONS = 43, // 0x2B
        SM_SECURE = 44, // 0x2C
        SM_CXEDGE = 45, // 0x2D
        SM_CYEDGE = 46, // 0x2E
        SM_CXMINSPACING = 47, // 0x2F
        SM_CYMINSPACING = 48, // 0x30
        SM_CXSMICON = 49, // 0x31
        SM_CYSMICON = 50, // 0x32
        SM_CYSMCAPTION = 51, // 0x33
        SM_CXSMSIZE = 52, // 0x34
        SM_CYSMSIZE = 53, // 0x35
        SM_CXMENUSIZE = 54, // 0x36
        SM_CYMENUSIZE = 55, // 0x37
        SM_ARRANGE = 56, // 0x38
        SM_CXMINIMIZED = 57, // 0x39
        SM_CYMINIMIZED = 58, // 0x3A
        SM_CXMAXTRACK = 59, // 0x3B
        SM_CYMAXTRACK = 60, // 0x3C
        SM_CXMAXIMIZED = 61, // 0x3D
        SM_CYMAXIMIZED = 62, // 0x3E
        SM_NETWORK = 63, // 0x3F
        SM_CLEANBOOT = 67, // 0x43
        SM_CXDRAG = 68, // 0x44
        SM_CYDRAG = 69, // 0x45
        SM_SHOWSOUNDS = 70, // 0x46
        SM_CXMENUCHECK = 71, // 0x47
        SM_CYMENUCHECK = 72, // 0x48
        SM_SLOWMACHINE = 73, // 0x49
        SM_MIDEASTENABLED = 74, // 0x4A
        SM_MOUSEWHEELPRESENT = 75, // 0x4B
        SM_XVIRTUALSCREEN = 76, // 0x4C
        SM_YVIRTUALSCREEN = 77, // 0x4D
        SM_CXVIRTUALSCREEN = 78, // 0x4E
        SM_CYVIRTUALSCREEN = 79, // 0x4F
        SM_CMONITORS = 80, // 0x50
        SM_SAMEDISPLAYFORMAT = 81, // 0x51
        SM_IMMENABLED = 82, // 0x52
        SM_CXFOCUSBORDER = 83, // 0x53
        SM_CYFOCUSBORDER = 84, // 0x54
        SM_TABLETPC = 86, // 0x56
        SM_MEDIACENTER = 87, // 0x57
        SM_STARTER = 88, // 0x58
        SM_SERVERR2 = 89, // 0x59
        SM_MOUSEHORIZONTALWHEELPRESENT = 91, // 0x5B
        SM_CXPADDEDBORDER = 92, // 0x5C
        SM_DIGITIZER = 94, // 0x5E
        SM_MAXIMUMTOUCHES = 95, // 0x5F

        SM_REMOTESESSION = 0x1000, // 0x1000
        SM_SHUTTINGDOWN = 0x2000, // 0x2000
        SM_REMOTECONTROL = 0x2001, // 0x2001


        SM_CONVERTABLESLATEMODE = 0x2003,
        SM_SYSTEMDOCKED = 0x2004,
    }

    public enum FlashWindowFlags : uint
    {
        /// <summary>
        /// Stop flashing. The system restores the window to its original state. 
        /// </summary>    
        FLASHW_STOP = 0,

        /// <summary>
        /// Flash the window caption 
        /// </summary>
        FLASHW_CAPTION = 1,

        /// <summary>
        /// Flash the taskbar button. 
        /// </summary>
        FLASHW_TRAY = 2,

        /// <summary>
        /// Flash both the window caption and taskbar button.
        /// This is equivalent to setting the FLASHW_CAPTION | FLASHW_TRAY flags. 
        /// </summary>
        FLASHW_ALL = 3,

        /// <summary>
        /// Flash continuously, until the FLASHW_STOP flag is set.
        /// </summary>
        FLASHW_TIMER = 4,

        /// <summary>
        /// Flash continuously until the window comes to the foreground. 
        /// </summary>
        FLASHW_TIMERNOFG = 12
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct TRACKMOUSEEVENT
    {
        public Int32 cbSize;    // using Int32 instead of UInt32 is safe here, and this avoids casting the result  of Marshal.SizeOf()
        [MarshalAs(UnmanagedType.U4)]
        public TMEFlags dwFlags;
        public IntPtr hWnd;
        public UInt32 dwHoverTime;

        public TRACKMOUSEEVENT(TMEFlags dwFlags, IntPtr hWnd, UInt32 dwHoverTime)
        {
            this.cbSize = Marshal.SizeOf(typeof(TRACKMOUSEEVENT));
            this.dwFlags = dwFlags;
            this.hWnd = hWnd;
            this.dwHoverTime = dwHoverTime;
        }
    }

    [Flags]
    public enum TMEFlags : uint
    {
        /// <summary>
        /// The caller wants to cancel a prior tracking request. The caller should also specify the type of tracking that it wants to cancel. For example, to cancel hover tracking, the caller must pass the TME_CANCEL and TME_HOVER flags.
        /// </summary>
        TME_CANCEL = 0x80000000,
        /// <summary>
        /// The caller wants hover notification. Notification is delivered as a WM_MOUSEHOVER message.
        /// If the caller requests hover tracking while hover tracking is already active, the hover timer will be reset.
        /// This flag is ignored if the mouse pointer is not over the specified window or area.
        /// </summary>
        TME_HOVER = 0x00000001,
        /// <summary>
        /// The caller wants leave notification. Notification is delivered as a WM_MOUSELEAVE message. If the mouse is not over the specified window or area, a leave notification is generated immediately and no further tracking is performed.
        /// </summary>
        TME_LEAVE = 0x00000002,
        /// <summary>
        /// The caller wants hover and leave notification for the nonclient areas. Notification is delivered as WM_NCMOUSEHOVER and WM_NCMOUSELEAVE messages.
        /// </summary>
        TME_NONCLIENT = 0x00000010,
        /// <summary>
        /// The function fills in the structure instead of treating it as a tracking request. The structure is filled such that had that structure been passed to TrackMouseEvent, it would generate the current tracking. The only anomaly is that the hover time-out returned is always the actual time-out and not HOVER_DEFAULT, if HOVER_DEFAULT was specified during the original TrackMouseEvent request. 
        /// </summary>
        TME_QUERY = 0x40000000,
    }

    [Serializable, StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
    public struct TEXTMETRIC
    {
        public int tmHeight;
        public int tmAscent;
        public int tmDescent;
        public int tmInternalLeading;
        public int tmExternalLeading;
        public int tmAveCharWidth;
        public int tmMaxCharWidth;
        public int tmWeight;
        public int tmOverhang;
        public int tmDigitizedAspectX;
        public int tmDigitizedAspectY;
        public byte tmFirstChar;    // for compatibility issues it must be byte instead of char (see the comment for further details above)
        public byte tmLastChar;    // for compatibility issues it must be byte instead of char (see the comment for further details above)
        public byte tmDefaultChar;    // for compatibility issues it must be byte instead of char (see the comment for further details above)
        public byte tmBreakChar;    // for compatibility issues it must be byte instead of char (see the comment for further details above)
        public byte tmItalic;
        public byte tmUnderlined;
        public byte tmStruckOut;
        public byte tmPitchAndFamily;
        public byte tmCharSet;
    }
}
