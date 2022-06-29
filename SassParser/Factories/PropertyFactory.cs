using System;
using System.Collections.Generic;
using System.Linq;

namespace SassParser
{
    internal sealed class PropertyFactory 
    {
        private static readonly Lazy<PropertyFactory> Lazy =
            new Lazy<PropertyFactory>(() => new PropertyFactory());

        internal static PropertyFactory Instance => Lazy.Value;

        private delegate Property LonghandCreator(Token token);
        private delegate ShorthandProperty ShorthandCreator(Token token);

        private readonly Dictionary<string, LonghandCreator> _longhands = new Dictionary<string, LonghandCreator>(StringComparer.OrdinalIgnoreCase);
        private readonly Dictionary<string, ShorthandCreator> _shorthands = new Dictionary<string, ShorthandCreator>(StringComparer.OrdinalIgnoreCase);
        private readonly Dictionary<string, LonghandCreator> _fonts = new Dictionary<string, LonghandCreator>(StringComparer.OrdinalIgnoreCase);
        private readonly Dictionary<string, string[]> _mappings = new Dictionary<string, string[]>();
        private readonly List<string> _animatables = new List<string>();

        private PropertyFactory()
        {
            AddShorthand(PropertyNames.Animation, (t) => new AnimationProperty(t),
                PropertyNames.AnimationName,
                PropertyNames.AnimationDuration,
                PropertyNames.AnimationTimingFunction,
                PropertyNames.AnimationDelay,
                PropertyNames.AnimationDirection,
                PropertyNames.AnimationFillMode,
                PropertyNames.AnimationIterationCount,
                PropertyNames.AnimationPlayState);
            AddLonghand(PropertyNames.AnimationDelay, (t) => new AnimationDelayProperty(t));
            AddLonghand(PropertyNames.AnimationDirection, (t) => new AnimationDirectionProperty(t));
            AddLonghand(PropertyNames.AnimationDuration, (t) => new AnimationDurationProperty(t));
            AddLonghand(PropertyNames.AnimationFillMode, (t) => new AnimationFillModeProperty(t));
            AddLonghand(PropertyNames.AnimationIterationCount, (t) => new AnimationIterationCountProperty(t));
            AddLonghand(PropertyNames.AnimationName, (t) => new AnimationNameProperty(t));
            AddLonghand(PropertyNames.AnimationPlayState, (t) => new AnimationPlayStateProperty(t));
            AddLonghand(PropertyNames.AnimationTimingFunction, (t) => new AnimationTimingFunctionProperty(t));

            AddShorthand(PropertyNames.Background, (t) => new BackgroundProperty(t),
                PropertyNames.BackgroundAttachment,
                PropertyNames.BackgroundClip,
                PropertyNames.BackgroundColor,
                PropertyNames.BackgroundImage,
                PropertyNames.BackgroundOrigin,
                PropertyNames.BackgroundPosition,
                PropertyNames.BackgroundRepeat,
                PropertyNames.BackgroundSize);
            AddLonghand(PropertyNames.BackgroundAttachment, (t) => new BackgroundAttachmentProperty(t));
            AddLonghand(PropertyNames.BackgroundColor, (t) => new BackgroundColorProperty(t), true);
            AddLonghand(PropertyNames.BackgroundClip, (t) => new BackgroundClipProperty(t));
            AddLonghand(PropertyNames.BackgroundOrigin, (t) => new BackgroundOriginProperty(t));
            AddLonghand(PropertyNames.BackgroundSize, (t) => new BackgroundSizeProperty(t), true);
            AddLonghand(PropertyNames.BackgroundImage, (t) => new BackgroundImageProperty(t));
            AddLonghand(PropertyNames.BackgroundPosition, (t) => new BackgroundPositionProperty(t), true);
            AddLonghand(PropertyNames.BackgroundRepeat, (t) => new BackgroundRepeatProperty(t));

            AddLonghand(PropertyNames.BorderSpacing, (t) => new BorderSpacingProperty(t));
            AddLonghand(PropertyNames.BorderCollapse, (t) => new BorderCollapseProperty(t));
            AddLonghand(PropertyNames.BoxShadow, (t) => new BoxShadowProperty(t), true);
            AddLonghand(PropertyNames.BoxDecorationBreak, (t) => new BoxDecorationBreak(t));
            AddLonghand(PropertyNames.BreakAfter, (t) => new BreakAfterProperty(t));
            AddLonghand(PropertyNames.BreakBefore, (t) => new BreakBeforeProperty(t));
            AddLonghand(PropertyNames.BreakInside, (t) => new BreakInsideProperty(t));
            AddLonghand(PropertyNames.BackfaceVisibility, (t) => new BackfaceVisibilityProperty(t));

            AddShorthand(PropertyNames.BorderRadius, (t) => new BorderRadiusProperty(t),
                PropertyNames.BorderTopLeftRadius,
                PropertyNames.BorderTopRightRadius,
                PropertyNames.BorderBottomRightRadius,
                PropertyNames.BorderBottomLeftRadius);
            AddLonghand(PropertyNames.BorderTopLeftRadius, (t) => new BorderTopLeftRadiusProperty(t), true);
            AddLonghand(PropertyNames.BorderTopRightRadius, (t) => new BorderTopRightRadiusProperty(t), true);
            AddLonghand(PropertyNames.BorderBottomLeftRadius, (t) => new BorderBottomLeftRadiusProperty(t), true);
            AddLonghand(PropertyNames.BorderBottomRightRadius, (t) => new BorderBottomRightRadiusProperty(t), true);

            AddShorthand(PropertyNames.BorderImage, (t) => new BorderImageProperty(t),
                PropertyNames.BorderImageOutset,
                PropertyNames.BorderImageRepeat,
                PropertyNames.BorderImageSlice,
                PropertyNames.BorderImageSource,
                PropertyNames.BorderImageWidth);
            AddLonghand(PropertyNames.BorderImageOutset, (t) => new BorderImageOutsetProperty(t));
            AddLonghand(PropertyNames.BorderImageRepeat, (t) => new BorderImageRepeatProperty(t));
            AddLonghand(PropertyNames.BorderImageSource, (t) => new BorderImageSourceProperty(t));
            AddLonghand(PropertyNames.BorderImageSlice, (t) => new BorderImageSliceProperty(t));
            AddLonghand(PropertyNames.BorderImageWidth, (t) => new BorderImageWidthProperty(t));

            AddShorthand(PropertyNames.BorderColor, (t) => new BorderColorProperty(t),
                PropertyNames.BorderTopColor,
                PropertyNames.BorderRightColor,
                PropertyNames.BorderBottomColor,
                PropertyNames.BorderLeftColor);
            AddShorthand(PropertyNames.BorderStyle, (t) => new BorderStyleProperty(t),
                PropertyNames.BorderTopStyle,
                PropertyNames.BorderRightStyle,
                PropertyNames.BorderBottomStyle,
                PropertyNames.BorderLeftStyle);
            AddShorthand(PropertyNames.BorderWidth, (t) => new BorderWidthProperty(t),
                PropertyNames.BorderTopWidth,
                PropertyNames.BorderRightWidth,
                PropertyNames.BorderBottomWidth,
                PropertyNames.BorderLeftWidth);
            AddShorthand(PropertyNames.BorderTop, (t) => new BorderTopProperty(t),
                PropertyNames.BorderTopWidth,
                PropertyNames.BorderTopStyle,
                PropertyNames.BorderTopColor);
            AddShorthand(PropertyNames.BorderRight, (t) => new BorderRightProperty(t),
                PropertyNames.BorderRightWidth,
                PropertyNames.BorderRightStyle,
                PropertyNames.BorderRightColor);
            AddShorthand(PropertyNames.BorderBottom, (t) => new BorderBottomProperty(t),
                PropertyNames.BorderBottomWidth,
                PropertyNames.BorderBottomStyle,
                PropertyNames.BorderBottomColor);
            AddShorthand(PropertyNames.BorderLeft, (t) => new BorderLeftProperty(t),
                PropertyNames.BorderLeftWidth,
                PropertyNames.BorderLeftStyle,
                PropertyNames.BorderLeftColor);

            AddShorthand(PropertyNames.Border, (t) => new BorderProperty(t),
                PropertyNames.BorderTopWidth,
                PropertyNames.BorderTopStyle,
                PropertyNames.BorderTopColor,
                PropertyNames.BorderRightWidth,
                PropertyNames.BorderRightStyle,
                PropertyNames.BorderRightColor,
                PropertyNames.BorderBottomWidth,
                PropertyNames.BorderBottomStyle,
                PropertyNames.BorderBottomColor,
                PropertyNames.BorderLeftWidth,
                PropertyNames.BorderLeftStyle,
                PropertyNames.BorderLeftColor);
            AddLonghand(PropertyNames.BorderTopColor, (t) => new BorderTopColorProperty(t), true);
            AddLonghand(PropertyNames.BorderLeftColor, (t) => new BorderLeftColorProperty(t), true);
            AddLonghand(PropertyNames.BorderRightColor, (t) => new BorderRightColorProperty(t), true);
            AddLonghand(PropertyNames.BorderBottomColor, (t) => new BorderBottomColorProperty(t), true);
            AddLonghand(PropertyNames.BorderTopStyle, (t) => new BorderTopStyleProperty(t));
            AddLonghand(PropertyNames.BorderLeftStyle, (t) => new BorderLeftStyleProperty(t));
            AddLonghand(PropertyNames.BorderRightStyle, (t) => new BorderRightStyleProperty(t));
            AddLonghand(PropertyNames.BorderBottomStyle, (t) => new BorderBottomStyleProperty(t));
            AddLonghand(PropertyNames.BorderTopWidth, (t) => new BorderTopWidthProperty(t), true);
            AddLonghand(PropertyNames.BorderLeftWidth, (t) => new BorderLeftWidthProperty(t), true);
            AddLonghand(PropertyNames.BorderRightWidth, (t) => new BorderRightWidthProperty(t), true);
            AddLonghand(PropertyNames.BorderBottomWidth, (t) => new BorderBottomWidthProperty(t), true);

            AddLonghand(PropertyNames.Bottom, (t) => new BottomProperty(t), true);

            AddShorthand(PropertyNames.Columns, (t) => new ColumnsProperty(t),
                PropertyNames.ColumnWidth,
                PropertyNames.ColumnCount);
            AddLonghand(PropertyNames.ColumnCount, (t) => new ColumnCountProperty(t), true);
            AddLonghand(PropertyNames.ColumnWidth, (t) => new ColumnWidthProperty(t), true);

            AddLonghand(PropertyNames.ColumnFill, (t) => new ColumnFillProperty(t));
            AddLonghand(PropertyNames.ColumnGap, (t) => new ColumnGapProperty(t), true);
            AddLonghand(PropertyNames.ColumnSpan, (t) => new ColumnSpanProperty(t));

            AddShorthand(PropertyNames.ColumnRule, (t) => new ColumnRuleProperty(t),
                PropertyNames.ColumnRuleWidth,
                PropertyNames.ColumnRuleStyle,
                PropertyNames.ColumnRuleColor);
            AddLonghand(PropertyNames.ColumnRuleColor, (t) => new ColumnRuleColorProperty(t), true);
            AddLonghand(PropertyNames.ColumnRuleStyle, (t) => new ColumnRuleStyleProperty(t));
            AddLonghand(PropertyNames.ColumnRuleWidth, (t) => new ColumnRuleWidthProperty(t), true);

            AddLonghand(PropertyNames.CaptionSide, (t) => new CaptionSideProperty(t));
            AddLonghand(PropertyNames.Clear, (t) => new ClearProperty(t));
            AddLonghand(PropertyNames.Clip, (t) => new ClipProperty(t), true);
            AddLonghand(PropertyNames.Color, (t) => new ColorProperty(t), true);
            AddLonghand(PropertyNames.Content, (t) => new ContentProperty(t));
            AddLonghand(PropertyNames.CounterIncrement, (t) => new CounterIncrementProperty(t));
            AddLonghand(PropertyNames.CounterReset, (t) => new CounterResetProperty(t));
            AddLonghand(PropertyNames.Cursor, (t) => new CursorProperty(t));
            AddLonghand(PropertyNames.Direction, (t) => new DirectionProperty(t));
            AddLonghand(PropertyNames.Display, (t) => new DisplayProperty(t));
            AddLonghand(PropertyNames.EmptyCells, (t) => new EmptyCellsProperty(t));
            AddLonghand(PropertyNames.Fill, (t) => new FillProperty(t), true);
            AddLonghand(PropertyNames.FillOpacity, (t) => new FillOpacityProperty(t), true);
            AddLonghand(PropertyNames.FillRule, (t) => new FillRuleProperty(t), true);
            AddLonghand(PropertyNames.Float, (t) => new FloatProperty(t));

            AddShorthand(PropertyNames.Font, (t) => new FontProperty(t),
                PropertyNames.FontFamily,
                PropertyNames.FontSize,
                PropertyNames.FontStretch,
                PropertyNames.FontStyle,
                PropertyNames.FontVariant,
                PropertyNames.FontWeight,
                PropertyNames.LineHeight);
            AddLonghand(PropertyNames.FontFamily, (t) => new FontFamilyProperty(t), false, true);
            AddLonghand(PropertyNames.FontSize, (t) => new FontSizeProperty(t), true);
            AddLonghand(PropertyNames.FontSizeAdjust, (t) => new FontSizeAdjustProperty(t), true);
            AddLonghand(PropertyNames.FontStyle, (t) => new FontStyleProperty(t), false, true);
            AddLonghand(PropertyNames.FontVariant, (t) => new FontVariantProperty(t), false, true);
            AddLonghand(PropertyNames.FontWeight, (t) => new FontWeightProperty(t), true, true);
            AddLonghand(PropertyNames.FontStretch, (t) => new FontStretchProperty(t), true, true);
            AddLonghand(PropertyNames.LineHeight, (t) => new LineHeightProperty(t), true);

            AddLonghand(PropertyNames.Height, (t) => new HeightProperty(t), true);
            AddLonghand(PropertyNames.JustifyContent, (t) => new JustifyContentProperty(t));
            AddLonghand(PropertyNames.Left, (t) => new LeftProperty(t), true);
            AddLonghand(PropertyNames.LetterSpacing, (t) => new LetterSpacingProperty(t));

            AddShorthand(PropertyNames.ListStyle, (t) => new ListStyleProperty(t),
                PropertyNames.ListStyleType,
                PropertyNames.ListStyleImage,
                PropertyNames.ListStylePosition);
            AddLonghand(PropertyNames.ListStyleImage, (t) => new ListStyleImageProperty(t));
            AddLonghand(PropertyNames.ListStylePosition, (t) => new ListStylePositionProperty(t));
            AddLonghand(PropertyNames.ListStyleType, (t) => new ListStyleTypeProperty(t));

            AddShorthand(PropertyNames.Margin, (t) => new MarginProperty(t),
                PropertyNames.MarginTop,
                PropertyNames.MarginRight,
                PropertyNames.MarginBottom,
                PropertyNames.MarginLeft);
            AddLonghand(PropertyNames.MarginRight, (t) => new MarginRightProperty(t), true);
            AddLonghand(PropertyNames.MarginLeft, (t) => new MarginLeftProperty(t), true);
            AddLonghand(PropertyNames.MarginTop, (t) => new MarginTopProperty(t), true);
            AddLonghand(PropertyNames.MarginBottom, (t) => new MarginBottomProperty(t), true);

            AddLonghand(PropertyNames.MaxHeight, (t) => new MaxHeightProperty(t), true);
            AddLonghand(PropertyNames.MaxWidth, (t) => new MaxWidthProperty(t), true);
            AddLonghand(PropertyNames.MinHeight, (t) => new MinHeightProperty(t), true);
            AddLonghand(PropertyNames.MinWidth, (t) => new MinWidthProperty(t), true);
            AddLonghand(PropertyNames.Opacity, (t) => new OpacityProperty(t), true);
            AddLonghand(PropertyNames.Orphans, (t) => new OrphansProperty(t));

            AddShorthand(PropertyNames.Outline, (t) => new OutlineProperty(t),
                PropertyNames.OutlineWidth,
                PropertyNames.OutlineStyle,
                PropertyNames.OutlineColor);
            AddLonghand(PropertyNames.OutlineColor, (t) => new OutlineColorProperty(t), true);
            AddLonghand(PropertyNames.OutlineStyle, (t) => new OutlineStyleProperty(t));
            AddLonghand(PropertyNames.OutlineWidth, (t) => new OutlineWidthProperty(t), true);

            AddLonghand(PropertyNames.Overflow, (t) => new OverflowProperty(t));
            AddLonghand(PropertyNames.OverflowWrap, (t) => new OverflowWrapProperty(t));

            AddShorthand(PropertyNames.Padding, (t) => new PaddingProperty(t),
                PropertyNames.PaddingTop,
                PropertyNames.PaddingRight,
                PropertyNames.PaddingBottom,
                PropertyNames.PaddingLeft);
            AddLonghand(PropertyNames.PaddingTop, (t) => new PaddingTopProperty(t), true);
            AddLonghand(PropertyNames.PaddingRight, (t) => new PaddingRightProperty(t), true);
            AddLonghand(PropertyNames.PaddingLeft, (t) => new PaddingLeftProperty(t), true);
            AddLonghand(PropertyNames.PaddingBottom, (t) => new PaddingBottomProperty(t), true);

            AddLonghand(PropertyNames.PageBreakAfter, (t) => new PageBreakAfterProperty(t));
            AddLonghand(PropertyNames.PageBreakBefore, (t) => new PageBreakBeforeProperty(t));
            AddLonghand(PropertyNames.PageBreakInside, (t) => new PageBreakInsideProperty(t));
            AddLonghand(PropertyNames.Perspective, (t) => new PerspectiveProperty(t), true);
            AddLonghand(PropertyNames.PerspectiveOrigin, (t) => new PerspectiveOriginProperty(t), true);
            AddLonghand(PropertyNames.Position, (t) => new PositionProperty(t));
            AddLonghand(PropertyNames.Quotes, (t) => new QuotesProperty(t));
            AddLonghand(PropertyNames.Right, (t) => new RightProperty(t), true);
            AddLonghand(PropertyNames.Stroke, (t) => new StrokeProperty(t), true);
            AddLonghand(PropertyNames.StrokeDasharray, (t) => new StrokeDasharrayProperty(t), true);
            AddLonghand(PropertyNames.StrokeDashoffset, (t) => new StrokeDashoffsetProperty(t), true);
            AddLonghand(PropertyNames.StrokeLinecap, (t) => new StrokeLinecapProperty(t), true);
            AddLonghand(PropertyNames.StrokeLinejoin, (t) => new StrokeLinejoinProperty(t), true);
            AddLonghand(PropertyNames.StrokeMiterlimit, (t) => new StrokeMiterlimitProperty(t), true);
            AddLonghand(PropertyNames.StrokeOpacity, (t) => new StrokeOpacityProperty(t), true);
            AddLonghand(PropertyNames.StrokeWidth, (t) => new StrokeWidthProperty(t), true);
            AddLonghand(PropertyNames.TableLayout, (t) => new TableLayoutProperty(t));
            AddLonghand(PropertyNames.TextAlign, (t) => new TextAlignProperty(t));
            AddLonghand(PropertyNames.TextAlignLast, (t) => new TextAlignLastProperty(t));
            AddLonghand(PropertyNames.TextAnchor, (t) => new TextAnchorProperty(t));

            AddShorthand(PropertyNames.TextDecoration, (t) => new TextDecorationProperty(t),
                PropertyNames.TextDecorationLine,
                PropertyNames.TextDecorationStyle,
                PropertyNames.TextDecorationColor);
            AddLonghand(PropertyNames.TextDecorationStyle, (t) => new TextDecorationStyleProperty(t));
            AddLonghand(PropertyNames.TextDecorationLine, (t) => new TextDecorationLineProperty(t));
            AddLonghand(PropertyNames.TextDecorationColor, (t) => new TextDecorationColorProperty(t), true);

            AddLonghand(PropertyNames.TextIndent, (t) => new TextIndentProperty(t), true);
            AddLonghand(PropertyNames.TextJustify, (t) => new TextJustifyProperty(t));
            AddLonghand(PropertyNames.TextTransform, (t) => new TextTransformProperty(t));
            AddLonghand(PropertyNames.TextShadow, (t) => new TextShadowProperty(t), true);
            AddLonghand(PropertyNames.Transform, (t) => new TransformProperty(t), true);
            AddLonghand(PropertyNames.TransformOrigin, (t) => new TransformOriginProperty(t), true);
            AddLonghand(PropertyNames.TransformStyle, (t) => new TransformStyleProperty(t));

            AddShorthand(PropertyNames.Transition, (t) => new TransitionProperty(t),
                PropertyNames.TransitionProperty,
                PropertyNames.TransitionDuration,
                PropertyNames.TransitionTimingFunction,
                PropertyNames.TransitionDelay);
            AddLonghand(PropertyNames.TransitionDelay, (t) => new TransitionDelayProperty(t));
            AddLonghand(PropertyNames.TransitionDuration, (t) => new TransitionDurationProperty(t));
            AddLonghand(PropertyNames.TransitionTimingFunction, (t) => new TransitionTimingFunctionProperty(t));
            AddLonghand(PropertyNames.TransitionProperty, (t) => new TransitionPropertyProperty(t));

            AddLonghand(PropertyNames.Top, (t) => new TopProperty(t), true);
            AddLonghand(PropertyNames.UnicodeBidirectional, (t) => new UnicodeBidirectionalProperty(t));
            AddLonghand(PropertyNames.VerticalAlign, (t) => new VerticalAlignProperty(t), true);
            AddLonghand(PropertyNames.Visibility, (t) => new VisibilityProperty(t), true);
            AddLonghand(PropertyNames.WhiteSpace, (t) => new WhiteSpaceProperty(t));
            AddLonghand(PropertyNames.Widows, (t) => new WidowsProperty(t));
            AddLonghand(PropertyNames.Width, (t) => new WidthProperty(t), true);
            AddLonghand(PropertyNames.WordBreak, (t) => new WordBreakProperty(t), true);
            AddLonghand(PropertyNames.WordSpacing, (t) => new WordSpacingProperty(t), true);
            AddLonghand(PropertyNames.WordWrap, (t) => new OverflowWrapProperty(t));
            AddLonghand(PropertyNames.ZIndex, (t) => new ZIndexProperty(t), true);
            AddLonghand(PropertyNames.ObjectFit, (t) => new ObjectFitProperty(t));
            AddLonghand(PropertyNames.ObjectPosition, (t) => new ObjectPositionProperty(t), true);

            _fonts.Add(PropertyNames.Src, (t) => new SrcProperty(t));
            _fonts.Add(PropertyNames.UnicodeRange, (t) => new UnicodeRangeProperty(t));
        }

