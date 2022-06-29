using Microsoft.CognitiveServices.Speech;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Utils;

namespace CognitiveServices
{
    public static class Extensions
    {
        public static Dictionary<PropertyId, object> GetProperties(this SpeechSynthesisResult speechSynthesisResult)
        {
            var dictionary = new Dictionary<PropertyId, object>();
            var properties = speechSynthesisResult.Properties;

            foreach (var id in EnumUtils.GetValues<PropertyId>())
            {
                try
                {
                    dictionary.Add(id, properties.GetProperty(id));
                }
                catch
                {
                }
            }

            return dictionary;
        }
    }
}
