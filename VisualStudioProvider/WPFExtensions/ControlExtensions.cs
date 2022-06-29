using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using System.Collections.Generic;
using System.Diagnostics;
using System.Windows.Controls.Primitives;
using Utils;
using System.Linq;
using System.Threading;
using Window = System.Windows.Window;
using System.Text;

namespace WPFControlLibrary
{
    public static class ControlExtensions
    {
        //        public const MouseButtons NoMouseButton = (MouseButtons)(-1);
        //        public static Thickness ThicknessEmpty = new Thickness();

        //        static ControlExtensions()
        //        {
        //            Graphics.OnGetGraphics += (sender, e) =>
        //            {
        //                if (sender is FrameworkElement)
        //                {
        //                    var frameworkElement = (FrameworkElement)sender;

        //                    e.Value = new SLControlLibrary.Drawing.Graphics(frameworkElement);
        //                }
        //            };
        //        }

        //        public static IControlHost GetControlHost(this Application app)
        //        {
        //            var provider = Globals.GetEnvironmentVariable<IControlHostProvider>(Globals.CONTROL_HOST_PROVIDER);

        //            if (provider != null)
        //            {
        //                return provider.GetControlHost(app);
        //            }
        //            else
        //            {
        //                return null;
        //            }
        //        }

        //#if STEPTHROUGHCONTROLEXTENSIONS
        //        [DebuggerStepThrough]
        //#endif
        //        public static object Invoke(this IControl control, string method, params object[] parms)
        //        {
        //            var properties = control.Properties;

        //            if (properties == null)
        //            {
        //                properties = new ControlProperties(control);
        //            }

        //            return properties.Invoke(method, parms);
        //        }

        //#if STEPTHROUGHCONTROLEXTENSIONS
        //        [DebuggerStepThrough]
        //#endif
        //        public static T Invoke<T>(this IControl control, string method, params object[] parms)
        //        {
        //            var properties = control.Properties;

        //            if (properties == null)
        //            {
        //                properties = new ControlProperties(control);
        //            }

        //            return (T) properties.Invoke(method, parms);
        //        }

        //#if STEPTHROUGHCONTROLEXTENSIONS
        //        [DebuggerStepThrough]
        //#endif
        //        public static void Set(this IControl control, string property, object value)
        //        {
        //            var properties = control.Properties;

        //            if (properties == null)
        //            {
        //                properties = new ControlProperties(control);
        //            }

        //            properties[property] = value;
        //        }

        //#if STEPTHROUGHCONTROLEXTENSIONS
        //        [DebuggerStepThrough]
        //#endif
        //        public static object Get(this IControl control, string property)
        //        {
        //            var properties = control.Properties;

        //            if (properties == null)
        //            {
        //                properties = new ControlProperties(control);
        //            }

        //            return properties[property];
        //        }

        //#if STEPTHROUGHCONTROLEXTENSIONS
        //        [DebuggerStepThrough]
        //#endif
        //        public static T Get<T>(this IControl control, string property)
        //        {
        //            return (T) control.Get(property);
        //        }

        //#if STEPTHROUGHCONTROLEXTENSIONS
        //        [DebuggerStepThrough]
        //#endif
        //        public static IControlEventsContainer GetEvents(this IControl control)
        //        {
        //            return control.Get<IControlEventsContainer>("Events");
        //        }

        //#if STEPTHROUGHCONTROLEXTENSIONS
        //        [DebuggerStepThrough]
        //#endif
        //        public static IListBoxControlEventsContainer GetEvents(this IListBox control)
        //        {
        //            return control.Get<IListBoxControlEventsContainer>("Events");
        //        }

        //#if STEPTHROUGHCONTROLEXTENSIONS
        //        [DebuggerStepThrough]
        //#endif
        //        public static Font GetFont(this IControl control)
        //        {
        //            return control.Get<Font>("Font");
        //        }

        //#if STEPTHROUGHCONTROLEXTENSIONS
        //        [DebuggerStepThrough]
        //#endif
        //        public static string GetText(this IControl control)
        //        {
        //            return control.Get<string>("Text");
        //        }

        //#if STEPTHROUGHCONTROLEXTENSIONS
        //        [DebuggerStepThrough]
        //#endif
        //        public static void BringToFront(this IControl control)
        //        {
        //            control.Invoke("BringToFront");
        //        }

        //#if STEPTHROUGHCONTROLEXTENSIONS
        //        [DebuggerStepThrough]
        //#endif
        //        public static bool GetAutoScale(this IControl control)
        //        {
        //            return control.Get<bool>("AutoScale");
        //        }

        //#if STEPTHROUGHCONTROLEXTENSIONS
        //        [DebuggerStepThrough]
        //#endif
        //        public static AutoScaleMode GetAutoScaleMode(this IControl control)
        //        {
        //            return control.Get<AutoScaleMode>("AutoScaleMode");
        //        }

        //#if STEPTHROUGHCONTROLEXTENSIONS
        //        [DebuggerStepThrough]
        //#endif
        //        public static void SetAutoScaleMode(this IControl control, AutoScaleMode mode)
        //        {
        //            control.Set("AutoScaleMode", mode);
        //        }

        //#if STEPTHROUGHCONTROLEXTENSIONS
        //        [DebuggerStepThrough]
        //#endif
        //        public static void SetAutoSize(this IControl control, bool autoSize)
        //        {
        //            control.Set("AutoSize", autoSize);
        //        }

        //#if STEPTHROUGHCONTROLEXTENSIONS
        //        [DebuggerStepThrough]
        //#endif
        //        public static void SetBorderStyle(this IControl control, BorderStyle style)
        //        {
        //            control.Set("BorderStyle", style);
        //        }

        //#if STEPTHROUGHCONTROLEXTENSIONS
        //        [DebuggerStepThrough]
        //#endif
        //        public static void SetAcceptButton(this IDocument control, IButtonControl button)
        //        {
        //            control.Set("AcceptButton", button);
        //        }

        //#if STEPTHROUGHCONTROLEXTENSIONS
        //        [DebuggerStepThrough]
        //#endif
        //        public static void SetCancelButton(this IDocument control, IButtonControl button)
        //        {
        //            control.Set("CancelButton", button);
        //        }

        //#if STEPTHROUGHCONTROLEXTENSIONS
        //        [DebuggerStepThrough]
        //#endif
        //        public static IButtonControl GetAcceptButton(this IDocument control)
        //        {
        //            return control.Get<IButtonControl>("AcceptButton");
        //        }

        //#if STEPTHROUGHCONTROLEXTENSIONS
        //        [DebuggerStepThrough]
        //#endif
        //        public static IButtonControl GetCancelButton(this IDocument control)
        //        {
        //            return control.Get<IButtonControl>("CancelButton");
        //        }

        //#if STEPTHROUGHCONTROLEXTENSIONS
        //        [DebuggerStepThrough]
        //#endif
        //        public static void SetWindowState(this IDocument control, WindowState state)
        //        {
        //            control.Set("WindowState", state);
        //        }

        //#if STEPTHROUGHCONTROLEXTENSIONS
        //        [DebuggerStepThrough]
        //#endif
        //        public static void SuspendLayout(this IControl control)
        //        {
        //            control.Invoke("SuspendLayout");
        //        }

        //#if STEPTHROUGHCONTROLEXTENSIONS
        //        [DebuggerStepThrough]
        //#endif
        //        public static void ResumeLayout(this IControl control, bool b)
        //        {
        //            control.Invoke("ResumeLayout", b);
        //        }

        //#if STEPTHROUGHCONTROLEXTENSIONS
        //        [DebuggerStepThrough]
        //#endif
        //        public static void ResumeLayout(this IControl control)
        //        {
        //            control.Invoke("ResumeLayout");
        //        }

        //#if STEPTHROUGHCONTROLEXTENSIONS
        //        [DebuggerStepThrough]
        //#endif
        //        public static void PerformLayout(this IControl control)
        //        {
        //            control.Invoke("PerformLayout");
        //        }

        //#if STEPTHROUGHCONTROLEXTENSIONS
        //        [DebuggerStepThrough]
        //#endif
        //        public static string GetToolTip(this ToolTip toolTip, IControl control)
        //        {
        //            return control.Get<string>("ToolTip");
        //        }

        //#if STEPTHROUGHCONTROLEXTENSIONS
        //        [DebuggerStepThrough]
        //#endif
        //        public static void SetToolTip(this ToolTip toolTip, IControl control, string text)
        //        {
        //            control.Set("ToolTip", text);
        //        }

        //#if STEPTHROUGHCONTROLEXTENSIONS
        //        [DebuggerStepThrough]
        //#endif
        //        public static void SetParent(this IControl control, IControl parent)
        //        {
        //            control.Set("Parent", parent);
        //        }

        //#if STEPTHROUGHCONTROLEXTENSIONS
        //        [DebuggerStepThrough]
        //#endif
        //        public static Rect GetDisplayRectangle(this IControl control)
        //        {
        //            return control.Get<Rect>("DisplayRectangle");
        //        }

        //#if STEPTHROUGHCONTROLEXTENSIONS
        //        [DebuggerStepThrough]
        //#endif
        //        public static Rect GetBounds(this IControl control)
        //        {
        //            return control.Get<Rect>("Bounds");
        //        }

        //#if STEPTHROUGHCONTROLEXTENSIONS
        //        [DebuggerStepThrough]
        //#endif
        //        public static void SetBounds(this IControl control, int left, int top, int width, int height, BoundsSpecified boundsSpecified)
        //        {
        //            control.Invoke("SetBounds", left, top, width, height, boundsSpecified);
        //        }

        //#if STEPTHROUGHCONTROLEXTENSIONS
        //        [DebuggerStepThrough]
        //#endif
        //        public static void SetBounds(this IControl control, int left, int top, int width, int height)
        //        {
        //            var rect = new Rect(left, top, width, height);

        //            control.Invoke("SetBounds", rect);
        //        }

        //#if STEPTHROUGHCONTROLEXTENSIONS
        //        [DebuggerStepThrough]
        //#endif
        //        public static void SetBounds(this IControl control, Rect bounds)
        //        {
        //            control.Set("Bounds", bounds);
        //        }

        //#if STEPTHROUGHCONTROLEXTENSIONS
        //        [DebuggerStepThrough]
        //#endif
        //        public static void SetTop(this IControl control, int top)
        //        {
        //            control.Set("Top", top);
        //        }

        //#if STEPTHROUGHCONTROLEXTENSIONS
        //        [DebuggerStepThrough]
        //#endif
        //        public static void SetTabStop(this IControl control, bool set)
        //        {
        //            control.Set("TabStop", set);
        //        }

        //#if STEPTHROUGHCONTROLEXTENSIONS
        //        [DebuggerStepThrough]
        //#endif
        //        public static void SetTabIndex(this IControl control, int index)
        //        {
        //            control.Set("TabIndex", index);
        //        }

        //#if STEPTHROUGHCONTROLEXTENSIONS
        //        [DebuggerStepThrough]
        //#endif
        //        public static bool GetTabStop(this IControl control)
        //        {
        //            return control.Get<bool>("TabStop");
        //        }

        //#if STEPTHROUGHCONTROLEXTENSIONS
        //        [DebuggerStepThrough]
        //#endif
        //        public static void SetLeft(this IControl control, int left)
        //        {
        //            control.Set("Left", left);
        //        }

        //#if STEPTHROUGHCONTROLEXTENSIONS
        //        [DebuggerStepThrough]
        //#endif
        //        public static void SetLocation(this IControl control, Point point)
        //        {
        //            control.Set("Location", point);
        //        }

        //#if STEPTHROUGHCONTROLEXTENSIONS
        //        [DebuggerStepThrough]
        //#endif
        //        public static void SetName(this IControl control, string name)
        //        {
        //            control.Set("Name", name);
        //        }

        //#if STEPTHROUGHCONTROLEXTENSIONS
        //        [DebuggerStepThrough]
        //#endif
        //        public static void SetDock(this IControl control, DockStyle style)
        //        {
        //            control.Set("Dock", style);
        //        }

        //#if STEPTHROUGHCONTROLEXTENSIONS
        //        [DebuggerStepThrough]
        //#endif
        //        public static void SetCursor(this IControl control, Cursor cursor)
        //        {
        //            control.Set("Cursor", cursor);
        //        }

        //#if STEPTHROUGHCONTROLEXTENSIONS
        //        [DebuggerStepThrough]
        //#endif
        //        public static void Refresh(this IControl control)
        //        {
        //            if (control is IOverrideControl)
        //            {
        //                ((IOverrideControl)control).Refresh();
        //            }
        //            else
        //            {
        //                control.Invoke("Refresh");
        //            }
        //        }

