
namespace SassParser
{
    internal sealed class FirstTypeSelector : ChildSelector
    {
        public FirstTypeSelector(Token token) : base(PseudoClassNames.NthOfType, token)
        {
        }
    }
}