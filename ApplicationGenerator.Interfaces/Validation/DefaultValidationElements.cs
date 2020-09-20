using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AbstraX.Validation
{
    public class InputValidationElement : ValidationElement
    {
        public InputValidationElement() : base("ion-input")
        {

        }
    }

    public class TextAreaValidationElement : ValidationElement
    {
        public TextAreaValidationElement() : base("ion-input")
        {

        }

        public TextAreaValidationElement(int rows) : base("ion-input", "text", new KeyValuePair<string, string>("rows", rows.ToString()))
        {
        }

        public TextAreaValidationElement(int cols, int rows) : base("ion-input", "text", new KeyValuePair<string, string>("rows", rows.ToString()), new KeyValuePair<string, string>("cols", cols.ToString()))
        {
        }
    }
}
