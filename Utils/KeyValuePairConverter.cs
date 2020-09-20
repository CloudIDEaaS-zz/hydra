using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Script.Serialization;

namespace Utils
{
    public class KeyValuePairConverter : JavaScriptConverter
    {
        public override IEnumerable<Type> SupportedTypes
        {
            get
            {
                return new List<Type> { typeof(KeyValuePair<string, object>), typeof(string[][]) };
            }
        }

        public override IDictionary<string, object> Serialize(object obj, JavaScriptSerializer serializer)
        {
            var keyValuePairs = (KeyValuePair<string, object>)obj;
            var dictionary = new Dictionary<string, object>();

            dictionary.Add(keyValuePairs.Key, keyValuePairs.Value);

            return dictionary;
        }

        public override object Deserialize(IDictionary<string, object> dictionary, Type type, JavaScriptSerializer serializer)
        {
            return dictionary.SingleOrDefault();
        }
    }
}