        //#if STEPTHROUGHCONTROLEXTENSIONS
        //        [DebuggerStepThrough]
        //#endif
        //        public static void Update(this IControl control)
        //        {
        //            control.Invoke("Update");
        //        }

        //#if STEPTHROUGHCONTROLEXTENSIONS
        //        [DebuggerStepThrough]
        //#endif
        //        public static void Invalidate(this IControl control, bool b = true)
        //        {
        //            control.Invoke("Invalidate", b);
        //        }

        //#if STEPTHROUGHCONTROLEXTENSIONS
        //        [DebuggerStepThrough]
        //#endif
        //        public static bool IsTopLevel(this IDocument control)
        //        {
        //            return control.Get<bool>("IsTopLevel");
        //        }

        //#if STEPTHROUGHCONTROLEXTENSIONS
        //        [DebuggerStepThrough]
        //#endif
        //        public static void Invalidate(this IControl control, Region region)
        //        {
        //            control.Invoke("Invalidate", region);
        //        }

        //#if STEPTHROUGHCONTROLEXTENSIONS
        //        [DebuggerStepThrough]
        //#endif
        //        public static void SetStyle(this IControl control, ControlStyles style, bool apply)
        //        {
        //            control.Invoke("SetStyle", style, apply);
        //        }

        //#if STEPTHROUGHCONTROLEXTENSIONS
        //        [DebuggerStepThrough]
        //#endif
        //        public static void Invalidate(this IControl control, Region region, bool b)
        //        {
        //            control.Invoke("Invalidate", region);
        //        }

        //#if STEPTHROUGHCONTROLEXTENSIONS
        //        [DebuggerStepThrough]
        //#endif
        //        public static void Invalidate(this IControl control, Rect rect)
        //        {
        //            control.Invoke("Invalidate", rect);
        //        }

        //#if STEPTHROUGHCONTROLEXTENSIONS
        //        [DebuggerStepThrough]
        //#endif
        //        public static Point[] MapWindowPoints(this FrameworkElement control, FrameworkElement mapTo, Point point)
        //        {
        //            return Globals.MapWindowPoints(control, mapTo, point);
        //        }

        //#if STEPTHROUGHCONTROLEXTENSIONS
        //        [DebuggerStepThrough]
        //#endif
        //        public static Point[] MapWindowPoints(this IControl control, IControl mapTo, Point point)
        //        {
        //            return control.Invoke<Point[]>("MapWindowPoints", mapTo, point);
        //        }

        //#if STEPTHROUGHCONTROLEXTENSIONS
        //        [DebuggerStepThrough]
        //#endif
        //        public static Point[] MapWindowPoints(this IControl control, IControl mapTo, Point[] points)
        //        {
        //            return control.Invoke<Point[]>("MapWindowPoints", mapTo, points);
        //        }

        //#if STEPTHROUGHCONTROLEXTENSIONS
        //        [DebuggerStepThrough]
        //#endif
        //        public static Point[] MapWindowPoints(this IControl control, FrameworkElement mapTo, Point point)
        //        {
        //            return control.Invoke<Point[]>("MapWindowPoints", mapTo, point);
        //        }

        //#if STEPTHROUGHCONTROLEXTENSIONS
        //        [DebuggerStepThrough]
        //#endif
        //        public static Point[] MapWindowPoints(this IControl control, FrameworkElement mapTo, Point[] points)
        //        {
        //            return control.Invoke<Point[]>("MapWindowPoints", mapTo, points);
        //        }

        //#if STEPTHROUGHCONTROLEXTENSIONS
        //        [DebuggerStepThrough]
        //#endif
        //        public static UIElement GetUIAncestor(this IControl control)
        //        {
        //            return control.Get<UIElement>("UIAncestor");
        //        }

        //#if STEPTHROUGHCONTROLEXTENSIONS
        //        [DebuggerStepThrough]
        //#endif
        //        public static Point GetOffsetToUIAncestor(this IControl control)
        //        {
        //            return control.Get<Point>("OffsetToUIAncestor");
        //        }

        //#if STEPTHROUGHCONTROLEXTENSIONS
        //        [DebuggerStepThrough]
        //#endif
        //        public static IControl GetOwner(this IControl control)
        //        {
        //            return control.Get<IControl>("Owner");
        //        }

        //#if STEPTHROUGHCONTROLEXTENSIONS
        //        [DebuggerStepThrough]
        //#endif
        //        public static void Invalidate(this IControl control, Rect rect, bool b)
        //        {
        //            control.Invoke("Invalidate", rect, b);
        //        }

        //#if STEPTHROUGHCONTROLEXTENSIONS
        //        [DebuggerStepThrough]
        //#endif
        //        public static bool IsMirrored(this IControl control)
        //        {
        //            return control.Get<bool>("IsMirrored");
        //        }

        //#if STEPTHROUGHCONTROLEXTENSIONS
        //        [DebuggerStepThrough]
        //#endif
        //        public static bool IsAutoSize(this IControl control)
        //        {
        //            return control.Get<bool>("IsAutoSize");
        //        }

        //#if STEPTHROUGHCONTROLEXTENSIONS
        //        [DebuggerStepThrough]
        //#endif
        //        public static int GetWidth(this IControl control)
        //        {
        //            return control.Get<int>("Width");
        //        }

        //#if STEPTHROUGHCONTROLEXTENSIONS
        //        [DebuggerStepThrough]
        //#endif
        //        public static int GetHeight(this IControl control)
        //        {
        //            return control.Get<int>("Height");
        //        }

        //#if STEPTHROUGHCONTROLEXTENSIONS
        //        [DebuggerStepThrough]
        //#endif
        //        public static void SetWidth(this IControl control, int width)
        //        {
        //            control.Set("Width", width);
        //        }

        //#if STEPTHROUGHCONTROLEXTENSIONS
        //        [DebuggerStepThrough]
        //#endif
        //        public static void SetHeight(this IControl control, int height)
        //        {
        //            control.Set("Height", height);
        //        }

        //#if STEPTHROUGHCONTROLEXTENSIONS
        //        [DebuggerStepThrough]
        //#endif
        //        public static DragDropOperation DoDragDrop(this IControl control, object data, DragDropEffects allowedEffects)
        //        {
        //            return control.Invoke<DragDropOperation>("DoDragDrop", data, allowedEffects);
        //        }

        //#if STEPTHROUGHCONTROLEXTENSIONS
        //        [DebuggerStepThrough]
        //#endif
        //        public static Graphics CreateGraphics(this IControl control)
        //        {
        //            return control.Invoke<Graphics>("CreateGraphics");
        //        }

        //#if STEPTHROUGHCONTROLEXTENSIONS
        //        [DebuggerStepThrough]
        //#endif
        //        public static Size GetClientSize(this IControl control)
        //        {
        //            return control.Get<Size>("ClientSize");
        //        }

        //#if STEPTHROUGHCONTROLEXTENSIONS
        //        [DebuggerStepThrough]
        //#endif
        //        public static void SetClientSize(this IControl control, Size size)
        //        {
        //            control.Set("ClientSize", size);
        //        }

        //#if STEPTHROUGHCONTROLEXTENSIONS
        //        [DebuggerStepThrough]
        //#endif
        //        public static bool GetIsMdiContainer(this IDocument control)
        //        {
        //            return control.Get<bool>("MdiContainer");
        //        }

        //#if STEPTHROUGHCONTROLEXTENSIONS
        //        [DebuggerStepThrough]
        //#endif
        //        public static bool GetAllowDrop(this IControl control)
        //        {
        //            return control.Get<bool>("AllowDrop");
        //        }

        //#if STEPTHROUGHCONTROLEXTENSIONS
        //        [DebuggerStepThrough]
        //#endif
        //        public static bool? GetAllowDrag(this IControl control)
        //        {
        //            return control.Get<bool?>("AllowDrag");
        //        }

        //#if STEPTHROUGHCONTROLEXTENSIONS
        //        [DebuggerStepThrough]
        //#endif
        //        public static void SetAllowDrag(this IControl control, bool b)
        //        {
        //            control.Set("AllowDrag", b);
        //        }

        //#if STEPTHROUGHCONTROLEXTENSIONS
        //        [DebuggerStepThrough]
        //#endif
        //        public static void SetAllowDrop(this IControl control, bool b)
        //        {
        //            control.Set("AllowDrop", b);
        //        }

        //#if STEPTHROUGHCONTROLEXTENSIONS
        //        [DebuggerStepThrough]
        //#endif
        //        public static void SetText(this IControl control, string text)
        //        {
        //            control.Set("Text", text);
        //        }

        //#if STEPTHROUGHCONTROLEXTENSIONS
        //        [DebuggerStepThrough]
        //#endif
        //        public static bool IsEnabled(this IControl control)
        //        {
        //            return control.Get<bool>("Enabled");
        //        }

        //#if STEPTHROUGHCONTROLEXTENSIONS
        //        [DebuggerStepThrough]
        //#endif
        //        public static bool IsDisposed(this IControl control)
        //        {
        //            return control.Get<bool>("IsDisposed");
        //        }

        //#if STEPTHROUGHCONTROLEXTENSIONS
        //        [DebuggerStepThrough]
        //#endif
        //        public static void SetEnabled(this IControl control, bool b)
        //        {
        //            control.Set("Enabled", b);
        //        }

        //#if STEPTHROUGHCONTROLEXTENSIONS
        //        [DebuggerStepThrough]
        //#endif
        //        public static bool GetDragDropEnabled(this IControl control)
        //        {
        //            return control.Get<bool>("DragDropEnabled");
        //        }

        //        public static void SetDragDropEnabled(this IControl control, bool b)
        //        {
        //            control.Set("DragDropEnabled", b);
        //        }

        //#if STEPTHROUGHCONTROLEXTENSIONS
        //        [DebuggerStepThrough]
        //#endif
        //        public static bool IsVisible(this IControl control)
        //        {
        //            return control.Get<bool>("Visible");
        //        }

        //#if STEPTHROUGHCONTROLEXTENSIONS
        //        [DebuggerStepThrough]
        //#endif
        //        public static void SetVisible(this IControl control, bool b)
        //        {
        //            control.Set("Visible", b);
        //        }

        //#if STEPTHROUGHCONTROLEXTENSIONS
        //        [DebuggerStepThrough]
        //#endif
        //        public static string GetName(this IControl control)
        //        {
        //            return control.Get<string>("Name");
        //        }

        //#if STEPTHROUGHCONTROLEXTENSIONS
        //        [DebuggerStepThrough]
        //#endif
        //        public static string GetTypeAndName(this IControl control, string delimiter = null)
        //        {
        //            if (control is FrameworkElement)
        //            {
        //                var element = (FrameworkElement)control;

        //                return element.GetTypeAndName(delimiter);
        //            }
        //            else
        //            {
        //                var name = control.GetName();

        //                if (string.IsNullOrEmpty(name))
        //                {
        //                    return control.GetType().Name;
        //                }
        //                else if (delimiter != null)
        //                {
        //                    return control.GetType().Name + delimiter + name;
        //                }
        //                else
        //                {
        //                    return control.GetType().Name + " (" + name + ")";
        //                }
        //            }
        //        }

        //#if STEPTHROUGHCONTROLEXTENSIONS
        //        [DebuggerStepThrough]
        //#endif
        //        public static IControl GetParent(this IControl control)
        //        {
        //            return control.Get<IControl>("Parent");
        //        }

        //#if STEPTHROUGHCONTROLEXTENSIONS
        //        [DebuggerStepThrough]
        //#endif
        //        public static Thickness GetMargin(this IControl control)
        //        {
        //            return control.Get<Thickness>("Margin");
        //        }

        //#if STEPTHROUGHCONTROLEXTENSIONS
        //        [DebuggerStepThrough]
        //#endif
        //        public static Thickness GetPadding(this IControl control)
        //        {
        //            return control.Get<Thickness>("Padding");
        //        }

        //#if STEPTHROUGHCONTROLEXTENSIONS
        //        [DebuggerStepThrough]
        //#endif
        //        public static void SetMargin(this IControl control, Thickness t)
        //        {
        //            control.Set("Margin", t);
        //        }

        //#if STEPTHROUGHCONTROLEXTENSIONS
        //        [DebuggerStepThrough]
        //#endif
        //        public static void SetIsMdiContainer(this IDocument control, bool b)
        //        {
        //            control.Set("IsMdiContainer", b);
        //        }

        //#if STEPTHROUGHCONTROLEXTENSIONS
        //        [DebuggerStepThrough]
        //#endif
        //        public static int GetChildIndex(this IControl control, IControl child)
        //        {
        //            return control.Invoke<int>("GetChildIndex", child);
        //        }

        //#if STEPTHROUGHCONTROLEXTENSIONS
        //        [DebuggerStepThrough]
        //#endif
        //        public static int GetChildIndex(this IControl control, IControl child, bool b)
        //        {
        //            return control.Invoke<int>("GetChildIndex", child, b);
        //        }

