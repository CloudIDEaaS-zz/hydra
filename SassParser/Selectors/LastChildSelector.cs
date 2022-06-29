
namespace SassParser
{
    internal sealed class LastChildSelector : ChildSelector
    {
        public LastChildSelector(Token token) : base(PseudoClassNames.NthLastChild, token)
        {
        }
    }
}