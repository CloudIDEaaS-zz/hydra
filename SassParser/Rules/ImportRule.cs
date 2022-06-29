﻿using System.IO;
using System.Linq;

namespace SassParser
{
    internal sealed class ImportRule : Rule
    {
        private Stylesheet _stylesheet;

        internal ImportRule(Token token, StylesheetParser parser) : base(RuleType.Import, token, parser)
        {
            AppendChild(new MediaList(token, parser));
        }

        public override void ToCss(TextWriter writer, IStyleFormatter formatter)
        {
            var media = Media.MediaText;
            var space = string.IsNullOrEmpty(media) ? string.Empty : " ";
            var value = string.Concat(Href.StylesheetUrl(), space, media);
            writer.Write(formatter.Rule("@import", value));
        }

        protected override void ReplaceWith(IRule rule)
        {
            var newRule = rule as ImportRule;
            Href = newRule?.Href;
            _stylesheet = null;
            base.ReplaceWith(rule);
        }


        public string Href { get; set; }
        public MediaList Media => Children.OfType<MediaList>().FirstOrDefault();
        public Stylesheet Sheet => _stylesheet;
    }
}