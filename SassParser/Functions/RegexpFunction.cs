using System.Text.RegularExpressions;

namespace SassParser
{
    internal sealed class RegexpFunction : DocumentFunction
    {
        readonly Regex _regex;

        public RegexpFunction(string url, Token token) : base(FunctionNames.Regexp, url, token)
        {
            _regex = new Regex(url, RegexOptions.ECMAScript | RegexOptions.CultureInvariant);
        }

        public override bool Matches(Url url)
        {
            return _regex.IsMatch(url.Href);
        }
    }
}