        //#if STEPTHROUGHCONTROLEXTENSIONS
        //        [DebuggerStepThrough]
        //#endif
        //        public static IWindowTarget GetWindowTarget(this IControl control)
        //        {
        //            return control.Get<IWindowTarget>("WindowTarget");
        //        }

        //#if STEPTHROUGHCONTROLEXTENSIONS
        //        [DebuggerStepThrough]
        //#endif
        //        public static IControlPropertyHandler GetWindowTargetHandler(this IControl control)
        //        {
        //            return control.Get<IControlPropertyHandler>("WindowTargetHandler");
        //        }

        //#if STEPTHROUGHCONTROLEXTENSIONS
        //        [DebuggerStepThrough]
        //#endif
        //        public static bool IsLoaded(this IControl control)
        //        {
        //            return control.Get<bool>("IsLoaded");
        //        }

        //#if STEPTHROUGHCONTROLEXTENSIONS
        //        [DebuggerStepThrough]
        //#endif
        //        public static bool IsCreated(this IControl control)
        //        {
        //            return control.Get<bool>("IsCreated");
        //        }

        //#if STEPTHROUGHCONTROLEXTENSIONS
        //        [DebuggerStepThrough]
        //#endif
        //        public static bool IsWindow(this IControl control)
        //        {
        //            return control.Get<bool>("IsWindow");
        //        }

        //#if STEPTHROUGHCONTROLEXTENSIONS
        //        [DebuggerStepThrough]
        //#endif
        //        public static void SetWindowTarget(this IControl control, IWindowTarget target)
        //        {
        //            control.Set("WindowTarget", target);
        //        }

        //#if STEPTHROUGHCONTROLEXTENSIONS
        //        [DebuggerStepThrough]
        //#endif
        //        public static Point GetLocation(this IControl control)
        //        {
        //            return control.Get<Point>("Location");
        //        }

        //#if STEPTHROUGHCONTROLEXTENSIONS
        //        [DebuggerStepThrough]
        //#endif
        //        public static Point ClientToScreen(this IControl control, Point point)
        //        {
        //            return control.Invoke<Point>("ClientToScreen", point);
        //        }

        //#if STEPTHROUGHCONTROLEXTENSIONS
        //        [DebuggerStepThrough]
        //#endif
        //        public static void SetRegion(this IControl control, Region region)
        //        {
        //            control.Set("Region", region);
        //        }

        //#if STEPTHROUGHCONTROLEXTENSIONS
        //        [DebuggerStepThrough]
        //#endif
        //        public static IVirtualParent GetVirtualParent(this IOverrideControl control)
        //        {
        //            return control.Get<IVirtualParent>("VirtualParent");
        //        }

        //#if STEPTHROUGHCONTROLEXTENSIONS
        //        [DebuggerStepThrough]
        //#endif
        //        public static void SetVirtualParent(this IOverrideControl control, IVirtualParent parent)
        //        {
        //            control.Set("VirtualParent", parent);
        //        }

        //#if STEPTHROUGHCONTROLEXTENSIONS
        //        [DebuggerStepThrough]
        //#endif
        //        public static Color GetBackColor(this IControl control)
        //        {
        //            return control.Get<Color>("BackColor");
        //        }

        //#if STEPTHROUGHCONTROLEXTENSIONS
        //        [DebuggerStepThrough]
        //#endif
        //        public static Color GetForeColor(this IControl control)
        //        {
        //            return control.Get<Color>("ForeColor");
        //        }

        //#if STEPTHROUGHCONTROLEXTENSIONS
        //        [DebuggerStepThrough]
        //#endif
        //        public static void SetBackColor(this IControl control, Color color)
        //        {
        //            control.Set("BackColor", color);
        //        }

        //#if STEPTHROUGHCONTROLEXTENSIONS
        //        [DebuggerStepThrough]
        //#endif
        //        public static void SetForeColor(this IControl control, Color color)
        //        {
        //            control.Set("ForeColor", color);
        //        }

        //#if STEPTHROUGHCONTROLEXTENSIONS
        //        [DebuggerStepThrough]
        //#endif
        //        public static int GetLeft(this IControl control)
        //        {
        //            return control.Get<int>("Left");
        //        }

        //#if STEPTHROUGHCONTROLEXTENSIONS
        //        [DebuggerStepThrough]
        //#endif
        //        public static int GetTop(this IControl control)
        //        {
        //            return control.Get<int>("Top");
        //        }

        //#if STEPTHROUGHCONTROLEXTENSIONS
        //        [DebuggerStepThrough]
        //#endif
        //        public static ISite GetSite(this IControl control)
        //        {
        //            return control.Get<ISite>("Site");
        //        }

        //#if STEPTHROUGHCONTROLEXTENSIONS
        //        [DebuggerStepThrough]
        //#endif
        //        public static void SetTag(this IControl control, object tag)
        //        {
        //            control.Set("Tag", tag);
        //        }

        //#if STEPTHROUGHCONTROLEXTENSIONS
        //        [DebuggerStepThrough]
        //#endif
        //        public static object GetTag(this IControl control)
        //        {
        //            return control.Get<object>("Tag");
        //        }

        //#if STEPTHROUGHCONTROLEXTENSIONS
        //        [DebuggerStepThrough]
        //#endif
        //        public static void SetSite(this IControl control, ISite site)
        //        {
        //            control.Set("Site", site);
        //        }

        //#if STEPTHROUGHCONTROLEXTENSIONS
        //        [DebuggerStepThrough]
        //#endif
        //        public static Size GetMinimumSize(this IControl control)
        //        {
        //            return control.Get<Size>("MinimumSize");
        //        }

        //#if STEPTHROUGHCONTROLEXTENSIONS
        //        [DebuggerStepThrough]
        //#endif
        //        public static Size GetSize(this IControl control)
        //        {
        //            return control.Get<Size>("Size");
        //        }

        //#if STEPTHROUGHCONTROLEXTENSIONS
        //        [DebuggerStepThrough]
        //#endif
        //        public static void SetSize(this IControl control, Size size)
        //        {
        //            control.Set("Size", size);
        //        }

        //#if STEPTHROUGHCONTROLEXTENSIONS
        //        [DebuggerStepThrough]
        //#endif
        //        public static void RemoveChildControl(this IControl control, IControl child)
        //        {
        //            control.Invoke("RemoveChildControl", child);
        //        }

        //#if STEPTHROUGHCONTROLEXTENSIONS
        //        [DebuggerStepThrough]
        //#endif
        //        public static bool HasChildren(this IControl control)
        //        {
        //            return control.GetChildControls().Count > 0;
        //        }

        //#if STEPTHROUGHCONTROLEXTENSIONS
        //        [DebuggerStepThrough]
        //#endif
        //        public static void AddChildControl(this IControl control, IControl child)
        //        {
        //            control.Invoke("AddChildControl", child);
        //        }

        //#if STEPTHROUGHCONTROLEXTENSIONS
        //        [DebuggerStepThrough]
        //#endif
        //        public static void SetChildIndex(this IControl control, IControl child, int index)
        //        {
        //            control.Invoke("SetChildIndex", child, index);
        //        }

        //#if STEPTHROUGHCONTROLEXTENSIONS
        //        [DebuggerStepThrough]
        //#endif
        //        public static IControl GetChildControlAtIndex(this IControl control, int index)
        //        {
        //            return control.Invoke<IControl>("GetChildControlAtIndex", index);
        //        }

        //#if STEPTHROUGHCONTROLEXTENSIONS
        //        [DebuggerStepThrough]
        //#endif
        //        public static void CopyChildControlsTo(this IControl control, IControl[] childControls, int index = 0)
        //        {
        //            control.Invoke("CopyChildControlsTo", childControls, index);
        //        }

        //#if STEPTHROUGHCONTROLEXTENSIONS
        //        [DebuggerStepThrough]
        //#endif
        //        public static List<IControl> GetChildControls(this IControl control)
        //        {
        //            return control.Get<List<IControl>>("ChildControls");
        //        }

        //#if STEPTHROUGHCONTROLEXTENSIONS
        //        [DebuggerStepThrough]
        //#endif
        //        public static int GetChildControlCount(this IControl control)
        //        {
        //            return control.Get<int>("ChildControlCount");
        //        }

        //#if STEPTHROUGHCONTROLEXTENSIONS
        //        [DebuggerStepThrough]
        //#endif
        //        public static void SetAutoScrollMargin(this IControl control, Size size)
        //        {
        //            control.Set("AutoMargin", size);
        //        }

        //#if STEPTHROUGHCONTROLEXTENSIONS
        //        [DebuggerStepThrough]
        //#endif
        //        public static bool IsShown(this IControl control)
        //        {
        //            return control.Get<bool>("IsShown");
        //        }

        //#if STEPTHROUGHCONTROLEXTENSIONS
        //        [DebuggerStepThrough]
        //#endif
        //        public static void IsShown(this IControl control, bool shown)
        //        {
        //            control.Set("IsShown", shown);
        //        }

        //#if STEPTHROUGHCONTROLEXTENSIONS
        //        [DebuggerStepThrough]
        //#endif
        //        public static List<object> GetDataBindings(this IControl control)
        //        {
        //            return control.Get<List<object>>("DataBindings");
        //        }

        //#if STEPTHROUGHCONTROLEXTENSIONS
        //        [DebuggerStepThrough]
        //#endif
        //        public static DockStyle GetDockStyle(this IControl control)
        //        {
        //            return control.Get<DockStyle>("DockStyle");
        //        }

        //#if STEPTHROUGHCONTROLEXTENSIONS
        //        [DebuggerStepThrough]
        //#endif
        //        public static bool ContainsChild(this IControl control, IControl child)
        //        {
        //            return control.Invoke<bool>("ContainsChild", child);
        //        }

        //#if STEPTHROUGHCONTROLEXTENSIONS
        //        [DebuggerStepThrough]
        //#endif
        //        public static void SetFont(this IControl control, Font font)
        //        {
        //            control.Invoke("SetFont", font);
        //        }

        //#if STEPTHROUGHCONTROLEXTENSIONS
        //        [DebuggerStepThrough]
        //#endif
        //        public static void SetCapture(this IControl control, bool capture = true)
        //        {
        //            control.Invoke("SetCapture", capture);
        //        }

        //#if STEPTHROUGHCONTROLEXTENSIONS
        //        [DebuggerStepThrough]
        //#endif
        //        public static Rect RectangleToClient(this IControl control, Rect rc)
        //        {
        //            return control.Invoke<Rect>("RectangleToClient", rc);
        //        }

        //#if STEPTHROUGHCONTROLEXTENSIONS
        //        [DebuggerStepThrough]
        //#endif
        //        public static object WndProc(this IControl control, Message m)
        //        {
        //            return control.Invoke<object>("WndProc", m);
        //        }

        //#if STEPTHROUGHCONTROLEXTENSIONS
        //        [DebuggerStepThrough]
        //#endif
        //        public static Rect RectangleToScreen(this IControl control, Rect rc)
        //        {
        //            return control.Invoke<Rect>("RectangleToScreen", rc);
        //        }

        //#if STEPTHROUGHCONTROLEXTENSIONS
        //        [DebuggerStepThrough]
        //#endif
        //        public static Rect GetClientRectangle(this IControl control)
        //        {
        //            return control.Get<Rect>("ClientRectangle");
        //        }

        //#if STEPTHROUGHCONTROLEXTENSIONS
        //        [DebuggerStepThrough]
        //#endif
        //        public static Rect GetWindowRect(this IControl control)
        //        {
        //            return control.Get<Rect>("WindowRect");
        //        }

        //#if STEPTHROUGHCONTROLEXTENSIONS
        //        [DebuggerStepThrough]
        //#endif
        //        public static Point PointToClient(this IControl control, Point point)
        //        {
        //            return control.Invoke<Point>("PointToClient", point);
        //        }

        //#if STEPTHROUGHCONTROLEXTENSIONS
        //        [DebuggerStepThrough]
        //#endif
        //        public static Point PointToScreen(this IControl control, Point point)
        //        {
        //            return control.Invoke<Point>("PointToScreen", point);
        //        }

        //#if STEPTHROUGHCONTROLEXTENSIONS
        //        [DebuggerStepThrough]
        //#endif
        //        public static Rect PointToClient(this IControl control, Rect rect)
        //        {
        //            return control.Invoke<Rect>("PointToClient", rect);
        //        }

        //        public static GraphicsMode GetGraphicsMode(this UIElement element)
        //        {
        //            if (element is IControl)
        //            {
        //                var iControl = (IControl)element;

        //                return iControl.GraphicsMode;
        //            }

        //            return GraphicsMode.None;
        //        }

