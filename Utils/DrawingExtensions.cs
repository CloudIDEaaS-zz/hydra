using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Diagnostics;
using System.Windows.Forms;
using ColorMine.ColorSpaces;
using ColorMine.ColorSpaces.Comparisons;

namespace Utils
{
    public enum ColorCompareOption
    {
        CIE2000
    }

    [Flags]
    public enum EdgeStyle : uint
    {
        RaisedOuter = 0x0001,
        SunkenOuter = 0x0002,
        RaisedInner = 0x0004,
        SunkenInner = 0x0008,
        Raised = 0x0005,
        Sunken = 0x000a,
        EdgeRaised = (RaisedOuter | RaisedInner),
        EdgeSunken = (SunkenOuter | SunkenInner),
        EdgeEtched = (SunkenOuter | RaisedInner),
        EdgeBump = (RaisedOuter | SunkenInner)
    }

    [Flags]
    public enum BorderFlags : uint
    {
        Left = 0x0001,
        Top = 0x0002,
        Right = 0x0004,
        Bottom = 0x0008,
        Adjust = 0x2000,
        Flat = 0x4000,
        Middle = 0x0800,
        BottomLeft = Bottom | Left,
        BottomRight = Bottom | Right,
        Diagonal = 0x10,
        DiagonalEndBottomLeft = Diagonal | Bottom | Left,
        DiagonalEndBottomRight = Diagonal | Bottom | Right,
        DiagonalEndTopLeft = Diagonal | Top | Left,
        DiagonalEndTopRight = Diagonal | Top | Right,
        Mono = 0x8000,
        Rect = Left | Top | Right | Bottom,
        Soft = 0x1000,
        TopLeft = Top | Left,
        TopRight = Top | Right    
    }

    [Flags]
    public enum RegionFlags
    {
        ERROR = 0,
        NULLREGION = 1,
        SIMPLEREGION = 2,
        COMPLEXREGION = 3,
    }

    [DebuggerDisplay(" { DebugInfo } "), StructLayout(LayoutKind.Sequential)]
    public struct RECT
    {
        public int Left, Top, Right, Bottom;

        public RECT(int left, int top, int right, int bottom)
        {
            Left = left;
            Top = top;
            Right = right;
            Bottom = bottom;
        }

        public RECT(Rectangle r) : this(r.Left, r.Top, r.Right, r.Bottom) { }

        public int X
        {
            get { return Left; }
            set { Right -= (Left - value); Left = value; }
        }

        public int Y
        {
            get { return Top; }
            set { Bottom -= (Top - value); Top = value; }
        }

        public int Height
        {
            get { return Bottom - Top; }
            set { Bottom = value + Top; }
        }

        public int Width
        {
            get { return Right - Left; }
            set { Right = value + Left; }
        }

        public Point Location
        {
            get { return new Point(Left, Top); }
            set { X = value.X; Y = value.Y; }
        }

        public Size Size
        {
            get { return new Size(Width, Height); }
            set { Width = value.Width; Height = value.Height; }
        }

        public static implicit operator Rectangle(RECT r)
        {
            return new Rectangle(r.Left, r.Top, r.Width, r.Height);
        }

        public static implicit operator RECT(Rectangle r)
        {
            return new RECT(r);
        }

        public static bool operator ==(RECT r1, RECT r2)
        {
            return r1.Equals(r2);
        }

        public static bool operator !=(RECT r1, RECT r2)
        {
            return !r1.Equals(r2);
        }

        public bool Equals(RECT r)
        {
            return r.Left == Left && r.Top == Top && r.Right == Right && r.Bottom == Bottom;
        }

        public override bool Equals(object obj)
        {
            if (obj is RECT)
                return Equals((RECT)obj);
            else if (obj is Rectangle)
                return Equals(new RECT((Rectangle)obj));
            return false;
        }

        public string DebugInfo
        {
            get
            {
                return this.ToString();
            }
        }

        public override int GetHashCode()
        {
            return ((Rectangle)this).GetHashCode();
        }

        public override string ToString()
        {
            return string.Format(System.Globalization.CultureInfo.CurrentCulture, "{{Left={0},Top={1},Right={2},Bottom={3}}}", Left, Top, Right, Bottom);
        }

