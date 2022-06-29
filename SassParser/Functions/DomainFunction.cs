using System;

namespace SassParser
{
    internal sealed class DomainFunction : DocumentFunction
    {
        private readonly string _subdomain;

        public DomainFunction(string url, Token token) : base(FunctionNames.Domain, url, token)
        {
            _subdomain = "." + url;
        }

        public override bool Matches(Url url)
        {
            var domain = url.HostName;
            return domain.Isi(Data) || domain.EndsWith(_subdomain, StringComparison.OrdinalIgnoreCase);
        }
    }
}