        //        public static DialogResult ShowDialog(this FrameworkElement element, FrameworkElement owner)
        //        {
        //            throw new NotImplementedException();
        //        }

        //        public static void Clip(this Cursor control, Rect rc)
        //        {
        //            // kn - todo
        //        }

        public static DependencyObject GetParent(this DependencyObject obj)
        {
            //if (!Globals.UIDispatcher.CheckAccess())
            //{
            //    return ScheduleCall<DependencyObject>(() => GetParent(obj));
            //}

            return VisualTreeHelper.GetParent(obj);
        }

        //        public static bool HasFocus(this FrameworkElement control)
        //        {
        //            return control == FocusManager.GetFocusedElement();
        //        }

        //        public static void SetFocus(this Control control)
        //        {
        //            control.Focus();
        //        }

        //        public static UIElement GetActiveControl(this FrameworkElement control)
        //        {
        //            return (UIElement) FocusManager.GetFocusedElement();
        //        }

        //        public static bool ContainsFocus(this FrameworkElement control)
        //        {
        //            return control.GetChildren().OfType<FrameworkElement>().Any(c => c.HasFocus());
        //        }

        //        public static DependencyObject GetRoot(this DependencyObject obj)
        //        {
        //            return VisualTreeHelper.GetRoot(obj);
        //        }

        //        public static int KeyState(this Microsoft.Windows.DragEventArgs args)
        //        {
        //            Debugger.Break();

        //            return 0;
        //        }

        //        public static DragDropKeyStates ToDragDrop(this ModifierKeys keys) 
        //        {
        //            var dragDropKeyStates = DragDropKeyStates.None;

        //            if (keys.HasFlag(ModifierKeys.Control))
        //            {
        //                dragDropKeyStates |= DragDropKeyStates.ControlKey;
        //            }

        //            if (keys.HasFlag(ModifierKeys.Alt))
        //            {
        //                dragDropKeyStates |= DragDropKeyStates.AltKey;
        //            }

        //            if (keys.HasFlag(ModifierKeys.Shift))
        //            {
        //                dragDropKeyStates |= DragDropKeyStates.ShiftKey;
        //            }

        //            return dragDropKeyStates;
        //        }

        //        public static void ScheduleCall(Action call)
        //        {
        //            var resetEvent = new ManualResetEvent(false);

        //            Globals.UIDispatcher.BeginInvoke(() =>
        //            {
        //                call();

        //                resetEvent.Set();
        //            });

        //            if (!resetEvent.WaitOne(TimeSpan.FromSeconds(5)))
        //            {
        //                if (!Debugger.IsAttached)
        //                {
        //                    throw new TimeoutException(string.Format("Scheduled call times out, call info: {0}", call.Method.ToString()));
        //                }
        //            }
        //        }

        //        public static T ScheduleCall<T>(Func<T> call)
        //        {
        //            var resetEvent = new ManualResetEvent(false);
        //            T result = default(T);

        //            Globals.UIDispatcher.BeginInvoke(() =>
        //            {
        //                result = call();

        //                resetEvent.Set();
        //            });

        //            if (!resetEvent.WaitOne(TimeSpan.FromSeconds(5)))
        //            {
        //                if (!Debugger.IsAttached)
        //                {
        //                    throw new TimeoutException(string.Format("Scheduled call times out, call info: {0}", call.Method.ToString()));
        //                }
        //            }

        //            return result;
        //        }

        //        public static bool IsShown(this FrameworkElement control)
        //        {
        //            if (!Globals.UIDispatcher.CheckAccess())
        //            {
        //                return ScheduleCall<bool>(() => IsShown(control));
        //            }

        //            var visible = control.Visibility == Visibility.Visible && (control == Application.Current.RootVisual || control.Parent != null) && control.ActualHeight > 0 && control.ActualWidth > 0;

        //            if (visible)
        //            {
        //                visible = control.IsInVisualTree() && control.IsVisible();
        //            }

        //            return visible;
        //        }

        //        public static ImageCaptureOperation CaptureImage(this IControl element, int x, int y, int width, int height)
        //        {
        //            return element.CaptureImage(new Rect(x, y, width, height));
        //        }

        //        public static ImageCaptureOperation CaptureImage(this IControl element)
        //        {
        //            return element.CaptureImage(element.GetRect());
        //        }

        //        public static ImageCaptureOperation CaptureImage(this IControl control, Rect rect)
        //        {
        //            var bitmap = new WriteableBitmap((int)rect.Width, (int)rect.Height);
        //            var operation = control.RenderToBitmap(bitmap, rect);

        //            return operation;
        //        }

        //#if STEPTHROUGHCONTROLEXTENSIONS
        //        [DebuggerStepThrough]
        //#endif
        //        public static ImageCaptureOperation RenderToBitmap(this IControl control, WriteableBitmap bitmap, Rect rect)
        //        {
        //            if (control is FrameworkElement)
        //            {
        //                var element = (FrameworkElement)control;
        //                var transform = new TranslateTransform();
        //                var operation = new ImageCaptureOperation(bitmap);

        //                transform.X = -rect.Left;
        //                transform.Y = -rect.Top;

        //                bitmap.Render(element, transform);

        //                bitmap.Invalidate();

        //                operation.SetDelayed(new UIThread(Globals.CurrentRootVisual.Dispatcher), bitmap);

        //                return operation;
        //            }
        //            else
        //            {
        //                return control.Invoke<ImageCaptureOperation>("RenderToBitmap", bitmap, rect);
        //            }
        //        }

        //        public static WriteableBitmap CaptureImage(this FrameworkElement element, Rect rect)
        //        {
        //            var bitmap = new WriteableBitmap((int)rect.Width, (int)rect.Height);
        //            var transform = new TranslateTransform();

        //            transform.X = -rect.Left;
        //            transform.Y = -rect.Top;

        //            bitmap.Render(element, transform);

        //            bitmap.Invalidate();

        //            return bitmap;
        //        }

        //        public static WriteableBitmap CaptureImage(this FrameworkElement element, int x, int y, int width, int height)
        //        {
        //            return element.CaptureImage(new Rect(x, y, width, height));
        //        }

        //        public static WriteableBitmap CaptureImage(this FrameworkElement element)
        //        {
        //            return element.CaptureImage(element.GetRect());
        //        }

        //        public static Drawing.Graphics CreateGraphics(this FrameworkElement control)
        //        {
        //            return new Drawing.Graphics(control);
        //        }

        //        public static List<FrameworkElement> FilterToTopLevel(this IEnumerable<FrameworkElement> elements, Func<FrameworkElement, FrameworkElement, bool> childFilter)
        //        {
        //            return elements.Cast<UIElement>().FilterToTopLevel(childFilter);
        //        }

        //        public static List<FrameworkElement> FilterToTopLevel(this IEnumerable<FrameworkElement> elements)
        //        {
        //            return elements.Cast<UIElement>().FilterToTopLevel(null);
        //        }

        //        public static List<FrameworkElement> FilterToTopLevel(this IEnumerable<UIElement> elements, Func<FrameworkElement, FrameworkElement, bool> childFilter)
        //        {
        //            return elements.FilterToTopLevel(null, childFilter);
        //        }

        //        public static List<FrameworkElement> FilterToTopLevel(this IEnumerable<UIElement> elements, UIElement rootElement, Func<FrameworkElement, FrameworkElement, bool> childFilter = null)
        //        {
        //            var topLevelElements = new List<FrameworkElement>();
        //            var topLevelChildElements = new List<FrameworkElement>();

        //            // add top-level elements

        //            foreach (var topLevelElement in elements.OfType<FrameworkElement>())
        //            {
        //                if (rootElement != null && topLevelElement != rootElement)
        //                {
        //                    if (!rootElement.GetAncestors().OfType<FrameworkElement>().Any(a => a == topLevelElement))
        //                    {
        //                        if (!rootElement.GetDescendants().OfType<FrameworkElement>().Any(a => a == topLevelElement))
        //                        {
        //                            topLevelElements.Add(topLevelElement);
        //                        }
        //                    }
        //                }
        //                else
        //                {
        //                    topLevelElements.Add(topLevelElement);
        //                }
        //            }

        //            // weed out child elements belonging to same

        //            if (childFilter != null)
        //            {
        //                foreach (var topLevelElement in topLevelElements)
        //                {
        //                    if (topLevelElements.Any(e => e != topLevelElement && topLevelElement.GetAncestors().Any(a => a == e && childFilter(e, topLevelElement))))
        //                    {
        //                        topLevelChildElements.Add(topLevelElement);
        //                    }
        //                }
        //            }
        //            else
        //            {
        //                foreach (var topLevelElement in topLevelElements)
        //                {
        //                    if (topLevelElements.Any(e => e != topLevelElement && topLevelElement.GetAncestors().Any(a => a == e)))
        //                    {
        //                        topLevelChildElements.Add(topLevelElement);
        //                    }
        //                }
        //            }

        //            foreach (var childElement in topLevelChildElements)
        //            {
        //                topLevelElements.Remove(childElement);
        //            }

        //            return topLevelElements;
        //        }

        //        public static Point PointToScreen(this UIElement control, Point point)
        //        {
        //            if (!Globals.UIDispatcher.CheckAccess())
        //            {
        //                return ScheduleCall<Point>(() => PointToScreen(control, point));
        //            }

        //            var rootVisual = Application.Current.RootVisual;

        //            point = control.TransformToVisual(rootVisual).Transform(point);

        //            return point;
        //        }

        //        public static object SendMessage(this FrameworkElement control, WindowMessage msg, object wParam, object lParam)
        //        {
        //            return Globals.SendMessage(control, msg, wParam, lParam);
        //        }

        //        public static Point PointToClient(this UIElement control, Point point)
        //        {
        //            if (!Globals.UIDispatcher.CheckAccess())
        //            {
        //                return ScheduleCall<Point>(() => PointToClient(control, point));
        //            }

        //            var rootVisual = Application.Current.RootVisual;

        //            point = rootVisual.TransformToVisual(control).Transform(point);

        //            return point;
        //        }

        //        public static Rect RectangleToScreen(this FrameworkElement control, Rect rc)
        //        {
        //            if (!Globals.UIDispatcher.CheckAccess())
        //            {
        //                return ScheduleCall<Rect>(() => RectangleToScreen(control, rc));
        //            }

        //            var rect = new Rect();

        //            if (control.IsShown())
        //            {
        //                rect = control.TransformToVisual(null).TransformBounds(rc);
        //            }

        //            return rect;
        //        }

        //        public static Rect RectangleToClient(this Panel control, Rect rc)
        //        {
        //            if (!Globals.UIDispatcher.CheckAccess())
        //            {
        //                return ScheduleCall<Rect>(() => RectangleToClient(control, rc));
        //            }

        //            var rect = new Rect();

        //            if (control.IsShown())
        //            {
        //                var rootVisual = Application.Current.RootVisual;

        //                rect = rootVisual.TransformToVisual(control).TransformBounds(rc);
        //            }

        //            return rect;
        //        }

        //        public static Rect RectangleToClient(this FrameworkElement control, Rect rc)
        //        {
        //            if (!Globals.UIDispatcher.CheckAccess())
        //            {
        //                return ScheduleCall<Rect>(() => RectangleToClient(control, rc));
        //            }

        //            var rect = new Rect();

        //            if (control.IsShown())
        //            {
        //                var rootVisual = Application.Current.RootVisual;

        //                rect = rootVisual.TransformToVisual(control).TransformBounds(rc);
        //            }

        //            return rect;
        //        }

        //        public static IEnumerable<UIElement> FindElementsInHostCoordinates(this UIElement subtree, Point intersectingPoint)
        //        {
        //            if (!Globals.UIDispatcher.CheckAccess())
        //            {
        //                return ScheduleCall<IEnumerable<UIElement>>(() => FindElementsInHostCoordinates(subtree, intersectingPoint));
        //            }

        //            return VisualTreeHelper.FindElementsInHostCoordinates(intersectingPoint, subtree);
        //        }

        //        public static IEnumerable<UIElement> FindElementsInHostCoordinates(this Window window, Point intersectingPoint)
        //        {
        //            if (!Globals.UIDispatcher.CheckAccess())
        //            {
        //                return ScheduleCall<IEnumerable<UIElement>>(() => FindElementsInHostCoordinates(window, intersectingPoint));
        //            }

        //            return VisualTreeHelper.FindElementsInHostCoordinates(intersectingPoint, window);
        //        }

        //        public static IEnumerable<UIElement> FindElementsInHostCoordinates(this FrameworkElement subtree, Rect intersectingRect)
        //        {
        //            var elements = new List<UIElement>();

        //            using (var stopwatch = new UIRelationStopwatch(subtree))
        //            {
        //                if (!UIRelationCacheEngine.GetElementsInHostCoordinates(subtree, intersectingRect, ref elements))
        //                {
        //                    elements = VisualTreeHelper.FindElementsInHostCoordinates(intersectingRect, subtree).ToList();

