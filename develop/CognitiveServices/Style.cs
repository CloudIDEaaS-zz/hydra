using System;
using System.Collections.Generic;
using System.Text;

namespace CognitiveServices
{
    public class Style
    {
        public string Emotion { get; set; }
        public string StyleName { get; set; }
        public string Description { get; set; }

        public Style(string styleName, string emotion, string description)
        {
            this.Emotion = emotion;
            this.StyleName = styleName;
            this.Description = description;
        }
    }
}
