using System.IO;

namespace SassParser
{
    public sealed class SimpleSelector : StylesheetNode, ISelector
    {
        public SimpleSelector(Token token) : this(Priority.Zero, Keywords.Asterisk, token)
        {
        }

        public SimpleSelector(string match, Token token) : this(Priority.OneTag, match, token)
        {
        }

        public SimpleSelector(Priority specifify, string code, Token token) : base(token)
        {
            Specifity = specifify;
            Text = code;
        }

        public static readonly SimpleSelector All = new SimpleSelector(null);
        public Priority Specifity { get; }
        public string Text { get; }

        public static SimpleSelector PseudoElement(string pseudoElement, Token token)
        {
            return new SimpleSelector(Priority.OneTag, PseudoElementNames.Separator + pseudoElement, token);
        }

        public static SimpleSelector PseudoClass(string pseudoClass, Token token) 
        {
            return new SimpleSelector(Priority.OneClass, PseudoClassNames.Separator + pseudoClass, token);
        }

        public static SimpleSelector Class(string match, Token token)
        {
            return new SimpleSelector(Priority.OneClass, "." + match, token);
        }

        public static SimpleSelector Id(string match, Token token)
        {
            return new SimpleSelector(Priority.OneId, "#" + match, token);
        }

        public static SimpleSelector AttrAvailable(string match, Token token, string prefix = null)
        {
            var front = match;

            if (!string.IsNullOrEmpty(prefix))
            {
                front = FormFront(prefix, match);
                _ = FormMatch(prefix, match);
            }

            var code = FormCode(front);
            return new SimpleSelector(Priority.OneClass, code, token);
        }

        public static SimpleSelector AttrMatch(string match, string value, Token token, string prefix = null)
        {
            var front = match;

            if (!string.IsNullOrEmpty(prefix))
            {
                front = FormFront(prefix, match);
                _ = FormMatch(prefix, match);
            }

            var code = FormCode(front, "=", value.StylesheetString());
            return new SimpleSelector(Priority.OneClass, code, token);
        }

        public static SimpleSelector AttrNotMatch(string match, string value, Token token, string prefix = null)
        {
            var front = match;

            if (!string.IsNullOrEmpty(prefix))
            {
                front = FormFront(prefix, match);
                _ = FormMatch(prefix, match);
            }

            var code = FormCode(front, "!=", value.StylesheetString());
            return new SimpleSelector(Priority.OneClass, code, token);
        }

        public static SimpleSelector AttrList(string match, string value, Token token, string prefix = null)
        {
            var front = match;

            if (!string.IsNullOrEmpty(prefix))
            {
                front = FormFront(prefix, match);
                _ = FormMatch(prefix, match);
            }

            var code = FormCode(front, "~=", value.StylesheetString());
            return new SimpleSelector(Priority.OneClass, code, token);
        }

        public static SimpleSelector AttrBegins(string match, string value, Token token, string prefix = null)
        {
            var front = match;

            if (!string.IsNullOrEmpty(prefix))
            {
                front = FormFront(prefix, match);
                _ = FormMatch(prefix, match);
            }

            var code = FormCode(front, "^=", value.StylesheetString());
            return new SimpleSelector(Priority.OneClass, code, token);
        }

        public static SimpleSelector AttrEnds(string match, string value, Token token, string prefix = null)
        {
            var front = match;

            if (!string.IsNullOrEmpty(prefix))
            {
                front = FormFront(prefix, match);
                _ = FormMatch(prefix, match);
            }

            var code = FormCode(front, "$=", value.StylesheetString());
            return new SimpleSelector(Priority.OneClass, code, token);
        }

        public static SimpleSelector AttrContains(string match, string value, Token token, string prefix = null)
        {
            var front = match;

            if (!string.IsNullOrEmpty(prefix))
            {
                front = FormFront(prefix, match);
                _ = FormMatch(prefix, match);
            }

            var code = FormCode(front, "*=", value.StylesheetString());
            return new SimpleSelector(Priority.OneClass, code, token);
        }

        public static SimpleSelector AttrHyphen(string match, string value, Token token, string prefix = null)
        {
            var front = match;

            if (!string.IsNullOrEmpty(prefix))
            {
                front = FormFront(prefix, match);
                _ = FormMatch(prefix, match);
            }

            var code = FormCode(front, "|=", value.StylesheetString());
            return new SimpleSelector(Priority.OneClass, code, token);
        }

        public static SimpleSelector Type(string match, Token token)
        {
            return new SimpleSelector(match, token);
        }

        public override void ToCss(TextWriter writer, IStyleFormatter formatter)
        {
            writer.Write(Text);
        }

        private static string FormCode(string content)
        {
            return string.Concat("[", content, "]");
        }

        private static string FormCode(string name, string op, string value)
        {
            var content = string.Concat(name, op, value);
            return FormCode(content);
        }

        private static string FormFront(string prefix, string match)
        {
            return string.Concat(prefix, Combinators.Pipe, match);
        }

        private static string FormMatch(string prefix, string match)
        {
            return prefix.Is(Keywords.Asterisk) ? match : string.Concat(prefix, PseudoClassNames.Separator, match);
        }
    }
}