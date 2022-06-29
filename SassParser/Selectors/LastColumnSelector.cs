
namespace SassParser
{
    internal sealed class LastColumnSelector : ChildSelector
    {
        public LastColumnSelector(Token token) : base(PseudoClassNames.NthLastColumn, token)
        {
        }
    }
}