        public bool Contains(RECT rect)
        {
            var thisRect = (Rectangle)this;
            var otherRect = (Rectangle)rect;

            return thisRect.Contains(otherRect);
        }
    }

    public static class DrawingExtensions
    {
        [DllImport("gdi32.dll")]
        private static extern bool GetTextExtentPoint32(IntPtr hdc, string lpString, int cbString, ref Size lpSize);
        [DllImport("user32.dll")]
        private static extern bool DrawEdge(IntPtr hdc, ref RECT rc, EdgeStyle edge, BorderFlags grfFlags);
        [DllImport("GDI32.dll")]
        public static extern bool DeleteObject(IntPtr objectHandle);
        [DllImport("gdi32.dll", EntryPoint = "SelectObject")]
        public static extern IntPtr SelectObject([In] IntPtr hdc, [In] IntPtr hgdiobj);
        [DllImport("gdi32.dll")]
        private static extern RegionFlags SelectClipRgn(IntPtr hdc, IntPtr hrgn);
        [DllImport("gdi32.dll")]
        private static extern int GetClipRgn(IntPtr hdc, IntPtr hrgn);
        [DllImport("gdi32.dll")]
        private static extern RegionFlags ExcludeClipRect(IntPtr hdc, int nLeftRect, int nTopRect, int nRightRect, int nBottomRect);
        private const double DegToRad = Math.PI / 180;

        public static Rectangle GetCenteredRect(this Rectangle outerRect, Size innerRectSize)
        {
            var rect = new Rectangle(Point.Empty, innerRectSize);
            var x = outerRect.X + (outerRect.Width / 2 - innerRectSize.Width / 2);
            var y = outerRect.Y + (outerRect.Height / 2 - innerRectSize.Height / 2);
            
            rect.Location = new Point(x, y);

            return rect;
        }

        public static bool AdjacentWith(this Rectangle a, Rectangle b)
        {
            if (a.X == b.X)
            {
                return true;
            }
            else if (a.Y == b.Y)
            {
                return true;
            }
            else if (a.X + a.Width == b.X)
            {
                return true;
            }
            else if (a.Y + a.Height == b.Y)
            {
                return true;
            }
            else if (b.X + b.Width == a.X)
            {
                return true;
            }
            else if (b.Y + b.Height == a.Y)
            {
                return true;
            }

            return false;
        }

        public static Point Rotate(this Point point, double degrees)
        {
            var vector = new System.Windows.Vector(point.X, point.Y);

            vector = vector.RotateRadians(degrees * DegToRad);

            return new Point((int)vector.X, (int)vector.Y);
        }

        public static System.Windows.Vector Rotate(this System.Windows.Vector vector, double degrees)
        {
            return vector.RotateRadians(degrees * DegToRad);
        }

        public static System.Windows.Vector RotateRadians(this System.Windows.Vector vector, double radians)
        {
            var cos = Math.Cos(radians);
            var sin = Math.Sin(radians);

            return new System.Windows.Vector(cos * vector.X - sin * vector.Y, sin * vector.X + cos * vector.Y);
        }

        public static Color HexStringToColor(string text, bool leadingIdentifier = false)
        {
            var n = (int) text.FromHexString(leadingIdentifier);

            return Color.FromArgb(n);
       }

        public static string ToHexString(this Color color, bool leadingIdentifier = false)
        {
            var r = color.R.ToHexString();
            var g = color.G.ToHexString();
            var b = color.B.ToHexString();

            return r + g + b; 
        }

        public static Point GetCenteredRectPosition(this Rectangle outerRect, Size innerRectSize)
        {
            var x = outerRect.Width / 2 - innerRectSize.Width / 2;
            var y = outerRect.Height / 2 - innerRectSize.Height / 2;

            return new Point(x, y);
        }

        public static Rectangle OffsetNew(this Rectangle rect, int x, int y)
        {
            rect.Offset(x, y);

            return rect;
        }

        public static void Offset(this List<Point> points, int dx, int dy)
        {
            for (var x = 0; x < points.Count; x++)
            {
                var point = points[x];

                point.Offset(dx, dy);

                points[x] = point;
            }
        }

        public static void LineToOffset(this List<Point> points, int dx, int dy)
        {
            var point = points.Last();

            point.Offset(dx, dy);

            points.Add(point);
        }

