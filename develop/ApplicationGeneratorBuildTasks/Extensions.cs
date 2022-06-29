using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using System.Xml.Serialization;
using Utils;

namespace ApplicationGeneratorBuildTasks
{
    public static class Extensions
    {
        public static XElement ToXElement<T>(this object obj)
        {
            using (var memoryStream = new MemoryStream())
            {
                using (TextWriter streamWriter = new StreamWriter(memoryStream))
                {
                    var ns = new XmlSerializerNamespaces();
                    var xmlSerializer = new XmlSerializer(typeof(T));
                    XElement element;

                    xmlSerializer.Serialize(streamWriter, obj);

                    element = XElement.Parse(Encoding.ASCII.GetString(memoryStream.ToArray()));

                    return element;
                }
            }
        }

        public static T FromXElement<T>(this XElement xElement)
        {
            var xmlSerializer = new XmlSerializer(typeof(T), string.Empty);
            return (T)xmlSerializer.Deserialize(xElement.CreateReader());
        }
    }
}
