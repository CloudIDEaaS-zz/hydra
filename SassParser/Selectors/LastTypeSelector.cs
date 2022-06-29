
namespace SassParser
{
    internal sealed class LastTypeSelector : ChildSelector
    {
        public LastTypeSelector(Token token) : base(PseudoClassNames.NthLastOfType, token)
        {
        }
    }
}