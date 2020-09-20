#define DISABLE

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using System.Diagnostics;
using AbstraX;

namespace AbstraX.Generators
{
    public class EntityGeneratorX
    {
        public string TransformText()
        {
#if DISABLE
            //return base.TransformText();
#else
            var generatorAttribute = (ClientCodeGenerationAttribute) this.Type.GetCustomAttributes(typeof(ClientCodeGenerationAttribute), false).FirstOrDefault();
            var text = base.TransformText();

            if (generatorAttribute != null)
            {
                var generatorType = generatorAttribute.GeneratorType;
                var engine = new TemplateEngineHost(generatorType);

                Console.WriteLine(string.Format("AbstraX Code Generator - Generating code for '{0}'", this.Type.FullName));

                var output = engine.GenerateCode(this.Type);

                if (output == null)
                {
                    return text;
                }
                else
                {
                    return text + "\r\n\r\n" + output;
                }
            }
            else
            {
                return text;
            }
#endif

            return null;
        }
    }
}
