using System.IO;

namespace SassParser
{
    internal sealed class DeclarationCondition : StylesheetNode, IConditionFunction
    {
        private readonly Property _property;
        private readonly TokenValue _tokenValue;

        public DeclarationCondition(Property property, Token token, TokenValue tokenValue) : base(token)
        {
            _property = property;
            _tokenValue = tokenValue;
        }

        public bool Check()
        {
            return (_property is UnknownProperty == false) && 
                _property.TrySetValue(_tokenValue);
        }

        public override void ToCss(TextWriter writer, IStyleFormatter formatter)
        {
            var delcaration = formatter.Declaration(_property.Name, _tokenValue.Text, _property.IsImportant);
            writer.Write($"({delcaration})");
        }
    }
}