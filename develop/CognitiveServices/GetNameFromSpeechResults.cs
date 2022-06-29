using System;
using System.Collections.Generic;
using System.Text;

namespace CognitiveServices
{
    public class NameFromSpeechResults
    {
        public string PossibleName { get; set; }
        public string Error { get; set; }

        public NameFromSpeechResults(string name, string error)
        {
            this.PossibleName = name;
            this.Error = error;
        }
    }   
}
