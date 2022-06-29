
using System;

namespace SassParser
{
    internal sealed class UrlPrefixFunction : DocumentFunction
    {
        public UrlPrefixFunction(string url, Token token) : base(FunctionNames.UrlPrefix, url, token)
        {
        }

        public override bool Matches(Url url)
        {
            return url.Href.StartsWith(Data, StringComparison.OrdinalIgnoreCase);
        }
    }
}