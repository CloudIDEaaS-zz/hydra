using System;
using System.Collections.Generic;

namespace SassParser
{
    internal static class ParserExtensions
    {
        private static readonly Dictionary<string, Func<string, DocumentFunction>> FunctionTypes =
            new Dictionary<string, Func<string, DocumentFunction>>(StringComparer.OrdinalIgnoreCase)
            {
                {FunctionNames.Url, url => new UrlFunction(url, null)},
                {FunctionNames.Domain, url => new DomainFunction(url, null)},
                {FunctionNames.UrlPrefix, url => new UrlPrefixFunction(url, null)}
            };

        private static readonly Dictionary<string, Func<IEnumerable<IConditionFunction>, Token, IConditionFunction>>
            GroupCreators =
                new Dictionary<string, Func<IEnumerable<IConditionFunction>, Token, IConditionFunction>>(
                    StringComparer.OrdinalIgnoreCase)
                {
                    {Keywords.And, CreateAndCondition},
                    {Keywords.Or, CreateOrCondition}
                };

        private static IConditionFunction CreateAndCondition(IEnumerable<IConditionFunction> conditions, Token token)
        {
            var andCondition = new AndCondition(token);
            foreach (var condition in conditions)
            {
                andCondition.AppendChild(condition);
            }
            return andCondition;
        }

        private static IConditionFunction CreateOrCondition(IEnumerable<IConditionFunction> conditions, Token token)
        {
            var orCondition = new OrCondition(token);
            foreach (var condition in conditions)
            {
                orCondition.AppendChild(condition);
            }
            return orCondition;
        }

        public static TokenType GetTypeFromName(this string functionName)
        {
            return FunctionTypes.TryGetValue(functionName, out Func<string, DocumentFunction> creator) 
                ? TokenType.Url 
                : TokenType.Function;
        }

        public static Func<IEnumerable<IConditionFunction>, Token, IConditionFunction> GetCreator(this string conjunction)
        {
            GroupCreators.TryGetValue(conjunction, out Func<IEnumerable<IConditionFunction>, Token, IConditionFunction> creator);
            return creator;
        }

        public static int GetCode(this ParseError code)
        {
            return (int) code;
        }

        public static bool Is(this Token token, TokenType a, TokenType b)
        {
            var type = token.Type;
            return (type == a) || (type == b);
        }

        public static bool IsNot(this Token token, TokenType a, TokenType b)
        {
            var type = token.Type;
            return (type != a) && (type != b);
        }

        public static bool IsNot(this Token token, TokenType a, TokenType b, TokenType c)
        {
            var type = token.Type;
            return (type != a) && (type != b) && (type != c);
        }

        public static bool IsDeclarationName(this Token token)
        {
            return (token.Type != TokenType.EndOfFile) &&
                   (token.Type != TokenType.Colon) &&
                   (token.Type != TokenType.Whitespace) &&
                   (token.Type != TokenType.Comment) &&
                   (token.Type != TokenType.CurlyBracketOpen) &&
                   (token.Type != TokenType.Semicolon);
        }

        public static DocumentFunction ToDocumentFunction(this Token token)
        {
            if (token.Type == TokenType.Url)
            {
                var functionName = ((UrlToken)token).FunctionName;
                FunctionTypes.TryGetValue(functionName, out Func<string, DocumentFunction> creator);
                return creator(token.Data);
            }

            if ((token.Type == TokenType.Function) && token.Data.Isi(FunctionNames.Regexp))
            {
                var css = ((FunctionToken) token).ArgumentTokens.ToCssString();
                if (css != null)
                {
                    return new RegexpFunction(css, token);
                }
            }

            return null;
        }

        public static Rule CreateRule(this StylesheetParser parser, RuleType type, Token token)
        {
            switch (type)
            {
                case RuleType.Charset:
                    return new CharsetRule(token, parser);
                case RuleType.Document:
                    return new DocumentRule(token, parser);
                case RuleType.FontFace:
                    return new FontFaceRule(token, parser);
                case RuleType.Import:
                    return new ImportRule(token, parser);
                case RuleType.Keyframe:
                    return new KeyframeRule(token, parser);
                case RuleType.Keyframes:
                    return new KeyframesRule(token, parser);
                case RuleType.Media:
                    return new MediaRule(token, parser);
                case RuleType.Namespace:
                    return new NamespaceRule(token, parser);
                case RuleType.Page:
                    return new PageRule(token, parser);
                case RuleType.Style:
                    return new StyleRule(token, parser);
                case RuleType.Supports:
                    return new SupportsRule(token, parser);
                case RuleType.Viewport:
                    return new ViewportRule(token, parser);
                case RuleType.Unknown:
                case RuleType.RegionStyle:
                case RuleType.FontFeatureValues:
                case RuleType.CounterStyle:
                default:
                    return null;
            }
        }
    }
}