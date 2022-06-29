
namespace SassParser
{
    internal sealed class ViewportRule : DeclarationRule
    {
        internal ViewportRule(Token token, StylesheetParser parser) : base(RuleType.Viewport, RuleNames.ViewPort, token, parser)
        {
        }

        protected override Property CreateNewProperty(string name, Token token)
        {
            return PropertyFactory.Instance.CreateViewport(name, token);
        }
    }
}