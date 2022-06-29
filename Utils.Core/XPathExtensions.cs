using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;
using System.Xml.XPath;

namespace Utils.XPath
{
    public static class XPathExtensions
    {
        /// <summary>
        /// Get the absolute XPath to a given XElement
        /// (e.g. "/people/person[6]/name[1]/last[1]").
        /// </summary>
        public static string GetAbsoluteXPath(this XObject obj, bool withValue = false)
        {
            var element = obj as XElement;
            var attribute = obj as XAttribute;
            string xPath;

            if (obj == null)
            {
                throw new ArgumentNullException(nameof(obj));
            }

            if (element == null)
            {
                element = attribute.Parent;
            }

            Func<XElement, string> relativeXPath = e =>
            {
                int index = e.IndexPosition();
                string name = e.Name.LocalName;

                // If the element is the root, no index is required

                return (index == -1) ? "/" + name : string.Format
                (
                    "/{0}[{1}]",
                    name,
                    index.ToString()
                );
            };

            var ancestors = from e in element.Ancestors()
                            select relativeXPath(e);

            xPath = string.Concat(ancestors.Reverse().ToArray()) + relativeXPath(element);

            if (attribute != null)
            {
                if (withValue)
                {
                    xPath += string.Format("[@{0}={1}]", attribute.Name.LocalName, attribute.Value);
                }
                else
                {
                    xPath += string.Format("[@{0}]", attribute.Name.LocalName);
                }
            }
            else if (withValue)
            {
                xPath += "/" + element.Value;
            }

            return xPath;
        }

        /// <summary>
        /// Get the index of the given XElement relative to its
        /// siblings with identical names. If the given element is
        /// the root, -1 is returned.
        /// </summary>
        /// <param name="element">
        /// The element to get the index of.
        /// </param>
        public static int IndexPosition(this XElement element)
        {
            if (element == null)
            {
                throw new ArgumentNullException("element");
            }

            if (element.Parent == null)
            {
                return -1;
            }

            int i = 1; // Indexes for nodes start at 1, not 0

            foreach (var sibling in element.Parent.Elements(element.Name))
            {
                if (sibling == element)
                {
                    return i;
                }

                i++;
            }

            throw new InvalidOperationException
                ("element has been removed from its parent.");
        }

        public static string MergeAttributes(this string xpathExpression, string newAttributesPart)
        {
            return xpathExpression.RegexReplace("]$", " and " + newAttributesPart + "]");
        }

        public static XElement XPathSelectElement(this FileInfo file, string path, string prefix = null, string uri = null)
        {
            using (var reader = new StreamReader(file.FullName))
            {
                var document = XDocument.Load(reader);

                if (prefix != null)
                {
                    var namespaceManager = new XmlNamespaceManager(new NameTable());
                    namespaceManager.AddNamespace(prefix, uri);

                    return document.XPathSelectElement(path, namespaceManager);
                }
                else
                {
                    return document.XPathSelectElement(path);
                }
            }
        }

        public static IEnumerable<XElement> XPathSelectElements(this FileInfo file, string path, string prefix = null, string uri = null)
        {
            using (var reader = new StreamReader(file.FullName))
            {
                var document = XDocument.Load(reader);

                if (prefix != null)
                {
                    var namespaceManager = new XmlNamespaceManager(new NameTable());
                    namespaceManager.AddNamespace(prefix, uri);

                    return document.XPathSelectElements(path, namespaceManager);
                }
                else
                {
                    return document.XPathSelectElements(path);
                }
            }
        }
    }
}
