
namespace SassParser
{
    public class TokenizerError 
    {
        private readonly ParseError _code;
        public TextPosition Position { get; }
        public int Length { get; }
        public int Code => _code.GetCode();
        public string Message { get; }

        public TokenizerError(ParseError code, string message, TextPosition position, int length)
        {
            _code = code;
            Position = position;
            Message = message;
            Length = length;
        }
    }
}