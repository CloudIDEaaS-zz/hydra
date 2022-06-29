﻿using System.IO;
using System.Linq;

namespace SassParser
{
    internal sealed class MediaRule : ConditionRule, IMediaRule
    {
        internal MediaRule(Token token, StylesheetParser parser) : base(RuleType.Media, token, parser)
        {
            AppendChild(new MediaList(token, parser));
        }
        
        public override void ToCss(TextWriter writer, IStyleFormatter formatter)
        {
            var rules = formatter.Block(Rules);
            writer.Write(formatter.Rule("@media", Media.MediaText, rules));
        }
       
        public string ConditionText
        {
            get => Media.MediaText;
            set => Media.MediaText = value;
        }

        public MediaList Media => Children.OfType<MediaList>().FirstOrDefault();
    }
}