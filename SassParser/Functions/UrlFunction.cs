
namespace SassParser
{
    internal sealed class UrlFunction : DocumentFunction
    {
        readonly Url _expected;

        public UrlFunction(string url, Token token) : base(FunctionNames.Url, url, token)
        {
            _expected = Url.Create(Data);
        }

        public override bool Matches(Url actual)
        {
            return !_expected.IsInvalid && _expected.Equals(actual);
        }
    }
}