        public static void LineToXOffset(this List<Point> points, int dx)
        {
            var point = points.Last();

            point.Offset(dx, 0);

            points.Add(point);
        }

        public static void LineToYOffset(this List<Point> points, int dy)
        {
            var point = points.Last();

            point.Offset(0, dy);

            points.Add(point);
        }

        public static void LineTo(this List<Point> points, int x, int y)
        {
            var point = new Point(x, y);

            points.LineTo(point);
        }

        public static void LineTo(this List<Point> points, Point point)
        {
            points.Add(point);
        }

        public static void LineToX(this List<Point> points, int x)
        {
            var point = points.Last();
            
            point = new Point(x, point.Y);

            points.Add(point);
        }

        public static void LineToY(this List<Point> points, int y)
        {
            var point = points.Last();

            point = new Point(point.X, y);

            points.Add(point);
        }

        public static bool GetTextExtentPoint(this Graphics graphics, Font font, string text, ref Size size)
        {
            using (var hdc = graphics.GetDisposableHdc())
            {
                var hFont = font.ToHfont();

                using (var oldHFont = hdc.SelectObject(hFont))
                {
                    var result = GetTextExtentPoint32(hdc, text, text.Length, ref size);

                    return result;
                }
            }
        }

        public static bool GetTextExtentPoint(this Control control, string text, ref Size size)
        {
            using (var graphics = control.CreateGraphics())
            {
                var font = control.Font;

                using (var hdc = graphics.GetDisposableHdc())
                {
                    var hFont = font.ToHfont();

                    using (var oldHFont = hdc.SelectObject(hFont))
                    {
                        var result = GetTextExtentPoint32(hdc, text, text.Length, ref size);

                        return result;
                    }
                }
            }
        }

        public static DisposableHandle SelectObject(this Graphics graphics, Font font)
        {
            var hFont = font.ToHfont();

            return graphics.SelectObject(hFont);
        }

        public static DisposableHandle SelectObject(this DisposableHandle graphicsHandle, IntPtr gdiObject)
        {
            var hdc = graphicsHandle;
            var oldObject = SelectObject((IntPtr) hdc, gdiObject);
            var disposableHandle = new DisposableHandle(oldObject);

            disposableHandle.Disposed += (sender, e) =>
            {
                SelectObject(hdc, oldObject);
                DeleteObject(gdiObject);
            };

            return disposableHandle;
        }

        public static DisposableHandle SelectObject(this DisposableHandle graphicsHandle, Font font)
        {
            var hFont = font.ToHfont();

            return graphicsHandle.SelectObject(hFont);
        }

        public static DisposableHandle SelectObject(this Graphics graphics, IntPtr gdiObject)
        {
            var hdc = graphics.GetDisposableHdc();
            var oldObject = SelectObject(hdc, gdiObject);
            var disposableHandle = new DisposableHandle(oldObject);

            disposableHandle.Disposed += (sender, e) =>
            {
                SelectObject(hdc, oldObject);
                DeleteObject(gdiObject);

                hdc.Dispose();
            };

            return disposableHandle;
        }

        public static bool DrawEdge(this Graphics graphics, Rectangle rc, EdgeStyle edge, BorderFlags flags, Rectangle? _exclude = null)
        {
            var rect = new RECT(rc);

            using (var hdc = graphics.GetDisposableHdc())
            {
                bool returnValue;

                if (_exclude != null)
                {
                    var exclude = (Rectangle)_exclude;
                    var regionFlags = ExcludeClipRect(hdc, exclude.Left, exclude.Top, exclude.Right, exclude.Bottom);
                }

                returnValue = DrawEdge(hdc, ref rect, edge, flags);

                return returnValue;
            }
        }

        public static Size Swap(this Size size)
        {
            return new Size(size.Height, size.Width);
        }

        public static Color MidPoint(this Color color1, Color color2)
        {
            return color1.MidPoint(color2, .5f);
        }

        public static Point FindIntersection(this Line lineA, Line lineB)
        {
            double determinant = lineA.A * lineB.B - lineB.A * lineA.B;

            if (determinant.IsZero()) //lines are parallel
                return default(Point);

            //Cramer's Rule

            double x = (lineB.B * lineA.C - lineA.B * lineB.C) / determinant;
            double y = (lineA.A * lineB.C - lineB.A * lineA.C) / determinant;

            Point intersectionPoint = new Point((int) x, (int) y);

            return intersectionPoint;
        }

