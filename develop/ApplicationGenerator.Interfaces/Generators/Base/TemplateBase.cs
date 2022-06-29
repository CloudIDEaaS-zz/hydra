using Microsoft.VisualStudio.TextTemplating;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Utils;

namespace AbstraX.Generators.Base
{
    public class ToStringHelper
    {
        public string ToStringWithCulture(object objectToConvert)
        {
            return Microsoft.VisualStudio.TextTemplating.ToStringHelper.ToStringWithCulture(objectToConvert);
        }
    }

    public abstract class TemplateBase : TextTransformation
    {
        protected const string SPACE = " ";

        public ToStringHelper ToStringHelper
        {
            get
            {
                return new ToStringHelper();
            }
        }

        public new virtual void Write(string format, params object[] args)
        {
            base.Write(format, args);
        }

        public new virtual void Write(string textToAppend)
        {
            base.Write(textToAppend);
        }

        public new virtual void WriteLine(string textToAppend)
        {
            base.WriteLine(textToAppend);
        }

        public new virtual void WriteLine(string format, params object[] args)
        {
            base.WriteLine(format, args);
        }

        public string MakeVariable(string prefix, string variableName)
        {
            return variableName.PrependIfMissing(prefix).ToCamelCase();
        }

        public virtual void Write(int indent, string format, params object[] args)
        {
            this.Write(SPACE.Repeat(indent) + string.Format(format, args));
        }

        public virtual void Write(int indent, string textToAppend)
        {
            this.Write(SPACE.Repeat(indent) + textToAppend);
        }

        public virtual void WriteLine(int indent, string textToAppend)
        {
            this.WriteLine(SPACE.Repeat(indent) + textToAppend);
        }

        public virtual void WriteLine(int indent, string format, params object[] args)
        {
            this.WriteLine(SPACE.Repeat(indent) + string.Format(format, args));
        }

        public string GetIndent(int indent)
        {
            return SPACE.Repeat(indent);
        }
    }
}
