using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.VisualStudio.TextTemplating;
using System.Reflection;
using System.IO;
using System.CodeDom.Compiler;
using System.Diagnostics;
using System.Windows.Forms;
using Utils;

namespace AbstraX.Generators
{
    /// <summary>
    ///  if this is not working, check the following:
    ///  -  Service class name prefix matches first part of assembly name, i.e. assembly name: XmlSchemaProvider.Web = class name: XmlSchemaProviderService
    ///  -  Entities have ClientCodeGeneration attribute
    ///  -  Entities are in a namespace ending with .Entities
    ///  -  child properties are returned as List<>
    ///  -  property has the following attribute: [DataMember, Association("..", "ID", "ParentID")]
    ///  -  service methods returning "child entites" end with "parent type name" verbatim, i.e. GetModelsForProject
    ///  -  silverlight dll project has the following post-command: copy "$(TargetPath)" "$(SolutionDir)\Hydra.Web\ClientBin"
    /// </summary>

    public class TemplateEngineHost
    {
        private Type generatorType;
        private static bool bSkipErrors;

        public TemplateEngineHost(Type generatorType)
        {
            this.generatorType = generatorType;
        }

        private void DebugCallback(object sender, EventArgs e)
        {
            if (e is EventArgs<Type>)
            {
                var type = ((EventArgs<Type>)e).Value;
            }
        }

        public string GenerateCode(Type type)
        {
            try
            {
                var generator = Activator.CreateInstance(generatorType);
                var session = new TextTemplatingSession();

                session["BaseType"] = type;
                session["DebugCallback"] = new EventHandler(DebugCallback);

                //var generator2 = new AbstraXClientInterfaceGeneratorTestRun();

                //generator2.Session = session;
                //generator2.BaseType = type;
                //generator2.DebugCallback = new EventHandler(DebugCallback);

                //generator2.TransformText();

                generatorType.GetProperty("Session").SetValue(generator, session, null);
                generatorType.GetMethod("Initialize").Invoke(generator, null);

                var output = (string) generatorType.GetMethod("TransformText").Invoke(generator, null);

                return output;
            }
            catch (Exception ex)
            {
                if (MessageBox.Show(string.Format("Code generator threw an error '{0}'. Would you like to debug?", ex.Message), "Code generator error", MessageBoxButtons.YesNo) == DialogResult.Yes)
                {
                    bSkipErrors = false;
                    Debugger.Break();
                }
                else
                {
                    bSkipErrors = true;
                }
            }

            return null;
        }
    }
}
