using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AbstraX.Validation
{
    public class ValidationElement
    {
        public float Priority { get; }
        public string TagName { get; }
        public string InputType { get; }
        public List<KeyValuePair<string, string>> Attributes { get; }

        public ValidationElement(string tagName, string inputType = "text", float priority = 0f)
        {
            this.TagName = tagName;
            this.InputType = inputType;
            this.Priority = priority;
            this.Attributes = new List<KeyValuePair<string, string>>();
        }

        public ValidationElement(string tagName, string inputType, float priority, params KeyValuePair<string, string>[] attributes)
        {
            this.TagName = tagName;
            this.InputType = inputType;
            this.Priority = priority;
            this.Attributes = attributes.ToList();
        }

        public ValidationElement(string tagName, string inputType, params KeyValuePair<string, string>[] attributes)
        {
            this.TagName = tagName;
            this.InputType = inputType;
            this.Attributes = attributes.ToList();
        }
    }
}