        //                    if (!stopwatch.Reentrant)
        //                    {
        //                        UIRelationCacheEngine.SetElementsInHostCoordinates(subtree, intersectingRect, elements);
        //                    }
        //                }
        //            }

        //            return elements;            
        //        }

        //        public static IEnumerable<UIElement> FindElementsInHostCoordinates(this Window window, Rect intersectingRect)
        //        {
        //            if (!Globals.UIDispatcher.CheckAccess())
        //            {
        //                return ScheduleCall<IEnumerable<UIElement>>(() => FindElementsInHostCoordinates(window, intersectingRect));
        //            }

        //            return VisualTreeHelper.FindElementsInHostCoordinates(intersectingRect, window);
        //        }

        //        public static Rect GetClientRectangle(this FrameworkElement control)
        //        {
        //            return new Rect(new Point(0, 0), new Size(control.ActualWidth, control.ActualHeight));
        //        }

        //        public static Cursor GetCursor(this FrameworkElement control)
        //        {
        //            return new Cursor(control.Cursor);
        //        }

        //        public static Rect GetRect(this IControl control)
        //        {
        //            if (!Globals.UIDispatcher.CheckAccess())
        //            {
        //                return ScheduleCall<Rect>(() => GetRect(control));
        //            }

        //            var rect = new Rect();

        //            if (control.IsShown())
        //            {
        //                rect = control.GetClientRectangle();
        //            }

        //            return rect;
        //        }

        //        public static Rect GetRect(this FrameworkElement control)
        //        {
        //            if (!Globals.UIDispatcher.CheckAccess())
        //            {
        //                return ScheduleCall<Rect>(() => GetRect(control));
        //            }

        //            var rect = new Rect();

        //            if (control.IsShown())
        //            {
        //                var parent = (FrameworkElement)control.Parent;
        //                var position = control.TransformToVisual(parent).Transform(new Point());
        //                rect = new Rect(position.X, position.Y, control.ActualWidth, control.ActualHeight);
        //            }

        //            return rect;
        //        }

        //        public static Point GetWindowPosition(this FrameworkElement control)
        //        {
        //            if (!Globals.UIDispatcher.CheckAccess())
        //            {
        //                return ScheduleCall<Point>(() => GetWindowPosition(control));
        //            }

        //            var position = new Point();

        //            if (control.IsShown())
        //            {
        //                position = control.TransformToVisual(null).Transform(new Point());
        //            }

        //            return position;
        //        }

        //        public static Point GetLocation(this FrameworkElement control)
        //        {
        //            if (!Globals.UIDispatcher.CheckAccess())
        //            {
        //                return ScheduleCall<Point>(() => GetLocation(control));
        //            }

        //            var position = new Point();

        //            if (control.IsShown())
        //            {
        //                var parent = (UIElement) control.GetParent();

        //                position = control.TransformToVisual(parent).Transform(new Point());
        //            }

        //            return position;
        //        }

        //        public static bool HasControl(this KeyEventArgs e)
        //        {
        //            return (e.Key & Key.Ctrl) == Key.Ctrl;
        //        }

        //        public static bool HasShift(this KeyEventArgs e)
        //        {
        //            return (e.Key & Key.Shift) == Key.Shift;
        //        }

        //        public static bool HasAlt(this KeyEventArgs e)
        //        {
        //            return (e.Key & Key.Alt) == Key.Alt;
        //        }

        //        public static Key GetKeyCodeMask()
        //        {
        //            return (Key) 65535;
        //        }

        //        public static Key GetKeyCodeMask(this Key key)
        //        {
        //            return GetKeyCodeMask();
        //        }

        //        public static char KeyChar(this KeyEventArgs e)
        //        {
        //            var args = Globals.GetLastKeyPressEventArgs();

        //            return args.KeyChar;
        //        }

        //        public static void SetCapture(this FrameworkElement control, bool capture = true)
        //        {
        //            if (capture)
        //            {
        //                Globals.SetCapture(control);
        //            }
        //            else
        //            {
        //                Globals.ReleaseCapture();
        //            }
        //        }

        //        public static Rect GetWindowRect(this FrameworkElement control)
        //        {
        //            if (!Globals.UIDispatcher.CheckAccess())
        //            {
        //                return ScheduleCall<Rect>(() => GetWindowRect(control));
        //            }

        //            var rect = new Rect();

        //            if (control.IsShown())
        //            {
        //                var position = control.TransformToVisual(null).Transform(new Point());
        //                rect = new Rect(position.X, position.Y, control.ActualWidth, control.ActualHeight);
        //            }

        //            return rect;
        //        }

        //        public static Rect GetBounds(this FrameworkElement control)
        //        {
        //            if (!Globals.UIDispatcher.CheckAccess())
        //            {
        //                return ScheduleCall<Rect>(() => GetBounds(control));
        //            }

        //            var location = control.GetLocation();
        //            var rect = new Rect(location.X, location.Y, control.ActualWidth, control.ActualHeight);

        //            return rect;
        //        }

        //        public static Size GetSize(this FrameworkElement control)
        //        {
        //            if (!Globals.UIDispatcher.CheckAccess())
        //            {
        //                return ScheduleCall<Size>(() => GetSize(control));
        //            }

        //            var size = new Size(control.ActualWidth, control.ActualHeight);

        //            return size;
        //        }

        //        public static void SetWidthInGrid(this FrameworkElement control, Grid parentGrid)
        //        {
        //            var column = (int)control.GetValue(Grid.ColumnProperty);
        //            ColumnDefinition columnDefinition;

        //            if (parentGrid.ColumnDefinitions.Count > column)
        //            {
        //                columnDefinition = parentGrid.ColumnDefinitions[column];

        //                if (!double.IsNaN(columnDefinition.ActualWidth))
        //                {
        //                    if (control.Width != columnDefinition.ActualWidth)
        //                    {
        //                        control.Width = columnDefinition.ActualWidth;
        //                    }
        //                }
        //                else if (columnDefinition.Width.GridUnitType == GridUnitType.Pixel)
        //                {
        //                    if (control.Width != columnDefinition.Width.Value)
        //                    {
        //                        control.Width = columnDefinition.Width.Value;
        //                    }
        //                }
        //                else
        //                {
        //                    Debugger.Break();
        //                }
        //            }
        //            else
        //            {
        //                if (!double.IsNaN(parentGrid.ActualWidth))
        //                {
        //                    if (control.Width != parentGrid.ActualWidth)
        //                    {
        //                        control.Width = parentGrid.ActualWidth;
        //                    }
        //                }
        //                else if (!double.IsNaN(parentGrid.Width))
        //                {
        //                    Debugger.Break();
        //                }
        //                else
        //                {
        //                    var parent = parentGrid.Parent;
        //                    EventHandler layoutUpdated = null;

        //                    layoutUpdated = (sender, e) =>
        //                    {
        //                        parent = control.GetParent();

        //                        if (parent != null)
        //                        {
        //                            control.LayoutUpdated -= layoutUpdated;
        //                            SetWidthInGrid(control, parentGrid);
        //                        }
        //                    };

        //                    control.LayoutUpdated += layoutUpdated;
        //                }
        //            }
        //        }

        //        public static void AttachToBottomInGrid(this FrameworkElement control, Grid parentGrid)
        //        {
        //            var row = (int)control.GetValue(Grid.RowProperty);
        //            RowDefinition rowDefinition;

        //            if (parentGrid.RowDefinitions.Count > row)
        //            {
        //                rowDefinition = parentGrid.RowDefinitions[row];

        //                if (!double.IsNaN(rowDefinition.ActualHeight))
        //                {
        //                    if (control.VerticalAlignment != VerticalAlignment.Bottom)
        //                    {
        //                        control.VerticalAlignment = VerticalAlignment.Bottom;
        //                    }
        //                }
        //                else if (rowDefinition.Height.GridUnitType == GridUnitType.Pixel)
        //                {
        //                    if (control.Height != rowDefinition.Height.Value)
        //                    {
        //                        control.Height = rowDefinition.Height.Value;
        //                    }
        //                }
        //                else
        //                {
        //                    Debugger.Break();
        //                }
        //            }
        //            else
        //            {
        //                if (double.IsNaN(parentGrid.Height))
        //                {
        //                    var parent = parentGrid.Parent;
        //                    EventHandler layoutUpdated = null;

        //                    layoutUpdated = (sender, e) =>
        //                    {
        //                        parent = control.GetParent();

        //                        if (parent != null)
        //                        {
        //                            control.LayoutUpdated -= layoutUpdated;
        //                            AttachToBottomInGrid(control, parentGrid);
        //                        }
        //                    };

        //                    control.LayoutUpdated += layoutUpdated;
        //                }
        //                else
        //                {
        //                    Debugger.Break();
        //                }
        //            }
        //        }

        //        public static void SetHeightInGrid(this FrameworkElement control, Grid parentGrid)
        //        {
        //            var row = (int)control.GetValue(Grid.RowProperty);
        //            RowDefinition rowDefinition;

        //            if (parentGrid.RowDefinitions.Count > row)
        //            {
        //                rowDefinition = parentGrid.RowDefinitions[row];

        //                if (!double.IsNaN(rowDefinition.ActualHeight))
        //                {
        //                    if (control.Height != rowDefinition.ActualHeight)
        //                    {
        //                        control.Height = rowDefinition.ActualHeight;
        //                    }
        //                }
        //                else if (rowDefinition.Height.GridUnitType == GridUnitType.Pixel)
        //                {
        //                    if (control.Height != rowDefinition.Height.Value)
        //                    {
        //                        control.Height = rowDefinition.Height.Value;
        //                    }
        //                }
        //                else
        //                {
        //                    Debugger.Break();
        //                }
        //            }
        //            else
        //            {
        //                if (!double.IsNaN(parentGrid.ActualHeight))
        //                {
        //                    if (control.Height != parentGrid.ActualHeight)
        //                    {
        //                        control.Height = parentGrid.ActualHeight;
        //                    }
        //                }
        //                else if (!double.IsNaN(parentGrid.Height))
        //                {
        //                    Debugger.Break();
        //                }
        //                else
        //                {
        //                    var parent = parentGrid.Parent;
        //                    EventHandler layoutUpdated = null;

        //                    layoutUpdated = (sender, e) =>
        //                    {
        //                        parent = control.GetParent();

        //                        if (parent != null)
        //                        {
        //                            control.LayoutUpdated -= layoutUpdated;
        //                            SetHeightInGrid(control, parentGrid);
        //                        }
        //                    };

        //                    control.LayoutUpdated += layoutUpdated;
        //                }
        //            }
        //        }

        //        public static void SetDock(this FrameworkElement control, DockStyle dock)
        //        {
        //            var parent = control.GetParent();
        //            var stack = control.GetStackText(25);

        //            switch (dock)
        //            {
        //                case DockStyle.None:
        //                    break;
        //                case DockStyle.Bottom:

        //                    if (parent == null)
        //                    {
        //                        EventHandler layoutUpdated = null;

        //                        layoutUpdated = (sender, e) =>
        //                        {
        //                            parent = control.GetParent();

        //                            if (parent != null)
        //                            {
        //                                control.LayoutUpdated -= layoutUpdated;
        //                                SetDock(control, dock);
        //                            }
        //                        };

        //                        control.LayoutUpdated += layoutUpdated;
        //                    }
        //                    else if (parent is Control)
        //                    {
        //                        var parentControl = (Control)parent;

        //                        if (parentControl.HorizontalContentAlignment != HorizontalAlignment.Stretch)
        //                        {
        //                            parentControl.HorizontalContentAlignment = HorizontalAlignment.Stretch;
        //                        }
        //                    }
        //                    else if (parent is Canvas)
        //                    {
        //                        var parentCanvas = (Canvas)parent;

        //                        if (parentCanvas.Children.Count > 0)
        //                        {
        //                            Debugger.Break();
        //                        }

        //                        var width = control.GetValue(Canvas.WidthProperty).As<double>();
        //                        var height = control.GetValue(Canvas.HeightProperty).As<double>();

        //                        if (width != parentCanvas.ActualWidth)
        //                        {
        //                            control.SetValue(Canvas.WidthProperty, parentCanvas.ActualWidth);
        //                        }

        //                        if (height != parentCanvas.ActualHeight)
        //                        {
        //                            control.SetValue(Canvas.HeightProperty, parentCanvas.ActualHeight);
        //                        }
        //                    }
        //                    else if (parent is Grid)
        //                    {
        //                        var parentGrid = (Grid)parent;

        //                        control.SetWidthInGrid(parentGrid);
        //                        control.AttachToBottomInGrid(parentGrid);
        //                    }
        //                    else
        //                    {
        //                        Debugger.Break();
        //                    }

