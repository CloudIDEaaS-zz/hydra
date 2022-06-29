
namespace SassParser
{
    internal sealed class FontFaceRule : DeclarationRule, IFontFaceRule
    {
        internal FontFaceRule(Token token, StylesheetParser parser) : base(RuleType.FontFace, RuleNames.FontFace, token, parser)
        {
        }

        protected override Property CreateNewProperty(string name, Token token)
        {
            return PropertyFactory.Instance.CreateFont(name, token);
        }

        string IFontFaceRule.Family
        {
            get => GetValue(PropertyNames.FontFamily);
            set => SetValue(PropertyNames.FontFamily, value);
        }

        string IFontFaceRule.Source
        {
            get => GetValue(PropertyNames.Src);
            set => SetValue(PropertyNames.Src, value);
        }

        string IFontFaceRule.Style
        {
            get => GetValue(PropertyNames.FontStyle);
            set => SetValue(PropertyNames.FontStyle, value);
        }

        string IFontFaceRule.Weight
        {
            get => GetValue(PropertyNames.FontWeight);
            set => SetValue(PropertyNames.FontWeight, value);
        }

        string IFontFaceRule.Stretch
        {
            get => GetValue(PropertyNames.FontStretch);
            set => SetValue(PropertyNames.FontStretch, value);
        }

        string IFontFaceRule.Range
        {
            get => GetValue(PropertyNames.UnicodeRange);
            set => SetValue(PropertyNames.UnicodeRange, value);
        }

        string IFontFaceRule.Variant
        {
            get => GetValue(PropertyNames.FontVariant);
            set => SetValue(PropertyNames.FontVariant, value);
        }

        string IFontFaceRule.Features
        {
            get => string.Empty;
            set { }
        }
    }
}