        private void AddShorthand(string name, ShorthandCreator creator, params string[] longhands)
        {
            _shorthands.Add(name, creator);
            _mappings.Add(name, longhands);
        }

        private void AddLonghand(string name, LonghandCreator creator, bool animatable = false, bool font = false)
        {
            _longhands.Add(name, creator);

            if (animatable)
            {
                _animatables.Add(name);
            }

            if (font)
            {
                _fonts.Add(name, creator);
            }
        }

        public Property Create(string name, Token token)
        {
            return CreateLonghand(name, token) ?? CreateShorthand(name, token);
        }

        public Property CreateFont(string name, Token token)
        {
            return _fonts.TryGetValue(name, out var propertyCreator) ? propertyCreator(token) : null;
        }

        public Property CreateViewport(string name, Token token)
        {
            var feature = MediaFeatureFactory.Instance.Create(name, token);

            return feature != null ? new FeatureProperty(feature, token) : null;
        }

        public Property CreateLonghand(string name, Token token)
        {
            return _longhands.TryGetValue(name, out var createProperty) ? createProperty(token) : null;
        }

        public ShorthandProperty CreateShorthand(string name, Token token)
        {
            return _shorthands.TryGetValue(name, out var propertyCreator) ? propertyCreator(token) : null;
        }

        public Property[] CreateLonghandsFor(string name, Token token)
        {
            var propertyNames = GetLonghands(name);

            return propertyNames.Select(n => CreateLonghand(n, token)).ToArray();
        }

        public bool IsShorthand(string name)
        {
            return _shorthands.ContainsKey(name);
        }

        public bool IsAnimatable(string name)



        {
            return _longhands.ContainsKey(name) 
                ? _animatables.Contains(name) 
                : GetLonghands(name).Any(longhand => _animatables.Contains(name));
        }

        public string[] GetLonghands(string name)
        {
            return _mappings.ContainsKey(name) 
                ? _mappings[name] 
                : new string[0];
        }

        public IEnumerable<string> GetShorthands(string name)
        {
            foreach (var mapping in _mappings)
            {
                if (mapping.Value.Contains(name, StringComparison.OrdinalIgnoreCase))
                {
                    yield return mapping.Key;
                }
            }
        }
    }
}