        //                    if (control is Control)
        //                    {
        //                        var uiControl = control.As<Control>();

        //                        if (uiControl.HorizontalAlignment != HorizontalAlignment.Stretch)
        //                        {
        //                            uiControl.HorizontalAlignment = HorizontalAlignment.Stretch;
        //                        }

        //                        if (uiControl.VerticalAlignment != VerticalAlignment.Stretch)
        //                        {
        //                            uiControl.VerticalAlignment = VerticalAlignment.Stretch;
        //                        }
        //                    }

        //                    break;

        //                case DockStyle.Fill:

        //                    if (parent == null)
        //                    {
        //                        EventHandler layoutUpdated = null;

        //                        layoutUpdated = (sender, e) =>
        //                        {
        //                            parent = control.GetParent();

        //                            if (parent != null)
        //                            {
        //                                control.LayoutUpdated -= layoutUpdated;
        //                                SetDock(control, dock);
        //                            }
        //                        };

        //                        control.LayoutUpdated += layoutUpdated;
        //                    }
        //                    else if (parent is Control)
        //                    {
        //                        var uiParentControl = parent.As<Control>();

        //                        if (uiParentControl.HorizontalContentAlignment != HorizontalAlignment.Stretch)
        //                        {
        //                            uiParentControl.HorizontalContentAlignment = HorizontalAlignment.Stretch;
        //                        }

        //                        if (uiParentControl.VerticalContentAlignment != VerticalAlignment.Stretch)
        //                        {
        //                            uiParentControl.VerticalContentAlignment = VerticalAlignment.Stretch;
        //                        }
        //                    }
        //                    else if (parent is Canvas)
        //                    {
        //                        var parentCanvas = (Canvas)parent;

        //                        var width = control.ActualWidth;
        //                        var height = control.ActualHeight;

        //                        if (width != parentCanvas.ActualWidth)
        //                        {
        //                            control.SetValue(Canvas.WidthProperty, parentCanvas.ActualWidth);
        //                            control.Width = parentCanvas.ActualWidth;
        //                        }

        //                        if (height != parentCanvas.ActualHeight)
        //                        {
        //                            control.SetValue(Canvas.HeightProperty, parentCanvas.ActualHeight);
        //                            control.Height = parentCanvas.ActualHeight;
        //                        }

        //                        EventHandler layoutUpdated = null;

        //                        layoutUpdated = (sender, e) =>
        //                        {
        //                            parentCanvas.LayoutUpdated -= layoutUpdated;
        //                            control.SetDock(dock);
        //                        };

        //                        parentCanvas.LayoutUpdated += layoutUpdated;
        //                    }
        //                    else if (parent is Grid)
        //                    {
        //                        var parentGrid = (Grid)parent;
        //                        EventHandler layoutUpdated = null;

        //                        layoutUpdated = (sender, e) =>
        //                        {
        //                            parent = control.GetParent();

        //                            if (parent != null)
        //                            {
        //                                control.LayoutUpdated -= layoutUpdated;
        //                                SetDock(control, dock);
        //                            }
        //                        };

        //                        control.LayoutUpdated += layoutUpdated;

        //                        control.SetWidthInGrid(parentGrid);
        //                        control.SetHeightInGrid(parentGrid);
        //                    }
        //                    else if (parent is ContentPresenter)
        //                    {
        //                        var presenter = (ContentPresenter)parent;

        //                        if (presenter is ScrollContentPresenter)
        //                        {
        //                            var scrollPresenter = (ScrollContentPresenter)presenter;
        //                            var scrollViewer = scrollPresenter.ScrollOwner;
        //                            var width = scrollViewer.ActualWidth;
        //                            var height = scrollViewer.ActualHeight;
        //                            var contentPresenter = (FrameworkElement)presenter.GetAncestors().First(a => a is ContentPresenter);
        //                            var presenterParent = (FrameworkElement) contentPresenter.GetParent();

        //                            EventHandler layoutUpdated = null;

        //                            layoutUpdated = (sender, e) =>
        //                            {
        //                                presenterParent.LayoutUpdated -= layoutUpdated;
        //                                control.SetDock(dock);
        //                            };

        //                            presenterParent.LayoutUpdated += layoutUpdated;

        //                            if (width != presenterParent.ActualWidth)
        //                            {
        //                                scrollViewer.Width = presenterParent.ActualWidth;
        //                            }

        //                            if (height != presenterParent.ActualHeight)
        //                            {
        //                                scrollViewer.Height = presenterParent.ActualHeight;
        //                            }
        //                        }
        //                        else
        //                        {
        //                            var presenterParent = (FrameworkElement)presenter.GetParent();
        //                            var parentContentControl = presenterParent.GetAncestors().OfType<ContentControl>().FirstOrDefault();

        //                            if (parentContentControl == null)
        //                            {
        //                                // think we can safely ignore, not sure why this happens

        //                                return;
        //                            }

        //                            var width = presenter.ActualWidth;
        //                            var height = presenter.ActualHeight;

        //                            if (parentContentControl.HorizontalContentAlignment != HorizontalAlignment.Stretch)
        //                            {
        //                                parentContentControl.HorizontalContentAlignment = HorizontalAlignment.Stretch;
        //                            }

        //                            if (parentContentControl.VerticalContentAlignment != VerticalAlignment.Stretch)
        //                            {
        //                                parentContentControl.VerticalContentAlignment = VerticalAlignment.Stretch;
        //                            }

        //                            if (width != presenterParent.ActualWidth)
        //                            {
        //                                presenter.Width = presenterParent.ActualWidth;

        //                                control.Width = presenterParent.ActualWidth;
        //                                control.SetValue(ContentPresenter.WidthProperty, presenter.ActualWidth);
        //                                control.SetValue(Canvas.LeftProperty, 0.0);
        //                            }

        //                            if (height != presenterParent.ActualHeight)
        //                            {
        //                                presenter.Height = presenterParent.ActualHeight;

        //                                control.Height = presenterParent.ActualHeight;
        //                                control.SetValue(ContentPresenter.HeightProperty, presenter.ActualHeight);
        //                                control.SetValue(Canvas.TopProperty, 0.0);
        //                            }

        //                            EventHandler layoutUpdated = null;

        //                            layoutUpdated = (sender, e) =>
        //                            {
        //                                presenter.LayoutUpdated -= layoutUpdated;
        //                                control.SetDock(dock);
        //                            };

        //                            presenter.LayoutUpdated += layoutUpdated;
        //                        }
        //                    }
        //                    else
        //                    {
        //                        Debugger.Break();
        //                    }

        //                    if (control is Control)
        //                    {
        //                        var uiControl = control.As<Control>();

        //                        if (uiControl.HorizontalAlignment != HorizontalAlignment.Stretch)
        //                        {
        //                            uiControl.HorizontalAlignment = HorizontalAlignment.Stretch;
        //                        }

        //                        if (uiControl.VerticalAlignment != VerticalAlignment.Stretch)
        //                        {
        //                            uiControl.VerticalAlignment = VerticalAlignment.Stretch;
        //                        }
        //                    }

        //                    break;

        //                default:
        //                    Debugger.Break();
        //                    break;
        //            }
        //        }

        //        public static int X(this Microsoft.Windows.DragEventArgs args)
        //        {
        //            return (int)args.GetPosition((FrameworkElement)args.OriginalSource).X;
        //        }

        //        public static int Y(this Microsoft.Windows.DragEventArgs args)
        //        {
        //            return (int)args.GetPosition((FrameworkElement)args.OriginalSource).Y;
        //        }

        //        public static int X(this System.Windows.Input.MouseButtonEventArgs args)
        //        {
        //            return (int)args.GetPosition((FrameworkElement)args.OriginalSource).X;
        //        }

        //        public static int Y(this System.Windows.Input.MouseButtonEventArgs args)
        //        {
        //            return (int)args.GetPosition((FrameworkElement)args.OriginalSource).Y;
        //        }

        //        public static int X(this System.Windows.Input.MouseEventArgs args)
        //        {
        //            return (int)args.GetPosition((FrameworkElement)args.OriginalSource).X;
        //        }

        //        public static int Y(this System.Windows.Input.MouseEventArgs args)
        //        {
        //            return (int)args.GetPosition((FrameworkElement)args.OriginalSource).Y;
        //        }

        //        public static IEnumerable<FrameworkElement> AppendVirtual(this IEnumerable<FrameworkElement> elements, Rect screenRect)
        //        {
        //            var virtualElements = new List<FrameworkElement>();

        //            foreach (var element in elements.OfType<IControl>())
        //            {
        //                var descendants = element.GetDescendants().OfType<FrameworkElement>().Where(d => screenRect.IntersectsWith(d.GetWindowRect()) && !virtualElements.Any(e => e == d) && !elements.Any(e => e == d));

        //                virtualElements.AddRange(descendants);
        //            }

        //            return elements.Concat(virtualElements);
        //        }

        //        public static IControl FindDescendant(this IControl control, Func<IControl, bool> filter)
        //        {
        //            if (control is IVirtualParent)
        //            {
        //                var virtualParent = (IVirtualParent)control;
        //                var child = virtualParent.Child;

        //                if (filter(child))
        //                {
        //                    return child;
        //                }

        //                var descendant = FindDescendant(child, filter);

        //                if (descendant != null)
        //                {
        //                    return descendant;
        //                }
        //            }

        //            foreach (var child in control.GetChildControls())
        //            {
        //                if (filter(child))
        //                {
        //                    return child;
        //                }

        //                var descendant = FindDescendant(child, filter);

        //                if (descendant != null)
        //                {
        //                    return descendant;
        //                }
        //            }

        //            return null;
        //        }

        //        public static IEnumerable<IControl> GetDescendants(this IControl control, Func<IControl, bool> filter)
        //        {
        //            var descendants = new List<IControl>();

        //            control.AddDescendants(descendants, filter);

        //            return descendants;
        //        }

        //        private static void AddDescendants(this IControl control, List<IControl> list, Func<IControl, bool> filter)
        //        {
        //            if (control is IVirtualParent)
        //            {
        //                var virtualParent = (IVirtualParent)control;
        //                var child = virtualParent.Child;

        //                if (filter(child))
        //                {
        //                    list.Add(child);
        //                }

        //                child.AddDescendants(list, filter);
        //            }

        //            foreach (var child in control.GetChildControls())
        //            {
        //                if (filter(child))
        //                {
        //                    list.Add(child);
        //                }

        //                child.AddDescendants(list, filter);
        //            }
        //        }

        //        public static IEnumerable<IControl> GetDescendants(this IControl control)
        //        {
        //            var descendants = new List<IControl>();

        //            using (var stopwatch = new UIRelationStopwatch(control))
        //            {
        //                if (!UIRelationCacheEngine.GetDescendants(control, ref descendants))
        //                {
        //                    AddDescendants(control, descendants);

        //                    if (!stopwatch.Reentrant)
        //                    {
        //                        UIRelationCacheEngine.SetDescendants(control, descendants);
        //                    }
        //                }
        //            }

        //            return descendants;
        //        }

        //        private static void AddDescendants(object obj, List<IControl> list)
        //        {
        //            if (obj is IControl)
        //            {
        //                foreach (var child in ((IControl)obj).GetChildControls())
        //                {
        //                    list.Add(child);

        //                    AddDescendants(child, list);
        //                }
        //            }
        //            else if (obj is FrameworkElement)
        //            {
        //                var creator = ControlProperties.FindHandlerForType(typeof(FrameworkElement)) as IControlWrapperCreator;

        //                foreach (var child in ((FrameworkElement)obj).GetChildren().OfType<FrameworkElement>())
        //                {
        //                    if (child is IControl)
        //                    {
        //                        list.Add((IControl) child);
        //                    }
        //                    else if (creator != null)
        //                    {
        //                        var wrapper = creator.Create(child);

        //                        list.Add(creator.Create(child));
        //                    }

        //                    AddDescendants(child, list);
        //                }
        //            }
        //        }

        public static IEnumerable<T> GetDescendants<T>(this DependencyObject obj)
        {
            var descendants = new List<T>();

            obj.AddDescendants<T>(descendants);

            return descendants;
        }

        private static void AddDescendants<T>(this DependencyObject obj, List<T> list)
        {
            foreach (var child in obj.GetChildren())
            {
                if (child is T)
                {
                    list.Add((T)(object)child);
                }

                child.AddDescendants<T>(list);
            }
        }

        public static DependencyObject FindDescendant(this DependencyObject obj, Func<DependencyObject, bool> filter)
        {
            foreach (var child in obj.GetChildren())
            {
                if (filter(child))
                {
                    return child;
                }

                var descendant = child.FindDescendant(filter);

                if (descendant != null)
                {
                    return descendant;
                }
            }

            return null;
        }

