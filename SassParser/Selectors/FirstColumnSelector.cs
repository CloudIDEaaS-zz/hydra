
namespace SassParser
{
    internal sealed class FirstColumnSelector : ChildSelector
    {
        public FirstColumnSelector(Token token) : base(PseudoClassNames.NthColumn, token)
        {
        }
    }
}