        private static bool IsInsideLine(Line line, double x, double y)
        {
            return ((x >= line.x1 && x <= line.x2)
                        || (x >= line.x2 && x <= line.x1))
                   && ((y >= line.y1 && y <= line.y2)
                        || (y >= line.y2 && y <= line.y1));
        }

        public const double NegativeMachineEpsilon = 1.1102230246251565e-16D;
        public const double PositiveMachineEpsilon = 2D * NegativeMachineEpsilon;

        public static bool IsZero(this double value)
        {
            //return Math.Abs(value) <= PositiveMachineEpsilon;

            if (value > PositiveMachineEpsilon ||
                value < -PositiveMachineEpsilon)
                return false;

            return true;
        }

        public static Color MidPoint(this Color color1, Color color2, float percentTo)
        {
            var r1 = color1.R;
            var g1 = color1.G;
            var b1 = color1.B;
            var r2 = color2.R;
            var g2 = color2.G;
            var b2 = color2.B;
            var percent1 = 1f - percentTo;
            var percent2 = percentTo;

            return Color.FromArgb((int)((r1 * percent1) + (r2 * percent2)), (int)((g1 * percent1) + (g2 * percent2)), (int)((b1 * percent1) + (b2 * percent2)));
        }

        public static Color Lighten(this Color color, double percent)
        {
            var hslColor = (HSLColor) color;

            if (percent < 0)
            {
                var margin = (((float)hslColor.Luminosity) * -percent);

                hslColor.Luminosity -= margin;
            }
            else
            {
                var margin = ((double) HSLColor.SCALE - hslColor.Luminosity) * percent;

                hslColor.Luminosity += margin;
            }

            return hslColor;
        }

        public static double Compare(this Color color, Color colorCompare, ColorCompareOption option)
        {
            var rgbColor = new Rgb(color.R, color.G, color.B);
            var rgbCompare = new Rgb(colorCompare.R, colorCompare.G, colorCompare.B);

            switch (option)
            {
                case ColorCompareOption.CIE2000:
                    return rgbColor.Compare(rgbCompare, new CieDe2000Comparison());
                default:
                    DebugUtils.Break();
                    return -1;
            }
        }

        public static Color SetLight(this Color color, double luminosity)
        {
            var hslColor = (HSLColor)color;

            e.AssertInRange(luminosity, 0, HSLColor.SCALE);

            hslColor.Luminosity = luminosity;

            return hslColor;
        }

        public static void DrawString(this Graphics graphics, string text, Font font, Brush brush, Rectangle layoutRectangle, StringAlignment alignment)
        {
            graphics.DrawString(text, font, brush, layoutRectangle, new StringFormat { LineAlignment = alignment, Alignment = alignment});
        }

        public static void DrawString(this Graphics graphics, string text, Font font, Brush brush, Rectangle layoutRectangle, StringAlignment alignment, StringFormatFlags stringFormatFlags)
        {
            graphics.DrawString(text, font, brush, layoutRectangle, new StringFormat(stringFormatFlags) { LineAlignment = alignment, Alignment = alignment });
        }

        public static void DrawString(this Graphics graphics, string text, Font font, Brush brush, Rectangle layoutRectangle, StringAlignment alignment, StringFormatFlags stringFormatFlags, StringTrimming trimming)
        {
            graphics.DrawString(text, font, brush, layoutRectangle, new StringFormat(stringFormatFlags) { Trimming = trimming, LineAlignment = alignment, Alignment = alignment });
        }

        public static void DrawString(this Graphics graphics, string text, Font font, Brush brush, Rectangle layoutRectangle, StringAlignment alignment, StringTrimming trimming)
        {
            graphics.DrawString(text, font, brush, layoutRectangle, new StringFormat { Trimming = trimming, LineAlignment = alignment, Alignment = alignment });
        }

        public static DisposableHandle GetDisposableHdc(this Graphics graphics)
        {
            var handle = new DisposableHandle(graphics.GetHdc());

            handle.Disposed += (sender, e) => graphics.ReleaseHdc();

            return handle;
        }
    }
}