        public static IEnumerable<DependencyObject> GetDescendants(this DependencyObject obj, Func<DependencyObject, bool> filter)
        {
            var descendants = new List<DependencyObject>();

            obj.AddDescendants(descendants, filter);

            return descendants;
        }

        private static void AddDescendants(this DependencyObject obj, List<DependencyObject> list, Func<DependencyObject, bool> filter)
        {
            foreach (var child in obj.GetChildren())
            {
                if (filter(child))
                {
                    list.Add(child);
                }

                child.AddDescendants(list, filter);
            }
        }

        public static string GetDescendantHierarchy(this DependencyObject obj)
        {
            var list = new StringBuilder();

            list.AppendFormat("************ Children for: {0}\r\n", obj.GetTypeAndName());

            obj.AddDescendants(list, 1);

            list.AppendLine("*********************************************************");

            return list.ToString();
        }

        private static void AddDescendants(this DependencyObject obj, StringBuilder list, int indent)
        {
            foreach (var child in obj.GetChildren())
            {
                list.AppendFormat("{0}{1}\r\n", new string(' ', indent * 2), child.GetTypeAndName());

                child.AddDescendants(list, indent + 1);
            }
        }

        //public static IEnumerable<DependencyObject> GetDescendants(this DependencyObject obj)
        //{
        //    var descendants = new List<DependencyObject>();

        //    using (var stopwatch = new UIRelationStopwatch(obj))
        //    {
        //        if (!UIRelationCacheEngine.GetDescendants(obj, ref descendants))
        //        {
        //            obj.AddDescendants(descendants);

        //            if (!stopwatch.Reentrant)
        //            {
        //                UIRelationCacheEngine.SetDescendants(obj, descendants);
        //            }
        //        }
        //    }

        //    return descendants;
        //}

        private static void AddDescendants(this DependencyObject obj, List<DependencyObject> list)
        {
            foreach (var child in obj.GetChildren())
            {
                list.Add(child);

                child.AddDescendants(list);
            }
        }

        public static IEnumerable<DependencyObject> GetAncestors(this DependencyObject obj)
        {
            var ancestors = new List<DependencyObject>();
            var ancestor = obj.GetParent();

            while (ancestor != null)
            {
                ancestors.Add(ancestor);

                ancestor = ancestor.GetParent();
            }

            return ancestors;
        }

        public static DependencyObject FindAncestor(this DependencyObject obj, Func<DependencyObject, bool> where)
        {
            var ancestor = obj.GetParent();

            while (ancestor != null)
            {
                if (where(ancestor))
                {
                    return ancestor;
                }

                ancestor = ancestor.GetParent();
            }

            return null;
        }

        public static IEnumerable<DependencyObject> GetChildren(this DependencyObject obj)
        {
            var count = VisualTreeHelper.GetChildrenCount(obj);

            for (var x = 0; x < count; x++)
            {
                yield return VisualTreeHelper.GetChild(obj, x);
            }
        }

        //        public static void OnMessage(this IEnumerable<IWindowTarget> targets, ref Message m)
        //        {
        //            foreach (var target in targets)
        //            {
        //                target.OnMessage(ref m);
        //            }
        //        }

        //        public static Size GetSize(this UIElement control)
        //        {
        //            if (control is FrameworkElement)
        //            {
        //                var element = (FrameworkElement)control;

        //                return new Size(element.Width, element.Height);
        //            }

        //            Debugger.Break();

        //            return DrawingExtensions.SizeEmpty;
        //        }

        //        public static bool IsInVisualTree(this DependencyObject element)
        //        {
        //            return IsInVisualTree(element, Application.Current.RootVisual as DependencyObject);
        //        }

        //        public static bool IsInVisualTree(this DependencyObject element, DependencyObject ancestor)
        //        {
        //            var frameworkElement = element;

        //            while (frameworkElement != null)
        //            {
        //                if (frameworkElement == ancestor)
        //                {
        //                    return true;
        //                }

        //                frameworkElement = VisualTreeHelper.GetParent(frameworkElement) as DependencyObject;
        //            }

        //            return false;
        //        }

        //        public static bool IsVisible(this FrameworkElement element)
        //        {
        //            return element.Visibility == Visibility.Visible;
        //        }

        //        public static void SetVisible(this FrameworkElement element, bool visible)
        //        {
        //            element.Visibility = (visible ? Visibility.Visible : Visibility.Collapsed);
        //        }

        //        public static void SetBackColor(this FrameworkElement control, Color color)
        //        {
        //            if (control is Control)
        //            {
        //                var uiControl = (Control)control;

        //                uiControl.Background = new SolidColorBrush(color);
        //            }
        //            else if (control is Panel)
        //            {
        //                var panel = (Panel)control;

        //                panel.Background = new SolidColorBrush(color);
        //            }
        //            else
        //            {
        //                Debugger.Break();
        //            }
        //        }

        //        public static void SetImage(this ContentControl control, Bitmap image)
        //        {
        //            if (!image.IsOpened)
        //            {
        //                image.BitmapChanged += (sender, e) =>
        //                {
        //                    control.Content = new Image { Source = (WriteableBitmap)image, Stretch = Stretch.None };
        //                };
        //            }
        //            else
        //            {
        //                control.Content = new Image { Source = (WriteableBitmap)image, Stretch = Stretch.None };
        //            }
        //        }

        //        public static void SetTabIndex(this Control control, int tabIndex)
        //        {
        //            control.TabIndex = tabIndex;
        //        }

        //        public static void SetForeColor(this FrameworkElement control, Color color)
        //        {
        //            if (control is Control)
        //            {
        //                var uiControl = (Control)control;

        //                uiControl.Foreground = new SolidColorBrush(color);
        //            }
        //            else if (control is Panel)
        //            {
        //                var panel = (Panel)control;

        //                foreach (var childControl in panel.GetChildren().OfType<FrameworkElement>())
        //                {
        //                    childControl.SetForeColor(color);
        //                }
        //            }
        //            else if (control is TextBlock)
        //            {
        //                var textBlock = (TextBlock)control;

        //                textBlock.Foreground = new SolidColorBrush(color);
        //            }
        //            else
        //            {
        //                Debugger.Break();
        //            }
        //        }

        //        public static bool IsVisible(this FrameworkElement element, FrameworkElement topParent = null)
        //        {
        //            if (!element.IsInVisualTree()) return false;

        //            while (element != topParent && element != null)
        //            {
        //                if (element.Visibility == Visibility.Collapsed) return false;
        //                element = VisualTreeHelper.GetParent(element) as FrameworkElement;
        //            }

        //            return true;
        //        }

        //        public static T FindElementOfType<T>(this FrameworkElement element) where T : FrameworkElement
        //        {
        //            T correctlyTyped = element as T;

        //            if (correctlyTyped != null)
        //            {
        //                return correctlyTyped;
        //            }

        //            if (element != null)
        //            {
        //                int numChildren = VisualTreeHelper.GetChildrenCount(element);

        //                for (var i = 0; i < numChildren; i++)
        //                {
        //                    T child = FindElementOfType<T>(VisualTreeHelper.GetChild(element, i) as FrameworkElement);

        //                    if (child != null)
        //                    {
        //                        return child;
        //                    }
        //                }

        //                // Popups continue in another window, jump to that tree
        //                var popup = element as Popup;

        //                if (popup != null)
        //                {
        //                    return FindElementOfType<T>(popup.Child as FrameworkElement);
        //                }
        //            }

        //            return null;
        //        }

        //        public static object FindElementOfType(this FrameworkElement element, string typeName)
        //        {
        //            if (element.GetType().Name == typeName && element.GetType().FullName == typeName)
        //            {
        //                return element;
        //            }

        //            if (element != null)
        //            {
        //                var numChildren = VisualTreeHelper.GetChildrenCount(element);

        //                for (int i = 0; i < numChildren; i++)
        //                {
        //                    var child = FindElementOfType(VisualTreeHelper.GetChild(element, i) as FrameworkElement, typeName);

        //                    if (child != null)
        //                    {
        //                        return child;
        //                    }
        //                }

        //                // Popups continue in another window, jump to that tree
        //                Popup popup = element as Popup;

        //                if (popup != null)
        //                {
        //                    return FindElementOfType(popup.Child as FrameworkElement, typeName);
        //                }
        //            }

        //            return null;
        //        }

        //        public static FrameworkElement FindElementWithName(this FrameworkElement element, string name)
        //        {
        //            FrameworkElement obj = element as FrameworkElement;

        //            if (obj != null && obj.Name.Equals(name))
        //            {
        //                return obj;
        //            }

        //            if (element != null)
        //            {
        //                int numChildren = VisualTreeHelper.GetChildrenCount(element);

        //                for (int i = 0; i < numChildren; i++)
        //                {
        //                    var child = FindElementWithName(VisualTreeHelper.GetChild(element, i) as FrameworkElement, name);

        //                    if (child != null)
        //                    {
        //                        return child;
        //                    }
        //                }

        //                // Popups continue in another window, jump to that tree

        //                var popup = element as Popup;

        //                if (popup != null)
        //                {
        //                    return FindElementWithName(popup.Child as FrameworkElement, name);
        //                }
        //            }

        //            return null;
        //        }

        //        public static void ScrollControlIntoView(this ScrollViewer scrollViewer, FrameworkElement activeControl)
        //        {
        //            var transform = activeControl.TransformToVisual(scrollViewer);
        //            var rectangle = transform.TransformBounds(new Rect(new Point(activeControl.Margin.Left, activeControl.Margin.Top), activeControl.RenderSize));
        //            var newOffset = scrollViewer.VerticalOffset + (rectangle.Bottom - scrollViewer.ViewportHeight);

        //            scrollViewer.ScrollToVerticalOffset(newOffset);
        //        }

        //#if STEPTHROUGHCONTROLEXTENSIONS
        //        [DebuggerStepThrough]
        //#endif
        //        public static IControl GetControl(this EventArgs args)
        //        {
        //            Debugger.Break();

        //            return null;
        //        }

        //// begin Scroll methods

        //#if STEPTHROUGHCONTROLEXTENSIONS
        //        [DebuggerStepThrough]
        //#endif
        //        public static void SetPhantomScrollSize(this IScrollableContainer control, Size size)
        //        {
        //            var scrollViewer = control.ScrollBarHost as ScrollViewer;

        //            if (scrollViewer == null)
        //            {
        //                scrollViewer = control.GetDescendants().OfType<ScrollViewer>().FirstOrDefault();
        //            }

        //            if (scrollViewer == null)
        //            {
        //                var contentControl = control as ContentControl;

        //                if (contentControl != null && contentControl.Content is ScrollViewer)
        //                {
        //                    scrollViewer = (ScrollViewer)contentControl.Content;
        //                }
        //            }

        //            var stackPanel = scrollViewer.FindElementOfType<StackPanel>();
        //            var scrollBar = control.GetVerticalScrollBar();
        //            var verticalThumb = scrollViewer.FindElementOfType<Thumb>();

        //            if (verticalThumb != null)
        //            {
        //                verticalThumb.Height = 20;
        //            }

        //            //scrollViewer.InvalidateMeasure();
        //            //scrollViewer.UpdateLayout();
        //        }

        //#if STEPTHROUGHCONTROLEXTENSIONS
        //        [DebuggerStepThrough]
        //#endif
        //        public static ScrollViewer GetScrollViewer(this IScrollableContainer control)
        //        {
        //            var scrollViewer = control.ScrollBarHost as ScrollViewer;

        //            if (scrollViewer == null)
        //            {
        //                scrollViewer = control.GetDescendants().OfType<ScrollViewer>().FirstOrDefault();
        //            }

        //            if (scrollViewer == null)
        //            {
        //                var contentControl = control as ContentControl;

        //                if (contentControl != null && contentControl.Content is ScrollViewer)
        //                {
        //                    scrollViewer = (ScrollViewer)contentControl.Content;
        //                }
        //            }

        //            return scrollViewer;
        //        }

        //#if STEPTHROUGHCONTROLEXTENSIONS
        //        [DebuggerStepThrough]
        //#endif
        //        public static void SetAutoScroll(this IScrollableContainer control, bool b)
        //        {
        //            var scrollViewer = control.GetScrollViewer();

        //            if (scrollViewer != null)
        //            {
        //                scrollViewer.HorizontalScrollBarVisibility = ScrollBarVisibility.Auto;
        //                scrollViewer.VerticalScrollBarVisibility = ScrollBarVisibility.Auto;
        //            }
        //            else
        //            {
        //                Set(control, "AutoScroll", b);
        //            }
        //        }

