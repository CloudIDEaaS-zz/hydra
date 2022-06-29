using System;
using System.Collections.Generic;
using System.Text;

namespace CognitiveServices
{
    public class SpeechLocal
    {
        public string FullName { get; }
        public string CultureCode { get; }
        public string Ethnicity { get; }

        public SpeechLocal(string fullName, string cultureCode, string ethnicity)
        {
            this.FullName = fullName;
            this.CultureCode = cultureCode;
            this.Ethnicity = ethnicity;
        }
    }
}
