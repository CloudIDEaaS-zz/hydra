using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using System.Diagnostics;
using System.Linq;
using Utils;
using System.Collections.Generic;
using System.Windows.Threading;
using System.Windows.Media.Imaging;
using IDataObject = System.Windows.IDataObject;

namespace SLControlLibrary
{
    public static class Globals
    {
//        public static event EventHandler InstalledFontsChanged;
//        public static event UserPreferenceChangedEventHandler UserPreferenceChanged;
//        public static event EventHandler DisplaySettingsChanged;
//        public static event AllowTrackElementHandler AllowTrackElement;
//        public static event GetControlHostProviderHandler GetControlHostProvider;
//        public static event EventHandler OnLayoutUpdated;
//        private static MessagePump messagePump;
//        private static Cursor defaultCursor;
//        private static MouseButtons currentMouseButton;
//        private static Dispatcher uiDispatcher;
//        private static Dictionary<string, uint> registeredMessages;
//        private static ElementCollection elements;
//        private static object currentCapture;
//        private static ILog log;
//        private static Dictionary<string, object> environmentVariables;
//        private static DragDropManager dragDropManager;
//        public static string SET_BREAK_ON_THROWN_EXCEPTIONS = "SET_BREAK_ON_THROWN_EXCEPTIONS";
//        public static string DIVCONTROL_LAYOUT_DEBUGGER = "DIVCONTROL_LAYOUT_DEBUGGER";
//        public static string CONTROL_HOST_PROVIDER = "CONTROL_HOST_PROVIDER";

//        public static event EventHandler Idle
//        {
//            add
//            {
//                messagePump.Idle += value;
//            }

//            remove
//            {
//                messagePump.Idle -= value;
//            }
//        }

//        public static ILog Log
//        {
//            get 
//            {
//                if (log == null)
//                {
//                    log = new DefaultLog();
//                }

//                return log; 
//            }
//        }
    
//        static Globals()
//        {
//            defaultCursor = Cursors.Arrow;
//            registeredMessages = new Dictionary<string, uint>();
//            elements = new ElementCollection();
//            environmentVariables = new Dictionary<string, object>();
//            dragDropManager = new DragDropManager();

//            dragDropManager.OnDragStart += new EventHandler(OnDragStart);
//            dragDropManager.OnDragComplete += new EventHandler(OnDragComplete);
//            dragDropManager.OnDropStarting += new EventHandler(OnDropStarting);
//            dragDropManager.OnDrop += new EventHandler(OnDrop);

//            Debug.Assert(messagePump == null);

//            messagePump = new MessagePump(elements);
            
//            OverrideVirtualParent.MessagePump = messagePump;

//            messagePump.AllowTrackElement += new AllowTrackElementHandler(messagePump_AllowTrackElement);
//            messagePump.OnLayoutUpdated += new EventHandler(messagePump_OnLayoutUpdated);

//            uiDispatcher = Application.Current.RootVisual.Dispatcher;

//            SystemInformation.Initialize();
//        }

//        static void OnDragStart(object sender, EventArgs e)
//        {
//            var windowTarget = (IWindowTargetHandler) ControlProperties.FindHandlerForMember("WindowTarget");

//            windowTarget.SuspendMessages = true;

//            messagePump.SuspendMessages = true;
//            ControlRegistry.SuspendMessages = true;
//        }

//        static void OnDragComplete(object sender, EventArgs e)
//        {
//            var windowTarget = (IWindowTargetHandler)ControlProperties.FindHandlerForMember("WindowTarget");

//            windowTarget.SuspendMessages = false;

//            messagePump.SuspendMessages = false;
//            ControlRegistry.SuspendMessages = false;
//        }

//        static void OnDropStarting(object sender, EventArgs e)
//        {
//            var windowTarget = (IWindowTargetHandler)ControlProperties.FindHandlerForMember("WindowTarget");

//            windowTarget.SuspendMessages = false;

//            messagePump.SuspendMessages = false;
//            ControlRegistry.SuspendMessages = false;
//        }

//        static void OnDrop(object sender, EventArgs e)
//        {
//            var windowTarget = (IWindowTargetHandler)ControlProperties.FindHandlerForMember("WindowTarget");

//            windowTarget.SuspendMessages = false;

//            messagePump.SuspendMessages = false;
//            ControlRegistry.SuspendMessages = false;
//        }

//        public static bool SetEnvironmentVariable(string name, object value)
//        {
//            if (environmentVariables.ContainsKey(name))
//            {
//                environmentVariables.Remove(name);
//            }

//            if (value != null)
//            {
//                environmentVariables.Add(name, value);
//            }

//            return true;
//        }

//        public static object GetEnvironmentVariable(string name)
//        {
//            if (environmentVariables.ContainsKey(name))
//            {
//                return environmentVariables[name];
//            }
//            else
//            {
//                return null;
//            }
//        }

//        public static T GetEnvironmentVariable<T>(string name)
//        {
//            if (environmentVariables.ContainsKey(name))
//            {
//                return (T)environmentVariables[name];
//            }
//            else if (name == Globals.CONTROL_HOST_PROVIDER)
//            {
//                var eventArgs = new ControlHostProviderEventArgs();

//                if (GetControlHostProvider != null)
//                {
//                    GetControlHostProvider(Application.Current, eventArgs);

//                    if (eventArgs.Provider != null)
//                    {
//                        SetEnvironmentVariable(name, eventArgs.Provider);
//                    }
//                }

//                return (T)eventArgs.Provider;
//            }
//            else
//            {
//                return default(T);
//            }
//        }

//        public static void AddLog(ILog log)
//        {
//            Globals.log = log;
//        }

//        internal static ElementCollection Elements
//        {
//            get
//            {
//                return elements;
//            }
//        }

//        internal static IWindowTarget DefaultWindowTarget
//        {
//            get
//            {
//                // control drag issue
//                //return null;
//                return messagePump;
//            }
//        }

//        internal static IWindowMessageFilter WindowMessageFilter
//        {
//            get
//            {
//                return messagePump;
//            }
//        }

//        public static void StartMessagePump()
//        {
//            Debug.Assert(messagePump != null);

//            messagePump.Start();
//        }

//        private static void messagePump_OnLayoutUpdated(object sender, EventArgs e)
//        {
//            if (OnLayoutUpdated != null)
//            {
//                OnLayoutUpdated(sender, e);
//            }
//        }

//        private static void messagePump_AllowTrackElement(object sender, TrackElementEventArgs e)
//        {
//#if GRAPHICSDEBUGGING
//            AllowTrackElement(sender, e); 
//#endif
//        }

//        public static void StopMessagePump()
//        {
//            messagePump.AllowTrackElement -= messagePump_AllowTrackElement;
//            messagePump.OnLayoutUpdated -= messagePump_OnLayoutUpdated;

//            messagePump.Stop();
//        }

//        public static bool IsCriticalException(Exception ex)
//        {
//            throw new NotImplementedException();
//        }

//        public static object SecureCreateInstance(Type pageClass)
//        {
//            throw new NotImplementedException();
//        }

//        public static ModifierKeys CurrentModifierKeys
//        {
//            get
//            {
//                return Keyboard.Modifiers;
//            }
//        }

//        public static Dispatcher UIDispatcher
//        {
//            get
//            {
//                return uiDispatcher;
//            }
//        }

//        public static FrameworkElement CurrentRootVisual
//        {
//            get
//            {
//                if (!UIDispatcher.CheckAccess())
//                {
//                    return ControlExtensions.ScheduleCall<FrameworkElement>(() => Globals.CurrentRootVisual);
//                }

//               return Application.Current.RootVisual as FrameworkElement;
//            }
//        }

//        public static Point CurrentMousePosition
//        {
//            get
//            {
//                return messagePump.CurrentMousePosition;
//            }
//        }

//        public static Cursor CurrentCursor
//        {
//            get
//            {
//                return CurrentRootVisual.GetCursor();
//            }

//            set
//            {
//                CurrentRootVisual.Cursor = value;
//            }
//        }

//        public static Size GetAutoScaleSize(Font font)
//        {
//            throw new NotImplementedException();
//        }

//        public static MouseButtons CurrentMouseButton 
//        {
//            get
//            {
//                return currentMouseButton;
//            }

//            set
//            {
//                currentMouseButton = value;

//                Globals.Log.InfoFormat("Current mouse button set to {0}", Enum<MouseButtons>.GetName(currentMouseButton));
//            }
//        }

//        public static KeyPressEventArgs GetLastKeyPressEventArgs()
//        {
//            return messagePump.LastKeyPressEventArgs;
//        }

//        public static bool GetMessage(out Message msg, IControl window, WindowMessage wMsgFilterMin, WindowMessage wMsgFilterMax)
//        {
//            msg = messagePump.LastMessage;

//            messagePump.LastMessage = null;

//            return true;
//        }

//        public static bool PeekMessage(out Message msg, IControl window, WindowMessage wMsgFilterMin, WindowMessage wMsgFilterMax, bool removeMessage)
//        {
//            msg = messagePump.LastMessage;

//            if (removeMessage)
//            {
//                messagePump.LastMessage = null;
//            }

//            return true;
//        }

//        public static bool PeekMessage(out Message msg, IControl window, WindowMessage wMsgFilterMin, WindowMessage wMsgFilterMax, PeekMessageOptions options)
//        {
//            msg = messagePump.LastMessage;
//            return false;
//        }

//        public static object PostMessage(IControl window, WindowMessage msg, object wParam, ref object lParam)
//        {
//            Debugger.Break();

//            return false;
//        }

//        public static object PostMessage(IControl window, WindowMessage msg, object wParam, object lParam)
//        {
//            Debugger.Break();

//            return false;
//        }

//        public static object SendMessage(IControl window, WindowMessage msg, object wParam, object lParam)
//        {
//            var message = new Message(msg, window, wParam, lParam);

//            return window.CallWndProc(message);
//        }

//        internal static void SendMessage(WindowMessage windowMessage, object wParam, object lParam)
//        {
//        }

//        public static int SendInput(params IInput[] inputs)
//        {
//            return messagePump.SendInput(inputs);
//        }

//        public static object SendMessage(IControl window, WindowMessage msg, object wParam, ref object lParam)
//        {
//            Debugger.Break();

//            return false;
//        }

//        internal static object SendMessageInternal(FrameworkElement window, WindowMessage msg, object wParam, object lParam)
//        {
//            var message = new Message(msg, window, wParam, lParam);

//            if (!messagePump.PreFilterMessage(ref message))
//            {
//                messagePump.LastMessage = message;

//                return window.CallWndProcInternal(message);
//            }
//            else
//            {
//                return message.Result;
//            }
//        }

//        public static object SendMessageInternal(FrameworkElement window, WindowMessage msg, object wParam, ref object lParam)
//        {
//            var message = new Message(msg, window, wParam, lParam);

//            if (!messagePump.PreFilterMessage(ref message))
//            {
//                messagePump.LastMessage = message;

//                return window.CallWndProcInternal(message);
//            }
//            else
//            {
//                return message.Result;
//            }
//        }

//        public static object SendMessage(FrameworkElement window, WindowMessage msg, object wParam, ref object lParam)
//        {
//            var message = new Message(msg, window, wParam, lParam);

//            messagePump.LastMessage = message;

//            return window.CallWndProc(message);
//        }

//        public static object SendMessage(FrameworkElement window, WindowMessage msg, object wParam, object lParam)
//        {
//            var message = new Message(msg, window, wParam, lParam);

//            messagePump.LastMessage = message;

//            return window.CallWndProc(message);
//        }

//        public static void TranslateMessage(Message msg)
//        {
//            throw new NotImplementedException();
//        }

//        public static void DispatchMessage(Message msg)
//        {
//            throw new NotImplementedException();
//        }

//        public static Cursor DefaultCursor 
//        {
//            get
//            {
//                return defaultCursor;
//            }
//        }

//        public static Cursor HSplitCursor
//        {
//            get
//            {
//                return new Cursor(Globals.CurrentCursor);
//            }
//        }

//        public static Cursor VSplitCursor
//        {
//            get
//            {
//                return new Cursor(Globals.CurrentCursor);
//            }
//        }

//        public static Cursor SizeAllCursor
//        {
//            get
//            {
//                throw new NotImplementedException();
//            }
//        }

//        public static int HorizontalScrollBarHeight
//        {
//            get
//            {
//                throw new NotImplementedException();
//            }
//        }

//        public static int VerticalScrollBarWidth
//        {
//            get
//            {
//                throw new NotImplementedException();
//            }
//        }

//        public static int DoubleClickTime
//        {
//            get
//            {
//                return 500;
//            }
//        }

//        public static long GetTickCount()
//        {
//            return messagePump.TickCount;
//        }

//        public static void RegisterForDragDrop(IControl control)
//        {
//            control.SetDragDropEnabled(true);
//        }

//        public static void RevokeDragDrop(IControl control)
//        {
//            control.SetDragDropEnabled(false);
//        }

//        public static Point GetCursorPos()
//        {
//            return messagePump.CurrentMousePosition;
//        }

//        public static void SetWindowRgn(FrameworkElement control, Region region, bool redraw)
//        {
//            var element = (IControl)control;
//            var graphics = element.CreateGraphics();
//            var trackedElement = Globals.Elements[(UIElement)element];

//            trackedElement.WindowRegion = region;
//            trackedElement.InvalidRegion = region;

//            if (redraw)
//            {
//                graphics.Erase(region);

//                control.CallWndProc(new Message(WindowMessage.Paint));
//                trackedElement.UpdateRegion = null;
//            }
//        }

//        public static void SetWindowPos(IControl control, IControl controlAfter, int x, int y, int cx, int cy, SetWindowPosFlags flags)
//        {
//            throw new NotImplementedException();
//        }

//        public static void SetWindowPos(IControl control, WindowPos controlAfter, int x, int y, int cx, int cy, SetWindowPosFlags flags)
//        {
//            var parent = control.GetParent();

//            if (parent is IControlWrapper)
//            {
//                var uiParent = ((IControlWrapper)parent).InternalElement;
//                var uiElement = (UIElement)control;

//                if (uiParent is Canvas)
//                {
//                    var canvas = (Canvas)uiParent;

//                    if (controlAfter == WindowPos.HWND_TOP)
//                    {
//                        var highestZIndex = 0;

//                        foreach (var existingControl in canvas.Children)
//                        {
//                            var zIndex = (int) existingControl.GetValue(Canvas.ZIndexProperty);

//                            if (zIndex > highestZIndex)
//                            {
//                                highestZIndex = zIndex;
//                            }
//                        }

//                        uiElement.SetValue(Canvas.ZIndexProperty, highestZIndex + 1);
//                    }
//                    else
//                    {
//                        throw new NotImplementedException();
//                    }
//                }
//                else
//                {
//                    throw new NotImplementedException();
//                }
//            }
//            else
//            {
//                throw new NotImplementedException();
//            }
//        }

//        public static void SetControlOpacity(FrameworkElement control, double opacity)
//        {
//            var trackedElement = Globals.Elements[control];

//            trackedElement.Opacity = opacity;
//        }

//        public static void SetControlExtendedStyle(FrameworkElement control, ControlExtendedStyle style)
//        {
//            var trackedElement = Globals.Elements[control];

//            trackedElement.ExtendedStyle = style;
//        }

//        public static GraphicsProcessor GetGraphicsWorker(FrameworkElement control)
//        {
//            var trackedElement = Globals.Elements[control];

//            return trackedElement.GraphicsWorker;
//        }

//        public static WriteableBitmap GetControlBuffer(FrameworkElement control, ControlBufferType type)
//        {
//            var trackedElement = Globals.Elements[control];

//            if (type == ControlBufferType.BackBuffer)
//            {
//                return trackedElement.BackBuffer;
//            }
//            else if (type == ControlBufferType.ClipBuffer)
//            {
//                return trackedElement.ClipBuffer;
//            }
//            else
//            {
//                throw new ArgumentException("Invalid control buffer type");
//            }
//        }

//        public static void Invalidate(FrameworkElement control, Rect rect)
//        {
//            Invalidate(control, rect, false);
//        }

//        public static void Invalidate(FrameworkElement control, bool erase)
//        {
//            var rect = control.GetClientRectangle();

//            Invalidate(control, rect, erase);
//        }

//        public static bool InvokeRequired
//        {
//            get
//            {
//                return !UIDispatcher.CheckAccess();
//            }
//        }

//        public static void Invalidate(FrameworkElement control, Region region)
//        {
//            if (!UIDispatcher.CheckAccess())
//            {
//                ControlExtensions.ScheduleCall(() => Invalidate(control, region));
//                return;
//            }

//            if (Globals.Elements[control].InvalidRegion == null)
//            {
//                Globals.Elements[control].InvalidRegion = region;
//            }
//            else
//            {
//                Globals.Elements[control].InvalidRegion.Complement(region);
//            }

//            foreach (var invalidRect in region.GetRegionData().Rectangles)
//            {
//                var screenRect = control.RectangleToScreen(invalidRect);
//                var hitElements = VisualTreeHelper.FindElementsInHostCoordinates(screenRect, Globals.CurrentRootVisual).Where(e => control == e || control.GetDescendants().Any(c => c == e)).OfType<FrameworkElement>();

//                hitElements = hitElements.AppendVirtual(screenRect);

//                foreach (var element in hitElements)
//                {
//                    if (Globals.Elements.ContainsKey(element))
//                    {
//                        var clientRect = element.RectangleToClient(screenRect);

//                        if (clientRect != DrawingExtensions.RectangleEmpty)
//                        {
//                            var trackedElement = Globals.Elements[element];

//                            if (trackedElement.InvalidRegion != null)
//                            {
//                                trackedElement.InvalidRegion.Complement(clientRect);
//                            }
//                            else
//                            {
//                                trackedElement.InvalidRegion = new Region(clientRect);
//                            }
//                        }
//                    }
//                }
//            }
//        }

//        public static void Invalidate(FrameworkElement control, Rect rect, bool erase)
//        {
//            if (!UIDispatcher.CheckAccess())
//            {
//                ControlExtensions.ScheduleCall(() => Invalidate(control, rect, erase));
//                return;
//            }

//            var region = new Region(rect);

//            if (Globals.Elements[control].InvalidRegion == null)
//            {
//                Globals.Elements[control].InvalidRegion = region;
//            }
//            else
//            {
//                Globals.Elements[control].InvalidRegion.Complement(rect);
//            }

//            foreach (var invalidRect in region.GetRegionData().Rectangles)
//            {
//                var screenRect = control.RectangleToScreen(invalidRect);
//                var hitElements = VisualTreeHelper.FindElementsInHostCoordinates(screenRect, Globals.CurrentRootVisual).Where(e => control == e || control.GetChildren().Any(c => c == e)).OfType<FrameworkElement>();

//                hitElements = hitElements.AppendVirtual(screenRect);

//                foreach (var element in hitElements)
//                {
//                    if (Globals.Elements.ContainsKey(element))
//                    {
//                        var clientRect = element.RectangleToClient(screenRect);

//                        if (clientRect != DrawingExtensions.RectangleEmpty)
//                        {
//                            var trackedElement = Globals.Elements[element];

//                            if (trackedElement.InvalidRegion != null)
//                            {
//                                trackedElement.InvalidRegion.Complement(clientRect);
//                            }
//                            else
//                            {
//                                trackedElement.InvalidRegion = new Region(clientRect);
//                            }
//                        }

//                        if (erase)
//                        {
//                            var graphics = element.CreateGraphics();

//                            graphics.Erase(clientRect);
//                        }
//                    }
//                }
//            }
//        }

//        public static void Update(FrameworkElement control)
//        {
//            if (!UIDispatcher.CheckAccess())
//            {
//                ControlExtensions.ScheduleCall(() => Update(control));
//                return;
//            }

//            TrackedElement trackedElement;
//            var region = Globals.Elements[control].InvalidRegion;
//            var combinedHitElements = new List<FrameworkElement>();

//            if (region == null)
//            {
//                return;
//            }

//            combinedHitElements.Add(control);

//            foreach (var rect in region.GetRegionData().Rectangles)
//            {
//                var screenRect = control.RectangleToScreen(rect);
//                var hitElements = VisualTreeHelper.FindElementsInHostCoordinates(screenRect, Globals.CurrentRootVisual).Where(e => control == e || control.GetChildren().Any(c => c == e)).OfType<FrameworkElement>();

//                hitElements = hitElements.AppendVirtual(screenRect);

//                if (control is IControl && ((IControl)control).GraphicsMode != GraphicsMode.DoubleBuffered)
//                {
//                    var element = (IControl)control;
//                    var graphics = element.CreateGraphics();
                    
//                    trackedElement = Globals.Elements[(UIElement) element];

//                    graphics.Erase(rect);

//                    trackedElement.UpdateRegion = trackedElement.InvalidRegion;
//                    trackedElement.InvalidRegion = null;
//                }

//                foreach (var element in hitElements.OfType<IControl>())
//                {
//                    if (element.GraphicsMode == GraphicsMode.DoubleBuffered)
//                    {
//                        var clientRect = element.RectangleToClient(screenRect);
//                        var graphics = element.CreateGraphics();

//                        graphics.Erase(clientRect);
//                    }
//                }

//                combinedHitElements.AddRange(hitElements.Where(e => !combinedHitElements.Any(c => c == e)));
//            }

//            foreach (var element in combinedHitElements.OfType<FrameworkElement>())
//            {
//                trackedElement = Globals.Elements[element];

//                trackedElement.UpdateRegion = trackedElement.InvalidRegion;
//                trackedElement.InvalidRegion = null;

//                //if (element.GetType().Name == "AdornerWindow")
//                //{
//                //    Debugger.Break();
//                //}

//                element.CallWndProc(new Message(WindowMessage.Paint));

//                trackedElement.UpdateRegion = null;
//            }
//        }

//        internal static bool IsInputMessage(this WindowMessage message)
//        {
//            return message.IsBetween(WindowMessage.MOUSEFIRST, WindowMessage.MOUSELAST) || message.IsBetween(WindowMessage.KEYFIRST, WindowMessage.KEYLAST);
//        }

//        internal static object CallWndProcInternal(this FrameworkElement element, Message msg)
//        {
//            FrameworkElement ancestorElement;
//            var eventStrategy = element.GetAncestorStrategy(out ancestorElement);

//            if (msg.Msg.IsInputMessage() && eventStrategy == EventStrategy.Tunneling)
//            {
//                var wndProc = ancestorElement.GetType().GetMethod("WndProc");

//                if (wndProc != null)
//                {
//                    msg.Target = element;
//                    var args = new object[] { msg };

//                    if (!messagePump.PreFilterMessage(ref msg))
//                    {
//                        return wndProc.Invoke(element, args);
//                    }
//                }
//                else if (ancestorElement is IOverrideControl)
//                {
//                    var overrideControl = (IOverrideControl)ancestorElement;
//                    var virtualParent = overrideControl.GetVirtualParent();

//                    virtualParent.HandleWndProc(ref msg);
//                }
//            }
//            else
//            {
//                var wndProc = element.GetType().GetMethod("WndProc");

//                if (wndProc != null)
//                {
//                    msg.Target = element;
//                    var args = new object[] { msg };

//                    if (!messagePump.PreFilterMessage(ref msg))
//                    {
//                        return wndProc.Invoke(element, args);
//                    }
//                }
//                else if (element is IOverrideControl)
//                {
//                    var overrideControl = (IOverrideControl)element;
//                    var virtualParent = overrideControl.GetVirtualParent();

//                    virtualParent.HandleWndProc(ref msg);
//                }
//            }

//            return null;
//        }

//        internal static object CallWndProc(this FrameworkElement element, Message msg)
//        {
//            var wndProc = element.GetType().GetMethod("WndProc");

//            if (wndProc != null)
//            {
//                msg.Target = element;
//                var args = new object[] { msg };

//                if (!messagePump.PreFilterMessage(ref msg))
//                {
//                    return wndProc.Invoke(element, args);
//                }
//            }
//            else if (element is IOverrideControl)
//            {
//                var overrideControl = (IOverrideControl)element;
//                var virtualParent = overrideControl.GetVirtualParent();

//                virtualParent.HandleWndProc(ref msg);
//            }

//            return null;
//        }

//        private static object CallWndProc(this IControl control, Message msg)
//        {
//            return control.WndProc(msg);
//        }

//        private static void CallWndProc(this IEnumerable<FrameworkElement> elements, Message msg)
//        {
//            foreach (var element in elements)
//            {
//                var wndProc = element.GetType().GetMethod("WndProc");

//                if (wndProc != null)
//                {
//                    msg.Target = element;
//                    var args = new object[] { msg };

//                    if (!messagePump.PreFilterMessage(ref msg))
//                    {
//                        wndProc.Invoke(element, args);
//                    }
//                }
//            }
//        }

//        public static Point[] MapWindowPoints(IControl control, IControl mapTo, params Point[] points)
//        {
//            if (control is UIElement && mapTo is UIElement)
//            {
//                return MapWindowPoints((UIElement)control, (UIElement)mapTo, points);
//            }
//            else if (mapTo is UIElement)
//            {
//                return MapWindowPoints(control, (UIElement)mapTo, points);
//            } 
//            else if (control is UIElement)
//            {
//                return MapWindowPoints((UIElement)control, mapTo, points);
//            }
//            else
//            {
//                throw new NotImplementedException();
//            }
//        }

//        public static Point[] MapWindowPoints(UIElement control, IControl mapTo, params Point[] points)
//        {
//            if (mapTo is UIElement)
//            {
//                return MapWindowPoints(control, (UIElement)mapTo, points);
//            }
//            else
//            {
//                throw new NotImplementedException();
//            }
//        }

//        public static Point[] MapWindowPoints(IControl control, UIElement mapTo, params Point[] points)
//        {
//            var newPoints = new List<Point>();
//            var uiAncestor = control.GetUIAncestor();
//            var pointOffset = control.GetOffsetToUIAncestor();

//            foreach (var point in points)
//            {
//                var newPoint = uiAncestor.TransformToVisual(mapTo).Transform(point);

//                newPoint.X += pointOffset.X;
//                newPoint.Y += pointOffset.Y;

//                newPoints.Add(newPoint);
//            }

//            return newPoints.ToArray(); 
//        }

//        public static Rect MapWindowRect(UIElement control, UIElement mapTo, Rect rect)
//        {
//            var points = new Point[2];
//            var topLeft = rect.GetLocation();
//            var bottomRight = topLeft.Translate((int) rect.Width, (int) rect.Height);

//            points[0] = topLeft;
//            points[1] = bottomRight;

//            if (control == null)
//            {
//                control = Globals.CurrentRootVisual;
//            }

//            for (var x = 0; x < 2; x++)
//            {
//                points[x] = control.TransformToVisual(mapTo).Transform(points[x]);
//            }

//            return new Rect(points[0], points[1]);
//        }

//        public static Point[] MapWindowPoints(UIElement control, UIElement mapTo, params Point[] points)
//        {
//            var newPoints = new List<Point>();

//            if (control == null)
//            {
//                control = Globals.CurrentRootVisual;
//            }

//            foreach (var point in points)
//            {
//                var newPoint = control.TransformToVisual(mapTo).Transform(point);

//                newPoints.Add(newPoint);
//            }

//            return newPoints.ToArray(); 
//        }

//        internal static uint RegisterWindowMessage(string messageText)
//        {
//            uint maxMessage = 0;

//            if (registeredMessages.Count == 0)
//            {
//                maxMessage = (uint)Enum<WindowMessage>.GetMaxValue();

//                maxMessage++;

//                registeredMessages.Add(messageText, maxMessage);
//            }
//            else
//            {
//                maxMessage = registeredMessages.Values.Max();

//                maxMessage++;

//                registeredMessages.Add(messageText, maxMessage);
//            }

//            return maxMessage;
//        }

//        internal static Region CreateRectRgn(int left, int top, int right, int bottom)
//        {
//            var rect = new WindowsRect(left, top, right, bottom);

//            return new Region(rect);
//        }

//        internal static RegionFlag GetUpdateRgn(object target, ref Region region, bool erase)
//        {
//            var flag = RegionFlag.ERROR;

//            if (target is FrameworkElement)
//            {
//                var frameworkElement = (FrameworkElement)target;
//                var trackedElement = Globals.Elements[frameworkElement];
//                var updateRegion = trackedElement.UpdateRegion;

//                if (updateRegion == null)
//                {
//                    updateRegion = new Region(frameworkElement.GetClientRectangle());
//                }

//                var count = updateRegion.GetRegionData().Rectangles.Count;

//                if (erase)
//                {
//                    if (updateRegion != null)
//                    {
//                        var graphics = frameworkElement.CreateGraphics();

//                        graphics.Erase(updateRegion);
//                    }
//                }

//                region = updateRegion;

//                if (count > 1)
//                {
//                    flag = RegionFlag.COMPLEXREGION;
//                }
//                else if (count == 1)
//                {
//                    flag = RegionFlag.SIMPLEREGION;
//                }
//                else
//                {
//                    flag = RegionFlag.NULLREGION;
//                }
//            }

//            return flag;
//        }

//        internal static bool GetUpdateRect(object target, out WindowsRect clip, bool erase)
//        {
//            clip = WindowsRect.Empty;

//            if (target is FrameworkElement)
//            {
//                var frameworkElement = (FrameworkElement)target;
//                var clipRect = new Rect();
//                Region region = new Region(clipRect);

//                if (erase)
//                {
//                    GetUpdateRgn(target, ref region, true);
//                }
//                else
//                {
//                    region = Globals.Elements[frameworkElement].UpdateRegion;
//                }

//                foreach (var rect in region.GetRegionData().Rectangles)
//                {
//                    clipRect.Union(rect);
//                }

//                clip = clipRect.ToWindowsRect();
//            }

//            return clip != WindowsRect.Empty;
//        }

//        internal static bool ReleaseCapture()
//        {
//            return ReleaseCapture(null);
//        }

//        private static bool ReleaseCapture(object newCapture)
//        {
//            var messagePumpSet = false;

//            if (currentCapture != null)
//            {
//                Globals.Log.InfoFormat("ReleaseCapture for {0}", currentCapture.GetType().Name);
//            }

//            if (currentCapture is IControl)
//            {
//                var iControl = (IControl)currentCapture;
//                var handler = (iControl).GetWindowTargetHandler() as IWindowCaptureHandler;

//                if (handler != null)
//                {
//                    handler.SetCapture(iControl, false);
//                }

//                var uiAncestor = iControl.GetUIAncestor();
//                uiAncestor.ReleaseMouseCapture();

//                dragDropManager.NotifyReleaseCapture(uiAncestor);

//                messagePump.SetCapture(iControl, false);
//                messagePumpSet = true;
//            }
            
//            if (currentCapture is FrameworkElement)
//            {
//                var element = (FrameworkElement)currentCapture;

//                SendMessage(element, WindowMessage.CaptureChanged, null, newCapture);

//                element.ReleaseMouseCapture();
//                dragDropManager.NotifyReleaseCapture(element);

//                if (!messagePumpSet)
//                {
//                    messagePump.SetCapture(element, false);
//                }
//            }

//            currentCapture = null;

//            return true;
//        }

//        internal static object GetCapture()
//        {
//            return currentCapture;
//        }

//        internal static object SetCapture(IControl control)
//        {
//            if (control != null)
//            {
//                var handler = control.GetWindowTargetHandler() as IWindowCaptureHandler;

//                ReleaseCapture(control);

//                Globals.Log.InfoFormat("SetCapture for {0}", control.GetType().Name);

//                if (handler != null)
//                {
//                    handler.SetCapture(control, true);
//                }

//                var uiAncestor = control.GetUIAncestor();
//                uiAncestor.CaptureMouse();

//                dragDropManager.NotifySetCapture(uiAncestor);

//                currentCapture = control;
//            }
//            else if (control == currentCapture)
//            {
//                Debugger.Break();

//                ReleaseCapture(control);
//            }

//            return currentCapture;
//        }

//        internal static object SetCapture(FrameworkElement control)
//        {
//            if (control != null && currentCapture != control)
//            {
//                ReleaseCapture(control);

//                if (control is IControl)
//                {
//                    SetCapture((IControl)control);
//                }
//                else
//                {
//                    Globals.Log.InfoFormat("SetCapture for {0}", control.GetType().Name);
//                }

//                control.CaptureMouse();
//                currentCapture = control;
//            }

//            return currentCapture;
//        }

//        internal static void DefWndProc(ref Message m)
//        {
//            messagePump.DefWndProc(ref m);
//        }

//        internal static MouseHook SetWindowsHook(SetHookCode setHookCode, MouseHookProc hook, ReceiverNameScope receiverNameScope, Dispatcher dispatcher)
//        {
//            var mouseHook = new MouseHook(hook);

//            messagePump.AddWindowsHook(mouseHook);

//            return mouseHook;
//        }

//        internal static object CallNextHook(MouseHook mouseHook, HookCode code, WindowMessage msg, MouseHookInfo hookInfo)
//        {
//            return messagePump.CallNextHook(mouseHook, code, msg, hookInfo);
//        }

//        internal static void UnhookWindowsHookEx(MouseHook mouseHook)
//        {
//            messagePump.RemoveWindowsHook(mouseHook);
//        }

//        public static IEnumerable<IControl> GetAncestors(this IControl control)
//        {
//            var ancestor = control.GetParent();

//            while (ancestor != null)
//            {
//                yield return ancestor;

//                ancestor = ancestor.GetParent();
//            }
//        }

//        public static IControl FindAncestor(this IControl control, Func<IControl, bool> where)
//        {
//            var ancestor = control.GetParent();

//            while (ancestor != null)
//            {
//                if (where(ancestor))
//                {
//                    return ancestor;
//                }

//                ancestor = ancestor.GetParent();
//            }

//            return null;
//        }

//        internal static bool IsChild(FrameworkElement control, object child)
//        {
//            bool isChild = false;

//            using (var stopwatch = new UIRelationStopwatch(control))
//            {
//                if (!UIRelationCacheEngine.GetIsChild(control, child, ref isChild))
//                {
//                    if (child is IControl)
//                    {
//                        var iAncestor = ((IControl)child).FindAncestor(i =>
//                        {
//                            if (i == control)
//                            {
//                                return true;
//                            }

//                            return false;
//                        });

//                        if (iAncestor != null)
//                        {
//                            isChild = true;
//                        }
//                        else if (child is IVirtualChild)
//                        {
//                            isChild = IsChild(control, ((IVirtualChild)child).VirtualParent);
//                        }
//                    }
//                    else if (child is DependencyObject)
//                    {
//                        var dependencyObject = (DependencyObject)child;
//                        var ancestor = dependencyObject.FindAncestor(a =>
//                        {
//                            if (a == control)
//                            {
//                                return true;
//                            }
//                            else if (a is IControl)
//                            {
//                                var iAncestor = ((IControl)a).FindAncestor(i =>
//                                {
//                                    if (i == control)
//                                    {
//                                        return true;
//                                    }

//                                    return false;
//                                });

//                                if (a is IVirtualChild)
//                                {
//                                    return IsChild(control, ((IVirtualChild)a).VirtualParent);
//                                }
//                            }

//                            return false;
//                        });

//                        if (ancestor != null)
//                        {
//                            isChild = true;
//                        }
//                    }

//                    if (!stopwatch.Reentrant)
//                    {
//                        UIRelationCacheEngine.SetIsChild(control, child, isChild);
//                    }
//                }
//            }

//            return isChild;
//        }

//        internal static bool IsChild(IControl control, object child)
//        {
//            bool isChild = false;

//            using (var stopwatch = new UIRelationStopwatch(control))
//            {
//                if (!UIRelationCacheEngine.GetIsChild(control, child, ref isChild))
//                {
//                    if (child is IControl)
//                    {
//                        var iAncestor = ((IControl)child).FindAncestor(i =>
//                        {
//                            if (i == control)
//                            {
//                                return true;
//                            }

//                            return false;
//                        });

//                        if (iAncestor != null)
//                        {
//                            isChild = true;
//                        }
//                        else if (child is IVirtualChild)
//                        {
//                            isChild = IsChild(control, ((IVirtualChild)child).VirtualParent);
//                        }
//                    }
//                    else if (child is DependencyObject)
//                    {
//                        var dependencyObject = (DependencyObject)child;
//                        var ancestor = dependencyObject.FindAncestor(a =>
//                        {
//                            if (a == control)
//                            {
//                                return true;
//                            }
//                            else if (a is IControl)
//                            {
//                                var iAncestor = ((IControl)a).FindAncestor(i =>
//                                {
//                                    if (i == control)
//                                    {
//                                        return true;
//                                    }

//                                    return false;
//                                });

//                                if (a is IVirtualChild)
//                                {
//                                    return IsChild(control, ((IVirtualChild)a).VirtualParent);
//                                }
//                            }

//                            return false;
//                        });

//                        if (ancestor != null)
//                        {
//                            isChild = true;
//                        }
//                    }

//                    if (!stopwatch.Reentrant)
//                    {
//                        UIRelationCacheEngine.SetIsChild(control, child, isChild);
//                    }
//                }
//            }

//            return isChild;
//        }

//        internal static long GetMessageTime()
//        {
//            return messagePump.LastMessageTime;
//        }

//        internal static void ValidateRect(object p, object rect)
//        {
//            throw new NotImplementedException();
//        }

//        internal static void ValidateRect(object p, Rect rect)
//        {
//            throw new NotImplementedException();
//        }

//        internal static WindowsRect MapWindowRect(IControl iControl, IControl Control, WindowsRect clip)
//        {
//            throw new NotImplementedException();
//        }

//        internal static int GetKeyState(int p)
//        {
//            throw new NotImplementedException();
//        }

//        internal static void RedrawWindow(IControl control, Rect rect, Region region, RedrawWindowFlags redrawWindowFlags)
//        {
//        }

//        internal static DragDropOperation DoDragDrop(UIElement control, object data, DragDropEffects allowedEffects)
//        {
//            return dragDropManager.DoDragDrop(control, data, allowedEffects);
//        }

//        public static void RegisterDragDrop(FrameworkElement element, IAcceptDrop dropTarget)
//        {
//            dragDropManager.RegisterDragDrop(element, dropTarget);
//        }

//        public static bool IsWindow(IControl child)
//        {
//            return child.IsWindow();
//        }

//        public static IOverlayCanvas GetDesktopWindow()
//        {
//            var rootVisual = Application.Current.RootVisual;

//            if (rootVisual is UserControl)
//            {
//                var userControl = (UserControl)rootVisual;

//                if (userControl.Content is Grid)
//                {
//                    var layoutRoot = (Grid)userControl.Content;

//                    if (layoutRoot.Children.Count > 0)
//                    {
//                        var canvas = layoutRoot.Children.OfType<IOverlayCanvas>().SingleOrDefault();

//                        return canvas;
//                    }
//                }
//            }

//            return null;
//        }

//        internal static bool SetFocus(UIElement control)
//        {
//            if (control is Control)
//            {
//                ((Control)control).Focus();
//                return true;
//            }

//            return false;
//        }

//        internal static FrameworkElement GetForegroundWindow()
//        {
//            var desktopWindow = GetDesktopWindow();

//            return null;
//        }

//        internal static object GetFocus()
//        {
//            return FocusManager.GetFocusedElement();
//        }

//        internal static bool IsEnumValid<T>(T value, T lower, T higher)
//        {
//            foreach (var enumValue in Enum.GetValues(typeof(T)))
//            {
//                if (((int)(object)enumValue) < ((int)(object)lower))
//                {
//                    return false;
//                }
//                else if (((int)(object)enumValue) > ((int)(object)higher))
//                {
//                    return false;
//                }
//            }

//            return true;
//        }

//        public const int S_OK = 0;
    }
}