        //#if STEPTHROUGHCONTROLEXTENSIONS
        //        [DebuggerStepThrough]
        //#endif
        //        public static void SetAutoScrollPosition(this IScrollableContainer control, Point position)
        //        {
        //            var horizontalScrollBar = control.GetHorizontalScrollBar();
        //            var verticalScrollBar = control.GetVerticalScrollBar();

        //            if (horizontalScrollBar.Value != (int)position.X)
        //            {
        //                horizontalScrollBar.Value = (int)position.X;
        //            }

        //            if (verticalScrollBar.Value != (int)position.Y)
        //            {
        //                verticalScrollBar.Value = (int)position.Y;
        //            }
        //        }

        //#if STEPTHROUGHCONTROLEXTENSIONS
        //        [DebuggerStepThrough]
        //#endif
        //        public static Point GetAutoScrollPosition(this IScrollableContainer control)
        //        {
        //            var scrollViewer = control.GetScrollViewer();

        //            if (scrollViewer != null)
        //            {
        //                return new Point(scrollViewer.HorizontalOffset, scrollViewer.VerticalOffset);
        //            }
        //            else
        //            {
        //                return Get<Point>(control, "AutoScrollPosition");
        //            }
        //        }

        //#if STEPTHROUGHCONTROLEXTENSIONS
        //        [DebuggerStepThrough]
        //#endif
        //        public static bool IsAutoScroll(this IScrollableContainer control)
        //        {
        //            var scrollViewer = control.GetScrollViewer();

        //            if (scrollViewer != null)
        //            {
        //                return scrollViewer.HorizontalScrollBarVisibility == ScrollBarVisibility.Auto || scrollViewer.VerticalScrollBarVisibility == ScrollBarVisibility.Auto;
        //            }
        //            else
        //            {
        //                return Get<bool>(control, "AutoScroll");
        //            }
        //        }

        //#if STEPTHROUGHCONTROLEXTENSIONS
        //        [DebuggerStepThrough]
        //#endif
        //        public static void ScrollControlIntoView(this IScrollableContainer scrollContainer, FrameworkElement control)
        //        {
        //            var scrollViewer = scrollContainer.GetScrollViewer();

        //            if (scrollViewer != null)
        //            {
        //                scrollViewer.ScrollControlIntoView(control);
        //            }
        //            else
        //            {
        //                Invoke(scrollContainer, "ScrollControlIntoView", control);
        //            }
        //        }


        //#if STEPTHROUGHCONTROLEXTENSIONS
        //        [DebuggerStepThrough]
        //#endif
        //        public static void SetSize(this FrameworkElement control, Size size)
        //        {
        //            control.Width = size.Width;
        //            control.Height = size.Height;
        //        }

        //#if STEPTHROUGHCONTROLEXTENSIONS
        //        [DebuggerStepThrough]
        //#endif
        //        public static HorizontalScrollProperties GetHorizontalScroll(this IScrollableContainer control)
        //        {
        //            return (HorizontalScrollProperties)control.Get<HorizontalScrollProperties>("HorizontalScroll");
        //        }

        //#if STEPTHROUGHCONTROLEXTENSIONS
        //        [DebuggerStepThrough]
        //#endif
        //        public static VerticalScrollProperties GetVerticalScroll(this IScrollableContainer control)
        //        {
        //            return (VerticalScrollProperties)control.Get<VerticalScrollProperties>("VerticalScroll");
        //        }

        //#if STEPTHROUGHCONTROLEXTENSIONS
        //        [DebuggerStepThrough]
        //#endif
        //        public static IScrollBar GetVerticalScrollBar(this IScrollableContainer control)
        //        {
        //            var scrollViewer = control.GetScrollViewer();

        //            if (scrollViewer != null)
        //            {
        //                return ((ScrollViewer)control.ScrollBarHost).GetVerticalScrollBar();
        //            }
        //            else
        //            {
        //                return Get<IScrollBar>(control, "VerticalScrollBar");
        //            }
        //        }

        //#if STEPTHROUGHCONTROLEXTENSIONS
        //        [DebuggerStepThrough]
        //#endif
        //        public static IScrollBar GetHorizontalScrollBar(this IScrollableContainer control)
        //        {
        //            var scrollViewer = control.GetScrollViewer();

        //            if (scrollViewer != null)
        //            {
        //                return ((ScrollViewer)control.ScrollBarHost).GetHorizontalScrollBar();
        //            }
        //            else
        //            {
        //                return Get<IScrollBar>(control, "HorizontalScrollBar");
        //            }
        //        }

        //#if STEPTHROUGHCONTROLEXTENSIONS
        //        [DebuggerStepThrough]
        //#endif
        //        public static Rect GetDisplayRectangle(this IScrollableContainer control)
        //        {
        //            var scrollViewer = control.GetScrollViewer();

        //            if (scrollViewer != null)
        //            {
        //                return new Rect(scrollViewer.Padding.Left, scrollViewer.Padding.Top, scrollViewer.ViewportWidth, scrollViewer.ViewportHeight);
        //            }
        //            else
        //            {
        //                return Get<Rect>(control, "DisplayRectangle");
        //            }
        //        }

        //#if STEPTHROUGHCONTROLEXTENSIONS
        //        [DebuggerStepThrough]
        //#endif
        //        public static HorizontalScrollBar GetHorizontalScrollBar(this ScrollViewer control)
        //        {
        //            return new HorizontalScrollBar((ScrollBar)control.FindElementWithName("HorizontalScrollBar"));
        //        }

        //#if STEPTHROUGHCONTROLEXTENSIONS
        //        [DebuggerStepThrough]
        //#endif
        //        public static VerticalScrollBar GetVerticalScrollBar(this ScrollViewer control)
        //        {
        //            return new VerticalScrollBar((ScrollBar)control.FindElementWithName("VerticalScrollBar"));
        //        }


        //#if STEPTHROUGHCONTROLEXTENSIONS
        //        [DebuggerStepThrough]
        //#endif
        //        public static ScrollBar GetHorizontalScrollBarNative(this ScrollViewer control)
        //        {
        //            return (ScrollBar)control.FindElementWithName("HorizontalScrollBar");
        //        }

        //#if STEPTHROUGHCONTROLEXTENSIONS
        //        [DebuggerStepThrough]
        //#endif
        //        public static ScrollBar GetVerticalScrollBarNative(this ScrollViewer control)
        //        {
        //            return (ScrollBar)control.FindElementWithName("VerticalScrollBar");
        //        }

        //#if STEPTHROUGHCONTROLEXTENSIONS
        //        [DebuggerStepThrough]
        //#endif
        //        public static bool IsVisible(this IScrollBar control)
        //        {
        //            return control.Visible;
        //        }

        //#if STEPTHROUGHCONTROLEXTENSIONS
        //        [DebuggerStepThrough]
        //#endif
        //        public static Rect GetBounds(this IScrollBar control)
        //        {
        //            return ((ScrollBarBase)control).InternalScrollBar.GetBounds();
        //        }

        //#if STEPTHROUGHCONTROLEXTENSIONS
        //        [DebuggerStepThrough]
        //#endif
        //        public static void SetFont(this TextBlock control, Font font)
        //        {
        //            control.FontFamily = font.Family;
        //            control.FontStretch = font.Stretch;
        //            control.FontStyle = font.Style;
        //            control.FontWeight = font.Weight;
        //            control.FontSize = font.Size;
        //        }

        //#if STEPTHROUGHCONTROLEXTENSIONS
        //        [DebuggerStepThrough]
        //#endif
        //        public static Size GetSize(this IScrollBar scrollBar)
        //        {
        //            throw new NotImplementedException();
        //        }

        //#if STEPTHROUGHCONTROLEXTENSIONS
        //        [DebuggerStepThrough]
        //#endif
        //        public static void SetBounds(this ScrollBar scrollBar, Rect rect)
        //        {
        //            throw new NotImplementedException();
        //        }

        //#if STEPTHROUGHCONTROLEXTENSIONS
        //        [DebuggerStepThrough]
        //#endif
        //        public static void SetToolTip(this UIElement element, string text)
        //        {
        //            ToolTipService.SetToolTip(element, text);
        //        }

        //#if STEPTHROUGHCONTROLEXTENSIONS
        //        [DebuggerStepThrough]
        //#endif
        //        internal static ElementProperties GetExtendedProperties(this UIElement element)
        //        {
        //            var trackedElement = Globals.Elements[element];

        //            return trackedElement.ExtendedProperties;
        //        }

        //#if STEPTHROUGHCONTROLEXTENSIONS
        //        [DebuggerStepThrough]
        //#endif
        //        public static void SetDisplayStyle(this BarItem item, ToolStripItemDisplayStyle style)
        //        {
        //            var properties = item.GetExtendedProperties();

        //            properties["DisplayStyle"] = style;
        //        }

        //#if STEPTHROUGHCONTROLEXTENSIONS
        //        [DebuggerStepThrough]
        //#endif
        //        public static void SetImageIndex(this BarItem item, int index)
        //        {
        //            var properties = item.GetExtendedProperties();

        //            properties["ImageIndex"] = index;
        //        }

        //#if STEPTHROUGHCONTROLEXTENSIONS
        //        [DebuggerStepThrough]
        //#endif
        //        public static void SetImageScaling(this BarItem item, ToolStripItemImageScaling imageScaling)
        //        {
        //            var properties = item.GetExtendedProperties();

        //            properties["ImageScaling"] = imageScaling;
        //        }

        //#if STEPTHROUGHCONTROLEXTENSIONS
        //        [DebuggerStepThrough]
        //#endif
        //        public static ToolStripItemDisplayStyle GetDisplayStyle(this BarItem item)
        //        {
        //            var properties = item.GetExtendedProperties();

        //            if (properties["DisplayStyle"] != null)
        //            {
        //                return (ToolStripItemDisplayStyle)properties["DisplayStyle"];
        //            }
        //            else
        //            {
        //                return ToolStripItemDisplayStyle.None;
        //            }
        //        }

        //#if STEPTHROUGHCONTROLEXTENSIONS
        //        [DebuggerStepThrough]
        //#endif
        //        public static int GetImageIndex(this BarItem item)
        //        {
        //            var properties = item.GetExtendedProperties();

        //            if (properties["ImageIndex"] != null)
        //            {
        //                return (int)properties["ImageIndex"];
        //            }
        //            else
        //            {
        //                return -1;
        //            }
        //        }

        //#if STEPTHROUGHCONTROLEXTENSIONS
        //        [DebuggerStepThrough]
        //#endif
        //        public static ToolStripItemImageScaling GetImageScaling(this BarItem item)
        //        {
        //            var properties = item.GetExtendedProperties();

        //            if (properties["ImageScaling"] != null)
        //            {
        //                return (ToolStripItemImageScaling)properties["ImageScaling"];
        //            }
        //            else
        //            {
        //                return ToolStripItemImageScaling.None;
        //            }
        //        }

        //#if STEPTHROUGHCONTROLEXTENSIONS
        //        [DebuggerStepThrough]
        //#endif
        //        public static EventStrategy GetAncestorStrategy(this FrameworkElement element, out FrameworkElement ancestorElement)
        //        {
        //            ancestorElement = null;

        //            foreach (var ancestor in element.GetAncestors())
        //            {
        //                var type = ancestor.GetType();

        //                if (type.HasCustomAttribute<EventHandlingAttribute>())
        //                {
        //                    var eventHandlingAttr = type.GetCustomAttribute<EventHandlingAttribute>();

        //                    ancestorElement = ancestor.As<FrameworkElement>();

        //                    return eventHandlingAttr.EventStrategy;
        //                }
        //            }

        //            return EventStrategy.Bubbling;
        //        }

        public static string GetTypeAndName(this object obj, string delimiter = null)
        {
            if (obj is DependencyObject)
            {
                return GetTypeAndName((DependencyObject)obj, delimiter);
            }
            else
            {
                return obj.GetType().Name;
            }
        }

        public static string GetTypeAndName(this DependencyObject dependencyObject, string delimiter = null)
        {
            if (dependencyObject is FrameworkElement)
            {
                var text = string.Empty;
                var element = (FrameworkElement)dependencyObject;

                if (string.IsNullOrEmpty(element.Name))
                {
                    text = element.GetType().Name;
                }
                else if (delimiter != null)
                {
                    text = element.GetType().Name + delimiter + element.Name;
                }
                else
                {
                    text = element.GetType().Name + " (" + element.Name + ")";
                }

                if (element is TextBlock textBlock)
                {
                    text += " Text:" + textBlock.Text;
                }
                else if (element is TextBox textBox)
                {
                    text += " Text:" + textBox.Text;
                }
                else if (element is Label label)
                {
                    text += " Text:" + label;
                }
                else if (element is AccessText accessText)
                {
                    text += " Text:" + accessText.Text;
                }

                return text;
            }
            else
            {
                return dependencyObject.GetType().Name;
            }
        }
    }
}

